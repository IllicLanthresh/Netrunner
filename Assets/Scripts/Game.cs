using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{

    static string api_url = "https://netrunnerdb.com/api/2.0/public/cards";

    // References to the basic prefabs(filled in ispector)
    public GameObject deckPrefab;
    public GameObject cardPrefab;

    //Paths to deck lists, also filled in inspector(Currently using a TXT file)
    //The format is '(Quantity)x(Name of card)' -- Ignore the '()'
    public TextAsset RunnerDeckTxt;
    public TextAsset CorpDeckTxt;
    
    //Callback after the api data has been downloaded
    Action<string> apiDownloadedCB;
    CardDBEntry[] cardDBEntries;

    GameObject RunnerAreaGO;
    GameObject CorpAreaGO;

    GameObject RunnerDeckGO;
    GameObject CorpDeckGO;

    Deck RunnerDeck;
    Deck CorpDeck;


    void Start () {
        
        //Register the 'api downloaded' callback function
        apiDownloadedCB += OnAPIDownloaded;

        //Download the api data
        CardDB.SetApiUrl(api_url);
        StartCoroutine(CardDB.DownloadAPI(apiDownloadedCB));

        //Get the game areas GO
        RunnerAreaGO = GameObject.FindGameObjectWithTag("Runner");
        CorpAreaGO = GameObject.FindGameObjectWithTag("Corp");

        setupDecks();
    }

    void OnAPIDownloaded(string parsedApi)
    {
        cardDBEntries =  CardDB.ParseApi(parsedApi);

        OnApiParsed();
    }

    void setupDecks()
    {
        //Create decks GO
        RunnerDeckGO = Instantiate(deckPrefab, Vector3.zero, Quaternion.identity).gameObject;
        CorpDeckGO = Instantiate(deckPrefab, Vector3.zero, Quaternion.identity).gameObject;

        //Get the deck managers
        RunnerDeck = RunnerDeckGO.GetComponent<Deck>();
        CorpDeck = CorpDeckGO.GetComponent<Deck>();

        //Set deck names for hierarchy
        RunnerDeckGO.name = "Runner Deck";
        CorpDeckGO.name = "Corp Deck";

        //Create Vectors for deck placement(position and rotation)
        Vector3 CorpDeckPosition = new Vector3(3.5f, 0.25f, -5f);
        Vector3 CorpDeckRotation = new Vector3(90, 0, 0);
        Vector3 RunnerDeckPosition = new Vector3(-3.5f, 0.25f, 5f);
        Vector3 RunnerDeckRotation = new Vector3(90, 180, 0);

        //Apply such positions and rotations
        //TODO: Not final positions, this will change!
        CorpDeckGO.transform.Translate(CorpDeckPosition);
        RunnerDeckGO.transform.Translate(RunnerDeckPosition);
        CorpDeckGO.transform.Rotate(CorpDeckRotation, Space.World);
        RunnerDeckGO.transform.Rotate(RunnerDeckRotation, Space.World);

        //Set decks as a child of its player's game area
        CorpDeckGO.transform.SetParent(CorpAreaGO.transform, true);
        RunnerDeckGO.transform.SetParent(RunnerAreaGO.transform, true);

        //Tell each Deck Manager wich side is it(Runner or Corp)
        CorpDeck.DeckSide = Deck.side.Corp;
        RunnerDeck.DeckSide = Deck.side.Runner;
    }

    void OnApiParsed()
    {
        //Parse deck files
        List<CardDBEntry> CorpCardsList = CorpDeck.parseDeckFile(CorpDeckTxt, cardDBEntries);
        List<CardDBEntry> RunnerCardsList = RunnerDeck.parseDeckFile(RunnerDeckTxt, cardDBEntries);

        //Get the cards for the decks
        StartCoroutine(CorpDeck.startDeck(cardPrefab));
        StartCoroutine(RunnerDeck.startDeck(cardPrefab));
        

        //TODO: Get identity card out(of the deck)

        //TODO: Shuffle decks

        //TODO: Start turn manager

        //TODO: More stuff???
    }
}
