using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public class Lexer
    {
        protected int position;
        protected char currentCh; // очередной считанный символ
        protected int currentCharValue; // целое значение очередного считанного символа
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            var o = new StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new string(' ', position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            currentCharValue = inputReader.Read();
            currentCh = (char)currentCharValue;
            position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }
        protected bool IsReadingEnd() => currentCharValue == -1;

        protected char Take(Func<char, bool> checker)
        {
            if (checker(currentCh))
            {
                var ch = currentCh;
                NextCh();
                return ch;
            }
            throw new LexerException("");
        }
        protected string Take(string str)
        {
            foreach (var ch in str)
                Take(c => c == ch);
            return str;
        }
        protected string TakeGroup(Func<char, bool> checker)
        {
            var groupBuilder = new StringBuilder();
            while (checker(currentCh) && !IsReadingEnd())
            {
                groupBuilder.Append(currentCh);
                NextCh();
            }
            return groupBuilder.ToString();
        }
        protected string TakeGroupEndBy(string stopWord)
        {
            var stopWordCurrentCheckingPosition = 0;
            var groupBuilder = new StringBuilder();
            while (!IsReadingEnd() && stopWordCurrentCheckingPosition < stopWord.Length)
            {
                if (currentCh == stopWord[stopWordCurrentCheckingPosition]) ++stopWordCurrentCheckingPosition;
                else stopWordCurrentCheckingPosition = 0;
                groupBuilder.Append(currentCh);
                NextCh();
            }
            if (stopWordCurrentCheckingPosition != stopWord.Length) throw new LexerException("");
            return groupBuilder.ToString();
        }
        protected char Take()
        {
            var ch = currentCh;
            NextCh();
            return ch;
        }
    }

    public class IntLexer : Lexer
    {
        protected StringBuilder intString;
        public int parseResult = 0;

        public IntLexer(string input) : base(input)
            => intString = new StringBuilder();
        private bool IsNegative()
        {
            var isNegative = currentCh == '-';
            if (currentCh == '+' || isNegative) NextCh();
            return isNegative;
        }
        public override bool Parse()
        {
            NextCh();
            var isNegative = IsNegative();
            if (IsReadingEnd()) Error();
            while (!IsReadingEnd())
                intString.Append(Take(char.IsDigit));
            parseResult = int.Parse(intString.ToString()) * (isNegative ? -1 : 1);
            return true;
        }
    }

    public class IdentLexer : Lexer
    {
        protected StringBuilder builder;
        private string parseResult;
        public string ParseResult
        {
            get => parseResult;
        }

        public IdentLexer(string input) : base(input)
            => builder = new StringBuilder();
        protected virtual bool IsAcceptableSymbol(char symbol) => char.IsLetterOrDigit(symbol) || symbol == '_';
        public override bool Parse()
        {
            NextCh();
            if (IsReadingEnd()) Error();
            while (IsAcceptableSymbol(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            if (!IsReadingEnd()) Error();
            parseResult = builder.ToString();
            return true;
        }

    }

    public class IntNoZeroLexer : IntLexer
    {
        public IntNoZeroLexer(string input)
            : base(input)
        {
        }

        public override bool Parse()
        {
            NextCh();
            var isNegative = currentCh == '-';
            if (currentCh == '+' || isNegative) NextCh();
            if (IsReadingEnd() || currentCh == '0') Error();
            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            if (!IsReadingEnd()) Error();
            parseResult = int.Parse(intString.ToString());
            if (isNegative) parseResult *= -1;
            return true;

        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (IsReadingEnd()) Error();
            while (!IsReadingEnd())
            {
                builder.Append(Take(char.IsLetter));
                if (IsReadingEnd()) break;
                builder.Append(Take(char.IsDigit));
            }
            parseResult = builder.ToString();
            return true;
        }

    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;

        public List<char> ParseResult
        {
            get { return parseResult; }
        }

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
        }

        public override bool Parse()
        {
            var builder = new List<char>();

            NextCh();
            if (IsReadingEnd()) Error();
            if (char.IsLetter(currentCh)) builder.Add(currentCh);
            NextCh();

            while (!IsReadingEnd())
            {
                Take(c => c == ',' || c == ';');
                builder.Add(Take(char.IsLetter));
            }
            parseResult = builder;
            return true;
        }
    }

    public class DigitListLexer : Lexer
    {
        protected List<int> parseResult;

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input)
            : base(input)
        {
            parseResult = new List<int>();
        }

        private int SkipSpaces()
        {
            var countSpaces = 0;
            while (currentCh == ' ' && !IsReadingEnd())
            {
                NextCh();
                countSpaces++;
            }
            return countSpaces;
        }
        public override bool Parse()
        {
            var builder = new List<int>();

            NextCh();
            if (IsReadingEnd() || !char.IsDigit(currentCh)) Error();
            builder.Add(currentCh - '0');
            NextCh();

            while (!IsReadingEnd())
            {
                var countSpaces = SkipSpaces();
                if (IsReadingEnd() || countSpaces < 1) throw new LexerException("");
                builder.Add(Take(char.IsDigit) - '0');
            }
            parseResult = builder;
            return true;
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        private string TakeGroup(Func<char, bool> checker)
        {
            string group = Take(checker).ToString();
            if (checker(currentCh))
            {
                group += currentCh;
                NextCh();
            }
            return group;
        }

        public override bool Parse()
        {
            NextCh();
            if (IsReadingEnd()) Error();
            builder.Append(TakeGroup(char.IsLetter));

            while (!IsReadingEnd())
            {
                builder.Append(TakeGroup(char.IsDigit));
                if (IsReadingEnd()) break;
                builder.Append(TakeGroup(char.IsLetter));
            }
            parseResult = builder.ToString();
            return true;
        }

    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;

        public double ParseResult
        {
            get { return parseResult; }

        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }
        string TakeNumberGroup()
        {
            var builder = new StringBuilder();
            while (char.IsDigit(currentCh))
                builder.Append(Take());
            return builder.ToString();
        }
        string TakeNumberPart()
        {
            if (IsReadingEnd()) throw new LexerException("");
            var numberPart = TakeNumberGroup();
            if (numberPart.Length == 0) Error();
            return numberPart;
        }
        public override bool Parse()
        {
            NextCh();
            builder.Append(TakeNumberPart());
            if (!IsReadingEnd())
            {
                builder.Append(Take(c => c == '.'));
                builder.Append(TakeNumberPart());
            }
            parseResult = double.Parse(builder.ToString(), CultureInfo.InvariantCulture);
            return true;
        }

    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            builder.Append(Take(c => c == '\''));
            builder.Append(TakeGroup(c => c != '\''));
            builder.Append(Take(c => c == '\''));
            if (!IsReadingEnd()) throw new LexerException("");
            parseResult = builder.ToString();
            return true;
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        
        public override bool Parse()
        {
            NextCh();
            builder.Append(Take("/*"));
            builder.Append(TakeGroupEndBy("*/"));
            if (!IsReadingEnd()) throw new LexerException("");
            parseResult = builder.ToString();
            return true;
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }

        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        string TakeId()
        {
            var letter = Take(char.IsLetter);
            var id = TakeGroup(c => char.IsLetterOrDigit(c) || c == '_');
            if (id.Length == 0) throw new LexerException("");
            return letter + id;
        }

        public override bool Parse()
        {
            var builder = new List<string>();

            NextCh();
            if (IsReadingEnd()) Error();
            builder.Add(TakeId());

            while (!IsReadingEnd())
            {
                Take(c => c == '.');
                builder.Add(TakeId());
            }
            parseResult = builder;
            return true; ;
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}