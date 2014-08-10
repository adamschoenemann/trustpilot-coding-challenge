using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Permutations
{

  public static class ExtensionMethods
  {
    public static string ShiftLeft(this string s)
    {
      return s.Substring(1) + s[0];
    }
  }

  public class Permutation
  {

    private string val;
    private Permutation[] subs;

    public Permutation(string input) : this(String.Empty, input)
    {

    }

    private Permutation(string val, string rest)
    {
      this.val = val;
      if (rest.Length == 0)
      {
        return;
      }
      subs = new Permutation[rest.Length];
      for (int i = 0; i < rest.Length; i++)
      {
        subs[i] = new Permutation(rest[0].ToString(), rest.Substring(1));
        rest = rest.ShiftLeft();
      }
    }

    public List<string> Resolve()
    {
      List<string> l = new List<string>();

      if (subs == null)
      {
        l.Add(val);
        return l;
      }

      for (int i = 0; i < subs.Length; i++)
      {
        List<string> perms = subs[i].Resolve();
        for (int j = 0; j < perms.Count; j++)
        {
          l.Add(val + perms[j]);
        }
      }

      return l;
    }

    public static void Main(string[] args)
    {
      string ana = "resistance";
      var perm = new Permutation(ana);
      List<string> perms = perm.Resolve();
      foreach (string p in perms)
        Console.WriteLine(p);
      Console.WriteLine("Count: " + perms.Count);
    }
  }

}
