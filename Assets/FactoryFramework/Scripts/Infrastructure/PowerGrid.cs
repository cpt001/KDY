using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace FactoryFramework
{
    [System.Serializable]
    public class PowerGrid
    {
        public HashSet<PowerGridComponent> nodes;
        public HashSet<(PowerGridComponent, PowerGridComponent)> edges;

        public PowerGrid()
        {
            nodes = new HashSet<PowerGridComponent>();
            edges = new HashSet<(PowerGridComponent, PowerGridComponent)>();
        }

        public float Load
        {
            get
            {
                if (nodes == null) return 0f;
                return nodes.Where(n => n.basePowerDraw > 0).Sum(n => n.basePowerDraw);
            }
        }
        public float Production
        {
            get
            {
                if (nodes == null) return 0f;
                return Mathf.Abs(nodes.Where(n => n.basePowerDraw < 0).Sum(n => n.basePowerDraw));
            }
        }
        private float NetPower { get { return nodes.Sum(n => n.basePowerDraw); } }
        public float Efficiency { get { return (Production > 0f) ? Mathf.Clamp01(Production / Load) : 0f; } }

        /// <summary>
        /// Internal connection between the underlying node classes
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(PowerGridComponent node)
        {
            nodes.Add(node);
        }

        public void AddEdge(PowerGridComponent a, PowerGridComponent b)
        {
            if (a == b) return;
            if (a.grid.Equals(b.grid))
            {
                if (a.Guid.CompareTo(b.Guid) > 0)
                {
                    (a, b) = (b, a);
                }
                edges.Add((a, b));
            }
            else
            {
                var unioned = UnionGraphs(a.grid, b.grid);
                a.grid = unioned;
                b.grid = unioned;
                edges.Add((a, b));
            }
        }

        public static PowerGrid UnionGraphs(PowerGrid a, PowerGrid b)
        {
            if (a == b) return a;
            // keep bigger graph
            if (a.nodes.Count < b.nodes.Count)
                return UnionGraphs(b, a);
            // a is definitely the bigger graph
            foreach (var n in b.nodes)
            {
                // add to grid and set the active grid
                a.AddNode(n);
                n.grid = a;
            }
            foreach (var e in b.edges)
            {
                a.AddEdge(e.Item1, e.Item2);
            }
            return a;
        }

        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            s.AppendJoin('\n', nodes.Select(n => n.ToString()));
            s.Append(":");
            s.AppendJoin('\n', edges.Select(e => $"{e.Item1} -> {e.Item2}"));

            return s.ToString();
        }
        public void RemoveNode(PowerGridComponent node)
        {
            var connections = edges.Where(e => e.Item1 == node || e.Item2 == node).ToArray();
            foreach (var edge in connections)
            {
                Disconnect(edge.Item1, edge.Item2);
            }

            nodes.Remove(node);
            node.grid = new PowerGrid();
        }
        public static void Disconnect(PowerGridComponent a, PowerGridComponent b)
        {
            if (a.grid != b.grid) return;

            var edgeToRemove = (a, b);
            a.grid.edges.Remove(edgeToRemove);

            // Perform BFS/DFS to check for disconnected components
            var visited = new HashSet<PowerGridComponent>();
            var components = new List<PowerGrid>();
            foreach (var node in a.grid.nodes)
            {
                if (!visited.Contains(node))
                {
                    var component = new PowerGrid();
                    a.grid.BFS(node, visited, component);
                    components.Add(component);
                }
            }
            //if (components.Count() > 1)
            //{
            //    a.grid = components.Where(c => c.nodes.Contains(a)).FirstOrDefault();
            //    b.grid = components.Where(c => c.nodes.Contains(b)).FirstOrDefault();
            //    if (a.grid == null) a.grid = new PowerGrid();
            //    if (b.grid == null) b.grid = new PowerGrid();
            //}
            foreach (var subGraph in components)
            {
                foreach(var n in subGraph.nodes)
                {
                    n.grid = subGraph;
                }
            }
        }
        protected void BFS(PowerGridComponent node, HashSet<PowerGridComponent> visited, PowerGrid component)
        {
            var queue = new Queue<PowerGridComponent>();
            queue.Enqueue(node);
            visited.Add(node);
            component.AddNode(node);


            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in this.edges.Where(e => e.Item1 == current || e.Item2 == current))
                {
                    var neighbor = edge.Item1 == current ? edge.Item2 : edge.Item1;
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        component.AddNode(neighbor);
                        component.AddEdge(current, neighbor);
                    }
                }

            }

        }

        public (string[], (string, string)[]) ToDOT()
        {
            var nodes = this.nodes.Select(n => n.Guid.ToString()).ToArray();
            var edges = this.edges.Select(e => (e.Item1.Guid.ToString(), e.Item2.Guid.ToString())).ToArray();
            return (nodes, edges);
        }

        public override bool Equals(object other)
        {
            if (other == null || other as PowerGrid == null) return false;
            var otherGrid = other as PowerGrid;
            // check for total intersection of nodes
            return this.nodes.SetEquals(otherGrid.nodes) && this.edges.SetEquals(otherGrid.edges);
        }
        public override int GetHashCode()
        {
            // custom hash code of all edges and nodes
            return this.nodes.GetHashCode() ^ this.edges.GetHashCode();
        }
        //override == op
        public static bool operator ==(PowerGrid a, PowerGrid b)
        {
            return a.Equals(b);
        }
        //override != op
        public static bool operator !=(PowerGrid a, PowerGrid b)
        {
            return !a.Equals(b);
        }
    }
}