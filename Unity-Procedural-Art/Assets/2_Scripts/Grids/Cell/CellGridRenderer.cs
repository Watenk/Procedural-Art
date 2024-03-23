using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class CellGridRenderer : IUpdateable
{
    private MeshFilter[,] meshFilters;
    private Dictionary<Cell, Vector2> atlasUV00 = new Dictionary<Cell, Vector2>();
    private Dictionary<Cell, Vector2> atlasUV11 = new Dictionary<Cell, Vector2>();

    // Cache
    private List<Vector2Int>[,] changedCells;
    private Vector2Int chunkSize;
    private Vector2Int chunksAmount;
    private Vector2Int gridSize;

    // Dependencies
    private ICellGridDrawable changedCellsGrid;
    private ICellGrid cellGrid;

    //-----------------------------------------------

    public CellGridRenderer(){
        cellGrid = GameManager.GetService<CellGridManager>();
        changedCellsGrid = GameManager.GetService<CellGridManager>();
        gridSize = cellGrid.GridSize;
        chunkSize = Settings.Instance.DesiredChunkSize;

        if (gridSize.x < chunkSize.x) chunkSize.x = gridSize.x;
        if (gridSize.y < chunkSize.y) chunkSize.y = gridSize.y;
        chunksAmount = new Vector2Int(Mathf.CeilToInt((float)gridSize.x / (float)chunkSize.x), Mathf.CeilToInt((float)gridSize.y / (float)chunkSize.y));
        changedCells = new List<Vector2Int>[chunksAmount.x ,chunksAmount.y];
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                changedCells[x, y] = new List<Vector2Int>();
            }
        }

        InitAtlas();
        InitMeshes();
    }

    public void OnUpdate(){

        // // StressTest
        // for (int y = 0; y < gridSize.y; y++){
        //     for (int x = 0; x < gridSize.x; x++){
        //         gridChangedCells.Add(new Vector2Int(x, y));
        //     }
        // }

        List<Vector2Int> gridChangedCells = changedCellsGrid.GetChangedCells();

        // Sort changed Cells
        foreach (Vector2Int current in gridChangedCells){
            
            Vector2Int chunkIndex = PosToChunkIndex(current);
            changedCells[chunkIndex.x, chunkIndex.y].Add(current);
        }

        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                UpdateMesh(changedCells[x, y], meshFilters[x, y] , new Vector2Int(x, y));
                changedCells[x, y].Clear();
            }
        }

        changedCellsGrid.ClearChangedCells();
    }

    //---------------------------------------------------------------

    private void InitAtlas(){
        int cellsAmount = Enum.GetNames(typeof(Cell)).Length;
        int index = 0;
        for (int y = 0; y < Settings.Instance.AtlasSize.y; y += Settings.Instance.SpriteTextureSize.y){
            for (int x = 0; x < Settings.Instance.AtlasSize.x; x += Settings.Instance.SpriteTextureSize.x){
                if (index >= cellsAmount) { break; }
                
                Vector2 uv00 = AtlasPosToUV00(new Vector2Int(x, y));
                Vector2 uv11 = AtlasPosToUV11(new Vector2Int(x, y));

                uv00 += Settings.Instance.UVFloatErrorMargin;
                uv11 -= Settings.Instance.UVFloatErrorMargin;

                atlasUV00.Add((Cell)index, uv00);
                atlasUV11.Add((Cell)index, uv11);

                index++; 
            }
        }
    }

    private void InitMeshes(){
        meshFilters = new MeshFilter[chunksAmount.x, chunksAmount.y];

        // Instance MeshFilter GameObjects
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                GameObject newMesh = new GameObject("MeshGrid(" + x + ", " + y + ")");
                newMesh.transform.SetParent(GameManager.Instance.transform);
                newMesh.transform.position = new Vector3(x * chunkSize.x, -y * chunkSize.y - 1, 0);

                MeshRenderer meshRenderer = newMesh.AddComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.material = Settings.Instance.Atlas;
                meshFilters[x, y] = newMesh.AddComponent<MeshFilter>();
            }
        }

        // Generate Meshes
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                GenerateMesh(meshFilters[x, y], chunkSize, new Vector2Int(x, y));
            }
        }
    }

    private void GenerateMesh(MeshFilter meshFilter, Vector2Int size, Vector2Int chunkIndex){
        
        Mesh mesh = new Mesh();

        // Data
        Vector3[] vertices = new Vector3[4 * size.x * size.y]; // Points in the grid (4 because a quad has 4 vertices)
        Vector2[] uv = new Vector2[4 * size.x * size.y]; // Part of texture that matches with vertice (uv lenght is the same is vertices lenght)
        int[] triangles = new int[6 * size.x * size.y]; // Defines the index of the vertices (6 because a quad had 6 sides (2 triangles))

        int index = 0;
        int verticeIndex;
        int triangleIndex;
        for (int y = 0; y < size.y; y++){
            for (int x = 0; x < size.x; x++){

                Vector2Int chunkOrgin = CalcChunkOrgin(chunkIndex);
                ref Cell currentCell = ref cellGrid.GetCell(new Vector2Int(chunkOrgin.x + x, chunkOrgin.y + y));

                // Vertices
                // This generates 4 vertices clockwise because a quad had 4 vertices
                // Vertices start rendering from the bottom left
                verticeIndex = index * 4; // 
                vertices[verticeIndex + 0] = new Vector3(x    , -y    , 0);
                vertices[verticeIndex + 1] = new Vector3(x    , -y + 1, 0);
                vertices[verticeIndex + 2] = new Vector3(x + 1, -y + 1, 0);
                vertices[verticeIndex + 3] = new Vector3(x + 1, -y    , 0);

                // Triangles
                // This assigns the order the vertices are connected
                triangleIndex = index * 6;
                // Triangle 1
                triangles[triangleIndex + 0] = verticeIndex + 0;
                triangles[triangleIndex + 1] = verticeIndex + 1;
                triangles[triangleIndex + 2] = verticeIndex + 2;
                // Triangle 2
                triangles[triangleIndex + 3] = verticeIndex + 0;
                triangles[triangleIndex + 4] = verticeIndex + 2;
                triangles[triangleIndex + 5] = verticeIndex + 3;

                // UV
                // UV's start rendering from the bottom left (UV00 is bottom left and UV11 is top right)
                atlasUV00.TryGetValue(currentCell, out Vector2 uv00);
                atlasUV11.TryGetValue(currentCell, out Vector2 uv11);
                uv[verticeIndex + 0] = new Vector2(uv00.x, uv00.y);
                uv[verticeIndex + 1] = new Vector2(uv11.x, uv00.y);
                uv[verticeIndex + 2] = new Vector2(uv00.x, uv11.y);
                uv[verticeIndex + 3] = new Vector2(uv11.x, uv11.y);

                index++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
    }

    private void UpdateMesh(List<Vector2Int> changedCellsInChunk, MeshFilter meshFilter, Vector2Int chunkIndex){

        if (changedCellsInChunk.Count == 0) return;
        Vector2[] uv = meshFilter.mesh.uv;

        foreach (Vector2Int currentCellPos in changedCellsInChunk){

            Vector2Int chunkOrgin = CalcChunkOrgin(chunkIndex);
            ref Cell currentCell = ref cellGrid.GetCell(currentCellPos);
            int index = (currentCellPos.x - chunkOrgin.x) + (currentCellPos.y - chunkOrgin.y) * chunkSize.y;
            int verticeIndex = 4 * index;

            // UV
            atlasUV00.TryGetValue(currentCell, out Vector2 uv00);
            atlasUV11.TryGetValue(currentCell, out Vector2 uv11);
            uv[verticeIndex + 0] = new Vector2(uv00.x, uv00.y);
            uv[verticeIndex + 1] = new Vector2(uv11.x, uv00.y);
            uv[verticeIndex + 2] = new Vector2(uv00.x, uv11.y);
            uv[verticeIndex + 3] = new Vector2(uv11.x, uv11.y);
        }
        meshFilter.mesh.uv = uv;
    }

    private Vector2 AtlasPosToUV00(Vector2Int pos){
        float xUV = MathUtility.Map(pos.x, 0, Settings.Instance.AtlasSize.x, 0, 1);
        float yUV = MathUtility.Map(pos.y + Settings.Instance.SpriteTextureSize.y, 0, Settings.Instance.AtlasSize.y, 1, 0);
        return new Vector2(xUV, yUV);
    }

    private Vector2 AtlasPosToUV11(Vector2Int pos){
        float xUV = MathUtility.Map(pos.x + Settings.Instance.SpriteTextureSize.x, 0, Settings.Instance.AtlasSize.x, 0, 1);
        float yUV = MathUtility.Map(pos.y, 0, Settings.Instance.AtlasSize.y, 1, 0);
        return new Vector2(xUV, yUV);
    }

    private Vector2Int CalcChunkOrgin(Vector2Int chunkIndex){
        return new Vector2Int(chunkIndex.x * chunkSize.x, chunkIndex.y * chunkSize.y);
    }

    private Vector2Int PosToChunkIndex(Vector2Int pos){
        int xIndex = pos.x / chunkSize.x;
        int yIndex = pos.y / chunkSize.y;
        return new Vector2Int(xIndex, yIndex);
    }
}
