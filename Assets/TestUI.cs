using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    [TransformBind("Text")]
    private Text _text;

    [TransformBind("Button")]
    private Button _btn;
    
    [TransformBind("Button/Text")]
    private Text _textBtn;

    private void Awake()
    {
        UGUIBinder.BuildCache(GetType());
        UGUIBinder.AutoBind(this);
        
        _btn.onClick.AddListener(() =>
        {
            _textBtn.text = "666";
            _text.text = "test";
        });
    }
}
