using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sleepingmat
{
    internal class Lexer
    {
        private string inputString;
        private int _currentCharPosition = 0;
        private int _start = 0;
        private List<Token> _tks = new List<Token>();
        private int _line = 1;
        private int _char = 1;

        public Lexer(string code)
        {
            inputString = code;
        }

        public List<Token> GetAllTokens()
        {
            _tks.Clear();
            while (!IsEnd())
            {
                _start = _currentCharPosition;
                ScanToken();
            }
            _tks.Add(new Token(ETokenType.EOF, null, _line, _char));
            return _tks;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case ';': _tks.Add(new Token(ETokenType.Semicolon, null, _line, _char)); break;
                case '#': _tks.Add(new Token(ETokenType.Pound, null, _line, _char)); break;
                case ':': _tks.Add(new Token(ETokenType.Colon, null, _line, _char)); break;
                case '(': _tks.Add(new Token(ETokenType.LParen, null, _line, _char)); ; break;
                case ')': _tks.Add(new Token(ETokenType.RParen, null, _line, _char)); break;
                case '{': _tks.Add(new Token(ETokenType.LBrace, null, _line, _char)); break;
                case '}': _tks.Add(new Token(ETokenType.RBrace, null, _line, _char)); ; break;
                case '[': _tks.Add(new Token(ETokenType.LSquareBracket, null, _line, _char)); break;
                case ']': _tks.Add(new Token(ETokenType.RSquareBracket, null, _line, _char)); break;
                case '.': _tks.Add(new Token(ETokenType.Period, null, _line, _char)); break;
                case ',': _tks.Add(new Token(ETokenType.Comma, null, _line, _char)); break;
                case '"': GetStringLiteral(); break;
                case '\n': _line++; _char = 1; break;
                case ' ': case '\r': case '\t': break;
                default:
                    if (char.IsDigit(c))
                        GetNumberLiteral();
                    else if (IsAlpha(c))
                        GetIdentifier();
                    else
                        _tks.Add(new Token(ETokenType.UnknownTokenType, c.ToString(), _line, _char));
                    break;
            }
        }

        private char Advance()
        {
            _currentCharPosition++;
            _char++;
            return inputString[_currentCharPosition - 1];
        }

        private char GetCurrentChar()
        {
            if (!IsEnd())
                return inputString[_currentCharPosition];
            else
                return '\0';
        }

        private char GetNextChar()
        {
            if (!IsEnd())
                return inputString[_currentCharPosition + 1];
            else
                return '\0';
        }

        private bool IsMatchToNext(char e)
        {
            if (IsEnd())
                return false;
            if (GetCurrentChar() != e)
                return false;
            _currentCharPosition++;
            return true;
        }

        private void GetNumberLiteral()
        {
            bool f = false;
            while (char.IsDigit(GetCurrentChar()))
                Advance();
            if (GetCurrentChar() == '.' && char.IsDigit(GetNextChar()))
            {
                f = true;
                Advance();
                while (char.IsDigit(GetCurrentChar()))
                    Advance();
            }
            string s = inputString.Substring(_start, _currentCharPosition - _start).Replace(".", ",");
            if (!f)
                _tks.Add(new Token(ETokenType.NumberLiteral, int.Parse(s), _line, _char));
        }

        private void GetStringLiteral()
        {
            while (GetCurrentChar() != '"' && !IsEnd())
                Advance();
            Advance();
            _tks.Add(new Token(ETokenType.StringLiteral, inputString.Substring(_start + 1, _currentCharPosition - (_start + 2)), _line, _char));
        }

        private void GetIdentifier()
        {
            while (IsAlphaNumeric(GetCurrentChar()))
                Advance();
            Token t = null;
            string s = inputString.Substring(_start, _currentCharPosition - _start);
            t = new Token(ETokenType.Identifier, s, _line, _char);
            _tks.Add(t);
        }

        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        private bool IsEnd() => _currentCharPosition >= inputString.Length;
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || char.IsDigit(c);
    }
}
