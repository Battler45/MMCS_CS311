using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;


namespace SimpleLang.Visitors
{
    public class MaxIfCycleNestVisitor : AutoVisitor
    {
        public int MaxNest = 0;
        private int IfCycleOpenedBodyCount { get; set; }
        void OpenBody()
        {
            ++IfCycleOpenedBodyCount;
            if (IfCycleOpenedBodyCount > MaxNest) MaxNest = IfCycleOpenedBodyCount;
        }
        void CloseBody() => --IfCycleOpenedBodyCount;
        public override void VisitCycleNode(CycleNode c)
        {
            OpenBody();
            //c.Expr.Visit(this);
            c.Stat.Visit(this);
            CloseBody();
        }
        public override void VisitIfNode(IfNode w)
        {
            //w.Expr.Visit(this);
            OpenBody();
            w.ThenStat.Visit(this);
            if (w.ElseStat != null)
                w.ElseStat.Visit(this);
            CloseBody();
        }
    }
}