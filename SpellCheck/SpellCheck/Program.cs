using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellCheck
{
    class Program
    {
        static List<WordData> Dictionary;
        static List<WordData> MisspelledWords;

        static void Main(string[] args)
        {
            Dictionary = new List<WordData>();
            MisspelledWords = new List<WordData>();
            ReadData();
            SetWordProjections(MisspelledWords);
            SetWordProjections(Dictionary);
            SetWordCorrections();
            PrintData();
        }

        static void SetWordProjections(List<WordData> words)
        {
            for(int i = 0; i != words.Count; ++i)
            {
                WordData item = words[i];
                for(int j = 0; j < item.Word.Length; ++j)
                {
                    string word = item.Word;
                    item.Projections1.Add(word.Remove(j, 1));
                    for(int k = j + 1; k < item.Word.Length - 1; ++k)
                    {
                        string word2 = word;
                        item.Projections2.Add(word2.Remove(k, 1));
                    }
                }
            }
        }

        static void SetWordCorrections()
        {
            foreach(WordData itemWord in MisspelledWords)
            {
                int level = 2;
                foreach (WordData itemDictWord in Dictionary)
                {
                    //Word is correct
                    if(itemDictWord.Word == itemWord.Word)
                    {
                        itemWord.Corrections.Clear();
                        itemWord.Corrections.Add(itemDictWord.Word);
                        level = 0;
                        continue;
                    }

                    if (level > 0)
                    {
                        List<string> selfV = new List<string>();
                        selfV.Add(itemDictWord.Word);
                        var intersection = itemWord.Projections1.Intersect(selfV);
                        if (intersection.Count<string>() > 0)
                        {
                            itemWord.Corrections.Clear();
                            itemWord.Corrections.Add(itemDictWord.Word);
                            level = 1;
                            continue;
                        }
                        intersection = null;
                        selfV.Clear();
                        selfV.Add(itemWord.Word);
                        intersection = itemDictWord.Projections1.Intersect(selfV);
                        if (intersection.Count<string>() > 0)
                        {
                            if (level != 1)
                            {
                                itemWord.Corrections.Clear();
                            }
                            itemWord.Corrections.Add(itemDictWord.Word);
                            level = 1;
                            continue;
                        }
                    }
                    if (level > 1)
                    {
                        List<string> selfV = new List<string>();
                        selfV.Add(itemDictWord.Word);
                        var intersection = itemWord.Projections2.Intersect(selfV);
                        if (intersection.Count<string>() > 0)
                        {
                            itemWord.Corrections.Add(itemDictWord.Word);
                            continue;
                        }
                        intersection = null;
                        selfV.Clear();
                        selfV.Add(itemWord.Word);
                        intersection = itemDictWord.Projections2.Intersect(selfV);
                        if (intersection.Count<string>() > 0)
                        {
                            itemWord.Corrections.Add(itemDictWord.Word);
                            continue;
                        }
                    }
                    if (level > 1)
                    {
                        var intersection = itemDictWord.Projections1.Intersect(itemWord.Projections1);
                        if (intersection.Count<string>() > 0)
                        {
                            itemWord.Corrections.Add(itemDictWord.Word);
                            continue;
                        }
                    }
                }
            }
        }

        static void ReadData()
        {
            bool dictFinished = false;
            bool misspStarted = false;
            string line;
            while (true)
            {
                if (!misspStarted && dictFinished)
                    break;
                line = Console.ReadLine();
                if (line == "") continue;
                if (line == "===")
                {
                    dictFinished = true;
                    misspStarted = !misspStarted;

                    continue;
                }
                if (!dictFinished)
                {
                    string[] words = line.Split(" ");
                    if (words.Length > 0)
                    {
                        int widx = words.Length - 1;
                        int i = 0;
                        foreach(string item in words)
                        {
                            WordData w = new WordData(item);
                            w.SetLast(i == widx);
                            Dictionary.Add(w);
                            ++i;
                        }
                    }
                }
                else if (misspStarted)
                {
                    string[] misspWords = line.Split(" ");
                    if (misspWords.Length > 0)
                    {
                        int widx = misspWords.Length - 1;
                        int i = 0;
                        foreach (string item in misspWords)
                        {
                            WordData w = new WordData(item);
                            w.SetLast(i == widx);
                            MisspelledWords.Add(w);
                            ++i;
                        }
                    }
                }
            }
        }

        static void PrintData()
        {
            int i = 0;
            foreach(WordData wItem in MisspelledWords)
            {
                int corCount = wItem.Corrections.Count();
                if (corCount == 0)
                {
                    Console.Write("{" + wItem.Word + "?}");
                }
                else if (corCount == 1)
                {
                    Console.Write(wItem.Corrections[0]);
                }
                else
                {
                    Console.Write("{");
                    foreach(string cor in wItem.Corrections){
                        Console.Write(cor);
                        if (cor != wItem.Corrections[wItem.Corrections.Count - 1])
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.Write("}");
                }
                if (i != (MisspelledWords.Count - 1))
                {
                    if (wItem.IsLast())
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                ++i;
            }
        }
    }

    class WordData
    {
        private bool Last; // last word in line
        public string Word;
        // we need sorted items for using intersections
        public HashSet<string> Projections1;
        public HashSet<string> Projections2;
        public List<string> Corrections;

        public WordData(string word)
        {
            this.Word = word;
            this.Last = false;
            Projections1 = new HashSet<string>();
            Projections2 = new HashSet<string>();
            Corrections = new List<string>();
        }

        public void SetLast(bool isLast)
        {
            this.Last = isLast;
        }
        public bool IsLast()
        {
            return Last;
        } 
    };
}
