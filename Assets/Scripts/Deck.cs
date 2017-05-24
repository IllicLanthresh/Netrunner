using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

   public enum side {Runner, Corp};

    //This stores which side the deck is(Runner or Corp)
    public side DeckSide;

    List<CardDBEntry> ListOfCards = new List<CardDBEntry>();


    public List<CardDBEntry> parseDeckFile(TextAsset deckFile, CardDBEntry[] cardDBEntries)
    {

        foreach (string line in deckFile.text.Split(System.Environment.NewLine.ToCharArray(), System.StringSplitOptions.None))
        {
            
            if (line != null && line != "")
            {
                int nOfCards;
                string nameOfCard;
                char[] Xletter = { 'x' };

                nOfCards = int.Parse(line.Split(Xletter)[0]);
                nameOfCard = line.Split(Xletter, 2)[1].Trim();

                for (int i = 0; i < nOfCards; i++)
                {
                    Debug.Log("Looking for " + nameOfCard + " on DB");
                    foreach (CardDBEntry cardDBEntry in cardDBEntries)
                    {

                        if (cardDBEntry.title == nameOfCard)
                        {
                            Debug.Log("Found: " + cardDBEntry.title + ", adding...");
                            ListOfCards.Add(cardDBEntry);
                            break;
                        }
                    }
                }
            }
        }

        return ListOfCards;
    }

    public IEnumerator startDeck(GameObject cardPrefab)
    {
       //Multiplier to put each card on top of the other
        int increment = 0; //TODO: Find a better way to do this
        string previousCardCode = null;
        Texture2D imgCarta = new Texture2D(0,0);
        
        foreach (CardDBEntry card in ListOfCards)
        {
            if (previousCardCode != card.code)
            {
                string img_url = CardDB.GetBaseImgUrl().Replace("{code}", card.code);
                WWW url = new WWW(img_url);
                Debug.Log("Downloading image for " + card.title);

                while (!url.isDone)
                {
                    //Debug.Log("Downloading image: " + url.progress * 100 + "%");
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

    public void Shuffle()
    { 
        for (int i = 0; i < ListOfCards.Count; i++)
        {
            CardDBEntry temp = ListOfCards[i];
            int randomIndex = Random.Range(i, ListOfCards.Count);
            ListOfCards[i] = ListOfCards[randomIndex];
            ListOfCards[randomIndex] = temp;
        }
        UpdateVisual();
    }

    public void UpdateVisual()
    {

    }

}