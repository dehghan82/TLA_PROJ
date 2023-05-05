using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using TLA_LIB;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        var d = File.ReadAllText(@"..\Results\phase3-sample\in\input1.json");
        var fa_in = JsonSerializer.Deserialize<FA>(d);
        IAutomata q = fa_in.set();
        string s = Console.ReadLine();
        System.Console.WriteLine(q.accpet_reject(s));
    }
    static public string accpet_reject(string s,NFA nfa)
    {
        State root = nfa._initial_state;
        Queue<Tuple<int, State>> que = new Queue<Tuple<int, State>>();
        que.Enqueue(new Tuple<int, State>(0, root));
        int i = 0;
        while (que.Count() != 0)
        {
            var m = que.Dequeue();
            i = m.Item1;
            root = m.Item2;
            if (i == s.Length && nfa._final_states.Contains(root))
                return "Accepted";

            if (i == s.Length)
                break;

            if (root.ntransitions.Keys.Contains(s[i].ToString()))
            {
                foreach (var item in root.ntransitions[s[i].ToString()])
                    que.Enqueue(new Tuple<int, State>(i + 1, item));
            }
            if (root.ntransitions.Keys.Contains(""))
            {
                foreach (var item in root.ntransitions[""])
                    que.Enqueue(new Tuple<int, State>(i, item));
            }
        }
        return "Rejected";
    }
    public string accpet_reject(string s,DFA dfa)
    {
        State root = dfa._initial_state;
        for (int i = 0; i < s.Length; i++)
        {
            root = root.dtransitions[s[i].ToString()];
        }
        if (dfa._final_states.Contains(root))
            return "Accepted";
        else
            return "Rejected";
    }
}
