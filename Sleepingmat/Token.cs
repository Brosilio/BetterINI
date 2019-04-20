namespace Sleepingmat
{
    internal class Token
    {
        public ETokenType Type;
        public object Value;
        public int line, character;

        public Token(ETokenType tokenType, object Value, int line, int character)
        {
            this.Type = tokenType;
            this.Value = Value;
            this.line = line;
            this.character = character;
        }

        public override string ToString()
        {
            return $"Token({Type}, {Value}, {line}, {character})";
        }
    }
}
