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
    public static void Main(string[] args)
    {
        //Read from a Json File and convert to FA

        //var d = File.ReadAllText(@"E:\TLA01-Projects\samples\phase2-sample\in\input1.json");
        var d = File.ReadAllText(@"C:\Users\Lenovo\Documents\TLA Dr Entezari\TLA01-Projects\samples\phase2-sample\in\input1.json");
        var fa_in = JsonSerializer.Deserialize<FA>(d);
        IAutomata q = fa_in.set();
        DFA dfa = q.to_DFA();
        DFA new_dfa=simplify_DFA(dfa);
        //FA fa_out = dfa.SetR();
        FA fa_out = new_dfa.SetR();
        string jason = JsonSerializer.Serialize(fa_out, new JsonSerializerOptions { WriteIndented = true });
        jason = Regex.Unescape(jason);
        File.WriteAllText(@"C:\Users\Lenovo\Documents\TLA Dr Entezari\test.json", jason, Encoding.UTF8);
    }
    static public DFA simplify_DFA(DFA dfa)
    {
        int n = dfa._states.Count;
        bool[,] table = new bool[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                table[i, j] = false;
            }
        }//staet initializing table
         //for (int i = 0; i < n; i++)
         //{
         //    for (int j = 0; j < dfa._states[i].dtransitions.Count; j++)
         //    {
         //        var x = dfa._states[i].dtransitions.ElementAt(j);
         //        int q = dfa._states.FindIndex(y => y.Name == x.Value.Name);// return the index of the state that its name is equal to name of the adj state
         //        for(int z = 0; z < dfa._final_states.Count; z++)
         //        {
         //            if(dfa._final_states[z].Name == dfa._states[q].Name || dfa._final_states[z].Name==dfa._states[i].Name)
         //            {
         //                table[i, q] = true;
         //                break;
         //            }
         //        }

        //    }
        //}//end initializing
        for (int j = 0; j < n; j++)
        {

            for (int i = j + 1; i < n; i++)
            {
                int cnt = 0;
                if (dfa._final_states.Contains(dfa._states[j]))
                {
                    cnt++;
                }
                if (dfa._final_states.Contains(dfa._states[i]))
                {
                    cnt++;
                }
                if (cnt == 1)
                {
                    table[i, j] = true;
                }
            }
        }

        while (true)
        {
            int changes = 0;
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (table[i, j] == false)
                    {
                        foreach (string l in dfa._input_symbols)
                        {
                            State qa = dfa._states[i].dtransitions[l];
                            State qj = dfa._states[j].dtransitions[l];
                            int q1 = dfa._states.FindIndex(y => y.Name == qa.Name);
                            int q2 = dfa._states.FindIndex(y => y.Name == qj.Name);
                            if (table[q1, q2] == true || table[q2, q1] == true)
                            {
                                changes++;
                                table[j, j] = true;
                                table[j, i] = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (changes == 0) { break; }

        }
        List<List<int>> combine = new List<List<int>>();
        for (int i = 0; i < n; i++)
        {
            combine.Add(new List<int>());
        }

        for (int j = 0; j < n; j++)
        {
            combine[j].Add(j);
            for (int i = j + 1; i < n; i++)
            {

                if (table[i, j] == false)
                {
                    combine[i].Add(j);
                    combine[j].Add(i);
                }
            }
        }
        for (int i = 0; i < n; i++)
        {
            combine[i].Sort();
        }
        var new_states = combine.Distinct().ToList();
        List<State> states = new List<State>();// states of final dfa 
        List<State> final_states = new List<State>();
        for (int i = 0; i < new_states.Count; i++)
        {
            string name = "";
            for (int k = 0; k < new_states[i].Count; k++)
            {
                name += "q" + new_states[i][k].ToString();
            }
            State state = new State(name);
            states.Add(state);
            string nam = "q" + new_states[i].First().ToString();
            for (int j = 0; j < dfa._final_states.Count; j++)
            {
                if (dfa._final_states[j].Name == nam)
                {
                    final_states.Add(state);
                }
            }
        }//biulding new states with new names
        for (int i = 0; i < new_states.Count; i++)
        {

            var transition = dfa._states[new_states[i].First()].dtransitions;
            for (int j = 0; j < dfa._input_symbols.Count; j++)
            {
                State x = transition[dfa._input_symbols[j]];
                int q1 = dfa._states.FindIndex(y => y.Name == x.Name);//index of previous state
                int qj = new_states.FindIndex(y => y.Contains(q1));//index of new state
                states[i].dtransitions.Add(dfa._input_symbols[j], states[qj]);
            }
        }

        return new DFA(states, states[0], final_states, dfa._input_symbols);
    }
}
