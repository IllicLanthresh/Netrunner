using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

    // References to the basic prefabs
    public GameObject deckPrefab;
    public GameObject cardPrefab;

    //Paths to deck lists (Currently using a TXT)
    //The format is '(Quantity)x(Name of card)' -- Ignore the '()'
    public string pathToRunnerDeck;
    public string pathToCorpDeck;
    
    //Callback after api data has been downloaded
    Action apiDownloadedCB;

    //Declare the card data base
    CardDB cardDB = new CardDB();

    void Start () {

        //Register the 'api downloaded' callback function
        apiDownloadedCB += OnDBDownloaded;

        //Download the api data
        Debug.Log("Start DB Download");
        StartCoroutine(cardDB.DownloadAPI(apiDownloadedCB));
    }

    void OnDBDownloaded()
    {
        Debug.Log("DB Download finished");

        //Get the game areas GO
        GameObject RunnerArea = GameObject.FindGameObjectWithTag("Runner");
        GameObject CorpArea = GameObject.FindGameObjectWithTag("Corp");

        //Create decks GO
        GameObject Runner_deck = Instantiate(deckPrefab, Vector3.zero, Quaternion.identity).gameObject;
        GameObject Corp_deck = Instantiate(deckPrefab, Vector3.zero, Quaternion.identity).gameObject;

        //Get the deck managers
        DeckManager RunnerDeckManager = Runner_deck.GetComponent<DeckManager>();
        DeckManager CorpDeckManager = Corp_deck.GetComponent<DeckManager>();

        //Set deck names for hierarchy
        Runner_deck.name = "Runner Deck";
        Corp_deck.name = "Corp Deck";

        //Create Vectors for deck placement(position and rotation)
        Vector3 CorpDeckPosition = new Vector3(3.5f, 0.25f, -5f); 
        Vector3 CorpDeckRotation = new Vector3(90, 0, 0);
        Vector3 RunnerDeckPosition = new Vector3(-3.5f, 0.25f, 5f); 
        Vector3 RunnerDeckRotation = new Vector3(90, 180, 0);

        //Apply such positions and rotations
        //TODO: Not final positions, this will change!
        Corp_deck.transform.Translate(CorpDeckPosition);
        Runner_deck.transform.Translate(RunnerDeckPosition);
        Corp_deck.transform.Rotate(CorpDeckRotation, Space.World);
        Runner_deck.transform.Rotate(RunnerDeckRotation, Space.World);

        //Set decks as a child of its player's game area
        Corp_deck.transform.SetParent(CorpArea.transform, true);
        Runner_deck.transform.SetParent(RunnerArea.transform, true);

        //Tell each Deck Manager wich side is it(Runner or Corp)
        CorpDeckManager.DeckSide = DeckManager.side.Corp;
        RunnerDeckManager.DeckSide = DeckManager.side.Runner;

        //Get the cards for the decks
        StartCoroutine(RunnerDeckManager.getCards(Application.dataPath + "/" + pathToRunnerDeck, cardDB, cardPrefab));
        StartCoroutine(CorpDeckManager.getCards(Application.dataPath + "/" + pathToCorpDeck, cardDB, cardPrefab));

        //TODO: Get identity card out(of the deck)

        //TODO: Shuffle decks

        //TODO: Start turn manager

        //TODO: More stuff???

    }
}
