using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FactoryFramework;
using NUnit.Framework.Constraints;

public class PathTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void PathTestsSimplePasses()
    {
        // Use the Assert class to test conditions
        Assert.True(true);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PathTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
        Assert.True(true);
    }


    [Test]
    public void ArcPathLeft90()
    {
        Vector3 center = Vector3.zero;
        Vector3 start = new Vector3(1, 0, 0);
        Vector3 end = new Vector3(0, 0, 1);
        Vector3 forward = new Vector3(0, 0, 1);
        ArcPath a = new ArcPath(center, start, end, 1, forward, Vector3.up);

        Assert.AreEqual(a.mStruct.angle, -Mathf.PI / 2);
        Assert.True(a.GetDirectionAtPoint(0) == forward);
        Assert.True(a.GetDirectionAtPoint(1) == new Vector3(-1, 0, 0));
        Assert.True(a.GetDirectionAtPoint(0.5f) == new Vector3(-1,0,1).normalized);

        Assert.True(a.GetWorldPointFromPathSpace(0) == start);
        Assert.True(a.GetWorldPointFromPathSpace(1) == end);
        Assert.True(a.GetWorldPointFromPathSpace(0.5f) == new Vector3(0.7071067f, 0, 0.7071067f));
    }

    [Test]
    public void ArcPathLeft180()
    {
        Vector3 center = Vector3.zero;
        Vector3 start = new Vector3(1, 0, 0);
        Vector3 end = new Vector3(-1, 0, 0);
        Vector3 forward = new Vector3(0, 0, 1);
        ArcPath a = new ArcPath(center, start, end, 1, forward, Vector3.up);

        Assert.AreEqual(a.mStruct.angle, -Mathf.PI);
        Assert.True(a.GetDirectionAtPoint(0) == forward);
        Assert.True(a.GetDirectionAtPoint(1) == new Vector3(0, 0, -1));
        Assert.True(a.GetDirectionAtPoint(0.5f) == new Vector3(-1, 0, 0).normalized);

        Assert.True(a.GetWorldPointFromPathSpace(0) == start);
        Assert.True(a.GetWorldPointFromPathSpace(1) == end);
        Assert.True(a.GetWorldPointFromPathSpace(0.5f) == new Vector3(0,0,1));
    }

    [Test]
    public void ArcPathRight180()
    {
        Vector3 center = Vector3.zero;
        Vector3 start = new Vector3(-1, 0, 0);
        Vector3 end = new Vector3(1, 0, 0);
        Vector3 forward = new Vector3(0, 0, 1);
        ArcPath a = new ArcPath(center, start, end, 1, forward, Vector3.up);

        Assert.AreEqual(a.mStruct.angle, Mathf.PI);
        Assert.True(a.GetDirectionAtPoint(0) == forward);
        Assert.True(a.GetDirectionAtPoint(1) == new Vector3(0, 0, -1));
        Assert.True(a.GetDirectionAtPoint(0.5f) == new Vector3(1, 0, 0).normalized);

        Assert.True(a.GetWorldPointFromPathSpace(0) == start);
        Assert.True(a.GetWorldPointFromPathSpace(1) == end);
        Assert.True(a.GetWorldPointFromPathSpace(0.5f) == new Vector3(0, 0, 1));
    }


    [Test]
    public void ArcPathRight250()
    {
        float targetAngRad = Mathf.Deg2Rad * 250f;
        Vector3 center = Vector3.zero;
        Vector3 start = new Vector3(-1, 0, 0);
        Vector3 end = new Vector3(-Mathf.Cos(targetAngRad), 0, Mathf.Sin(targetAngRad));
        Vector3 forward = new Vector3(0, 0, 1);
        ArcPath a = new ArcPath(center, start, end, 1, forward, Vector3.up);

        Assert.AreEqual(a.mStruct.angle, targetAngRad);
        Assert.True(a.GetDirectionAtPoint(0) == forward);

        Assert.True(a.GetWorldPointFromPathSpace(0) == start);
        Assert.True(a.GetWorldPointFromPathSpace(1) == end);
    }

    [Test]
    public void ArcPathVertical90()
    {
        float targetAngRad = Mathf.Deg2Rad * 90;
        Vector3 center = new Vector3(0,1,0);
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 end = new Vector3(0, 1, 1);
        Vector3 forward = new Vector3(0, 0, 1);

        ArcPath a = new ArcPath(center, start, end, 1, forward, Vector3.right);

        Assert.AreEqual(a.mStruct.angle, -targetAngRad);
        Assert.True(a.GetDirectionAtPoint(0) == forward);
        Assert.True(a.GetDirectionAtPoint(1) == Vector3.up);
        Assert.True(a.GetDirectionAtPoint(0.5f) == new Vector3(0, 1, 1).normalized);

        Assert.True(a.GetWorldPointFromPathSpace(0) == start);
        Assert.True(a.GetWorldPointFromPathSpace(1) == end);
        Assert.True(a.GetWorldPointFromPathSpace(0.5f) == new Vector3(0, 1-0.7071067f, 0.7071067f));
    }

    [Test]
    public void SmartPathStraight()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = Vector3.zero,
            right = Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = new Vector3(0, 0, 1),
            right = Vector3.right
        };

        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.subPaths.Count == 1);
        Assert.True(smartPath.GetTotalLength() == 1);

        smartPath.CleanUp();
    }

    [Test]
    public void SmartPath180ArcsRight()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = Vector3.zero,
            right = Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = new Vector3(4, 0, 0),
            right = Vector3.right
        };

        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.subPaths.Count == 3);
        Assert.True(smartPath.GetTotalLength() == 6.28318530718f);

        Assert.True(smartPath.GetDirectionAtPoint(0f) == Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(0.25f) == Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(0.5f) == -Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(0.75f) == Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(1f) == Vector3.forward);

        Assert.True(smartPath.GetWorldPointFromPathSpace(0f) == Vector3.zero);
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.25f) == new Vector3(1,0,1));
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.5f) == new Vector3(2,0,0));
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.75f) == new Vector3(3,0,-1));
        Assert.True(smartPath.GetWorldPointFromPathSpace(1f) == new Vector3(4,0,0));

        smartPath.CleanUp();
    }

    [Test]
    public void SmartPath180ArcsLeft()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = Vector3.zero,
            right = Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = new Vector3(-4, 0, 0),
            right = Vector3.right
        };

        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.subPaths.Count == 3);
        Assert.AreEqual(smartPath.GetTotalLength(), 6.28318530718f,0.01f);

        Assert.True(smartPath.GetDirectionAtPoint(0f) == Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(0.25f) == -Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(0.5f) == -Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(0.75f) == -Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(1f) == Vector3.forward);

        Assert.True(smartPath.GetWorldPointFromPathSpace(0f) == Vector3.zero);
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.25f) == new Vector3(-1, 0, 1));
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.5f) == new Vector3(-2, 0, 0));
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.75f) == new Vector3(-3, 0, -1));
        Assert.True(smartPath.GetWorldPointFromPathSpace(1f) == new Vector3(-4, 0, 0));

        smartPath.CleanUp();
    }


    [Test]
    public void SmartPath180RightOffset()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = Vector3.zero,
            right = Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = new Vector3(4, 0, -10),
            right = Vector3.right
        };

        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.subPaths.Count == 3);
        Assert.AreEqual(smartPath.GetTotalLength(), 10f + 6.28318530718f, 0.01f);

        float length = smartPath.GetTotalLength();

        float arc1End = Mathf.PI / length;
        float arc1Half = arc1End / 2;

        float arc2Start = (10 + Mathf.PI) / length;
        float arc2Half = arc2Start + arc1Half;
        
        Assert.True(smartPath.GetDirectionAtPoint(0f) == Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(arc1Half) == Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(arc1End) == -Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(0.5f) == -Vector3.forward);
        Assert.True(smartPath.GetDirectionAtPoint(arc2Half) == Vector3.right);
        Assert.True(smartPath.GetDirectionAtPoint(1f) == Vector3.forward);

        Assert.True(smartPath.GetWorldPointFromPathSpace(0f) == Vector3.zero);
        Assert.True(smartPath.GetWorldPointFromPathSpace(arc1Half) == new Vector3(1, 0, 1));
        Assert.True(smartPath.GetWorldPointFromPathSpace(arc1End) == new Vector3(2, 0, 0));
        Assert.True(smartPath.GetWorldPointFromPathSpace(0.5f) == new Vector3(2,0,-5));
        Assert.True(smartPath.GetWorldPointFromPathSpace(arc2Half) == new Vector3(3, 0, -11));
        Assert.True(smartPath.GetWorldPointFromPathSpace(1f) == new Vector3(4, 0, -10));

        smartPath.CleanUp();
    }

    [Test]
    public void SmartPathVerticalOnly()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = Vector3.zero,
            right = Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = Vector3.forward,
            pos = new Vector3(0,5,5),
            right = Vector3.right
        };
        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.IsValid);
        Assert.True(smartPath.subPaths.Count == 3);

        Assert.True(smartPath.GetWorldPointFromPathSpace(0f) == Vector3.zero);
        Assert.True(smartPath.GetWorldPointFromPathSpace(1f) == new Vector3(0, 5, 5));
    }

    [Test]
    public void SmartPathVerticalBackwards()
    {
        PathAnchor s = new PathAnchor()
        {
            forward = -Vector3.forward,
            pos = Vector3.zero,
            right = -Vector3.right
        };

        PathAnchor e = new PathAnchor()
        {
            forward = -Vector3.forward,
            pos = new Vector3(0, 5, -5),
            right = -Vector3.right
        };
        SmartPath smartPath = new SmartPath(s, e, 1, 0.01f, 1);
        Assert.True(smartPath.IsValid);
        Assert.True(smartPath.subPaths.Count == 3);

        Assert.True(smartPath.GetWorldPointFromPathSpace(0f) == Vector3.zero);
        Assert.True(smartPath.GetWorldPointFromPathSpace(1f) == new Vector3(0, 5, -5));

        var firstArc = (ArcPath)smartPath.subPaths[0].Item1;
        var lastArc = (ArcPath)smartPath.subPaths[2].Item1;
        Assert.True(firstArc.mStruct.center.y == 1);
        Assert.True(lastArc.mStruct.center.y == 4);

        Assert.AreEqual(firstArc.mStruct.angle, -0.89f, 0.01f);
        Assert.AreEqual(lastArc.mStruct.angle, 0.89f, 0.01f);
        Assert.True(firstArc.mStruct.angle < 90);
        Assert.True(lastArc.mStruct.angle < 90);
    }

}
