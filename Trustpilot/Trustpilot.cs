using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Trustpilot
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      string anagram = "poultry outwits ants";

      List<string> lines = new List<string>(File.ReadAllLines("wordlist.txt"));

      Console.WriteLine("Beginning search...");
      int nWords = anagram.CountChar(' ') + 1;

      List<string> anagrams = SearchForAnagram(anagram, lines, nWords);

      Console.WriteLine("Search done");
      File.WriteAllLines("anagrams.txt", anagrams.ToArray());

      Console.WriteLine("Checking for correct hash");
      string correctHash = "4624d200580677270a54ccff86b9610e";
      string correct = "";
      foreach (string s in anagrams)
      {
        Console.WriteLine(s);
        if (s.GetMD5() == correctHash)
        {
          correct = s;
        }
      }

      Console.WriteLine(correct + " is the correct phrase!!!!!!!");
    }

    public static List<string> FindSubsets(string superset, List<string> words)
    {
      List<string> subsets = new List<string>();
      foreach (string word in words)
        if (word.IsSubsetOf(superset))
          subsets.Add(word);

      return subsets;
    }

    public static List<string> SearchForAnagram(string anagram, List<string> words, int wordsLeft)
    {
      List<string> foundWords;
      List<string> subsets = FindSubsets(anagram, words);
      if (wordsLeft == 1)
      {
        foundWords = subsets;
      }
      else
      {
        foundWords = new List<string>(subsets.Count);
        #if DEBUG
        int i = 0;
        #endif
        foreach (string word in subsets)
        {
          #if DEBUG
          Console.WriteLine("Level {0} iter {1}/{2}", wordsLeft, i, subsets.Count);
          if (wordsLeft == 3)
          {
            Thread.Sleep(200);
          }
          #endif

          string rest = anagram.Subtract(word);

          if (String.IsNullOrEmpty(rest.Trim()))
            continue;
            
          List<string> nextWords = SearchForAnagram(rest, subsets, wordsLeft - 1);
          foreach (string nw in nextWords)
          {
            if (nw.Replace(" ", "").Length == rest.Replace(" ", "").Length)
            {
              string combined = word + " " + nw;
              foundWords.Add(combined);
            }
          }
          #if DEBUG
          i++;
          #endif
        }
      }

      return foundWords;

    }

  }

  public static class ExtensionMethods
  {

    public static int CountChar(this string s, char c)
    {
      int count = 0;
      foreach (char k in s)
      {
        if (k == c)
          count++;
      }
      return count;

    }

    public static string GetMD5(this string s)
    {
      using (MD5 md5Hash = MD5.Create())
      {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(s));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
          sBuilder.Append(data[i].ToString("x2"));

        return sBuilder.ToString();

      }
    }

    public static bool IsSubsetOf(this string s, string other)
    {
      if (s.Length > other.Length)
        return false;

      foreach (char c in other)
      {
        if (other.Contains(c.ToString()) == false)
          return false;
      }

      Dictionary<char, uint> countsS = s.Counts();
      Dictionary<char, uint> countsOther = other.Counts();

      foreach (KeyValuePair<char,uint> p in countsS)
      {
        if ((countsOther.ContainsKey(p.Key) && countsOther[p.Key] >= p.Value))
          continue;
        else
          return false;
      }

      return true;
    }

    public static Dictionary<char, uint> Counts(this string s)
    {
      Dictionary<char, uint> counts = new Dictionary<char, uint>(s.Length);
      foreach (char c in s)
      {
        if (counts.ContainsKey(c))
          counts[c]++;
        else
          counts.Add(c, 1);
      }
      return counts;
    }

    public static string Subtract(this string s, string other)
    {
      int[] drops = new int[other.Length];
      // initialize to -1
      for (int i = 0; i < drops.Length; i++)
        drops[i] = -1;

      for (int i = 0; i < other.Length; i++)
      {
        char c = other[i];
        int index = s.IndexOf(c);
        while (Array.IndexOf(drops, index) >= 0)
        {
          index += s.Substring(index + 1).IndexOf(c) + 1;
        }

        drops[i] = index;

      }

      return s.Drop(drops);
    }

    public static string Drop(this string s, int[] indices)
    {
      const char delChar = '_';
      char[] newChars = new char[s.Length - indices.Length];
      char[] sChars = s.ToCharArray();

      foreach (int i in indices)
      {
        sChars[i] = delChar;
      }

      int j = 0;
      foreach (char c in sChars)
      {
        if (c == delChar)
          continue;
        newChars[j++] = c;
      }

      return new string(newChars);
    }
  }
}
