using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WebView : MonoBehaviour
{
    private WebViewObject webViewObject;
    public GameObject m_webview_panel;
    public GameObject m_screen_panel;
    public void Close_WebView()
    {
        if (webViewObject != null)
        {
            webViewObject.SetVisibility(false);
        }
    }

    public void Open_WebView(string web_path)
    {
#if UNITY_EDITOR
        return;
#endif
        if (null == webViewObject)
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
            if (null != webViewObject)
            {
                webViewObject.Init((msg) =>
                {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                });
            }
        }

        //웹뷰가 보여질 크기를 가져온다(x, y, width, height)
        Rect in_rect = m_webview_panel.GetComponent<RectTransform>().rect;
        //웹뷰 패널이 이동값 만큼 움직여야 하기에 위치값도 가져온다.
        Vector3 in_position = m_webview_panel.transform.localPosition;

        //현재 보여지는 화면의 전체 크기를 가져온다.(x, y, width, height)
        //우선은 백그라운드 패널의 크기로 하고있다.
        //720 * 1280 고정값으로 해도 무방할듯??
        Rect out_rect = m_screen_panel.GetComponent<RectTransform>().rect;

        //바깥쪽 렉트에서 안쪽렉트의 크기를 계산하여 offset을 구한다.
        Rect result = new Rect(Mathf.Abs((out_rect.x - in_rect.x)),
                Mathf.Abs((out_rect.y - in_rect.y)),
                (int)(out_rect.width - in_rect.width - (int)Mathf.Abs((out_rect.x - in_rect.x))),
                (int)(out_rect.height - in_rect.height - (int)Mathf.Abs((out_rect.y - in_rect.y))));

        //개발환경의 종횡비와 각 모바일의 종횡비는 다르므로 
        //각 모바일의 종횡비에 맞게 크기작업을 해줘야한다. 그래서 크기 비율을 구함
        //개발환경의 x 값과 모바일의 x값을 나눔
        //y값을 해도 똑같은 값이 나옴
        float aspect = (float)out_rect.width / (float)Screen.width;

        if (null != webViewObject)
        {
            //해당 주소를 띄어주고
            webViewObject.LoadURL(web_path);
            //위에서 계산한 offset을 적용시킨다(각 위치에 위에서 구한 비율만큼 나눠준다)
            //패널의 이동값이 있을경우 더해준다.
            webViewObject.SetMargins((int)(result.x / aspect) + (int)(in_position.x / aspect),
                (int)(result.y / aspect) - (int)(in_position.y / aspect),
                (int)(result.width / aspect) - (int)(in_position.x / aspect),
                (int)(result.height / aspect) + (int)(in_position.y / aspect));
            webViewObject.SetVisibility(true);
        }
    }
}
