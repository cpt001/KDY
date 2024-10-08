using FactoryFramework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(IPathTester))]
public class IPathTesterEditor : Editor
{
    IPathTester holder;
    void OnEnable()
    {
        holder = (IPathTester)target;
        if (holder.p == null)
        {
            holder.Regen();
        }
    }
    void OnSceneGUI()
    {
        //Input();
        Draw();
    }

    public float pathP = 0.5f;
    public Vector3 closestPosTarget = new Vector3(0, 1, 0);

    void Draw()
    {
        bool changed = false;

        //draw anchors
        Handles.color = Color.white;
        var fmh_36_68_638575315281124243 = Quaternion.identity; Vector3 newStartPos = Handles.FreeMoveHandle(holder.start, 0.1f, Vector3.zero, Handles.CircleHandleCap);
        if (newStartPos != holder.start)
        {
            holder.start = newStartPos;
            changed = true;
        }

        Handles.color = Color.black;
        var fmh_44_64_638575315281144258 = Quaternion.identity; Vector3 newEndPos = Handles.FreeMoveHandle(holder.end, 0.1f, Vector3.zero, Handles.CircleHandleCap);
        if (newEndPos != holder.end)
        {
            holder.end = newEndPos;
            changed = true;
        }

        Handles.color = Color.yellow;
        Handles.DrawLine(holder.start, holder.start + holder.startdir * 0.2f);
        Quaternion newStartAng = Handles.Disc(Quaternion.LookRotation(holder.startdir, Vector3.up), holder.start, Vector3.up, 0.3f, false, 5);
        Vector3 newForward = newStartAng * Vector3.forward;
        if (newForward != holder.startdir)
        {
            holder.startdir = newForward;
            changed = true;
        }

        Handles.DrawLine(holder.end, holder.end + holder.enddir * 0.2f);
        Quaternion newEndAng = Handles.Disc(Quaternion.LookRotation(holder.enddir, Vector3.up), holder.end, Vector3.up, 0.3f, false, 5);
        Vector3 newEndForward = newEndAng * Vector3.forward;
        if (newEndForward != holder.enddir)
        {
            holder.enddir = newEndForward;
            changed = true;
        }

        Vector3 pMid = holder.p.GetWorldPointFromPathSpace(pathP);
        Handles.color = Color.red; //right
        Handles.DrawLine(pMid, pMid + holder.p.GetRightAtPoint(pathP) * 0.2f, 4f);

        Handles.color = Color.cyan; // up
        Handles.DrawLine(pMid, pMid + holder.p.GetUpAtPoint(pathP) * 0.2f, 4f);

        Handles.color = Color.green; // forward
        Handles.DrawLine(pMid, pMid + holder.p.GetDirectionAtPoint(pathP) * 0.2f, 4f);

        if (holder.p as SmartPath != null)
        {
            SmartPath bs = holder.p as SmartPath;
            var (a, b) = bs.subPaths[0];
            foreach (var (subP, _) in bs.subPaths)
            {
                if (subP.GetType() == typeof(ArcPath))
                {
                    ArcPath arc = subP as ArcPath;
                    Handles.color = Color.blue;
                    Handles.DrawWireArc(arc.mStruct.center, arc.mStruct.normal, arc.mStruct.GetFrom(), arc.mStruct.angle * Mathf.Rad2Deg, arc.mStruct.radius, 2);
                }
                else if (subP.GetType() == typeof(SegmentPath))
                {
                    Handles.color = Color.green;
                    SegmentPath segment = subP as SegmentPath;
                    Handles.DrawLine(segment.GetStart(), segment.GetEnd(), 2);
                }
            }
        }
        else if (holder.p as CubicBezierPath != null)
        {
            CubicBezierPath cbp = holder.p as CubicBezierPath;
            Handles.DrawBezier(cbp.mStruct.start, cbp.mStruct.end, cbp.mStruct.controlPointA, cbp.mStruct.controlPointB, Color.blue, null, 2f);
        }
        else if (holder.p as SegmentPath != null)
        {
            SegmentPath sp = holder.p as SegmentPath;
            Handles.color = Color.blue;
            Handles.DrawLine(sp.mStruct.start, sp.mStruct.end, 2f);
        }

        //Draw closestPoint holder
        Handles.color = Color.yellow;
        var fmh_114_69_638575315281147502 = Quaternion.identity; closestPosTarget = Handles.FreeMoveHandle(closestPosTarget, 0.1f, Vector3.zero, Handles.CircleHandleCap);
        var closest = holder.p.GetClosestPoint(closestPosTarget);
        var fmh_116_47_638575315281151066 = Quaternion.identity; Handles.FreeMoveHandle(closest.Item1, 0.05f, Vector3.zero, Handles.CircleHandleCap);


        //Draw points along path
        for(float i = 0; i<=0.98f; i+=0.02f)
        {
            Vector3 spoint = holder.p.GetWorldPointFromPathSpace(i);
            Vector3 epoint = holder.p.GetWorldPointFromPathSpace(i + 0.02f);
            Handles.DrawDottedLine(spoint, epoint, 2);
        }


        if (changed)
            holder.Regen();

    }

    public override void OnInspectorGUI()
    {
        bool changed = false;

        float pathPercent = EditorGUILayout.Slider(pathP, 0, 1);
        if (pathPercent != pathP)
        {
            pathP = pathPercent;
            changed = true;
        }

        GlobalLogisticsSettings.PathSolveType newType = (GlobalLogisticsSettings.PathSolveType)EditorGUILayout.EnumPopup(holder.pt);
        if (newType != holder.pt)
        {
            holder.pt = newType;
            changed = true;
        }

        EditorGUILayout.LabelField("Length: " + holder.p?.GetTotalLength());

        holder.frameBM = (BeltMeshSO)EditorGUILayout.ObjectField(holder.frameBM, typeof(BeltMeshSO), true);
        holder.beltBM = (BeltMeshSO)EditorGUILayout.ObjectField(holder.beltBM, typeof(BeltMeshSO), true);
        holder.frameFilter = (MeshFilter)EditorGUILayout.ObjectField(holder.frameFilter, typeof(MeshFilter), true);
        holder.beltFilter = (MeshFilter)EditorGUILayout.ObjectField(holder.beltFilter, typeof(MeshFilter), true);

        if (GUILayout.Button("Generate mesh"))
        {
            holder.GenerateMesh();
        }
        if(GUILayout.Button("Test Pos 1"))
        {
            holder.start = Vector3.zero;
            holder.startdir = Vector3.forward;
            holder.end = new Vector3(2, 0, 0);
            holder.enddir = Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 2"))
        {
            holder.start = Vector3.zero;
            holder.startdir = Vector3.forward;
            holder.end = new Vector3(2, 0, -3);
            holder.enddir = Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 3"))
        {
            holder.start = Vector3.zero;
            holder.startdir = Vector3.forward;
            holder.end = new Vector3(-2, 0, 0);
            holder.enddir = Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 4"))
        {
            holder.start = Vector3.zero;
            holder.startdir = -Vector3.forward;
            holder.end = new Vector3(-2, 0, 0);
            holder.enddir = -Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 5"))
        {
            holder.start = Vector3.zero;
            holder.startdir = -Vector3.forward;
            holder.end = new Vector3(2, 0, 0);
            holder.enddir = -Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 6"))
        {
            holder.start = Vector3.zero;
            holder.startdir = -Vector3.forward;
            holder.end = new Vector3(2.05f, 0, 0);
            holder.enddir = -Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 7"))
        {
            holder.start = Vector3.zero;
            holder.startdir = -Vector3.forward;
            holder.end = new Vector3(0, 3, -4);
            holder.enddir = -Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 8"))
        {
            holder.start = Vector3.zero;
            holder.startdir = Vector3.forward;
            holder.end = new Vector3(0, 3, 4);
            holder.enddir = Vector3.forward;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 9"))
        {
            holder.start = Vector3.zero;
            holder.startdir = Vector3.right;
            holder.end = new Vector3(4, 3, 0);
            holder.enddir = Vector3.right;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 10"))
        {
            holder.start = Vector3.zero;
            holder.startdir = -Vector3.right;
            holder.end = new Vector3(-4, 3, 0);
            holder.enddir = -Vector3.right;
            changed = true;
        }
        if (GUILayout.Button("Test Pos 11"))
        {
            holder.start = new Vector3(0,5,0);
            holder.startdir = -Vector3.forward;
            holder.end = new Vector3(0, 3, -5);
            holder.enddir = -Vector3.forward;
            changed = true;
        }

        if (changed)
        {
            holder.Regen();
            HandleUtility.Repaint();
            SceneView.RepaintAll();
        }
    }
}
