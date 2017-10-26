using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverScoreDisplay : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        Text text = GetComponent<Text>();
        int current_score = (int)SharedObject.Get<float>("score");
        text.text = "Score\r\n" + current_score.ToString();

        int saved_score = PlayerPrefs.GetInt("score");
        if(saved_score < current_score)
        {
            PlayerPrefs.SetInt("score", current_score);
            PlayerPrefs.Save();
        }
	}
}
