using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

using MiniGolf.Terrain.Data;

namespace MiniGolf.Terrain
{
    [SelectionBase]
    [RequireComponent(typeof(MeshCollider))]
    public class HoleGenerator : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private Tile[] startTilePrefabs;
        [SerializeField] private Tile[] tilePrefabs;
        [SerializeField] private HoleTile[] holeTilePrefabs;
        [Space]
        [SerializeField] private Hole hole;
        [SerializeField][Min(0f)] private float spawnInterval;
        [SerializeField] private MeshCollider wallCollider;
        [SerializeField] private bool usingWalls;
        [Space]
        [SerializeField] private Gradient cellGizmoColorGradient;
        [SerializeField] private bool showUsedCells;
        [Space]
        [SerializeField][Min(0f)] private float maxMergeDistance = 0.001f;
        [SerializeField][Min(0f)] private float maxMergeAngle = 1f;
        [SerializeField] private Gradient mergedVertexGizmoColorGradient;
        [SerializeField][Range(0f, 1f)] private float mergedNormalGizmoLength = 0.1f;
        [SerializeField] private bool showMergedVertices;
        [Space]
        [SerializeField] private Color boundsGizmoColor = Color.white;
        [SerializeField] private bool showBounds;
        [Space]
        public UnityEvent<HoleTile> OnGenerate;

        private readonly List<Tile> tileInstances = new();
        private readonly List<Vector3Int> usedCells = new();
        private readonly List<Vector3> mergedVertices = new();
        private MeshCollider meshCollider;
        private Coroutine generationRoutine;

        private MeshCollider MeshCollider => meshCollider ? meshCollider : GetComponent<MeshCollider>();
        private Tile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        public Hole HoleData => hole;
        public HoleTile CurrentHoleTile { get; private set; }
        public Bounds HoleBounds => MeshCollider.bounds;

        private void Awake()
        {
            if (wallCollider == null) Debug.LogError($"{nameof(wallCollider)} not assigned");
        }

        #region Generate
        public void Generate() => Generate(hole);

        public void Generate(Hole hole, bool withWalls = true)
        {
            if (startTilePrefabs.Length == 0 || tilePrefabs.Length == 0 || holeTilePrefabs.Length == 0)
            {
                Debug.LogError($"{nameof(startTilePrefabs)}, {nameof(tilePrefabs)}, or {nameof(holeTilePrefabs)} is empty");
                return;
            }

            if (generationRoutine != null) StopCoroutine(generationRoutine);
            Clear();

            if (!Application.isPlaying) withWalls = usingWalls;
            generationRoutine = StartCoroutine(GenerationRoutine(hole, withWalls));
        }

        public void Clear()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var children = transform.Cast<Transform>().ToArray();
            foreach (var child in children)
            {
                var tile = child.GetComponent<Tile>();
                if (tile) contextDestroy(tile.gameObject);
            }
            tileInstances.Clear();
            usedCells.Clear();
            mergedVertices.Clear();
            CurrentHoleTile = null;
            MeshCollider.sharedMesh = null;
            wallCollider.sharedMesh = null;
        }

        private IEnumerator GenerationRoutine(Hole hole, bool withWalls)
        {
            System.Random rng = new(hole.Seed);

            for (int tileIndex = 0; tileIndex < hole.TileCount; tileIndex++)
            {
                Tile[] tilePrefabOptions = GetTileOptionsFor(hole, tileIndex);
                var randomIndex = rng.Next(tilePrefabOptions.Length);
                var tile = SpawnTile(tilePrefabOptions[randomIndex], withWalls);

                if (tile is HoleTile holeTile) CurrentHoleTile = holeTile;

                if (Application.isPlaying && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            }

            MeshCollider.sharedMesh = CalculateGroundMesh();
            wallCollider.sharedMesh = withWalls ? CalculateWallMesh() : null;
            OnGenerate.Invoke(CurrentHoleTile);

            generationRoutine = null;
        }

        private Tile[] GetTileOptionsFor(Hole hole, int index)
        {
            if (index < 0 || index >= hole.TileCount) Debug.LogError($"Index '{index}' out of range [0, {hole.TileCount - 1}]");

            if (index == 0) return startTilePrefabs;
            else if (index == hole.TileCount - 1) return holeTilePrefabs;
            else return tilePrefabs;
        }
        #endregion

        #region Tile Spawning
        private Tile SpawnTile(Tile tilePrefab, bool withWall)
        {
            var (newCell, rotation) = GetCellAndRotationFor(tilePrefab);
            var newTile = ContextInstantiate(tilePrefab, CellToPosition(newCell), rotation, transform);
            newTile.SetWall(withWall);
            tileInstances.Add(newTile);

            return newTile;
        }

        private (Vector3Int, Quaternion) GetCellAndRotationFor(Tile tilePrefab)
        {
            if (usedCells.Count == 0)
            {
                usedCells.Add(Vector3Int.zero);
                return (Vector3Int.zero, Quaternion.identity);
            }

            var startCell = LocalCellToCell(LastCell, LastTile.transform.localRotation, LastTile.OutDirection);
            var rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(startCell - LastCell, Vector3.up));
            var availableCellCount = 0;
            foreach (var localCell in tilePrefab.LocalCells)
            {
                var endCell = LocalCellToCell(startCell, rotation, localCell);
                if (usedCells.Contains(endCell)) break;

                availableCellCount++;
                usedCells.Add(endCell);
            }

            if (availableCellCount < tilePrefab.LocalCells.Length) throw new Exception($"No cell found from {LastCell} for {tilePrefab.name}");

            return (startCell, rotation);
        }

        private Tile ContextInstantiate(Tile original, Vector3 position, Quaternion rotation, Transform parent)
        {
            Func<UnityEngine.Object, Transform, UnityEngine.Object> instantiate = Instantiate;
#if UNITY_EDITOR
            if (!Application.isPlaying) instantiate = PrefabUtility.InstantiatePrefab;
#endif

            var newTile = (Tile)instantiate(original, parent);
            newTile.transform.SetPositionAndRotation(position, rotation);

            return newTile;
        }
        #endregion

        #region Mesh Calculation
        private Mesh CalculateGroundMesh() => CalculateCombinedMesh("Ground Collision Mesh", tile => tile.GroundMeshFilters);

        private Mesh CalculateWallMesh() => CalculateCombinedMesh("Wall Collision Mesh", tile => tile.WallMeshFilters);

        private Mesh CalculateCombinedMesh(string meshName, Func<Tile, MeshFilter[]> GetMeshes)
        {
            var meshCount = tileInstances.Sum(tile => GetMeshes(tile).Length);
            var combines = new CombineInstance[meshCount];
            int combineIndex = 0;
            foreach (var tile in tileInstances)
            {
                var meshfilters = GetMeshes(tile);
                foreach (var meshFilter in meshfilters)
                {
                    combines[combineIndex].transform = transform.localToWorldMatrix.inverse * meshFilter.transform.localToWorldMatrix;
                    combines[combineIndex].mesh = meshFilter.sharedMesh;
                    combineIndex++;
                }
            }

            var combinedMesh = new Mesh();
            combinedMesh.name = meshName;
            combinedMesh.CombineMeshes(combines, true);
            MergeVertices(combinedMesh);

            return combinedMesh;
        }

        private void MergeVertices(Mesh mesh)
        {
            Dictionary<int, int> vertexIndexMap = new();
            List<Vector3> newVertices = new(), newNormals = new();

            var vertices = mesh.vertices;
            var normals = mesh.normals;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertexIndexMap.ContainsKey(i)) continue;
                if (i != newVertices.Count) vertexIndexMap.Add(i, newVertices.Count);

                if (!VertexIsCentral(vertices[i]))
                {
                    for (int j = i + 1; j < vertices.Length; j++)
                    {
                        if (vertexIndexMap.ContainsKey(j)) continue;
                        if (Vector3.Distance(vertices[i], vertices[j]) > maxMergeDistance) continue;
                        if (Vector3.Angle(normals[i], normals[j]) > maxMergeAngle) continue;

                        vertexIndexMap.Add(j, newVertices.Count);

                        mergedVertices.Add(vertices[i]);
                        mergedVertices.Add(normals[i]);
                    }
                }

                newVertices.Add(vertices[i]);
                newNormals.Add(normals[i]);
            }

            var triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i++)
            {
                if (vertexIndexMap.TryGetValue(triangles[i], out int newIndex)) triangles[i] = newIndex;
            }

            mesh.triangles = triangles;
            mesh.vertices = newVertices.ToArray();
            mesh.normals = newNormals.ToArray();
        }

        private bool VertexIsCentral(Vector3 vertex)
        {
            var xTiles = vertex.x / Tile.SCALE.x;
            var zTiles = vertex.z / Tile.SCALE.z;

            var xIsCentral = Mathf.Abs(Mathf.Round(xTiles) - xTiles) < .5f - maxMergeDistance;
            var zIsCentral = Mathf.Abs(Mathf.Round(zTiles) - zTiles) < .5f - maxMergeDistance;

            return xIsCentral && zIsCentral;
        }
        #endregion

        private Vector3Int LocalCellToCell(Vector3Int baseCell, Quaternion baseRotation, Vector3Int localCell)
        {
            return baseCell + Vector3Int.RoundToInt(baseRotation * localCell);
        }
        private Vector3 CellToPosition(Vector3Int cell)
        {
            return transform.position + transform.rotation * Vector3.Scale(Tile.SCALE, cell);
        }

        [ContextMenu("Show Walls")]
        public void ActivateWalls() => SetWalls(true);

        [ContextMenu("Hide Walls")]
        public void DeactivateWalls() => SetWalls(false);

        public void SetWalls(bool active)
        {
            foreach (var tile in tileInstances) tile.SetWall(active);
        }

        private void OnDrawGizmosSelected()
        {
            if (cellGizmoColorGradient == null)
            {
                Debug.LogWarning($"{nameof(cellGizmoColorGradient)} not set");
            }
            else if (showUsedCells && usedCells.Count > 1)
            {
                for (int i = 0; i < usedCells.Count; i++)
                {
                    Gizmos.color = cellGizmoColorGradient.Evaluate(i / (usedCells.Count - 1f));
                    Gizmos.DrawWireCube(CellToPosition(usedCells[i]), Tile.SCALE);
                }
            }

            if (mergedVertexGizmoColorGradient == null)
            {
                Debug.LogWarning($"{nameof(mergedVertexGizmoColorGradient)} not set");
            }
            else if (showMergedVertices)
            {
                for (int i = 0; i < mergedVertices.Count; i += 2)
                {
                    Gizmos.color = mergedVertexGizmoColorGradient.Evaluate(i / (mergedVertices.Count - 1f));
                    var start = mergedVertices[i];
                    var normalEnd = start + mergedNormalGizmoLength * mergedVertices[i + 1];
                    Gizmos.DrawSphere(start, maxMergeDistance);
                    Gizmos.DrawLine(start, normalEnd);
                }
            }

            if (showBounds && MeshCollider.sharedMesh)
            {
                var bounds = MeshCollider.sharedMesh.bounds;
                Gizmos.color = boundsGizmoColor;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}