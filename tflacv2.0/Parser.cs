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
        private static bool hasId;
        private static int state;
        private static bool hasEr;
        private static int braketLCounter;
        private static int braketRCounter;

        public static List<ParseErrors> GetErrors() 
        { 
            return errors; 
        }

        private static void Error(string msg)
        {
            if (IsNextEmpty())
                errors.Add(new ParseErrors(msg, currentTokenIndex));
            else
                errors.Add(new ParseErrors(msg, currentTokenIndex, next().GetValue()));
        }

        private static void Error(string msg, int index)
        {
            if (IsNextEmpty())
                errors.Add(new ParseErrors(msg, index));
            else
                errors.Add(new ParseErrors(msg, index, tokens[index].GetValue()));
        }

        public static void ClearErrorList() 
        { 
            errors.Clear(); 
            ParseErrors.SetLastId(); 
        }

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

        public static void Parse()
        {
            tokens = Token.GetTokens();
            currentTokenIndex = 0;
            state = 0;
            hasId = false;
            hasEr = false;
            braketLCounter = 0;
            braketRCounter = 0;
            try
            {
                Parsing();
                if (currentTokenIndex >= tokens.Count && !(next() == null))
                    Error("Нежиданный символ в коде.");
            }
            catch 
            { 
                return; 
            }
        }

        private static void Parsing()
        {
            if (next().GetTokenType() == TokenType.Оператор_присваивания)
            {
                Error("Ожидался идентификатор");
            }
            else
            {
                while (currentTokenIndex < tokens.Count && (!Check(TokenType.Оператор_присваивания)))
                {
                    if (hasId && !hasEr)
                    { 
                        Error("Слева может быть только 1 идентификатор"); 
                        hasEr = true; 
                    }
                    if (Check(TokenType.Идентификатор) || Check(TokenType.InvalidLetter))
                    {
                        Identifier();
                    }
                    else
                    {
                        nextIndex();
                    }
                }
                if (!hasId)
                {
                    Error("Ожидался идентификатор", currentTokenIndex - 1);
                }
            }

            Equal();
            Expression();

            if (braketLCounter > braketRCounter)
            {
                Error("Ожидался символ закрывающей скобки )");
                hasEr = true;
            } else if (braketRCounter > braketLCounter)
            {
                Error("Ожидался символ открывающей скобки (");
                hasEr = true;
            }
        }

        private static void Equal()
        {
            if (!Check(TokenType.Оператор_присваивания))
                Error("Ожидался оператор \"=\"");
            else
                state = 1;
            hasEr = false;
            nextIndex();
        }

        // Идентификатор -> Б {Б | Ц}
        private static void Identifier()
        {
            if (!hasId)
                hasId = true;
            if (Check(TokenType.InvalidLetter))
            {
                Error("Идентификаторы могут содержать только большие буквы");
                nextIndex();
            }
            else
                ExpectedTokenCompare(TokenType.Идентификатор);
        }

        // Выражение -> T {+ T} {- T}
        private static void Expression()
        {
            Term();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Plus) || Check(TokenType.Minus)))
            {
                nextIndex();
                if (!IsNextEmpty() && next().GetTokenType() == TokenType.BracketR)
                {
                    braketRCounter++;
                    nextIndex();
                }
                Term();
            }
            if (!IsNextEmpty() && next().GetTokenType() == TokenType.BracketR)
            {
                braketRCounter++;
                nextIndex();
            }
        }

        private static void Term()
        {
            Operand();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Mult) || Check(TokenType.Div)))
            {
                nextIndex();
                if (!IsNextEmpty() && next().GetTokenType() == TokenType.BracketR)
                {
                    braketRCounter++;
                    nextIndex();
                }
                Operand();
            }
            if (!IsNextEmpty() && (next().GetTokenType() == TokenType.Число_без_знака || next().GetTokenType() == TokenType.BracketL) && !IsSign(currentTokenIndex - 1))
            {
                nextIndex();
                if (!IsSign(currentTokenIndex))
                { Error("Пропущен арифметический знак"); nextIndex(); }
                if (!IsNextEmpty() && next().GetTokenType() == TokenType.BracketL)
                    Expression();
            }
            if (!IsNextEmpty() && next().GetTokenType() == TokenType.BracketR)
            {
                braketRCounter++;
                nextIndex();
            }
            if (!IsNextEmpty() && next().GetTokenType() == TokenType.Invalid)
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
            if (Check(TokenType.BracketL))
            {
                nextIndex();
                braketLCounter++;
                Expression();
            }
            else if (Check(TokenType.Идентификатор) || Check(TokenType.InvalidLetter))
                Identifier();
            else if (Check(TokenType.Число_без_знака))
                nextIndex();
            else if (Check(TokenType.Invalid))
            {
                Error("Неизвестный символ");
                nextIndex();
            }
            else
            {
                Error("Ожидался идентификатор или выражение");
                nextIndex();
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
                    Error($"Ожидалась лексема {Convert.ToString(expectedToken)}, получена {Convert.ToString(next().GetTokenType())}");
                    nextIndex();
                }
            }
        }

        private static bool Check(TokenType expectedToken)
        {
            if (currentTokenIndex >= tokens.Count)
                return false;
            return next().GetTokenType() == expectedToken;
        }
        private static bool CheckNext(TokenType expectedToken)
        {
            if (currentTokenIndex >= tokens.Count)
                return false;
            return tokens[currentTokenIndex + 1].GetTokenType() == expectedToken;
        }

        private static bool IsLitter()
        {
            return (next().GetTokenType() == TokenType.Число_без_знака
                    || next().GetTokenType() == TokenType.Plus
                    || next().GetTokenType() == TokenType.Minus
                    || next().GetTokenType() == TokenType.Mult
                    || next().GetTokenType() == TokenType.BracketL
                    || next().GetTokenType() == TokenType.BracketR);
        }
        private static bool IsSign(int currentIndex)
        {
            if (IsNextEmpty())
                return false;
            return (tokens[currentIndex].GetTokenType() == TokenType.Plus || tokens[currentIndex].GetTokenType() == TokenType.Minus
                || tokens[currentIndex].GetTokenType() == TokenType.Div
                || tokens[currentIndex].GetTokenType() == TokenType.Mult);
        }
    }
}
