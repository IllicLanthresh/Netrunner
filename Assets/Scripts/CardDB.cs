using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CardDB
{
    //TODO: Finish debug or delete it
    public bool DebugOn;
    public string api_url = "https://netrunnerdb.com/api/2.0/public/cards";

    [System.Serializable]

    public class NetrunnerDBApi
    { 
        public string imageUrlTemplate;

        [System.Serializable]
        public class cardData
        {
            public string code;

            public string faction_code;
            public string side_code;

            public string type_code;
            public string keywords;

            public string title;
            public string text;

            public int minimum_deck_size;
            public int influence_limit;

            public int base_link;

            public int cost;
            public int memory_cost;
            public int strength;

        }
        public cardData[] data;

        public int total;
        public bool success;
        public string version_number;
        public string last_updated;
    }

    NetrunnerDBApi.cardData[] data;

    

    public NetrunnerDBApi.cardData[] Data
    {
        get
        {
            return data;
        }
    }

    public string Base_img_url
    {
        get
        {
            return base_img_url;
        }
    }

    private string base_img_url;



    public IEnumerator DownloadAPI(Action callback)
    {
        WWW url = new WWW(api_url);
        while (!url.isDone)
        {
            Debug.Log("Downloading api: " + url.progress * 100 + "%");
            yield return null;
        }

        WWW unparsedAPI = url;
        NetrunnerDBApi Api = new NetrunnerDBApi();
        Api = JsonUtility.FromJson<NetrunnerDBApi>(unparsedAPI.text);
        base_img_url = Api.imageUrlTemplate;
        Debug.Log("API Version: " + Api.version_number);


        data = Api.data;
        callback();
    }
}
