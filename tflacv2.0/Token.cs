using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tflacv2._0
{
    public class Token
    {
        private string _value;
        private int _index;
        private int _posStart;
        private int _posEnd;
        private TokenType _tokenType;
        private static List<Token> _tokens;

        public string GetValue() 
        { 
            return this._value; 
        }

        public TokenType GetTokenType() 
        { 
            return this._tokenType; 
        }

        public int GetIndex() 
        { 
            return this._index; 
        }

        public int GetPosStart() 
        { 
            return _posStart; 
        }

        public int GetPosEnd() 
        { 
            return _posEnd; 
        }

        public static List<Token> GetTokens() 
        { 
            return _tokens; 
        }

        public static void TokensClear()
        {
            _tokens.Clear();
        }

        public Token(TokenType tokenType, string value, int index, int posStart, int posEnd)
        {
            if (_tokens == null)
                _tokens = new List<Token>();
            _value = value;
            _tokenType = tokenType;
            _index = index;
            _posStart = posStart;
            _posEnd = posEnd;
            _tokens.Add(this);
        }
    }
}
