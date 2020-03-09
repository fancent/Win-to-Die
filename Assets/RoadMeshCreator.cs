using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = .4f;
        public float wallWidth = .4f;
        public float wallHeight = .2f;
        public float grassWidth = .2f;

        [Range(0, 2f)]
        public float roadThickness = .15f;
        public bool flattenSurface;

        [Header("Material settings")]
        public Material grassMaterial;
        public Material roadMaterial;
        public Material undersideMaterial;
        public Material wallMaterial;
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
                CreateWall("left");
                CreateWall("right");
            }
        }

        void CreateRoadMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 10];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] grassTriangles = new int[numTris * 3];
            int[] leftRoadTriangles = new int[numTris * 3];
            int[] rightRoadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            int[] grassMap = {8, 18, 9, 9, 18, 19};
            int[] leftRoadMap = {0, 10, 8, 8, 10, 18};
            int[] rightRoadMap = {9, 19, 1, 1, 19, 11};
            int[] underRoadMap = {2, 3, 12, 3, 13, 12};
            int[] sidesTriangleMap = {4, 6, 16, 14, 4, 16, 5, 17, 7, 15, 17, 5};

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
                verts[vertIndex + 2] = vertSideA - localUp * roadThickness;
                verts[vertIndex + 3] = vertSideB - localUp * roadThickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Add the grass locations
                verts[vertIndex + 8] = path.GetPoint(i) - localRight * Mathf.Abs(grassWidth);
                verts[vertIndex + 9] = path.GetPoint(i) + localRight * Mathf.Abs(grassWidth);

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 8] = new Vector2(1, path.times[i]);
                uvs[vertIndex + 9] = new Vector2(0, path.times[i]);
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
                // Grass normals
                normals[vertIndex + 8] = localUp;
                normals[vertIndex + 9] = localUp;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < grassMap.Length; j++)
                    {
                        grassTriangles[triIndex + j] = vertIndex + grassMap[j] % verts.Length;
                        leftRoadTriangles[triIndex + j] = vertIndex + leftRoadMap[j] % verts.Length;
                        rightRoadTriangles[triIndex + j] = vertIndex + rightRoadMap[j] % verts.Length;
                        underRoadTriangles[triIndex + j] = vertIndex + underRoadMap[j] % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 10;
                triIndex += 6;
            }

            roadMesh.Clear();
            roadMesh.vertices = verts;
            roadMesh.uv = uvs;
            roadMesh.normals = normals;
            roadMesh.subMeshCount = 5;
            roadMesh.SetTriangles(grassTriangles, 0);
            roadMesh.SetTriangles(leftRoadTriangles, 1);
            roadMesh.SetTriangles(rightRoadTriangles, 2);
            roadMesh.SetTriangles(underRoadTriangles, 3);
            roadMesh.SetTriangles(sideOfRoadTriangles, 4);
            roadMesh.RecalculateBounds();
        }


        void CreateWall(string left_or_right)
        {
            //Clean way to do this: since left side and right side are symmetric, we multiply by either -1 or 1.
            int side = 1;
            if (left_or_right == "left"){
                side = -1;
            }

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

                // vertSideA is the BOTTOMLEFT of the wall, vertSideB is the BOTTOMRIGHT.
                Vector3 vertSideA = path.GetPoint(i) + side * localRight * Mathf.Abs(roadWidth + (1 - side)*wallWidth*0.5f);
                Vector3 vertSideB = path.GetPoint(i) + side * localRight * Mathf.Abs(roadWidth + (1 + side)*wallWidth*0.5f);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA + localUp * wallHeight;
                verts[vertIndex + 1] = vertSideB + localUp * wallHeight;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA;
                verts[vertIndex + 3] = vertSideB;

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
            if (side == -1){
                leftWallMesh.Clear();
                leftWallMesh.vertices = verts;
                leftWallMesh.uv = uvs;
                leftWallMesh.normals = normals;
                leftWallMesh.subMeshCount = 3;
                leftWallMesh.SetTriangles(roadTriangles, 0);
                leftWallMesh.SetTriangles(underRoadTriangles, 1);
                leftWallMesh.SetTriangles(sideOfRoadTriangles, 2);
                leftWallMesh.RecalculateBounds();
            } else {
                rightWallMesh.Clear();
                rightWallMesh.vertices = verts;
                rightWallMesh.uv = uvs;
                rightWallMesh.normals = normals;
                rightWallMesh.subMeshCount = 3;
                rightWallMesh.SetTriangles(roadTriangles, 0);
                rightWallMesh.SetTriangles(underRoadTriangles, 1);
                rightWallMesh.SetTriangles(sideOfRoadTriangles, 2);
                rightWallMesh.RecalculateBounds();
            }
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
                roadHolder = new GameObject("Road");
                setupMeshHolder(roadHolder);

                leftWallHolder = new GameObject("Left_Wall");
                setupMeshHolder(leftWallHolder);
                leftWallHolder.tag = "Left Wall";

                rightWallHolder = new GameObject("Right_Wall");
                setupMeshHolder(rightWallHolder);
                rightWallHolder.tag = "Right Wall";
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            if (roadHolder == null || leftWallHolder == null || rightWallHolder == null)
            {
                roadHolder = meshHolder.transform.Find("Road").gameObject;
                leftWallHolder = meshHolder.transform.Find("Left_Wall").gameObject;
                rightWallHolder = meshHolder.transform.Find("Right_Wall").gameObject;
            }

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
                roadMeshRenderer.sharedMaterials = new Material[] { grassMaterial, roadMaterial, roadMaterial, undersideMaterial, undersideMaterial };
                roadMeshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);

                leftWallRenderer.sharedMaterials = new Material[] { wallMaterial, wallMaterial, wallMaterial };
                rightWallRenderer.sharedMaterials = new Material[] { wallMaterial, wallMaterial, wallMaterial };
            }
        }

    }
}
