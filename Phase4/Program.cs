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
        var d = File.ReadAllText(@"..\Results\phase4-sample\concat\in\FA1.json");
        var fa_in = JsonSerializer.Deserialize<FA>(d);
        d = File.ReadAllText(@"..\Results\phase4-sample\concat\in\FA2.json");
        var fa_in2 = JsonSerializer.Deserialize<FA>(d);
        FA fa_out = Phase4_concat(fa_in,fa_in2);


        // var d = File.ReadAllText(@"..\Results\phase4-sample\union\in\FA1.json");
        // var fa_in = JsonSerializer.Deserialize<FA>(d);
        // d = File.ReadAllText(@"..\Results\phase4-sample\union\in\FA2.json");
        // var fa_in2 = JsonSerializer.Deserialize<FA>(d);
        // FA fa_out = Phase4_union(fa_in,fa_in2);


        // var d = File.ReadAllText(@"..\Results\phase4-sample\star\in\FA1.json");
        // var fa_in = JsonSerializer.Deserialize<FA>(d);
        // FA fa_out = Phase4_Star(fa_in);

        string jason = JsonSerializer.Serialize(fa_out,new JsonSerializerOptions {WriteIndented = true});
        jason = Regex.Unescape(jason);
        File.WriteAllText(@"..\Results\phase4-sample\concat\out\RFA2.json",jason,Encoding.UTF8);   
        // File.WriteAllText(@"..\Results\phase4-sample\union\out\RFA2.json",jason,Encoding.UTF8);   
        // File.WriteAllText(@"..\Results\phase4-sample\star\out\RFA2.json",jason,Encoding.UTF8);   
    }
    static public FA Phase4_union(FA jsn,FA jsn2)
    {
        IAutomata q = jsn.set();
        NFA nfa = q.to_NFA();
         q = jsn2.set();
        NFA nfa2 = q.to_NFA();
        Union(nfa,nfa2);
        return nfa.SetR();
    }
    
    static public FA Phase4_concat(FA jsn,FA jsn2)
    {
        IAutomata q = jsn.set();
        NFA nfa = q.to_NFA();
         q = jsn2.set();
        NFA nfa2 = q.to_NFA();
        Concat(nfa,nfa2);
        return nfa.SetR();
    }


    static public FA Phase4_Star(FA jsn)
    {
        IAutomata q = jsn.set();
        NFA nfa = q.to_NFA();
        Star(nfa);
        return nfa.SetR();
    }
    
    static public NFA Star(NFA nfa)
    {
        int len =nfa. _states.Count();
        State initial = new State($"q{len}");
        State final = new State($"q{len + 1}");
        nfa._states.Add(initial); nfa._states.Add(final);
        initial.ntransitions = new Dictionary<string, List<State>>() { { "", new List<State> { nfa._initial_state, final } } };
        final.ntransitions = new Dictionary<string, List<State>>() { { "", new List<State> { initial } } };
        foreach (var item in nfa._final_states)
        {
            if (item.ntransitions.Keys.Contains(""))
                item.ntransitions[""].Add(final);
            else
                item.ntransitions.Add("", new List<State> { final });
        }
        nfa._final_states = new List<State> { final };
        nfa._initial_state = initial;
        return nfa;
    }
    static public void Union(NFA nfa,NFA nfa2)
    {
        int len = nfa._states.Count() + nfa2._states.Count();
        State initial = new State($"q{len}");
        State final = new State($"q{len + 1}");
        initial.ntransitions = new Dictionary<string, List<State>>() { { "", new List<State>() { nfa._initial_state, nfa2._initial_state } } };
        foreach (var item in nfa._final_states)

            if (item.ntransitions.ContainsKey(""))
                item.ntransitions[""].Add(final);
            else
                item.ntransitions.Add("", new List<State>() { final });

        foreach (var item in nfa2._final_states)

            if (item.ntransitions.ContainsKey(""))
                item.ntransitions[""].Add(final);
            else
                item.ntransitions.Add("", new List<State>() { final });
        nfa._input_symbols = nfa._input_symbols.Concat(nfa2._input_symbols).ToList();
        int nn = nfa._states.Count();
        nfa2._states.ForEach(x => x.Name = $"q{nn++}");
        nfa._states = nfa._states.Concat(nfa2._states).ToList();
        nfa._states.Add(initial);
        nfa._states.Add(final);
        final.ntransitions = new Dictionary<string, List<State>>();
        nfa._initial_state = initial;
        nfa._final_states = new List<State>() { final };
    }

    static public void Concat(NFA nfa,NFA nfa2)
    {
        nfa._input_symbols = nfa._input_symbols.Concat(nfa2._input_symbols).ToList();
        int len = nfa._states.Count() + nfa2._states.Count();
        State final = new State($"q{len}");
        final.ntransitions=new Dictionary<string, List<State>>();
        int nn = nfa._states.Count();
        nfa2._states.ForEach(x => x.Name = $"q{nn++}");
        foreach (var item in nfa._final_states)
        {
            if (item.ntransitions.ContainsKey(""))
                item.ntransitions[""].Add(nfa2._initial_state);
            else
                item.ntransitions.Add("", new List<State>() { nfa2._initial_state });
        }
        foreach (var item in nfa2._final_states)
        {
            if (item.ntransitions.ContainsKey(""))
                item.ntransitions[""].Add(final);
            else
                item.ntransitions.Add("", new List<State>() { final });
        }
        nfa._final_states=new List<State>(){final};
        nfa._states = nfa._states.Concat(nfa2._states).ToList();
        nfa._states.Add(final);
    }
}
