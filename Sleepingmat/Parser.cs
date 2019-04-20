using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sleepingmat
{
    public class Parser
    {
        private Token[] tokens;
        private Lexer lex;

        private int pos = -1;

        public Dictionary<string, string> settings = new Dictionary<string, string>();

        public List<SettingObject> settingObjects = new List<SettingObject>();

        public Exception Parse(string input)
        {
            lex = new Lexer(input);
            tokens = lex.GetAllTokens().ToArray();

            return Build();
        }

        private Exception Build()
        {
            SettingObject current = null;
            while(!IsEnd())
            {
                Token t = Advance();
                if(t.Type == ETokenType.Identifier)
                {
                    if(NextIs(ETokenType.Colon))
                    {
                        Advance(); // colon
                        if(NextIs(ETokenType.StringLiteral) || NextIs(ETokenType.NumberLiteral))
                        {
                            if(current != null)
                            {
                                current.SetValue(t.Value, Advance().Value);
                            }
                            else
                            {
                                SetSetting(t.Value, Advance().Value);
                            }
                            if (!NextIs(ETokenType.Semicolon))
                            {
                                return new Exception("Lack of semicolon on line " + tokens[pos].line + ", char " + tokens[pos].character);
                            }
                            Advance();
                        }
                        else
                        {
                            t = Advance();
                            return new Exception("Unexpected thing: '" + t.Value + "' on line " + t.line + ", char " + t.character);
                        }
                    }
                    else if(NextIs(ETokenType.LBrace))
                    {
                        Advance();

                        if (NextIs(ETokenType.Identifier))
                        {
                            Token i = Advance(); // identifier

                            if(NextIs(ETokenType.Colon))
                            {
                                Advance(); // colon
                                if(NextIs(ETokenType.StringLiteral) || NextIs(ETokenType.NumberLiteral))
                                {
                                    if (current == null)
                                    {
                                        current = new SettingObject();
                                        current.name = t.Value.ToString();
                                    }

                                    Token j = Advance();
                                    current.SetValue(i.Value, j.Value);
                                    if (!NextIs(ETokenType.Semicolon))
                                    {
                                        return new Exception("Lack of semicolon on line " + tokens[pos].line + ", char " + tokens[pos].character);
                                    }
                                    Advance();
                                }
                            }
                        }
                        else
                        {
                            return new Exception("Identifier expected on line " + t.line + ", char " + t.character);
                        }
                    }
                    else if(NextIs(ETokenType.RBrace))
                    {
                        Advance();
                        settingObjects.Add(current);
                        current = null;
                    }
                }
                else if (t.Type == ETokenType.RBrace)
                {
                    Advance();
                    settingObjects.Add(current);
                    current = null;
                }
                else
                {
                    return new Exception("Identifier expected on line " + t.line + ", char " + t.character);
                }
            }

            return null;
        }

        private void SetSetting(object key, object value)
        {
            if(settings.ContainsKey(key.ToString()))
            {
                settings[key.ToString()] = value.ToString();
            }
            else
            {
                settings.Add(key.ToString(), value.ToString());
            }
        }

        private bool NextIs(ETokenType type)
        {
            if (IsEnd())
                return false;

            if (tokens[pos + 1].Type == type)
                return true;

            return false;
        }

        private Token Advance()
        {
            pos++;
            return tokens[pos];
        }

        private bool IsEnd()
        {
            if (pos == -1)
                return false;
            return pos >= tokens.Length || tokens[pos].Type == ETokenType.EOF;
        }
    }
}
