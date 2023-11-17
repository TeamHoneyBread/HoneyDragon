﻿using Cinemachine;
using RPG.Core.Manager;
using RPG.Core.UI;
using RPG.Util;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core.Scene
{
    public class GameScene : BaseScene
    {
        private Player currentPlayer;
        public Player Player { get 
            { 
                if(currentPlayer == null)
                {
                    Init();
                    initialized = true;
                }
                return currentPlayer;
            }
        }
        [field: SerializeField] public PlayerExpPresenter ExpPresenter { get; set; }    
        [field: SerializeField] public Camera MainCamera { get; set; }
        // [field: SerializeField] public MinimapCameraMovement MinimapCamera { get; set; }
        [field: SerializeField] public CinemachineVirtualCamera MainVirtualCam { get; set; }

        private MonsterSpawner monsterSpawner;
        public MonsterSpawner MonsterSpawner => monsterSpawner;

        public event Action<float> OnTimeChanged;
        public float GameElapsedTime => gameElapsedTime;
        private float gameElapsedTime = 0f;
        
        public override void Clear()
        {
            Managers.Instance.Game.GameScene = null;
            Destroy(Player.gameObject);
        }

        public override void Init()
        {
            if (initialized == true) return;
            initialized = true;
            Managers.Instance.Game.GameScene = this;
            monsterSpawner = FindObjectOfType<MonsterSpawner>();
            currentPlayer = CreatePlayer(Managers.Instance.Game.selectedCharacterId);

            // r
            Managers.Instance.Stage.InitStage(1);
            monsterSpawner.OnStageChanged(1);
            monsterSpawner.SetMapBoundData(Managers.Instance.Stage.CurrentStage.Grid);

            // UI Scene 로딩
            SceneManager.LoadScene((int)SceneType.UI, LoadSceneMode.Additive);

            Managers.Instance.Sound.PlaySound(SoundType.BGM, "Sound/BGM");

            // 2초 뒤 생성
            OnTimeChanged += monsterSpawner.OnTimeChanged;
            monsterSpawner.SpawnTask(2000).Forget();
        }

        private Player CreatePlayer(int playerModelId)
        {
            var playerData = Managers.Instance.Data.PlayerDataDict[playerModelId];
            var playerExpTable = Managers.Instance.Data.PlayerExpDataDict;
            var playerPrefab = ResourceCache.Load<Player>(playerData.PrefabPath);

            var currentPlayer = MonoBehaviour.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            currentPlayer.InitializePlayer(playerData, playerExpTable);


            MainCamera = Camera.main;
            var vcams = MonoBehaviour.FindObjectsOfType<CinemachineVirtualCamera>(true);
            MainVirtualCam = vcams[0];
            vcams[0].gameObject.SetActive(true);
            vcams[1].gameObject.SetActive(false);

            MainVirtualCam.transform.position = currentPlayer.transform.position + (Vector3.up * 30f);
            MainVirtualCam.Follow = currentPlayer.transform;

            //MinimapCamera.SetPlayer(currentPlayer.transform);
            return currentPlayer;
        }

        private void Update()
        {
            gameElapsedTime = Time.timeSinceLevelLoad;
            OnTimeChanged?.Invoke(gameElapsedTime);
        }
    }
}
