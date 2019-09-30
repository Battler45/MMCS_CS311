using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ExprComplexityVisitor : AutoVisitor
    {
        // список должен содержать сложность каждого выражения, встреченного при обычном порядке обхода AST
        public List<int> getComplexityList() => ComplexityExprs;
        private List<int> ComplexityExprs { get; set; } = new List<int>();
        public override void VisitIdNode(IdNode id)
        {
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            binop.Left.Visit(this);
            if (binop.Op == '+' || binop.Op == '-')
                ++ComplexityExprs[ComplexityExprs.Count - 1];
            else if (binop.Op == '*' || binop.Op == '/')
                ComplexityExprs[ComplexityExprs.Count - 1] += 3;
            binop.Right.Visit(this);
        }
        public override void VisitAssignNode(AssignNode a)
        {
            ComplexityExprs.Add(0);
            a.Expr.Visit(this);
        }
        public override void VisitCycleNode(CycleNode c)
        {
            ComplexityExprs.Add(0);
            c.Expr.Visit(this);
            c.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            foreach (var st in bl.StList)
                st.Visit(this);
        }
        public override void VisitWriteNode(WriteNode w)
        {
            ComplexityExprs.Add(0);
            w.Expr.Visit(this);
        }
        public override void VisitVarDefNode(VarDefNode w)
        {
        }
    }
}
