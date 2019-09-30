using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CountCyclesOpVisitor : AutoVisitor
    {
        
        public int MidCount()
        {
            return CycleCount == 0 ? 0 : CyclesOpCount / CycleCount;
        }
        private int CyclesOpCount = 0;
        private int CycleCount { get; set; }
        private int CycleOpenedBodyCount { get; set; }
        public override void VisitCycleNode(CycleNode c)
        {
            ++CycleCount;
            ++CycleOpenedBodyCount;
            c.Stat.Visit(this);
            --CycleOpenedBodyCount;
        }
        public override void VisitAssignNode(AssignNode a)
        {
            if (CycleOpenedBodyCount > 0)
                CyclesOpCount += 1;
        }
        public override void VisitBinOpNode(BinOpNode binop)
        {
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            foreach (var st in bl.StList)
                st.Visit(this);
        }
        public override void VisitWriteNode(WriteNode w)
        {
        }
        public override void VisitVarDefNode(VarDefNode w)
        {
        }
    }
}
