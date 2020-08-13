using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowValue : MonoBehaviour {

    public Text percentageText;
    private float currentSens;

    private void Start() {
        percentageText = GetComponent<Text>();

        // Initialise text value with player sensitivity
        currentSens = PlayerPrefs.GetFloat("playerSens");
        currentSens *= 10;
        textUpdate(currentSens);
    }

    public void textUpdate(float value) {
        // Using slider with whole number values, so player can increment in 0.1 jumps.
        // Therefore, value must be divided by 10 (max slider value = 80, so max value = 8).
        value /= 10;
        percentageText.text = value.ToString();
    }
}
