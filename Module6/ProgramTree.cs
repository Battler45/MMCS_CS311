using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class RepeatNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public RepeatNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class ForNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public StatementNode Assign { get; set; }
        public ForNode(StatementNode assign, ExprNode expr, StatementNode stat)
        {
            Assign = assign;
            Expr = expr;
            Stat = stat;
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public WriteNode(ExprNode expr)
        {
            Expr = expr;
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat0 { get; set; }
        public StatementNode Stat1 { get; set; }
        public IfNode(ExprNode expr, StatementNode stat0)
        {
            Expr = expr;
            Stat0 = stat0;
        }
        public IfNode(ExprNode expr, StatementNode stat0, StatementNode stat1)
        {
            Expr = expr;
            Stat0 = stat0;
            Stat1 = stat1;
        }
    }
    
    public class VarDefNode : StatementNode
    {
        public IdentListNode IdentList { get; set; }
        public VarDefNode(IdentListNode identList)
        {
            IdentList = identList;
        }
    }
    
    public class IdentListNode : StatementNode
    {
        public List<IdNode> StList = new List<IdNode>();
        public IdentListNode(IdNode id)
        {
            Add(id);
        }
        public void Add(IdNode id)
        {
            StList.Add(id);
        }
    }


    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

    public enum OperationType { Plus, Minus, Mult, Divide };
    public class ExprOperationNode : ExprNode
    {
        public OperationType Operation { get; set; }
        public ExprOperationNode(string token)
        { 
        
        }
    }
    public class BinaryNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public string Op { get; set; }
        public BinaryNode(ExprNode left, ExprNode right, string op)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }
}