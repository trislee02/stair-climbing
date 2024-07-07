using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void pressKey(string key)
    {
        if (key == "backspace")
        {
            if (inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
            return;
        }

        inputField.text += key;
    }

    public void pressEnter()
    {
        inputField.DeactivateInputField();
    }

}
