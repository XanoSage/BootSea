using UnityEngine;
using System;
using LinqTools;
using System.Collections;


public class UITabController : MonoBehaviour {

    [SerializeField]
    UITab _defaultActiveTab = null;

    [SerializeField]
    UITab[] _tabs = null;

    void Awake() {
        for (int i = 0; i < _tabs.Length; ++i) {
            //_tabs[i].SetActive(_tabs[i] == _defaultActiveTab);
            _tabs[i].index = i;
            _tabs[i].onClick += tab_onClick;
        }
    }

    void OnEnable() {
		foreach (var t in _tabs) {
            t.SetActive(t == _defaultActiveTab);
        }
    }

    void tab_onClick(UITab tab) {
        _defaultActiveTab = tab;
        foreach (var t in _tabs) {
			 t.SetActive(t == tab);
	
        }
		
	
		
		
    }
}
