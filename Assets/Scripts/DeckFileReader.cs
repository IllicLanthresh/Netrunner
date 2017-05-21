using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class DeckFileReader {

    List<CardDB.NetrunnerDBApi.cardData> cards = new List<CardDB.NetrunnerDBApi.cardData>();

    public List<CardDB.NetrunnerDBApi.cardData> Load(string fileName, CardDB cardDB)
    {
        Debug.Log("Open: " + fileName);
        // Handle any problems that might arise when reading the text
        try
        {
            Debug.Log("trying...");
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);
            
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();
                    Debug.Log("line: " + line);

                    if (line != null)
                    {
                        // Do whatever you need to do with the text line, it's a string now
                        // In this example, I split it into arguments based on comma
                        // deliniators, then send that array to DoStuff()
                        int nOfCards;
                        string nameOfCard;
                        char[] Xletter = {'x'};

                        nOfCards = int.Parse(line.Split(Xletter)[0]);
                        nameOfCard = line.Split(Xletter, 2)[1].Trim();

                        for (int i = 0; i < nOfCards; i++)
                        {
                            Debug.Log("Looking for " + nameOfCard + " on DB");
                            foreach (CardDB.NetrunnerDBApi.cardData card in cardDB.Data)
                            {
                                
                                if (card.title == nameOfCard)
                                {
                                    Debug.Log("Found: " + card.title + ", adding...");
                                    cards.Add(card);
                                    break;
                                }
                            }                            
                        }
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
                return cards;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
            return null;
        }
    }
 }
