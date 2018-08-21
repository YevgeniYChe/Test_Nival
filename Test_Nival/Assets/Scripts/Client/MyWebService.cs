using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;


public class MyWebService {


    public string urlNewGame = "http://localhost:60006/api/NewGame";
    public string urlGetStep = "http://localhost:60006/api/GetStep";
    



	public IEnumerator GetNewState(Action<StateClient> callback)
    {
        return GetItems(callback);
    }


    IEnumerator GetItems( Action<StateClient> callback )
    {
        WWW www = new WWW(urlNewGame);
        yield return www;
        var state = new StateClient();
        
        if (www.error == null)
        {
            state = JsonUtility.FromJson<StateClient>(www.text);
            callback(state);
        }
        else
        {
            Debug.Log(www.error);
        }

    }
    

    public IEnumerator GetNextState(StateClient state, Action<StateClient> callback)
    {
        return GetStep(state,callback);
    }

    IEnumerator GetStep(StateClient state, Action<StateClient> callback)
    {
        string jsonString = JsonUtility.ToJson(state);

        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());

        UnityWebRequest unityWebRequest = new UnityWebRequest(urlGetStep, "POST");
        unityWebRequest.uploadHandler = new UploadHandlerRaw(byteData);
        unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        yield return unityWebRequest.Send();

        var reqState = new StateClient();

        if (unityWebRequest.error == null)
        {
            string response = unityWebRequest.downloadHandler.text;

            reqState = JsonUtility.FromJson<StateClient>(response);

            callback(reqState);
        }
        else
        {
            Debug.Log(unityWebRequest.error);
        }
    }
}
