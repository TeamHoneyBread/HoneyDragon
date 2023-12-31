﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Core.UI
{
    public class PopupOpener : MonoBehaviour
    {
        public KeyCode Key;
        public bool isOpen = false;
        public Popup PopupPrefab;

        private Popup popup;

        private void Update()
        {
            if(Input.GetKeyDown(Key))
            {
                if(isOpen == false)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        public void Open()
        {
            if (popup != null) return;

            popup = Instantiate(PopupPrefab, transform);
            popup.OnFadeinDone = () => isOpen = true;
            popup.OnFadeoutDone = () => {
                Destroy(popup.gameObject);
                isOpen = false;
                };
            popup.transform.SetAsLastSibling();
            popup.Open();
        }

        public void Close()
        {
            popup.Close();
        }
    }
}
