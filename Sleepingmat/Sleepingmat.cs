using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sleepingmat
{
    public class Mat
    {
        private Token[] tokens;
        private Lexer lex;
        private int pos = -1;

        /// <summary>
        /// All the key/value pairs not inside a block.
        /// </summary>
        public Dictionary<string, string> settings = new Dictionary<string, string>();

        /// <summary>
        /// All the blocks.
        /// </summary>
        public List<Block> blocks = new List<Block>();

        /// <summary>
        /// Parses some text formatted as a sleepingmat file. This does not take a file path.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Exception Parse(string input)
        {
            lex = new Lexer(input);
            tokens = lex.GetAllTokens().ToArray();

            return Build();
        }

        /// <summary>
        /// Sets a setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value. ToString() is used to convert.</param>
        public void SetValue(string key, object value)
        {
            if (settings.ContainsKey(key))
                settings[key] = value.ToString();
            else
                settings.Add(key, value.ToString());
        }

        /// <summary>
        /// Gets a setting. Returns false if it didn't exist.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The returned value</param>
        /// <returns></returns>
        public bool GetValue(string key, out string value)
        {
            if(settings.ContainsKey(key))
            {
                value = settings[key];
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Checks if a key exists in the file.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns></returns>
        public bool ValueExists(string key)
        {
            return (settings.ContainsKey(key));
        }

        /// <summary>
        /// Checks if a key exists in the first block with the specified name.
        /// Returns false if the block does not exist.
        /// </summary>
        /// <param name="block">The name of the block to check.</param>
        /// <param name="key">The key to check in the block.</param>
        /// <returns></returns>
        public bool ValueExists(string block, string key)
        {
            if(GetBlock(block, out Block b))
            {
                return b.ValueExists(key);
            }

            return false;
        }

        /// <summary>
        /// Since an infinite number of blocks can have the same identifier, this returns all blocks with the same identifier. Use GetFirstBlock() if you only want one.
        /// </summary>
        /// <param name="name">Name of the blocks</param>
        /// <param name="blocks">The blocks found</param>
        /// <returns>Returns true if any blocks by the name were found</returns>
        public bool GetBlocks(string name, out Block[] blocks)
        {
            List<Block> temp = new List<Block>();
            foreach (Block b in this.blocks)
                if (b.name == name)
                    temp.Add(b);
            if (temp.Count > 0)
            {
                blocks = temp.ToArray();
                return true;
            }
            else
            {
                blocks = null;
                return false;
            }
        }

        /// <summary>
        /// Returns the first instance of a block with a specified name.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="block">The block found, or null if none</param>
        /// <returns>Returns true if a block was found</returns>
        public bool GetBlock(string name, out Block block)
        {
            foreach(Block b in blocks)
            {
                if(b.name == name)
                {
                    block = b;
                    return true;
                }
            }

            block = null;
            return false;
        }

        /// <summary>
        /// Checks if a block exists. o(n) speeeeeeed.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        public bool BlockExists(string name)
        {
            foreach (Block b in blocks)
                if (b.name == name)
                    return true;
            return false;
        }

        /// <summary>
        /// "captures" this instance with modifications as a string.
        /// </summary>
        /// <returns>This configuration as a valid string ready to be saved to be disk.</returns>
        public string Capture()
        {
            return ToString();
        }

        /// <summary>
        /// Literally does the same thing as capture but sometimes people are retarded and don't check ToString() overrides.
        /// </summary>
        /// <returns>see Capture()</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(string key in settings.Keys)
            {
                sb.AppendLine(string.Format("{0}: {1};", key, settings[key]));
            }
            foreach(Block so in blocks)
            {
                sb.AppendLine(so.name);
                sb.AppendLine("{");
                foreach(string key in so.settings.Keys)
                {
                    sb.AppendLine(string.Format("{0}: {1};", key, so.settings[key]));
                }
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private Exception Build()
        {
            Block current = null;
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
                                        current = new Block();
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
                        blocks.Add(current);
                        current = null;
                    }
                }
                else if (t.Type == ETokenType.RBrace)
                {
                    Advance();
                    blocks.Add(current);
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
