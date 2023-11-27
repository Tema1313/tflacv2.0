using tflacv2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TokenType
{
    Letter,     //Буква
    Число_без_знака,     //Цифра
    Идентификатор, //Идентификатор
    Separator,  //Разделитель
    Plus,       //Плюс
    Minus,      //Минус
    Mult,       //Умножение
    Div,        //Деление
    Оператор_присваивания,      //Оператор присваивания
    BracketR,   //Правая скобка
    BracketL,   //Левая скобка
    Double,     //Вещественное число
    EndOfLine,  //Конец строки
    Invalid,    //Недопустимый символ
    EndOfInput,  //Конец обрабатываемого текста
    InvalidLetter,  // Токен с маленькой буквой
}

public static class LexicalAnalyzer
{
    // Метод для проверки, является ли символ буквой
    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    // Метод для проверки, является ли символ цифрой
    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    //Функция для выделения токенов
    public static void Tokenize(string input)
    {
        if (Token.GetTokens() != null)
            Token.TokensClear();
        char liter;
        int pos = 0;
        int number = 0;
        int startPos;
        while (pos < input.Length)
        {
            liter = input[pos];
            if (char.IsWhiteSpace(liter)) //Если пробел-пропускаем
            {
                pos++;
                continue;
            }

            if (IsLetter(liter)) //Если литер - буква
            {
                startPos = pos;
                while (pos < input.Length && (IsLetter(input[pos]) || IsDigit(input[pos])))
                {
                    pos++;
                }

                new Token(TokenType.Идентификатор, input.Substring(startPos, pos - startPos), number++, startPos, pos);
                continue;
            }

            if (IsDigit(liter)) //Если литер - цифра
            {
                startPos = pos;
                bool hasPoint = false; // Была ли точка или запятая
                bool invalid = false; //Корректно или нет
                while (pos < input.Length && (IsDigit(input[pos]) || (!hasPoint && (input[pos] == '.' || input[pos] == ','))))
                {
                    if (input[pos] == '.' || input[pos] == ',')
                    { 
                        hasPoint = true;
                        if (!IsDigit(input[pos])) 
                        {
                            invalid = true;
                        }
                    }
                    pos++;
                }
                if (invalid)
                    new Token(TokenType.Invalid, input.Substring(startPos, pos - startPos), number++, startPos, pos);
                else
                    new Token(TokenType.Число_без_знака, input.Substring(startPos, pos - startPos), number++, startPos, pos);
                continue;
            }

            switch (liter)
            {
                case '+':
                    new Token(TokenType.Plus, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                case '-':
                    new Token(TokenType.Minus, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                case '*':
                    {
                        new Token(TokenType.Mult, input.Substring(pos, 1), number++, pos, pos);
                        pos++;
                    }
                    break;
                case '/':
                    new Token(TokenType.Div, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                case '=':
                    new Token(TokenType.Оператор_присваивания, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                case '(':
                    new Token(TokenType.BracketL, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                case ')':
                    new Token(TokenType.BracketR, input.Substring(pos, 1), number++, pos, pos);
                    pos++;
                    break;
                default:
                    int start = pos;
                    while (pos < input.Length && !char.IsWhiteSpace(input[pos]) && !IsLetter(input[pos]) && !IsDigit(input[pos]))
                    {
                        pos++;
                    }
                    string invalidToken = input.Substring(start, pos - start);
                    new Token(TokenType.Invalid, invalidToken, number++, start + 1, pos);
                    break;
            }
        }
    }
}
