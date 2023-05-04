using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Configuration;

namespace TLA_LIB;

public class DFA : IAutomata
{
    public DFA(List<State> states, State initial_state, List<State> final_states,

                     List<string> input_symbols, Dictionary<string, Dictionary<string, string>> transitions)
    {
        _states = states;
        _initial_state = initial_state;
        _final_states = final_states;
        _input_symbols = input_symbols;
        for (int i = 0; i < _states.Count(); i++)
        {
            var trans = transitions[_states[i].Name];
            _states[i].dtransitions = new Dictionary<string, State>();
            foreach (var item in trans)
            {
                int q = _states.FindIndex(x => x.Name == item.Value);
                _states[i].dtransitions.Add(item.Key, _states[q]);
            }
        }
    }

    public DFA(List<State> states, State initial_state, List<State> final_states, List<string> input_symbols)
    {
        _states = states;
        _initial_state = initial_state;
        _final_states = final_states;
        _input_symbols = input_symbols;
    }

    public Dictionary<State, Dictionary<string, State>> _transitions
    {
        get
        {
            var m = new Dictionary<State, Dictionary<string, State>>();
            foreach (var item in _states)
                m.Add(item, item.dtransitions);
            return m;
        }
    }
    public List<State> _states { get; set; }
    public State _initial_state { get; set; }
    public List<State> _final_states { get; set; }
    public List<string> _input_symbols { get; set; }
    public FA SetR()
    {
        string states = "{" + string.Join(',', _states.Select(x => $"\u0027{x.Name}\u0027")) + "}";
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

    public NFA to_NFA()
    {
        foreach (var item in this._states)
        {
            item.ntransitions = new Dictionary<string, List<State>>();
            foreach (var tran in item.dtransitions)
            {
                item.ntransitions.Add(tran.Key, new List<State> { tran.Value });
            }
            item.dtransitions = null;
        }
        return new NFA(this._states, this._initial_state, this._final_states, this._input_symbols);
    }
    public DFA to_DFA() => this;
    public string accpet_reject(string s)
    {
        State root = _initial_state;
        for (int i = 0; i < s.Length; i++)
        {
            root = root.dtransitions[s[i].ToString()];
        }
        if (_final_states.Contains(root))
            return "Accepted";
        else
            return "Rejected";
    }

}
