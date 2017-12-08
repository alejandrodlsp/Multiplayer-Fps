using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour {

    public static void SetLayerRecursively(GameObject _gm, int _layerIndex) {
        if (_gm == null)
            return;

        _gm.layer = _layerIndex;

        foreach (Transform _child in _gm.transform) {
            if (_child == null)
                continue;
            SetLayerRecursively(_child.gameObject, _layerIndex);
        }
    }
}
