namespace TLA_LIB;
public interface IAutomata
{
     List<State> _states{get;set;}
     State _initial_state{get;set;}
     List<State> _final_states{get;set;}
     List<string> _input_symbols{get;set;}
     FA SetR();
     NFA to_NFA();
     DFA to_DFA();
     string accpet_reject(string s);

}