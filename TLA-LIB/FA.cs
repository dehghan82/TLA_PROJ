using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Configuration;
namespace TLA_LIB;

public class FA
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }
    public IAutomata set()
    {
        var sta = Regex.Replace(states, @"[{}']", "").Split(",").ToList();
        var _states = sta.Select(x => new State(x)).ToList();
        var _input_symbols = Regex.Replace(input_symbols, @"[{}']", "").Split(",").ToList();
        sta = Regex.Replace(final_states, @"[{}']", "").Split(",").ToList();
        List<State> _final_states = new List<State>();
        for (int i = 0; i < sta.Count(); i++)
            _final_states.Add(_states.Where(x => x.Name == sta[i]).First());
        var _initial_state = _states.Where(x => x.Name == initial_state).First();
        var m = transitions.First().Value;
        if (m.First().Value.Contains('{'))
            return new NFA(_states, _initial_state, _final_states, _input_symbols, transitions);
        else
            return new DFA(_states, _initial_state, _final_states, _input_symbols, transitions);
    }
}
