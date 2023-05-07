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
        var d = File.ReadAllText(@"..\Results\phase1-sample\in\input2.json");
        var fa_in = JsonSerializer.Deserialize<FA>(d);
        IAutomata q = fa_in.set();
        NFA nfa = q.to_NFA();
        var dfa =NFA_TO_DFA(nfa);
        FA fa_out= dfa.SetR();
        string jason = JsonSerializer.Serialize(fa_out,new JsonSerializerOptions {WriteIndented = true});
        jason = Regex.Unescape(jason);
        File.WriteAllText(@"..\Results\phase1-sample\out\RFA2.json",jason,Encoding.UTF8);   
    }
    
    static public DFA NFA_TO_DFA(NFA nfa)
    {
        Stack<Tuple<List<State>, State>> queue = new Stack<Tuple<List<State>, State>>();
        List<State> s = new List<State>();
        List<string> str = new List<string>();
        List<State> finals = new List<State>();
        State Trap = new State("TRAP");
        Trap.dtransitions = new Dictionary<string, State>();
        int o = 0;
        foreach (var item in nfa._input_symbols)
        {
            Trap.dtransitions.Add(item, Trap);
        }
        var s0 = new State(nfa._initial_state.Name);
        s.Add(s0);
        queue.Push(new Tuple<List<State>, State>(new List<State> { nfa._initial_state }, s0));
        while (queue.Count() != 0)
        {
            var que = queue.Pop();
            var s1 = que.Item2;
            s1.dtransitions = new Dictionary<string, State>();
            var mm = que.Item1;
            for (int i = 0; i < nfa._input_symbols.Count(); i++)
            {
                List<State> mv = new List<State>();
                StringBuilder sb = new StringBuilder();
                int j = 0;
                foreach (var tmp in mm)
                {
                    if (tmp.ntransitions.ContainsKey(nfa._input_symbols[i]))
                        foreach (var item in tmp.ntransitions[nfa._input_symbols[i]])
                        {
                            if (!mv.Contains(item))
                            {
                                sb.Append(item.Name[1]);
                                mv.Add(item);
                                if (nfa._final_states.Contains(item))
                                    j++;
                            }
                            if (item.ntransitions.ContainsKey(""))
                                foreach (var it in item.ntransitions[""])
                                {
                                    if (!mv.Contains(it))
                                    {
                                        mv.Add(it);
                                        sb.Append(it.Name[1]);
                                        if (nfa._final_states.Contains(it))
                                            j++;
                                    }
                                }
                        }
                    if (tmp.ntransitions.ContainsKey(""))
                        foreach (var item in tmp.ntransitions[""])
                            if (item.ntransitions.ContainsKey(nfa._input_symbols[i]))
                                foreach (var it in item.ntransitions[nfa._input_symbols[i]])
                                {
                                    if (!mv.Contains(it))
                                    {
                                        mv.Add(it);
                                        sb.Append(it.Name[1]);
                                        if (nfa._final_states.Contains(it))
                                            j++;
                                    }
                                }
                }
                string name = FixName(sb);
                if (name == "")
                {
                    s1.dtransitions.Add(nfa._input_symbols[i], Trap);
                    o++;
                }
                else if (s.Count(x => x.Name == name) == 0)
                {
                    s1.dtransitions.Add(nfa._input_symbols[i], new State(name));
                    s.Add(s1.dtransitions[nfa._input_symbols[i]]);
                    queue.Push(new Tuple<List<State>, State>(mv, s1.dtransitions[nfa._input_symbols[i]]));
                }
                else
                {
                    var q = s.Where(x => x.Name == name).First();
                    s1.dtransitions.Add(nfa._input_symbols[i], q);
                }
                if (j != 0 && !finals.Contains(s1.dtransitions[nfa._input_symbols[i]]))
                    finals.Add(s1.dtransitions[nfa._input_symbols[i]]);
            }
        }
        if (o != 0)
            s.Add(Trap);
        return new DFA(s, s[0], finals, nfa._input_symbols);
    }
    static public string FixName(StringBuilder sb)
    {
        var s = sb.ToString().OrderBy(x=>x);
        StringBuilder sb2=new StringBuilder();
        foreach (var item in s)
        {
            sb2.Append("q");
            sb2.Append(item);
        }
        return sb2.ToString();
    }    
}
