using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryScript : MonoBehaviour {

    public GameOption gameOption;

	void Awake()
    {
        DontDestroyOnLoad(this);
        GameSetting.RecordSetting(gameOption,gameObject);
        GameManager.Instance.Init();
    }
}
