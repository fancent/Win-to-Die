using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = .4f;
        public AnimationCurve roadCurve = AnimationCurve.Linear(0f, 1.0f, 0.0f, 1.0f);
        public float grassWidth = .2f;
        public AnimationCurve grassCurve = AnimationCurve.Linear(0f, 1.0f, 0.0f, 1.0f);
        [Range(0, 2f)]
        public float roadThickness = .15f;
        public bool flattenSurface;

        [Header("Wall settings")]
        public float wallWidth = .4f;
        public float wallHeight = .2f;
        public float invisibleHeight = .8f;

        [Header("Material settings")]
        public Material grassMaterial;
        public Material roadMaterial;
        public Material undersideMaterial;
        public Material wallMaterial;
        public float textureTiling = 10;

        [SerializeField]
        public GameObject meshHolder;

        GameObject grassHolder;
        MeshFilter grassMeshFilter;
        MeshRenderer grassMeshRenderer;
        Mesh grassMesh;

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

        GameObject invisibleHolder;
        Mesh invisibleMesh;
        MeshFilter invisibleFilter;
        MeshRenderer invisibleRenderer;


        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateRoadMesh();
                CreateGrassMesh();
                CreateWall("left", leftWallMesh);
                CreateWall("right", rightWallMesh);
                CreateInvisible();
            }
        }

        void CreateRoadMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 10];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] leftRoadTriangles = new int[numTris * 3];
            int[] rightRoadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

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
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth) * roadCurve.Evaluate(path.times[i]);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth) * roadCurve.Evaluate(path.times[i]);

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

                // Add the grass locations (wont be filled in by this function)
                verts[vertIndex + 8] = path.GetPoint(i) - localRight * grassCurve.Evaluate(path.times[i]) * Mathf.Abs(grassWidth);
                verts[vertIndex + 9] = path.GetPoint(i) + localRight * grassCurve.Evaluate(path.times[i]) * Mathf.Abs(grassWidth);

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
                    for (int j = 0; j < leftRoadMap.Length; j++)
                    {
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
            roadMesh.subMeshCount = 4;
            roadMesh.SetTriangles(leftRoadTriangles, 0);
            roadMesh.SetTriangles(rightRoadTriangles, 1);
            roadMesh.SetTriangles(underRoadTriangles, 2);
            roadMesh.SetTriangles(sideOfRoadTriangles, 3);
            roadMesh.RecalculateBounds();
        }

        void CreateGrassMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 4];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] grassTriangles = new int[numTris * 3];

            int vertIndex = 0;
            int triIndex = 0;
            float dist = 1.0f;

            int[] grassMap = {0, 2, 1, 1, 2, 3};

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                dist = grassCurve.Evaluate(path.times[i]) * Mathf.Abs(grassWidth);
                verts[vertIndex + 0] = path.GetPoint(i) - localRight * dist;
                verts[vertIndex + 1] = path.GetPoint(i) + localRight * dist;

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < grassMap.Length; j++)
                    {
                        grassTriangles[triIndex + j] = vertIndex + grassMap[j] % verts.Length;
                    }
                }
                vertIndex += 2;
                triIndex += 6;
            }

            grassMesh.Clear();
            grassMesh.vertices = verts;
            grassMesh.uv = uvs;
            grassMesh.normals = normals;
            grassMesh.subMeshCount = 1;
            grassMesh.SetTriangles(grassTriangles, 0);
            grassMesh.RecalculateBounds();
        }


        void CreateWall(string left_or_right, Mesh m)
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
            int[] topTriangles = new int[numTris * 3];
            int[] underTriangles = new int[numTris * 3];
            int[] sideTriangles = new int[numTris * 2 * 3];

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
                Vector3 vertSideA = path.GetPoint(i) + side * localRight * Mathf.Abs(roadWidth + (1 - side)*wallWidth*0.5f) * roadCurve.Evaluate(path.times[i]);
                Vector3 vertSideB = path.GetPoint(i) + side * localRight * Mathf.Abs(roadWidth + (1 + side)*wallWidth*0.5f) * roadCurve.Evaluate(path.times[i]);

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
                        topTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 6;
            }
            m.Clear();
            m.vertices = verts;
            m.uv = uvs;
            m.normals = normals;
            m.subMeshCount = 3;
            m.SetTriangles(topTriangles, 0);
            m.SetTriangles(underTriangles, 1);
            m.SetTriangles(sideTriangles, 2);
            m.RecalculateBounds();
        }

        void CreateInvisible()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] topTriangles = new int[numTris * 3 * 2];
            int[] leftTriangles = new int[numTris * 3 * 2];
            int[] rightTriangles = new int[numTris * 3 * 2];

            int vertIndex = 0;
            int triIndex = 0;

            int[] topMap = {0, 8, 1, 1, 8, 9, 4, 5, 12, 5, 13, 12};
            int[] leftMap = {0, 2, 8, 2, 10, 8, 4, 14, 6, 4, 12, 14};
            int[] rightMap = {1, 11, 3, 1, 9, 11, 5, 7, 15, 5, 15, 13};

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // vertSideA is the BOTTOMLEFT of the wall, vertSideB is the BOTTOMRIGHT.
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth) * roadCurve.Evaluate(path.times[i]);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth) * roadCurve.Evaluate(path.times[i]);

                // Add top
                verts[vertIndex + 0] = vertSideA + localUp * (wallHeight + invisibleHeight);
                verts[vertIndex + 1] = vertSideB + localUp * (wallHeight + invisibleHeight);
                // Add bottom
                verts[vertIndex + 2] = vertSideA + localUp * wallHeight;
                verts[vertIndex + 3] = vertSideB + localUp * wallHeight;

                // Duplicate for shading bothsides
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Roof normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localRight;
                normals[vertIndex + 3] = localRight;
                normals[vertIndex + 4] = normals[vertIndex + 0];
                normals[vertIndex + 5] = normals[vertIndex + 1];
                normals[vertIndex + 6] = normals[vertIndex + 2];
                normals[vertIndex + 7] = normals[vertIndex + 3];

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < topMap.Length; j++)
                    {
                        topTriangles[triIndex + j] = (vertIndex + topMap[j]) % verts.Length;
                        leftTriangles[triIndex + j] =  (vertIndex + leftMap[j]) % verts.Length;
                        rightTriangles[triIndex + j] =  (vertIndex + rightMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 12;
            }
            invisibleMesh.Clear();
            invisibleMesh.vertices = verts;
            invisibleMesh.uv = uvs;
            invisibleMesh.normals = normals;
            invisibleMesh.subMeshCount = 3;
            invisibleMesh.SetTriangles(topTriangles, 0);
            invisibleMesh.SetTriangles(leftTriangles, 1);
            invisibleMesh.SetTriangles(rightTriangles, 2);
            invisibleMesh.RecalculateBounds();
        }

        void setupMeshHolder(GameObject holder)
        {
            holder.transform.parent = meshHolder.transform;
            holder.gameObject.AddComponent<MeshFilter>();
            holder.gameObject.AddComponent<MeshRenderer>();
            holder.gameObject.AddComponent<MeshCollider>();
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
                grassHolder = new GameObject("Grass");
                setupMeshHolder(grassHolder);

                roadHolder = new GameObject("Road");
                setupMeshHolder(roadHolder);

                leftWallHolder = new GameObject("LeftWall");
                setupMeshHolder(leftWallHolder);

                rightWallHolder = new GameObject("RightWall");
                setupMeshHolder(rightWallHolder);

                invisibleHolder = new GameObject("Invisible");
                setupMeshHolder(invisibleHolder);

            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            if (grassHolder == null || roadHolder == null || leftWallHolder == null || rightWallHolder == null || invisibleHolder == null)
            {
                grassHolder = meshHolder.transform.Find("Grass").gameObject;
                roadHolder = meshHolder.transform.Find("Road").gameObject;
                leftWallHolder = meshHolder.transform.Find("LeftWall").gameObject;
                rightWallHolder = meshHolder.transform.Find("RightWall").gameObject;
                invisibleHolder = meshHolder.transform.Find("Invisible").gameObject;
            }

            grassMeshRenderer = grassHolder.GetComponent<MeshRenderer>();
            grassMeshFilter = grassHolder.GetComponent<MeshFilter>();
            MeshCollider grassCollider = grassHolder.GetComponent<MeshCollider>();

            roadMeshRenderer = roadHolder.GetComponent<MeshRenderer>();
            roadMeshFilter = roadHolder.GetComponent<MeshFilter>();
            MeshCollider roadCollider = roadHolder.GetComponent<MeshCollider>();

            leftWallRenderer = leftWallHolder.GetComponent<MeshRenderer>();
            leftWallFilter = leftWallHolder.GetComponent<MeshFilter>();
            MeshCollider leftWallCollider = leftWallHolder.GetComponent<MeshCollider>();

            rightWallRenderer = rightWallHolder.GetComponent<MeshRenderer>();
            rightWallFilter = rightWallHolder.GetComponent<MeshFilter>();
            MeshCollider rightWallCollider = rightWallHolder.GetComponent<MeshCollider>();


            invisibleRenderer = invisibleHolder.GetComponent<MeshRenderer>();
            invisibleFilter = invisibleHolder.GetComponent<MeshFilter>();
            MeshCollider invisibleCollider = invisibleHolder.GetComponent<MeshCollider>();

            if (grassMesh == null)
            {
                grassMesh = new Mesh();
            }

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

            if (invisibleMesh == null)
            {
                invisibleMesh = new Mesh();
            }

            grassMeshFilter.sharedMesh = grassMesh;
            roadMeshFilter.sharedMesh = roadMesh;
            leftWallFilter.sharedMesh = leftWallMesh;
            rightWallFilter.sharedMesh = rightWallMesh;
            invisibleFilter.sharedMesh = invisibleMesh;

            grassCollider.sharedMesh = grassMesh;
            roadCollider.sharedMesh = roadMesh;
            leftWallCollider.sharedMesh = leftWallMesh;
            rightWallCollider.sharedMesh = rightWallMesh;
            invisibleCollider.sharedMesh = invisibleMesh;

        }

        void AssignMaterials()
        {
            if (roadMaterial != null && undersideMaterial != null)
            {   
                grassMeshRenderer.sharedMaterials = new Material[] {grassMaterial};

                roadMeshRenderer.sharedMaterials = new Material[] { roadMaterial, roadMaterial, undersideMaterial, undersideMaterial };
                roadMeshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
                roadMeshRenderer.sharedMaterials[1].mainTextureScale = new Vector3(1, textureTiling);

                leftWallRenderer.sharedMaterials = new Material[] { wallMaterial, wallMaterial, wallMaterial };
                rightWallRenderer.sharedMaterials = new Material[] { wallMaterial, wallMaterial, wallMaterial };

                invisibleRenderer.sharedMaterials = new Material[] { grassMaterial, grassMaterial, grassMaterial };
            }
        }

    }
}