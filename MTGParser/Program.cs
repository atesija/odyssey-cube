using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Magic magic = new Magic();
            magic.LoadCards();
            magic.SetPrices("Torment");
            magic.SetPrices("Odyssey");
            magic.SetPrices("Judgement");
            //magic.GetDecks();
            magic.GedData();
            //magic.PrintCards();
            magic.Query();
        }
    }
}
