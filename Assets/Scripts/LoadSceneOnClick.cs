using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour
{
    public InputField nickname;
    public Button submit_button;

    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);     
    }

    public void OpenOnlineScoreboard()
    {
        Application.OpenURL("http://gz0.nl/gd/score.php");
    }

    public void SubmitScore()
    {
        if (nickname.text.Length > 0)
        {
            int current_score = (int)SharedObject.Get<float>("score");
            UnityWebRequest.Get("http://gz0.nl/gd/score.php?addscore=1&name=" + nickname.text.Replace("&", "").Replace(" ", "") + "&score=" + current_score.ToString()).SendWebRequest();
            nickname.gameObject.SetActive(false);
            submit_button.gameObject.SetActive(false);
            OpenOnlineScoreboard();
        }
    }
}
