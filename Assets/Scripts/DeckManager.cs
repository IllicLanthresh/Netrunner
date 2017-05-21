using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DeckManager : MonoBehaviour {

    DeckFileReader deckReader = new DeckFileReader();
    List<CardDB.NetrunnerDBApi.cardData> cards = new List<CardDB.NetrunnerDBApi.cardData>();
    string previousCardCode;
    WWW url;
    Texture2D imgCarta;

    public enum side {Runner, Corp};

    public side DeckSide;

    public IEnumerator getCards(string path, CardDB cardDB, GameObject cardPrefab)
    {
        cards = deckReader.Load(path, cardDB);
        int increment = 0;
        
        foreach (CardDB.NetrunnerDBApi.cardData card in cards)
        {
            if (previousCardCode != card.code)
            {
                string img_url = cardDB.Base_img_url.Replace("{code}", card.code);
                url = new WWW(img_url);
                Debug.Log("Downloading image for " + card.title);

                while (!url.isDone)
                {
                    Debug.Log("Downloading image: " + url.progress * 100 + "%");
                    yield return null;
                }
                imgCarta = url.texture;
                Debug.Log("Done!");                
            }
            else
            {
                Debug.Log("Using the same image.");
            }
            previousCardCode = card.code;

            Vector3 YincrementVector = new Vector3(0, 0.01f);
            Vector3 FlipCardRotation = new Vector3(0, 0, 180f);
            

            GameObject cardGO = Instantiate(cardPrefab, this.transform.position + YincrementVector*increment, this.transform.rotation).gameObject;
            cardGO.transform.Rotate(FlipCardRotation, Space.World);

            cardGO.transform.SetParent(this.transform);

            cardGO.name = card.title;


            cardGO.transform.Find("Card_model").Find("front").GetComponent<Renderer>().material.mainTexture = imgCarta;

            if (card.side_code == "runner")
            {
                cardGO.transform.Find("Card_model").Find("back").GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("Textures/Runner_back");
            }
            else if (card.side_code == "corp")
            {
                cardGO.transform.Find("Card_model").Find("back").GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("Textures/Corp_back");
            }
            increment++;
        }
        Debug.Log(DeckSide.ToString() + " side finished downloading card images");

    }

}