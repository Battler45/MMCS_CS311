using System;
using System.Collections.Generic;
using System.Text;
using SimpleLexer;
namespace SimpleLangParser
{
    public class ParserException : System.Exception
    {
        public ParserException(string msg)
            : base(msg)
        {
        }

    }

    public class Parser
    {
        private SimpleLexer.Lexer l;

        public Parser(SimpleLexer.Lexer lexer)
        {
            l = lexer;
        }

        public void Progr()
        {
            Block();
        }

        public void Expr()
        {
            if (l.LexKind == Tok.ID || l.LexKind == Tok.INUM)
            {
                l.NextLexem();
                if (l.LexKind == Tok.PLUS || l.LexKind == Tok.MINUS || l.LexKind == Tok.MULT || l.LexKind == Tok.DIVISION)
                {
                    l.NextLexem();
                    Expr();
                }
            }
            else if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem();
                Expr();
                if (l.LexKind == Tok.RIGHT_BRACKET)
                    l.NextLexem();
                else
                    throw new LexerException("");
            }
            else
            {
                SyntaxError("expression expected");
            }
        }

        public void Assign()
        {
            l.NextLexem();  // пропуск id
            if (l.LexKind == Tok.ASSIGN)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError(":= expected");
            }
            Expr();
        }

        public void StatementList()
        {
            Statement();
            while (l.LexKind == Tok.SEMICOLON)
            {
                l.NextLexem();
                Statement();
            }
        }

        public void Statement()
        {
            switch (l.LexKind)
            {
                case Tok.BEGIN:
                    {
                        Block();
                        break;
                    }
                case Tok.CYCLE:
                    {
                        Cycle();
                        break;
                    }
                case Tok.ID:
                    {
                        Assign();
                        break;
                    }
                case Tok.WHILE:
                    {
                        While();
                        break;
                    }
                case Tok.FOR:
                    {
                        For();
                        break;
                    }
                case Tok.IF:
                    {
                        If();
                        break;
                    }
                default:
                    {
                        SyntaxError("Operator expected");
                        break;
                    }
            }
        }

        public void Block()
        {
            l.NextLexem();    // пропуск begin
            StatementList();
            if (l.LexKind == Tok.END)
            {
                l.NextLexem();
            }
            else
            {
                SyntaxError("end expected");
            }

        }

        public void Cycle()
        {
            l.NextLexem();  // пропуск cycle
            Expr();
            Statement();
        }

        //WHILE expr DO statement
        public void While()
        {
            l.NextLexem();  // пропуск while
            Expr();
            l.NextLexem();  // пропуск do
            Statement();
        }

        //FOR ID := expr TO expr DO statement
        public void For()
        {
            l.NextLexem();  // пропуск for
            Assign();
            l.NextLexem();  // пропуск to
            Expr();
            l.NextLexem();  // пропуск do
            Statement();
        }

        //IF expr THEN stat ELSE stat
        public void If()
        {
            l.NextLexem();  // пропуск if
            Expr();
            l.NextLexem();  // пропуск THEN
            Statement();
            if (l.LexKind == Tok.ELSE)
            {
                l.NextLexem();  // пропуск Else
                Statement();
            }
        }

        public void SyntaxError(string message)
        {
            var errorMessage = "Syntax error in line " + l.LexRow.ToString() + ":\n";
            errorMessage += l.FinishCurrentLine() + "\n";
            errorMessage += new String(' ', l.LexCol - 1) + "^\n";
            if (message != "")
            {
                errorMessage += message;
            }
            throw new ParserException(errorMessage);
        }

    }
}
