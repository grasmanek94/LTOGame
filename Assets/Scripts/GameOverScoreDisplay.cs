using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverScoreDisplay : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        Text text = GetComponent<Text>();
        text.text = "Score\r\n" + ((int)SharedObject.Get<float>("score")).ToString();
	}
}
