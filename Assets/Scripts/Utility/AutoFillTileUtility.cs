using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Auto fill tilemap with the specified tile, in designated positions
public class AutoFillTileUtility : MonoBehaviour
{
    public Tilemap layoutTilemap;

    public TileBase replaceTile;

    public bool isRevert = true;

    public bool isRepeat = true;

    public float stepDelay = 1.5f;          // delay between each fill/un-fill action

    public float cycleInterval = 3.0f;      // interval between coonsecutive fill and un-fill action

    public Direction fillDirection = Direction.Up;

    private Tilemap selfTilemap;

    private List<Vector3Int> listFillTilePos;

    private void Awake()
    {
        selfTilemap = GetComponent<Tilemap>();
        listFillTilePos = new List<Vector3Int>();
    }

    [ContextMenu("Fill tiles")]
    private void FillTiles()
    {
        // for (int i = layoutTilemap.origin.x; i <= layoutTilemap.origin.x + layoutTilemap.size.x; ++i)
        // {
        //     for (int j = layoutTilemap.origin.y; j <= layoutTilemap.origin.y + layoutTilemap.size.y; ++j)
        //     {
        //         Vector3Int cellPosition = new Vector3Int(i, j, 0);

        //         TileBase currentTile = layoutTilemap.GetTile(cellPosition);
        //         bool toPlaceTile = (currentTile != null && currentTile.name.Equals(replaceTile.name));

        //         if (toPlaceTile)
        //         {
        //             targetTilemap.SetTile(cellPosition, replaceTile);
        //         }
        //         Debug.Log($"x: {i}, y: {j}");
        //         Debug.Log(toPlaceTile);
        //     }
        // }

        // foreach (var pos in layoutTilemap.cellBounds.allPositionsWithin)
        // {
        //     Vector3Int cellPos = new Vector3Int(pos.x, pos.y, pos.z);
        //     if (!layoutTilemap.HasTile(cellPos))
        //     {
        //         continue;
        //     }

        //     fillTilePos.Add(cellPos);
        //     selfTilemap.SetTile(cellPos, replaceTile);
        // }

        // fillTilePos.Sort(CompareCellPosY);
        // foreach (var pos in toFillTilePos)
        // {
        //     Debug.Log(pos);
        // }

        // Auto revert
        // Invoke("RevertTiles", fillDuration);

        StartCoroutine(FillTilesCoroutine());
    }

    private void RevertTiles()
    {
        foreach (var pos in listFillTilePos)
        {
            selfTilemap.SetTile(pos, null);
        }
    }

    private void UpdateFillTilePosList()
    {
        foreach (var pos in layoutTilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, pos.z);
            if (!layoutTilemap.HasTile(cellPos))
            {
                continue;
            }

            listFillTilePos.Add(cellPos);
        }

        switch (fillDirection)
        {
            case Direction.Up:
                listFillTilePos.Sort(CompareCellPosY);
                break;
            case Direction.Down:
                listFillTilePos.Sort((pos1, pos2) => -1 * CompareCellPosY(pos1, pos2));
                break;
            case Direction.Right:
                listFillTilePos.Sort(CompareCellPosX);
                break;
            case Direction.Left:
                listFillTilePos.Sort((pos1, pos2) => -1 * CompareCellPosX(pos1, pos2));
                break;
        }
    }

    private void ClearFillTilePosList()
    {
        listFillTilePos.Clear();
    }

    private static int CompareCellPosX(Vector3Int pos1, Vector3Int pos2)
    {
        return pos1.x.CompareTo(pos2.x);
    }

    private static int CompareCellPosY(Vector3Int pos1, Vector3Int pos2)
    {
        return pos1.y.CompareTo(pos2.y);
    }

    private IEnumerator FillTilesCoroutine()
    {
        UpdateFillTilePosList();

        bool isVertical = (fillDirection == Direction.Up || fillDirection == Direction.Down);

        // Fill
        float prevPosValue = isVertical ? listFillTilePos[0].y : listFillTilePos[0].x;
        for (int i = 0; i < listFillTilePos.Count; ++i)
        {
            Vector3Int currentPos = listFillTilePos[i];
            float currentPosValue = isVertical ? currentPos.y : currentPos.x;

            if (currentPosValue != prevPosValue)
            {
                yield return new WaitForSeconds(stepDelay);
                prevPosValue = currentPosValue;
            }

            selfTilemap.SetTile(currentPos, replaceTile);
        }

        // Un-fill (= revert back to normal)
        if (isRevert)
        {
            yield return new WaitForSeconds(cycleInterval);

            for (int i = listFillTilePos.Count - 1; i >= 0; --i)
            {
                Vector3Int currentPos = listFillTilePos[i];
                float currentPosValue = isVertical ? currentPos.y : currentPos.x;

                if (currentPosValue != prevPosValue)
                {
                    yield return new WaitForSeconds(stepDelay);
                    prevPosValue = currentPosValue;
                }

                selfTilemap.SetTile(currentPos, null);
            }
        }

        ClearFillTilePosList();
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}
