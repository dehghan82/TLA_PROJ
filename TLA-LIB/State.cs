using System;
namespace TLA_LIB;
public class State
{
    public State(string name)
    {
        Name = name;
    }
    public string Name;
    public Dictionary<string, string> transitions
    {
        get
        {
            Dictionary<string, string> m = new Dictionary<string, string>();
            if (dtransitions != null)
                foreach (var item in dtransitions)
                    m.Add(item.Key, item.Value.Name);
            else
            {
                foreach (var item in ntransitions)
                {
                    var q = "{" + string.Join(',', item.Value.Select(x => $"'{x.Name}'")) + "}";
                    m.Add(item.Key, q);
                }
            }
            return m;
        }
    }
    public Dictionary<string, State> dtransitions;
    public Dictionary<string, List<State>> ntransitions;
}
