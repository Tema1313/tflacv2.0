using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tflacv2._0
{
    public static class Parser
    {
        private static List<ParseErrors> errors = new List<ParseErrors>();
        private static List<Token> tokens;
        private static int currentTokenIndex;
        private static int state;
        private static bool hasIdentificator;
        private static bool hasError;
        private static int braketLeftCounter;
        private static int braketRightCounter;

        public static List<ParseErrors> GetErrors() 
        { 
            return errors; 
        }

        private static void Error(string msg)
        {
            if (IsNextEmpty())
            {
                errors.Add(new ParseErrors(msg, currentTokenIndex));
            }
            else
            {
                errors.Add(new ParseErrors(msg, currentTokenIndex, getCurrentToken().GetValue()));
            }
        }

        private static void Error(string msg, int index)
        {
            if (IsNextEmpty())
            {
                errors.Add(new ParseErrors(msg, index));
            }
            else
            {
                errors.Add(new ParseErrors(msg, index, tokens[index].GetValue()));
            }
        }

        public static void ClearErrorList() 
        { 
            errors.Clear(); 
            ParseErrors.SetLastId(); 
        }

        private static Token getCurrentToken()
        {
            if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex] != null)
            {
                return tokens[currentTokenIndex];
            }
            else {
                return null;
            }
        }

        private static void nextIndex()
        {
            if (currentTokenIndex < tokens.Count) {
                currentTokenIndex++;
            }
        }

        private static bool IsNextEmpty()
        {
            return getCurrentToken() == null;
        }


        private static bool IsEqualityWithCurrentToken(TokenType expectedToken)
        {
            if (currentTokenIndex >= tokens.Count)
            {
                return false;
            }
            return getCurrentToken().GetTokenType() == expectedToken;
        }

        private static bool IsSign(int currentIndex)
        {
            if (IsNextEmpty())
            {
                return false;
            }
            return (tokens[currentIndex].GetTokenType() == TokenType.Plus || tokens[currentIndex].GetTokenType() == TokenType.Minus
                || tokens[currentIndex].GetTokenType() == TokenType.Div
                || tokens[currentIndex].GetTokenType() == TokenType.Mult);
        }

        public static void Parse()
        {
            tokens = Token.GetTokens();
            currentTokenIndex = 0;
            state = 0;
            hasIdentificator = false;
            hasError = false;
            braketLeftCounter = 0;
            braketRightCounter = 0;
            try
            {
                Parsing();
                if (currentTokenIndex >= tokens.Count && !(getCurrentToken() == null))
                    Error("Нежиданный символ в коде.");
            }
            catch 
            { 
                return; 
            }
        }

        private static void Parsing()
        {
            while (currentTokenIndex < tokens.Count)
            {
                if (IsEqualityWithCurrentToken(TokenType.Identifier))
                {
                    Identifier();
                }
                else
                {
                    nextIndex();
                }
            }
            if (!hasIdentificator)
            {
                Error("Ожидался идентификатор", currentTokenIndex - 1);
            }

            Equal();
            Expression();

            if (braketLeftCounter > braketRightCounter)
            {
                Error("Ожидался символ закрывающей скобки )");
                hasError = true;
            } else if (braketRightCounter > braketLeftCounter)
            {
                Error("Ожидался символ открывающей скобки (");
                hasError = true;
            }
        }

        private static void Equal()
        {
            if (!IsEqualityWithCurrentToken(TokenType.AssignmentOperator))
            {
                Error("Ожидался оператор \"=\"");
            }
            else
            {
                state = 1;
            }
            hasError = false;
            nextIndex();
        }

        // Идентификатор -> Б {Б | Ц}
        private static void Identifier()
        {
            if (!hasIdentificator)
            {
                hasIdentificator = true;
            }
            else
            {
                ExpectedTokenCompare(TokenType.Identifier);
            }
        }

        // Выражение -> T {+ T} {- T}
        private static void Expression()
        {
            Terminal();
            while (currentTokenIndex < tokens.Count && (IsEqualityWithCurrentToken(TokenType.Plus) || IsEqualityWithCurrentToken(TokenType.Minus)))
            {
                nextIndex();
                if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.BracketRight)
                {
                    braketRightCounter++;
                    nextIndex();
                }
                Terminal();
            }
            if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.BracketRight)
            {
                braketRightCounter++;
                nextIndex();
            }
        }

        private static void Terminal()
        {
            Operand();
            while (currentTokenIndex < tokens.Count && (IsEqualityWithCurrentToken(TokenType.Mult) || IsEqualityWithCurrentToken(TokenType.Div)))
            {
                nextIndex();
                if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.BracketRight)
                {
                    braketRightCounter++;
                    nextIndex();
                }
                Operand();
            }
            if (!IsNextEmpty() && (getCurrentToken().GetTokenType() == TokenType.UnsignedNumber || getCurrentToken().GetTokenType() == TokenType.BracketLeft) && !IsSign(currentTokenIndex - 1))
            {
                nextIndex();
                if (!IsSign(currentTokenIndex))
                { 
                    Error("Пропущен арифметический знак"); nextIndex(); 
                }
                if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.BracketLeft)
                {
                    Expression();
                }
            }
            if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.BracketRight)
            {
                braketRightCounter++;
                nextIndex();
            }
            if (!IsNextEmpty() && getCurrentToken().GetTokenType() == TokenType.InvalidSymbol)
            {
                Error("Неизвестный символ");
                nextIndex();
            }
        }

        private static void Operand()
        {
            if (state == 0)
            {
                nextIndex();
                return;
            }
            if (IsEqualityWithCurrentToken(TokenType.BracketLeft))
            {
                nextIndex();
                braketLeftCounter++;
                Expression();
            } else if (IsEqualityWithCurrentToken(TokenType.Identifier))
            {
                Identifier();
            } else if (IsEqualityWithCurrentToken(TokenType.UnsignedNumber))
            {
                nextIndex();
            } else if (IsEqualityWithCurrentToken(TokenType.InvalidSymbol))
            {
                Error("Неизвестный символ");
                nextIndex();
            } else
            {
                Error("Ожидался идентификатор или выражение");
                nextIndex();
            }
        }

        static private void ExpectedTokenCompare(TokenType expectedToken)
        {
            if (currentTokenIndex < tokens.Count)
            {
                if (getCurrentToken().GetTokenType() == expectedToken)
                    nextIndex();
                else
                {
                    Error($"Ожидалась лексема {Convert.ToString(expectedToken)}, получена {Convert.ToString(getCurrentToken().GetTokenType())}");
                    nextIndex();
                }
            }
        }
    }
}
