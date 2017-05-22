using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DeckManager : MonoBehaviour {

    // Create a file reader
    DeckFileReader deckReader = new DeckFileReader();

    //List to store the cards of the deck
    List<CardDB.NetrunnerDBApi.cardData> cards = new List<CardDB.NetrunnerDBApi.cardData>();

    //Simple variable to store the code of the last card image downloaded to avoid downloading the same image multiple times
    string previousCardCode;

    WWW url;
    Texture2D imgCarta;

    public enum side {Runner, Corp};

    //This stores which side the deck is(Runner or Corp)
    public side DeckSide;

    public IEnumerator getCards(string path, CardDB cardDB, GameObject cardPrefab) //FIXME: Maybe wanna rename 'getCards' to something like 'downloadCards' or make this a constructor
    {
        //Get the instances in the database of every card our deck must have
        cards = deckReader.Load(path, cardDB);

        //Multiplier to put each card on top of the other
        int increment = 0; //FIXME: Find a better way to do this
        
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
            
            //Create the card GO
            GameObject cardGO = Instantiate(cardPrefab, this.transform.position + YincrementVector*increment, this.transform.rotation).gameObject;
            cardGO.transform.Rotate(FlipCardRotation, Space.World);

            cardGO.transform.SetParent(this.transform);

            cardGO.name = card.title;

            //Use the image downloaded
            cardGO.transform.Find("Card_model").Find("front").GetComponent<Renderer>().material.mainTexture = imgCarta;

            //Put the correct back image of the card depending on which side the card is(Runner or Corp)
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