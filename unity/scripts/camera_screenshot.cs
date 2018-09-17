// Author: luzan

/*
To get the Google Form's URL, go to "Preview" mode, right-click on the form and click "View Page Source".
Press Ctrl + F and look for "form action". Copy-paste the URL after "form action".
To get the entry #'s, right-click on the form and click "Inspect".
Apply this script to the Canvas.
Under the Canvas' Inspector, drag each input field to each component in the "Send_to_google_drive" script's section.
Under the "Submit" button's Inspector, add a new "OnClick()" and drag the Canvas onto the "Select Object" field.
Change the "No Function" to "send_to_google_drive" > "Send()".
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class send_to_google_drive : MonoBehaviour
{
    public GameObject name;
    public GameObject email;
    public GameObject phone;

    private string name_input;
    private string email_input;
    private string phone_input;

    [SerializeField]
    private string BASE_URL = "GOOGLE FORM URL";

    IEnumerator Post(string name, string email, string phone)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.###", name);
        form.AddField("entry.###", email);
        form.AddField("entry.###", phone);
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return www;
    }

    public void Send()
    {
        name_input = name.GetComponent<InputField>().text;
        email_input = email.GetComponent<InputField>().text;
        phone_input = phone.GetComponent<InputField>().text;
        StartCoroutine(Post(name_input, email_input, phone_input));
    }
}
