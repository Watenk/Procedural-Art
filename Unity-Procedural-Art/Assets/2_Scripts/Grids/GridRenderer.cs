using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class GridRenderer : IUpdateable, IGridRenderer
{
    private MeshFilter[,] meshFilters;
    private Dictionary<CellTypes, Vector2> atlasUV00 = new Dictionary<CellTypes, Vector2>();
    private Dictionary<CellTypes, Vector2> atlasUV11 = new Dictionary<CellTypes, Vector2>();
    private List<Vector2Short> changedCells = new List<Vector2Short>();

    // Cache
    private List<Vector2Short>[,] changedCellsArray;
    private Vector2Short chunkSize;
    private Vector2Short chunksAmount;
    private Vector2Short gridSize;
    private Vector2 uvFloatErrorMargin;

    // Dependencies
    private IGrid<Cell> cellGrid;

    //-----------------------------------------------

    public GridRenderer(IGrid<Cell> grid, Material atlas, Vector2Short atlasSize, Vector2Short spriteSize){
        this.cellGrid = grid;
        gridSize = grid.GridSize;
        chunkSize = Settings.Instance.DesiredChunkSize;
        uvFloatErrorMargin = Settings.Instance.UVFloatErrorMargin;

        if (gridSize.x < chunkSize.x) chunkSize.x = (short)gridSize.x;
        if (gridSize.y < chunkSize.y) chunkSize.y = (short)gridSize.y;
        chunksAmount = new Vector2Short(Mathf.CeilToInt((float)gridSize.x / (float)chunkSize.x), Mathf.CeilToInt((float)gridSize.y / (float)chunkSize.y));
        changedCellsArray = new List<Vector2Short>[chunksAmount.x ,chunksAmount.y];
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                changedCellsArray[x, y] = new List<Vector2Short>();
            }
        }

        InitAtlas(atlasSize, spriteSize);
        InitMeshes(atlas);
    }

    public void OnUpdate(){

        // // StressTest
        // for (int y = 0; y < gridSize.y; y++){
        //     for (int x = 0; x < gridSize.x; x++){
        //         gridChangedCells.Add(new Vector2Int(x, y));
        //     }
        // }

        // Sort changed Cells
        foreach (Vector2Short current in changedCells){
            
            Vector2Short chunkIndex = PosToChunkIndex(current);
            changedCellsArray[chunkIndex.x, chunkIndex.y].Add(current);
        }

        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                UpdateMesh(changedCellsArray[x, y], meshFilters[x, y] , new Vector2Short(x, y));
                changedCellsArray[x, y].Clear();
            }
        }

        changedCells.Clear();
    }

    public void Update(Vector2Short pos){
        if (!changedCells.Contains(pos)) changedCells.Add(pos);
    }

    //---------------------------------------------------------------

    private void InitAtlas(Vector2Short atlasSize, Vector2Short spriteSize){
        int cellsAmount = Enum.GetNames(typeof(CellTypes)).Length;
        int index = 0;
        for (int y = 0; y < atlasSize.y; y += spriteSize.y){
            for (int x = 0; x < atlasSize.x; x += spriteSize.x){
                if (index >= cellsAmount) { break; }
                
                // UV00 is bottom left and UV11 is top right
                Vector2 uv00 = AtlasPosToUV00(new Vector2Short(x, y), atlasSize, spriteSize);
                Vector2 uv11 = AtlasPosToUV11(new Vector2Short(x, y), atlasSize, spriteSize);

                uv00 += uvFloatErrorMargin;
                uv11 -= uvFloatErrorMargin;

                atlasUV00.Add((CellTypes)index, uv00);
                atlasUV11.Add((CellTypes)index, uv11);

                index++; 
            }
        }
    }

    private void InitMeshes(Material atlas){
        meshFilters = new MeshFilter[chunksAmount.x, chunksAmount.y];

        // Instance MeshFilter GameObjects
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                GameObject newMesh = new GameObject("MeshGrid(" + x + ", " + y + ")");
                newMesh.transform.SetParent(GameManager.Instance.transform);
                newMesh.transform.position = new Vector3(x * chunkSize.x, -y * chunkSize.y - 1, 0);

                MeshRenderer meshRenderer = newMesh.AddComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.material = atlas;
                meshFilters[x, y] = newMesh.AddComponent<MeshFilter>();
            }
        }

        // Generate Meshes
        for (int y = 0; y < chunksAmount.y; y++){
            for (int x = 0; x < chunksAmount.x; x++){
                GenerateMesh(meshFilters[x, y], chunkSize, new Vector2Short(x, y));
            }
        }
    }

    private void GenerateMesh(MeshFilter meshFilter, Vector2Short size, Vector2Short chunkIndex){
        
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

                Vector2Short chunkOrgin = CalcChunkOrgin(chunkIndex);
                CellTypes currentCell = cellGrid.Get(new Vector2Short(chunkOrgin.x + x, chunkOrgin.y + y)).CellType;

                // Vertices
                // Generates 4 vertices clockwise because a quad had 4 vertices
                verticeIndex = index * 4; // 
                vertices[verticeIndex + 0] = new Vector3(x    , -y    , 0);
                vertices[verticeIndex + 1] = new Vector3(x    , -y + 1, 0);
                vertices[verticeIndex + 2] = new Vector3(x + 1, -y + 1, 0);
                vertices[verticeIndex + 3] = new Vector3(x + 1, -y    , 0);

                // Triangles
                // Assigns the order the vertices are connected
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

    private void UpdateMesh(List<Vector2Short> changedCellsInChunk, MeshFilter meshFilter, Vector2Short chunkIndex){

        if (changedCellsInChunk.Count == 0) return;
        Vector2[] uv = meshFilter.mesh.uv;

        foreach (Vector2Short currentCellPos in changedCellsInChunk){

            Vector2Short chunkOrgin = CalcChunkOrgin(chunkIndex);
            CellTypes currentCell = cellGrid.Get(currentCellPos).CellType;
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

    private Vector2 AtlasPosToUV00(Vector2Short pos, Vector2Short atlasSize, Vector2Short spriteSize){
        float xUV = MathUtility.Map(pos.x, 0, atlasSize.x, 0, 1);
        float yUV = MathUtility.Map(pos.y + spriteSize.y, 0, atlasSize.y, 1, 0);
        return new Vector2(xUV, yUV);
    }

    private Vector2 AtlasPosToUV11(Vector2Short pos, Vector2Short atlasSize, Vector2Short spriteSize){
        float xUV = MathUtility.Map(pos.x + spriteSize.x, 0, atlasSize.x, 0, 1);
        float yUV = MathUtility.Map(pos.y, 0, atlasSize.y, 1, 0);
        return new Vector2(xUV, yUV);
    }

    private Vector2Short CalcChunkOrgin(Vector2Short chunkIndex){
        return new Vector2Short(chunkIndex.x * chunkSize.x, chunkIndex.y * chunkSize.y);
    }

    private Vector2Short PosToChunkIndex(Vector2Short pos){
        return new Vector2Short(pos.x / chunkSize.x, pos.y / chunkSize.y);
    }
}
