﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleLexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }
    }

    public enum Tok
    {
        EOF,
        ID,
        INUM,
        COLON,
        SEMICOLON,
        ASSIGN,
        BEGIN,
        END,
        CYCLE,
        COMMA,
        PLUS,
        MINUS,
        MULT,
        DIVISION,
        MOD,
        DIV,
        AND,
        OR,
        NOT,
        MULTASSIGN,
        DIVASSIGN,
        PLUSASSIGN,
        MINUSASSIGN,
        LT,  //lesser
        GT,  //greater
        LEQ, //less or equal
        GEQ, //greater or equal
        EQ,  //equal
        NEQ, //not equal
        WHILE,
        DO,
        FOR,
        TO,
        IF,
        THEN,
        ELSE,
        LEFT_BRACKET,
        RIGHT_BRACKET,
    }

    public class Lexer
    {
        private int position;
        private char currentCh;                      // Текущий символ
        public int LexRow, LexCol;                  // Строка-столбец начала лексемы. Конец лексемы = LexCol+LexText.Length
        private int row, col;                        // текущие строка и столбец в файле
        private TextReader inputReader;
        private Dictionary<string, Tok> keywordsMap; // Словарь, сопоставляющий ключевым словам константы типа TLex. Инициализируется процедурой InitKeywords 
        public Tok LexKind;                         // Тип лексемы
        public string LexText;                      // Текст лексемы
        public int LexValue;                        // Целое значение, связанное с лексемой LexNum

        private string CurrentLineText;  // Накапливает символы текущей строки для сообщений об ошибках


        public Lexer(TextReader input)
        {
            CurrentLineText = "";
            inputReader = input;
            keywordsMap = new Dictionary<string, Tok>();
            InitKeywords();
            row = 1; col = 0;
            NextCh();       // Считать первый символ в ch
            NextLexem();    // Считать первую лексему, заполнив LexText, LexKind и, возможно, LexValue
        }

        public void Init()
        {

        }

        private void PassSpaces()
        {
            while (char.IsWhiteSpace(currentCh))
            {
                NextCh();
            }
        }

        private void InitKeywords()
        {
            keywordsMap["begin"] = Tok.BEGIN;
            keywordsMap["end"] = Tok.END;
            keywordsMap["cycle"] = Tok.CYCLE;
            keywordsMap["div"] = Tok.DIV;
            keywordsMap["mod"] = Tok.MOD;
            keywordsMap["and"] = Tok.AND;
            keywordsMap["or"] = Tok.OR;
            keywordsMap["not"] = Tok.NOT;
            keywordsMap["while"] = Tok.WHILE;
            keywordsMap["do"] = Tok.DO;
            keywordsMap["for"] = Tok.FOR;
            keywordsMap["to"] = Tok.TO;
            keywordsMap["if"] = Tok.IF;
            keywordsMap["then"] = Tok.THEN;
            keywordsMap["else"] = Tok.ELSE;
        }

        public string FinishCurrentLine()
        {
            return CurrentLineText + inputReader.ReadLine();
        }

        private void LexError(string message)
        {
            var errorDescription = new StringBuilder();
            errorDescription.AppendFormat("Lexical error in line {0}:", row);
            errorDescription.Append("\n");
            errorDescription.Append(FinishCurrentLine());
            errorDescription.Append("\n");
            errorDescription.Append(new string(' ', col - 1) + '^');
            errorDescription.Append('\n');
            if (message != "")
            {
                errorDescription.Append(message);
            }
            throw new LexerException(errorDescription.ToString());
        }

        private void NextCh()
        {
            // В LexText накапливается предыдущий символ и считывается следующий символ
            LexText += currentCh;
            var nextChar = inputReader.Read();
            if (nextChar != -1)
            {
                currentCh = (char)nextChar;
                if (currentCh != '\n')
                {
                    col += 1;
                    CurrentLineText += currentCh;
                }
                else
                {
                    row += 1;
                    col = 0;
                    CurrentLineText = "";
                }
            }
            else
            {
                currentCh = (char)0; // если достигнут конец файла, то возвращается #0
            }
        }
        private void NextLine()
        {
            inputReader.ReadLine();
            NextCh();
        }
        private void SetNextLexKind(Tok tok)
        {
            NextCh();
            LexKind = tok;
        }
        private void PassAllBefore(char c)
        {
            while (currentCh != c && currentCh != 0)
                NextCh();
            if (currentCh == 0) throw new LexerException("");
            NextCh();
        }
        public void NextLexem()
        {
            PassSpaces();
            // R К этому моменту первый символ лексемы считан в ch
            LexText = "";
            LexRow = row;
            LexCol = col;
            // Тип лексемы определяется по ее первому символу
            // Для каждой лексемы строится синтаксическая диаграмма
            if (currentCh == ';')
                SetNextLexKind(Tok.SEMICOLON);
            else if (currentCh == ',')
                SetNextLexKind(Tok.COMMA);
            else if (currentCh == '=')
                SetNextLexKind(Tok.EQ);
            else if (currentCh == ':')
            {
                SetNextLexKind(Tok.COLON);
                if (currentCh == '=')
                    SetNextLexKind(Tok.ASSIGN);
            }
            else if (currentCh == '+')
            {
                SetNextLexKind(Tok.PLUS);
                if (currentCh == '=')
                    SetNextLexKind(Tok.PLUSASSIGN);
            }
            else if (currentCh == '-')
            {
                SetNextLexKind(Tok.MINUS);
                if (currentCh == '=')
                    SetNextLexKind(Tok.MINUSASSIGN);
            }
            else if (currentCh == '*')
            {
                SetNextLexKind(Tok.MULT);
                if (currentCh == '=')
                    SetNextLexKind(Tok.MULTASSIGN);
            }
            else if (currentCh == '/')
            {
                SetNextLexKind(Tok.DIVISION);
                if (currentCh == '=')
                    SetNextLexKind(Tok.DIVASSIGN);
                else if (currentCh == '/')
                {
                    NextLine();
                    NextLexem();
                }
            }
            else if (currentCh == '{')
            {
                PassAllBefore('}');
                NextLexem();
            }
            else if (currentCh == '(')
                SetNextLexKind(Tok.LEFT_BRACKET);
            else if (currentCh == ')')
                SetNextLexKind(Tok.RIGHT_BRACKET);
            else if (currentCh == '>')
            {
                SetNextLexKind(Tok.GT);
                if (currentCh == '=')
                    SetNextLexKind(Tok.GEQ);
            }
            else if (currentCh == '<')
            {
                SetNextLexKind(Tok.LT);
                if (currentCh == '=')
                    SetNextLexKind(Tok.LEQ);
                else if (currentCh == '>')
                    SetNextLexKind(Tok.NEQ);
            }
            else if (char.IsLetter(currentCh))
            {
                while (char.IsLetterOrDigit(currentCh))
                {
                    NextCh();
                }
                if (keywordsMap.ContainsKey(LexText))
                {
                    LexKind = keywordsMap[LexText];
                }
                else
                {
                    LexKind = Tok.ID;
                }
            }
            else if (char.IsDigit(currentCh))
            {
                while (char.IsDigit(currentCh))
                {
                    NextCh();
                }
                LexValue = Int32.Parse(LexText);
                LexKind = Tok.INUM;
            }
            else if ((int)currentCh == 0)
            {
                LexKind = Tok.EOF;
            }
            else
            {
                LexError("Incorrect symbol " + currentCh);
            }
        }

        public virtual void ParseToConsole()
        {
            do
            {
                Console.WriteLine(TokToString(LexKind));
                NextLexem();
            } while (LexKind != Tok.EOF);
        }

        public string TokToString(Tok t)
        {
            var result = t.ToString();
            switch (t)
            {
                case Tok.ID:
                    result += ' ' + LexText;
                    break;
                case Tok.INUM:
                    result += ' ' + LexValue.ToString();
                    break;
            }
            return result;
        }
    }
}