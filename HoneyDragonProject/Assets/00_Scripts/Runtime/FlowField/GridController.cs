﻿using Cysharp.Threading.Tasks;
using RPG.Core.Manager;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class GridController : MonoBehaviour
{
    public Vector2Int GridStartPoint;
    public Vector2Int GridSize;
    public float CellRadius = 0.5f;
    public FlowField CurFlowField;
    public Cell prevDestCell;
    private GridDebug gridDebug;

    private void Awake()
    {
        gridDebug = GetComponent<GridDebug>();
    }

    private void Start()
    {
        CreateGrid().Forget();
    }

    private void InitializedFlowField()
    {
        CurFlowField = new FlowField(CellRadius, GridSize, GridStartPoint);
        CurFlowField.CreateGrid();
        gridDebug.SetFlowField(CurFlowField, GridStartPoint);
    }

    private void UpdateField(Cell destinationCell)
    {
       if (destinationCell.IsObstacle) return;

        CurFlowField.CreateCostField();
        prevDestCell = destinationCell;
        gridDebug.SelectedCell = destinationCell;

        CurFlowField.CreateIntegrationField(destinationCell);
        CurFlowField.CreateFlowField();
    }

   

    async UniTaskVoid CreateGrid()
    {
        InitializedFlowField();
        while (true)
        {
            if (Managers.Instance.Game.GameScene && Managers.Instance.Game.GameScene.Player)
            {
                Cell destinationCell = CurFlowField.GetCellFromWorldPos(Managers.Instance.Game.GameScene.Player.transform.position);
                if (prevDestCell == destinationCell)
                {
                    await UniTask.Yield();
                }
                else
                {
                    UpdateField(destinationCell);
                    await UniTask.Delay(200, false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                }
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }
}
