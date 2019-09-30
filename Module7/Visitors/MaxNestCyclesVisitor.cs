using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class MaxNestCyclesVisitor : AutoVisitor
    {
        public int MaxNest = 0;
        private int CycleOpenedBodyCount { get; set; }
        public override void VisitCycleNode(CycleNode c)
        {
            ++CycleOpenedBodyCount;
            if (CycleOpenedBodyCount > MaxNest) MaxNest = CycleOpenedBodyCount;
            c.Expr.Visit(this);
            c.Stat.Visit(this);
            --CycleOpenedBodyCount;
        }
    }
}
