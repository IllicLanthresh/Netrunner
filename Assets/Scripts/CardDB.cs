using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class CardDB
{
    static string api_url;

    static string base_img_url;

    public static void SetApiUrl(string url)
    {
        api_url = url;
    }

    public static IEnumerator DownloadAPI(Action<string> callBack)
    {
        WWW unparsedAPI = new WWW(api_url);
        while (!unparsedAPI.isDone)
        {
            //Debug.Log("Downloading api: " + unparsedAPI.progress * 100 + "%"); //Enable this only when needed.
            yield return null;
        }

        callBack(unparsedAPI.text);
    }

    public static CardDBEntry[] ParseApi(string unparsedAPI)
    {
        NetrunnerDBApi Api = new NetrunnerDBApi();
        Api = JsonUtility.FromJson<NetrunnerDBApi>(unparsedAPI);
        base_img_url = Api.imageUrlTemplate;

        Debug.Log(Api.data);
        return Api.data;
    }

    public static string GetBaseImgUrl()
    {
        return base_img_url;
    }
}
