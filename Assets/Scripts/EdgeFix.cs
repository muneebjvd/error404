using UnityEngine;
using UnityEngine.UI;

public class AlphaClickFix : MonoBehaviour {
    void Start() {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}