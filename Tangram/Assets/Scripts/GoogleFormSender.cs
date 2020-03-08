using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleFormSender : MonoBehaviour
{
    public GameBoard gameBoard;

    [Header("Google form")]
    public string formURL = "https://docs.google.com/forms/d/e/1FAIpQLSccYxUJBq2FM6O4oHGhf_qf9Ugu2LaFizgwoJF6PoWf-UlV5A/formResponse";



    void Start()
    {
        //SendInfoToGoogleForm("https://www.google.com", "1", "3.1415926");
    }

    public void SendInfoToGoogleForm(string imgURL, string infoId, string solutionJson)
    {
        StartCoroutine(Post(imgURL, infoId, solutionJson));

    }

    IEnumerator Post(string imgURL, string infoId, string solutionJson)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1243234367", imgURL);
        form.AddField("entry.282779133", infoId);
        form.AddField("entry.1616316150", solutionJson);


        using (UnityWebRequest www = UnityWebRequest.Post(formURL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

        yield return null;

    }
}
