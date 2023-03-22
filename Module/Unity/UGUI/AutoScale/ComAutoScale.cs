namespace Module.Unity.UGUI
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ComAutoScale : MonoBehaviour
    {
        [SerializeField] Vector2 ratio;

        private Canvas canvas;
        private CanvasScaler canvasScaler;

        private Vector2Int lastScreenSize = new Vector2Int(0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();

            if(canvas ==null)
            {
                Debug.LogError("Not Found Canvase " + name);
                return;
            }

            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            if(Screen.width != lastScreenSize.x
                || Screen.height != lastScreenSize.y
                || Screen.orientation != lastOrientation)
            {
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
                lastOrientation = Screen.orientation;

                if(Screen.width > 0 && Screen.height >0)
                {
                    float aspectRatio = ratio.x / ratio.y;
                    float curAspectRatio = lastScreenSize.x / lastScreenSize.y;

                    if(aspectRatio < curAspectRatio)
                    {
                        canvasScaler.matchWidthOrHeight = 1;
                    }
                    else
                    {
                        canvasScaler.matchWidthOrHeight = 0;
                    }
                }
            }
        }
    }
}
