using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // 플레이어 아이콘 팝업 UI
    //===========================================================
    public class InGameUI_PlayerIcon : InGamePopupUI
    {
        [SerializeField]
        private Transform parentTransform; 
        [SerializeField]
        private GameObject PlayerIconPrefab;
        [SerializeField]
        private Button closeButton;

        public override void Init()
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/PlayerIcon");

            for(int i = 0; i < sprites.Length; i++)
            {
                // 프리팹 인스턴스화
                GameObject newIcon = Instantiate(PlayerIconPrefab, parentTransform);

                // 인스턴스화된 오브젝트에 스프라이트 적용 (이미지 컴포넌트가 있다고 가정)
                newIcon.GetComponent<InGameUI_PlayerIconItem>().SetItem(sprites[i], i + 1);
                Debug.Log(sprites[i].name);
            }

            closeButton.onClick.AddListener(Close);
/*
            foreach (Sprite sprite in sprites)
            {

            }*/
        }

        public override void Open()
        {
            //throw new System.NotImplementedException();
        }

        void Close()
        {
            Destroy(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
