﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.Threading;


namespace MPQNav.Collision
{
    class Collider
    {
        public ADT.ADTManager manager;

        public List<MPQNav.Collision._3D.OBB> m2BBList = new List<MPQNav.Collision._3D.OBB>();
        public List<MPQNav.Collision._3D.OBB> wmoBBLIST = new List<MPQNav.Collision._3D.OBB>();
        public MPQNav.Collision._3D.OBB characterOBB = new MPQNav.Collision._3D.OBB(new Vector3(0, .8f, 0), new Vector3(.3f, .8f, .3f), Matrix.Identity);

        public List<Vector3> standablePoints = new List<Vector3>();
        public List<Vector2> standablePoints2D = new List<Vector2>();

        public Collider(ADT.ADTManager m)
        {
            manager = m;
        }

        // NEW
        public void generateMesh(List<VertexPositionNormalColored> vList)
        {
            // First we get the points we can stand on
            this.standablePoints = this.getWalkablePoints(vList);
            foreach(Vector3 v in this.standablePoints)
            {
                 this.standablePoints2D.Add(new Vector2(v.X,v.Z));
            }
            // Next we check if we can walk between each of those points.
            foreach (Vector3 v in this.standablePoints)
            {

            }
        }

        public void walkableConnections(Vector3 vec)
        {
            // We can move in 8 directions as far as we are concerned
            // The distance beteween points is either 4.167 or 4.166 (stupid rounding)

            float change_1 = 4.167f;
            float change_2 = 4.166f;


            Vector3 v1 = new Vector3(vec.X, vec.Y, vec.Z + change_1);
            Vector3 v1a = new Vector3(vec.X, vec.Y, vec.Z + change_2);

            Vector3 v2 = new Vector3(vec.X + change_1, vec.Y, vec.Z + change_1);
            Vector3 v2a = new Vector3(vec.X + change_2, vec.Y, vec.Z + change_1);

            Vector3 v3 = new Vector3(vec.X + change_1, vec.Y, vec.Z);
            Vector3 v3a = new Vector3(vec.X + change_2, vec.Y, vec.Z);

            Vector3 v4 = new Vector3(vec.X + change_1, vec.Y, vec.Z - change_1);
            Vector3 v4a = new Vector3(vec.X + change_2, vec.Y, vec.Z - change_2);

            Vector3 v5 = new Vector3(vec.X, vec.Y, vec.Z - change_1);
            Vector3 v5a = new Vector3(vec.X, vec.Y, vec.Z - change_2);

            Vector3 v6 = new Vector3(vec.X - change_1, vec.Y, vec.Z - change_1);
            Vector3 v6a = new Vector3(vec.X - change_2, vec.Y, vec.Z - change_2);

            Vector3 v7 = new Vector3(vec.X - change_1, vec.Y, vec.Z );
            Vector3 v7a = new Vector3(vec.X - change_2, vec.Y, vec.Z);

            Vector3 v8 = new Vector3(vec.X - change_1, vec.Y, vec.Z + change_1);
            Vector3 v8a = new Vector3(vec.X - change_2, vec.Y, vec.Z + change_2);



            for (int i = 0; i < 8; i++)
            {
                
            }
        }


        public List<Vector3> getStandablePoints(Vector3 startPoint, float resolution, float max_x, float max_z, VertexPositionNormalColored[] triangleVertexList, short[] triangleIndexList)
        {
            List<Vector3> vList = new List<Vector3>();
            for (int i = 0; i < triangleVertexList.Length; i++)
            {
                vList.Add(triangleVertexList[i].Position);
            }
            List<int> iList = new List<int>();
            for (int i = 0; i < triangleIndexList.Length; i++)
            {
                iList.Add((int)triangleIndexList[i]);
            }
            return this.getStandablePoints(startPoint, resolution, max_x, max_z, vList, iList);

        }

        public List<Vector3> getStandablePoints(Vector3 startPoint, float resolution, float max_x, float max_z, List<VertexPositionNormalColored> triangleVertexList, List<short> triangleIndexList)
        {
            List<Vector3> vList = new List<Vector3>();
            for (int i = 0; i < triangleVertexList.Count; i++)
            {
                vList.Add(triangleVertexList[i].Position);
            }
            List<int> iList = new List<int>();
            for (int i = 0; i < triangleIndexList.Count; i++)
            {
                iList.Add((int)triangleIndexList[i]);
            }
            return this.getStandablePoints(startPoint, resolution, max_x, max_z, vList, iList);

        }

        /// <summary>
        /// Generates a list of vectors that a player can stand on in a given ADT. 
        /// </summary>
        /// <param name="startPoint">The starting point to test from (top-left corner)</param>
        /// <param name="resolution">The width between each point</param>
        /// <param name="max_x">The maximum value for x</param>
        /// <param name="max_z">The maximum value for y</param>
        /// <param name="triangleIndexList">List of indices for the traingles.</param>
        /// <param name="triangleVertexList">List of vertices for the traingles.</param>
        /// <returns>List of standable points</returns>
        public List<Vector3> getStandablePoints(Vector3 startPoint, float resolution, float max_x, float max_z, List<Vector3> triangleVertexList, List<int> triangleIndexList)
        {
            List<Vector3> returnList = new List<Vector3>();
            //max_x -= startPoint.X;
            //max_z -= startPoint.Z;
            max_x = startPoint.X - max_x;
            max_z = startPoint.Z - max_z;

            List<long> TriangleTimes = new List<long>();
            List<long> OBBTimes = new List<long>();
            int steps = (int)((startPoint.Z - max_z) / resolution);
            int step = 0;
            Stopwatch sw = new Stopwatch();
            //for (float z = startPoint.Z; z > max_z; z -= resolution)
            for (int z = 0; z < 5; )
            {
                sw.Start();
                for (float x = startPoint.X; x > max_x; x -= resolution)
                {
                    int count = 0;
                    Boolean found = false;
                    Vector3 intersectionPoint = new Vector3();
                    MPQNav.Collision._2D.Triangle tri = new MPQNav.Collision._2D.Triangle(Vector3.Zero, Vector3.Zero, Vector3.Zero);

                    while (count < triangleIndexList.Count / 3 && found == false)
                    {
                        Vector3 p1 = triangleVertexList[triangleIndexList[(count * 3) + 0]];
                        Vector3 p2 = triangleVertexList[triangleIndexList[(count * 3) + 1]];
                        Vector3 p3 = triangleVertexList[triangleIndexList[(count * 3) + 2]];
                        float t, u, v;
                        if (MPQNav.Collision._2D.Triangle.RayTriangleIntersect(new Vector3(x, 0, z), Vector3.Down, p1, p2, p3, out t, out u, out v))
                        {
                            found = true;
                            intersectionPoint.X = x;
                            intersectionPoint.Z = z;
                            intersectionPoint.Y = 0 - t;
                            tri = new MPQNav.Collision._2D.Triangle(p1, p2, p3);
                        }
                        else if (MPQNav.Collision._2D.Triangle.RayTriangleIntersect(new Vector3(x, 0, z), Vector3.Up, p1, p2, p3, out t, out u, out v))
                        {
                            found = true;
                            intersectionPoint.X = x;
                            intersectionPoint.Z = z;
                            intersectionPoint.Y = 0 + t;
                            tri = new MPQNav.Collision._2D.Triangle(p1, p2, p3);
                        }
                        count++;
                    }

                    //
                    // if (found == false)
                    // {
                    //    Console.WriteLine("Should have found an intersection, but didn't... hm...");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Found an intersection: " + intersectionPoint);
                    //}
                    // Now we have the intersection, it's time to test for the slope
                    if (tri.getSlope() > 45)
                    {
                        // Good slope, lets continue
                        characterOBB.Center = intersectionPoint + new Vector3(0, .8f, 0);

                        Boolean intersects = false;

                        int j = 0;
                        while (intersects == false && j < m2BBList.Count)
                        {
                            if (m2BBList[j].Intersects(characterOBB))
                            {
                                intersects = true;
                            }
                            j++;
                        }

                        j = 0;
                        while (intersects == false && j < wmoBBLIST.Count)
                        {
                            if (wmoBBLIST[j].Intersects(characterOBB))
                            {
                                intersects = true;
                            }
                            j++;
                        }

                        if (intersects == false)
                        {
                            returnList.Add(intersectionPoint);
                            //Console.WriteLine("Added: " + intersectionPoint);
                        }
                    }
                }
                sw.Stop();
                step++;
                Console.WriteLine((float)((float)step / (float)steps) * 100 + "% Done - " + (TimeSpan.FromTicks(sw.ElapsedTicks * (steps - step))) + " to go");
                sw.Reset();
                return returnList;
                
            }
            return returnList;
        }

        public List<Vector3> getStandablePoints(List<VertexPositionNormalColored> vList, List<short> iList)
        {
            List<Vector3> returnList = new List<Vector3>();
            for (int i = 0; i < iList.Count / 3; i++)
            {
                Vector3 v1 = vList[(int)iList[(i * 3) + 0]].Position;
                Vector3 v2 = vList[(int)iList[(i * 3) + 1]].Position;
                Vector3 v3 = vList[(int)iList[(i * 3) + 2]].Position;
                MPQNav.Collision._2D.Triangle t = new MPQNav.Collision._2D.Triangle(v1, v2, v3);
                // All triangles in this list should be right triangles and they should be axis aligned
                // The triangle should be half of a 4.166~ x 4.166~ square
                // 1/16th of that distance is .206416~
                float adjustment = .20641666666f;
                
                switch (t.rot)
                {
                    case MPQNav.Collision._2D.Triangle.rotation.rotation_1:
                        //
                        // 000
                        // 00
                        // 0 Start here and go up
                        //
                        int count = 1;
                        for (int r = 0; r < 16; r++)
                        {
                            for (int c = 0; c < count; c++)
                            {
                                float adjust_x = c * adjustment;
                                float adjust_z = r * adjustment;

                            }
                            count++;
                        }
                        break;

                    case MPQNav.Collision._2D.Triangle.rotation.rotation_2:

                        break;


                    case MPQNav.Collision._2D.Triangle.rotation.rotation_3:

                        break;

                    case MPQNav.Collision._2D.Triangle.rotation.rotation_4:

                        break;

                    case MPQNav.Collision._2D.Triangle.rotation.rotation_none:
                        Console.WriteLine("I screwed up");
                        break;
                }
                
                //Triangle t = new Triangle(vList[iList[i]],vList[iList[i + 1]],vList[iList[i+2]]);
            }
            return returnList;
        }

        public List<Vector3> getWalkablePoints(List<VertexPositionNormalColored> vList)
        {
            List<Vector3> returnList = new List<Vector3>();

            for (int i = 0; i < vList.Count; i++)
            {
                if (vList[i].Color != Color.Black)
                {
                    characterOBB.Center = vList[i].Position + new Vector3(0,.8f,0);

                    Boolean intersects = false;
                    int j = 0;
                    while (intersects == false && j < m2BBList.Count)
                    {
                        if (m2BBList[j].Intersects(characterOBB))
                        {
                            intersects = true;
                        }                        
                        j++;
                    }

                    j = 0;
                    while (intersects == false && j < wmoBBLIST.Count)
                    {
                        if (wmoBBLIST[j].Intersects(characterOBB))
                        {
                            intersects = true;
                        }
                        j++;
                    }

                    if (intersects == false)
                    {
                        returnList.Add(vList[i].Position);
                    }
                }
            }
            return returnList;
        }

    }
}
