using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tflacv2._0
{
    public class Parser
    {
        private static List<Token> tokens;
        private static int currentTokenIndex;
        private static string _error;
        public static string GetError() { return _error; }
        private static Token next()
        {
            if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex] != null)
                return tokens[currentTokenIndex];
            else return null;
        }
        private static void nextIndex()
        {
            if (currentTokenIndex < tokens.Count)
                currentTokenIndex++;
        }
        private static bool IsNextEmpty()
        {
            return next() == null;
        }
        public static bool Parse()
        {
            tokens = Token.GetTokens();
            currentTokenIndex = 0;
            try
            { Parsing(); }
            catch
            {
                return true;
            }
            return false;
        }

        private static void Parsing()
        {
            Identifier();
            Equal();
            Expression();
        }

        private static void Equal()
        {
            ExpectedTokenCompare(TokenType.Оператор_присваивания);
        }

        // Идентификатор -> Б {Б | Ц}
        private static void Identifier()
        {
            ExpectedTokenCompare(TokenType.Идентификатор);
        }
        // Выражение -> T {+ T} {- T}
        private static void Expression()
        {
            Term();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Plus) || Check(TokenType.Minus)))
            {
                nextIndex();
                Term();
            }
        }
        private static void Term()
        {
            Operand();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Mult) || Check(TokenType.Div)))
            {
                nextIndex();
                Operand();
            }
        }

        private static void Operand()
        {
            if (Check(TokenType.BracketL))
            {
                nextIndex();
                Expression();
                ExpectedTokenCompare(TokenType.BracketR);
            }
            else if (Check(TokenType.Идентификатор))
                Identifier();
            else if (Check(TokenType.Число_без_знака))
                nextIndex();
            else if (Check(TokenType.Invalid))
            {
                _error = "Неизвестный токен";
                throw new Exception();
            }
            else
            {
                _error = "Ожидался идентификатор или выражение";
                throw new Exception();
            }
        }

        static private void ExpectedTokenCompare(TokenType expectedToken)
        {
            if (currentTokenIndex < tokens.Count)
            {
                if (next().GetTokenType() == expectedToken)
                    nextIndex();
                else
                {
                    _error = $"Ожидалась лексема {Convert.ToString(expectedToken)}, получена {Convert.ToString(next().GetTokenType())}";
                    throw new Exception();
                }
            }
        }

        private static bool Check(TokenType expectedToken)
        {
            if (currentTokenIndex >= tokens.Count)
                return false;
            return next().GetTokenType() == expectedToken;
        }

    }
}
