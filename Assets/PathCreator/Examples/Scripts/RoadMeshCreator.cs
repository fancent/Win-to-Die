using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = .4f;
        [Range(0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;

        [Header("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;

        [SerializeField]
        public GameObject meshHolder;


        GameObject roadHolder;

        MeshFilter roadMeshFilter;
        MeshRenderer roadMeshRenderer;
        Mesh roadMesh;

        GameObject leftWallHolder;

        Mesh leftWallMesh;

        MeshFilter leftWallFilter;
        MeshRenderer leftWallRenderer;

        GameObject rightWallHolder;

        Mesh rightWallMesh;

        MeshFilter rightWallFilter;
        MeshRenderer rightWallRenderer;


        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateRoadMesh();
                CreateLeftWall();
            }
        }

        void CreateRoadMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 6;
            }

            roadMesh.Clear();
            roadMesh.vertices = verts;
            roadMesh.uv = uvs;
            roadMesh.normals = normals;
            roadMesh.subMeshCount = 3;
            roadMesh.SetTriangles(roadTriangles, 0);
            roadMesh.SetTriangles(underRoadTriangles, 1);
            roadMesh.SetTriangles(sideOfRoadTriangles, 2);
            roadMesh.RecalculateBounds();
        }


        void CreateLeftWall()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth) - localRight + localUp * thickness;
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth) - localRight - localRight + localUp * thickness;

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 6;
            }

            leftWallMesh.Clear();
            leftWallMesh.vertices = verts;
            leftWallMesh.uv = uvs;
            leftWallMesh.normals = normals;
            leftWallMesh.subMeshCount = 3;
            leftWallMesh.SetTriangles(roadTriangles, 0);
            leftWallMesh.SetTriangles(underRoadTriangles, 1);
            leftWallMesh.SetTriangles(sideOfRoadTriangles, 2);
            leftWallMesh.RecalculateBounds();
        }

        void setupMeshHolder(GameObject holder)
        {
            holder.transform.parent = meshHolder.transform;
            holder.gameObject.AddComponent<MeshFilter>();
            holder.gameObject.AddComponent<MeshRenderer>();
            holder.transform.rotation = Quaternion.identity;
            holder.transform.position = Vector3.zero;
            holder.transform.localScale = Vector3.one;
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {

            if (meshHolder == null)
            {
                meshHolder = new GameObject(this.transform.name + " Mesh Holder");
            }

            if (meshHolder.transform.childCount <= 0)
            {
                roadHolder = new GameObject("Road");
                setupMeshHolder(roadHolder);

                leftWallHolder = new GameObject("Left_Wall");
                setupMeshHolder(leftWallHolder);

                rightWallHolder = new GameObject("Right_Wall");
                setupMeshHolder(rightWallHolder);

            }

            // meshHolder.transform.rotation = Quaternion.identity;
            // meshHolder.transform.position = Vector3.zero;
            // meshHolder.transform.localScale = Vector3.one;


            roadMeshRenderer = roadHolder.GetComponent<MeshRenderer>();
            roadMeshFilter = roadHolder.GetComponent<MeshFilter>();

            leftWallRenderer = leftWallHolder.GetComponent<MeshRenderer>();
            leftWallFilter = leftWallHolder.GetComponent<MeshFilter>();

            rightWallRenderer = rightWallHolder.GetComponent<MeshRenderer>();
            rightWallFilter = rightWallHolder.GetComponent<MeshFilter>();

            if (roadMesh == null)
            {
                roadMesh = new Mesh();
            }

            if (leftWallMesh == null)
            {
                leftWallMesh = new Mesh();
            }

            if (rightWallMesh == null)
            {
                rightWallMesh = new Mesh();
            }




            roadMeshFilter.sharedMesh = roadMesh;
            leftWallFilter.sharedMesh = leftWallMesh;
            rightWallFilter.sharedMesh = rightWallMesh;

        }

        void AssignMaterials()
        {
            if (roadMaterial != null && undersideMaterial != null)
            {
                roadMeshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                roadMeshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);

                leftWallRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                leftWallRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);

                rightWallRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                rightWallRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
            }
        }

    }
}