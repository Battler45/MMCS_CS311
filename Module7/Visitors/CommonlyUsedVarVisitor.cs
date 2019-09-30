using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CommonlyUsedVarVisitor : AutoVisitor
    {
        public string mostCommonlyUsedVar()
            => VariablesStatistics.Count == 0 ? "" : VariablesStatistics.Aggregate((l, r) => l.Value > r.Value ? l : r).Key
            ;
        private Dictionary<string, int> VariablesStatistics { get; set; } = new Dictionary<string, int>();
        public override void VisitIdNode(IdNode id) 
        {
            if (!VariablesStatistics.ContainsKey(id.Name))
                VariablesStatistics.Add(id.Name, 0);
            ++VariablesStatistics[id.Name];
        }
    }
}
