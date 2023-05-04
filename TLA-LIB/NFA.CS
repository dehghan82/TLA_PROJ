using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Configuration;
using System.Text;

namespace TLA_LIB;

public class NFA : IAutomata
{
    public NFA(List<State> states, State initial_state, List<State> final_states, List<string> input_symbols, Dictionary<string, Dictionary<string, string>> transitions)
    {
        _states = states;
        _initial_state = initial_state;
        _final_states = final_states;
        _input_symbols = input_symbols;
        for (int i = 0; i < _states.Count(); i++)
        {
            var trans = transitions[_states[i].Name];
            _states[i].ntransitions = new Dictionary<string, List<State>>();
            foreach (var item in trans)
            {
                var sta = Regex.Replace(item.Value, @"[{}']", "").Split(",").ToList();
                var tr = sta.Select(x => _states[int.Parse(x.Substring(1))]).ToList();
                _states[i].ntransitions.Add(item.Key, tr);
            }
        }
    }
    public NFA(List<State> states, State initial_state, List<State> final_states, List<string> input_symbols)
    {
        _states = states;
        _initial_state = initial_state;
        _final_states = final_states;
        _input_symbols = input_symbols;
    }
    public Dictionary<State, Dictionary<string, List<State>>> _transitions
    {
        get
        {
            var m = new Dictionary<State, Dictionary<string, List<State>>>();
            foreach (var item in _states)
                m.Add(item, item.ntransitions);
            return m;
        }
    }
    public List<State> _states { get; set; }
    public State _initial_state { get; set; }
    public List<State> _final_states { get; set; }
    public List<string> _input_symbols { get; set; }
    public FA SetR()
    {
        string states = "{" + string.Join(',', _states.Select(x => $"'{x.Name}'")) + "}";
        string final_states = "{" + string.Join(',', _final_states.Select(x => $"'{x.Name}'")) + "}";
        string initial_state = _initial_state.Name;
        string input_symbols = "{" + string.Join(',', _input_symbols.Select(x => $"'{x}'")) + "}";
        var trans = new Dictionary<string, Dictionary<string, string>>();
        foreach (var item in _states)
        {
            trans.Add(item.Name, item.transitions);
        }
        return new FA() { states = states, final_states = final_states, initial_state = initial_state, input_symbols = input_symbols, transitions = trans };
    }

    public NFA to_NFA() => this;
    public DFA to_DFA() => null;
    public string accpet_reject(string s)
    {
        State root = _initial_state;
        Queue<Tuple<int, State>> que = new Queue<Tuple<int, State>>();
        que.Enqueue(new Tuple<int, State>(0, root));
        int i = 0;
        while (que.Count() != 0)
        {
            var m = que.Dequeue();
            i = m.Item1;
            root = m.Item2;
            if (i == s.Length && _final_states.Contains(root))
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
}