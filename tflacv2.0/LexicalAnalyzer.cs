using tflacv2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TokenType
{
    Letter,     //Буква
    UnsignedNumber,     //Цифра
    Identifier, //Идентификатор
    Separator,  //Разделитель
    Plus,       //Плюс
    Minus,      //Минус
    Mult,       //Умножение
    Div,        //Деление
    AssignmentOperator,      //Оператор присваивания
    BracketRight,   //Правая скобка
    BracketLeft,   //Левая скобка
    Double,     //Вещественное число
    EndOfLine,  //Конец строки
    InvalidSymbol,    //Недопустимый символ
    EndOfInput,  //Конец обрабатываемого текста
}

public static class LexicalAnalyzer
{
    // Метод для проверки, является ли символ буквой
    private static bool IsLetter(char currentSymbol)
    {
        return (currentSymbol >= 'a' && currentSymbol <= 'z') || (currentSymbol >= 'A' && currentSymbol <= 'Z');
    }

    // Метод для проверки, является ли символ цифрой
    private static bool IsDigit(char currentSymbol)
    {
        return currentSymbol >= '0' && currentSymbol <= '9';
    }

    //Функция для выделения токенов
    public static void Tokenize(string incomingString)
    {
        if (Token.GetTokens() != null)
        {
            Token.TokensClear();
        }
        char currentLiter;
        int currentPosition = 0;
        int number = 0;
        int startPosition;
        while (currentPosition < incomingString.Length)
        {
            currentLiter = incomingString[currentPosition];
            if (char.IsWhiteSpace(currentLiter)) //Если пробел-пропускаем
            {
                currentPosition++;
                continue;
            }

            if (IsLetter(currentLiter)) //Если литер - буква
            {
                startPosition = currentPosition;
                while (currentPosition < incomingString.Length && (IsLetter(incomingString[currentPosition]) || IsDigit(incomingString[currentPosition])))
                {
                    currentPosition++;
                }

                new Token(TokenType.Identifier, incomingString.Substring(startPosition, currentPosition - startPosition), number++, startPosition, currentPosition);
                continue;
            }

            if (IsDigit(currentLiter)) //Если литер - цифра
            {
                startPosition = currentPosition;
                bool hasPoint = false; // Была ли точка или запятая
                bool invalid = false; //Корректно или нет
                while (currentPosition < incomingString.Length && (IsDigit(incomingString[currentPosition]) || (!hasPoint && (incomingString[currentPosition] == '.'))))
                {
                    if (incomingString[currentPosition] == '.' || incomingString[currentPosition] == ',')
                    {
                        hasPoint = true;
                    } else if (!(IsDigit(incomingString[currentPosition]))) 
                    {
                        invalid = true;
                    }
                    currentPosition++;
                }
                if (invalid)
                {
                    new Token(TokenType.InvalidSymbol, incomingString.Substring(startPosition, currentPosition - startPosition), number++, startPosition, currentPosition);
                }
                else if (hasPoint)
                {
                    new Token(TokenType.Double, incomingString.Substring(startPosition, currentPosition - startPosition), number++, startPosition, currentPosition);
                }
                else {
                    new Token(TokenType.UnsignedNumber, incomingString.Substring(startPosition, currentPosition - startPosition), number++, startPosition, currentPosition);
                }
                continue;
            }

            switch (currentLiter)
            {
                case '+':
                    new Token(TokenType.Plus, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                case '-':
                    new Token(TokenType.Minus, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                case '*':
                    {
                        new Token(TokenType.Mult, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                        currentPosition++;
                    }
                    break;
                case '/':
                    new Token(TokenType.Div, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                case '=':
                    new Token(TokenType.AssignmentOperator, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                case '(':
                    new Token(TokenType.BracketLeft, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                case ')':
                    new Token(TokenType.BracketRight, incomingString.Substring(currentPosition, 1), number++, currentPosition, currentPosition);
                    currentPosition++;
                    break;
                default:
                    int start = currentPosition;
                    while (currentPosition < incomingString.Length && !char.IsWhiteSpace(incomingString[currentPosition]) && !IsLetter(incomingString[currentPosition]) && !IsDigit(incomingString[currentPosition]))
                    {
                        currentPosition++;
                    }
                    string invalidToken = incomingString.Substring(start, currentPosition - start);
                    new Token(TokenType.InvalidSymbol, invalidToken, number++, start + 1, currentPosition);
                    break;
            }
        }
    }
}
