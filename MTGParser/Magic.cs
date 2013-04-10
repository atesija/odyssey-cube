using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTGParser
{
    class Magic
    {
        enum CardColor
        {
            Black,
            Blue,
            Red,
            Green,
            White,
            Multi,
            Colorless
        }

        enum CardType
        {
            Creature,
            LegendaryCreature,
            Artifact,
            Enchantment,
            Sorcery,
            Instant,
            BasicLand,
            Land,
            ERROR
        }

        enum CardRarity
        {
            Common,
            Uncommon,
            Rare
        }

        struct Card
        {
            public string name, fullName, cardText, set;
            public double price;
            public CardType type;
            public CardColor color;
            public CardRarity rarity;
            public bool buy;
            public int pwoer, toughness;
            //Number of times it was seen in each of these
            public int mainHits, sideboardHits, totalHits;
            //Total number of times the card was seen
            public int mainCount, sideboardCount, totalCount;
        }

        private List<Card> cards;

        public Magic()
        {
            cards = new List<Card>();
        }

        //Loads the cards from a file
        public void LoadCards()
        {
            //Create a file reader
            StreamReader file = new StreamReader("Odyssey Block.txt");
            string line;
            string[] data = new string[10];
            int count;
            Card c;

            //Loop through the entire file
            while (!file.EndOfStream)
            {
                c = new Card();

                //Pull the data for each card out of the file
                count = 0;
                do
                {
                    line = file.ReadLine();
                    data[count] = line;
                    count++;
                }
                while (line != "");

                //Pass the data to make a card and add it to the collection
                c = MakeCard(data, count);
                cards.Add(c);
            }
        }

        //Sets the prices for every card in a given set
        public void SetPrices(string setName)
        {
            //Create a file reader
            StreamReader file = new StreamReader(setName + ".txt");
            string line;
            string[] split;
            Card c;
            int index;

            //Loop through the entire file
            while (!file.EndOfStream)
            {
                line = file.ReadLine();

                //Split at price
                split = line.Split('$');

                //Remove whitespace
                split[0] = split[0].Replace(" ", string.Empty);
                split[0] = split[0].Replace("\t", string.Empty);

                //Get index of the current card
                index = cards.FindIndex(item => item.name == split[0]);

                //Add the price and replace it
                if (index != -1)
                {
                    c = cards[index];
                    c.price = Convert.ToDouble(split[1]);
                    cards[index] = c;
                }
            }
        }

        //Printing for debug purposes
        public void PrintCards()
        {
            foreach (Card c in cards)
            {
                //Data to print
                Console.WriteLine("Name: " + c.fullName);
                Console.WriteLine("Type: " + c.type.ToString());
                Console.WriteLine("Color: " + c.color.ToString());
                Console.WriteLine("Cost: " + c.price.ToString());
                Console.WriteLine("Total Hits: " + c.totalHits.ToString());
                Console.WriteLine();
            }
            Console.ReadKey(true);
        }

        //Build a card with strings containing card data
        private Card MakeCard(string [] data, int count)
        {
            Card c = new Card();

            //Card name
            c.name = data[0].Replace(" ", string.Empty);
            c.fullName = data[0];

            //Figure out the card type and populate the card data
            string [] type = data[2].Split(' ');
            if (data[1] == "Land")
            {
                c.type = CardType.Land;
            }
            else if (data[1].Contains("Basic"))
            {
                c.type = CardType.BasicLand;
            }
            else if (type[0] == "Legendary")
            {
                c.type = CardType.LegendaryCreature;
                c.color = GetColor(data[1]);
            }
            else if (type[0] == "Creature")
            {
                c.type = CardType.Creature;
                c.color = GetColor(data[1]);
            }
            else if (type[0] == "Instant")
            {
                c.type = CardType.Instant;
                c.color = GetColor(data[1]);
            }
            else if (type[0] == "Sorcery")
            {
                c.type = CardType.Sorcery;
                c.color = GetColor(data[1]);
            }
            else if (type[0] == "Artifact")
            {
                c.type = CardType.Artifact;
                c.color = GetColor(data[1]);
            }
            else if (type[0] == "Enchantment")
            {
                c.type = CardType.Enchantment;
                c.color = GetColor(data[1]);
            }
            else
            {
                c.type = CardType.ERROR;
            }

            return c;
        }

        private CardColor GetColor(string data)
        {
            CardColor c = new CardColor();

            //Figure out the color
            c = CardColor.Colorless;
            if (data.Contains('G'))
                c = CardColor.Green;
            if (data.Contains('U') && c != CardColor.Colorless)
                c = CardColor.Multi;
            else if (data.Contains('U'))
                c = CardColor.Blue;
            if (data.Contains('W') && c != CardColor.Colorless)
                c = CardColor.Multi;
            else if (data.Contains('W'))
                c = CardColor.White;
            if (data.Contains('B') && c != CardColor.Colorless)
                c = CardColor.Multi;
            else if (data.Contains('B'))
                c = CardColor.Black;
            if (data.Contains('R') && c != CardColor.Colorless)
                c = CardColor.Multi;
            else if (data.Contains('R'))
                c = CardColor.Red;

            return c;
        }

        public void GetDecks()
        {            
            //Create a file reader
            StreamReader file = new StreamReader("odysseydecks.txt");
            StreamWriter output = new StreamWriter("decks.txt");
            string line;
            string[] split;
            int count;
            Card c;

            //Loop through the entire file
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                line = line.Replace(" ", string.Empty);
                if (line.Contains("creature"))
                {
                    split = line.Split('\t');
                    output.WriteLine(split[1]);
                }
                else if(line.Contains("sideboardcards") || line.Contains("otherspells") || line.Contains("cards") || line.Contains("lands"))
                {
                }
                else if (line.Contains("Worlds"))
                {
                    file.ReadLine();
                }
                else if (String.IsNullOrWhiteSpace(line))
                {
                }
                else if (line.Contains("Sideboard") || line.Contains("MainDeck"))
                {
                    line = "#" + line;
                    output.WriteLine(line);
                }
                else
                    output.WriteLine(line);
            }
        }

        public void GedData()
        {            
            //Create a file reader
            StreamReader file = new StreamReader("decks.txt");
            string line;
            char number;
            string[] split;
            string[] separators = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            bool main = true;
            int index, numberOfCards;
            Card c;

            //Loop through the entire file
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                if(line.Contains("MainDeck"))
                {
                    main = true;
                }
                else if (line.Contains("Sideboard"))
                {
                    main = false;
                }
                else
                {   
                    //Split card at number
                    number = line[0];
                    numberOfCards = Convert.ToInt32(number) - 48;
                    split = line.Split(separators, StringSplitOptions.None);

                    //Get index of the current card
                    index = cards.FindIndex(item => item.name == split[1]);

                    //Get the card and modify the info
                    if (index != -1)
                    {
                        c = cards[index];

                        //Edit card info
                        if (main)
                        {
                            c.mainCount += numberOfCards;
                            c.mainHits++;
                        }
                        else
                        {
                            c.sideboardCount += numberOfCards;
                            c.sideboardHits++;
                        }
                        c.totalCount += numberOfCards;
                        c.totalHits++;

                        //Insert it back in
                        cards[index] = c;
                    }
                }
            }
        }

        public void Query()
        {
            StreamWriter output = new StreamWriter("data2");
            List<Card> returnedCards = (from c in cards where c.totalHits != 0 orderby c.totalHits, c.totalCount select c).ToList();
            foreach (Card c in returnedCards)
            {
                //output.WriteLine(c.totalHits.ToString() + "\t" + c.totalCount.ToString() + "\t" + c.price.ToString() + "\t" + c.color.ToString() + "\t\t" + c.name);
                output.WriteLine("2 " + c.fullName);
            }
            output.WriteLine(returnedCards.Count.ToString());
            Console.ReadKey();
        }
    }
}
