using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to name matching app!");

            List<string> blackListNames = TryReadingFileUntilSuccess("Blacklist file path:").ToList();
            Console.WriteLine();

            List<string> noiseWords = TryReadingFileUntilSuccess("Noise words file path:").ToList();
            Console.WriteLine();

            while (true) //keep program alive by asking name repeatedly
            {
                string nameToValidate = ReadInputUntilSuccess("Name to validate:");

                char[] delimiters = { ' ', ',', '-' }; //delimiters that split the full name to first name, middle name, last name, etc.
                double matchThreshold = 0.65; //threshold % of parts of name that has to match (ex. Bin Laden <> Osama Bin Laden = 0.66)

                List<string> result = GetMatchingNames(nameToValidate, blackListNames, delimiters, noiseWords, matchThreshold);

                if (result.Any())
                {
                    Console.WriteLine("Results:");
                    result.ForEach(x => Console.WriteLine(x));
                }
                else
                {
                    Console.WriteLine("No results found.");
                }
                Console.WriteLine();
            }
        }

        static string[] TryReadingFileUntilSuccess(string displayMessage)
        {
            while(true)
            {
                Console.WriteLine(displayMessage);
                string filePath = Console.ReadLine();
                try
                {
                   return System.IO.File.ReadAllLines(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading the file.");
                    Console.WriteLine(e.Message);
                }
            }
            throw new Exception("Unexpected error while reading the file");
        }

        static string ReadInputUntilSuccess(string displayMessage)
        {
            while(true)
            {
                Console.WriteLine(displayMessage);
                string nameToValidate = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(nameToValidate))
                {
                    return nameToValidate;
                }
                Console.WriteLine("Invalid name. Try again.");
            }
        }

        static int GetMatchesCountBetweenLists(List<string> list1, List<string> list2)
        {
            int matches = 0;

            foreach (var list1Element in list1)
            {
                if (list2.Exists(x => x.ToLower() == list1Element.ToLower()))
                {
                    matches++;
                }
            }

            return matches;
        }

        static List<string> GetMatchingNames(string nameToValidate, 
                                            List<string> blackListNames, 
                                            char[] nameDelimiters, 
                                            List<string> noiseWords, 
                                            double matchPercentThreshold)
        {
            List<string> result = new List<string>();
            
            List<string> nameParts = nameToValidate.Split(nameDelimiters).ToList();
            nameParts.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            nameParts.RemoveAll(x => noiseWords.Exists(y => y.ToLower() == x.ToLower())); //remove noise words from input name

            foreach (var blackListName in blackListNames)
            {
                List<string> blackListNameParts = blackListName.Split(nameDelimiters).ToList();
                blackListNameParts.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                int matches = GetMatchesCountBetweenLists(blackListNameParts, nameParts);
                int total = blackListNameParts.Count > nameParts.Count ? blackListNameParts.Count : nameParts.Count; //depending on which has more names, is total

                double matchPercent = (double)matches / total;
                if (matchPercent >= matchPercentThreshold)
                {
                    result.Add(blackListName);
                }
            }

            return result;
        }
    }
}
