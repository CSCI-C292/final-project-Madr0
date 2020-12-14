using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] RuntimeData _runtimeData;

    void Awake() {
        _runtimeData.currentScore = 1000;
    }
}
