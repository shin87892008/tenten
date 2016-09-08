using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Awake_Bundle_Loading : MonoBehaviour {

    public Text _progress_txt;
    public Image _progress_image;

    public void Set_Progress(float p_progress, string p_bundle_name, int p_cur_count, int p_total_count)
    {
        _progress_txt.text = p_bundle_name + string.Format("({0} / {1})", p_cur_count, p_total_count);
        _progress_image.fillAmount = p_progress;
    }
}
