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

    public bool isProgressing;

    public bool hasRequestToStop;

    private Tilemap selfTilemap;

    private List<Vector3Int> listFillTilePos;

    private WaitForSeconds cycleWaitTimer;

    private WaitForSeconds stepWaitTimer;

    public void FillTiles()
    {
        if (isProgressing) return;
        
        isProgressing = true;
        StartCoroutine(FillTilesCoroutine());
    }

    // Force stop filling (tiles) at the end of the current cycle
    public void RequestStopFilling()
    {
        if (!isProgressing || hasRequestToStop) return;

        hasRequestToStop = true;
    }

    private void Awake()
    {
        selfTilemap = GetComponent<Tilemap>();
        listFillTilePos = new List<Vector3Int>();

        cycleWaitTimer = new WaitForSeconds(cycleInterval);
        stepWaitTimer = new WaitForSeconds(stepDelay);

        isProgressing = false;
        hasRequestToStop = false;
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

        do
        {
            // Fill
            float prevPosValue = isVertical ? listFillTilePos[0].y : listFillTilePos[0].x;
            for (int i = 0; i < listFillTilePos.Count; ++i)
            {
                Vector3Int currentPos = listFillTilePos[i];
                float currentPosValue = isVertical ? currentPos.y : currentPos.x;

                if (currentPosValue != prevPosValue)
                {
                    yield return stepWaitTimer;
                    prevPosValue = currentPosValue;
                }

                if (!selfTilemap.HasTile(currentPos))
                {
                    selfTilemap.SetTile(currentPos, replaceTile);
                }
            }

            // Un-fill (= revert back to normal)
            if (isRevert)
            {
                yield return cycleWaitTimer;

                for (int i = listFillTilePos.Count - 1; i >= 0; --i)
                {
                    Vector3Int currentPos = listFillTilePos[i];
                    float currentPosValue = isVertical ? currentPos.y : currentPos.x;

                    if (currentPosValue != prevPosValue)
                    {
                        yield return stepWaitTimer;
                        prevPosValue = currentPosValue;
                    }

                    selfTilemap.SetTile(currentPos, null);
                }
            }

            if (hasRequestToStop)
            {
                hasRequestToStop = false;
                break;
            }

            yield return cycleWaitTimer;
            
        } while (isRepeat);

        ClearFillTilePosList();

        isProgressing = false;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}
