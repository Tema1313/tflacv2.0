using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class LexicalAnalyzer
{
    // Перечисление для описания типов лексем
    enum TokenType
    {
        KeywordVar, // Ключевое слово var
        KeywordInt, // Ключевое слово int
        KeywordBegin, // Ключевое слово Begin
        KeywordEnd, // Ключевое слово end
        Identifier, // Идентификатор
        SeparatorSpace,  // Разделитель пробел
        SeparatorNewLine,  // Разделитель \n
        SeparatorTab,  // Разделитель \t
        Assignment, // Оператор присваивания
        Semicolon,  // Конец оператора
        Comment,     // Комментарий
        Integer,    // Целое число
        Addition, // Сложение
        Subtraction, // Вычитание
        Multiplication, // Умножение
        Division, // Деление
        DivisionWithoutRemainder, // Оператор деления без остатка
        OperatorOfTakingRemainder, // Оператор взятия остатка
        Parentheses, // Круглые скобки
        Invalid     // Недопустимый символ
    }

    // Словарь для хранения ключевых слов
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "int", TokenType.KeywordInt },
        { "var", TokenType.KeywordVar },
        { "begin", TokenType.KeywordBegin },
        { "end", TokenType.KeywordEnd },
        { "div", TokenType.DivisionWithoutRemainder },
        { "mod", TokenType.OperatorOfTakingRemainder }
    };
    //Метод для проверки, является ли символ буквой
    private static bool IsLetter(char symbol)
    {
        return (symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z');
    }

    //Метод для проверки, является ли символ цифрой
        private static bool IsDigit(char symbol)
    {
        return symbol >= '0' && symbol <= '9';
    }
    // Метод для выделения лексем
    private static List<Tuple<int, int, TokenType, string, int, int>> Tokenize(string input)
    {
        List<Tuple<int, int, TokenType, string, int, int>> tokens = new List<Tuple<int, int, TokenType, string, int, int>>(); // Список для хранения лексем
        int i = 0; // Индекс текущего символа
        int line = 1; //Номер текущей строки

        while (i < input.Length)
        {
            char symbol = input[i];

            // Если текущий символ является разделителем, пропускаем его
            if (char.IsWhiteSpace(symbol))
            {
                // Если это пробел или перевод строки, добавляем соответствующую лексему
                if (symbol == ' ')
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorSpace, TokenType.SeparatorSpace, "(space)", i + 1, i + 1));
                    line++;
                }
                else if (symbol == '\n')
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorNewLine, TokenType.SeparatorNewLine, "\\n", i + 1, i + 1));
                    line++;
                }
                else if (symbol == '\t')
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorTab, TokenType.SeparatorTab, "\\t", i + 1, i + 1));
                    line++;
                }
                i++;
                continue;
            }

            // Если текущий символ является оператором присваивания (=)
            if (symbol == '=')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Assignment, TokenType.Assignment, "=", i + 1, i + 1));
                i++;
                line++;
                continue;
            }

            // Если текущий символ является концом оператора (;)
            if (symbol == ';')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Semicolon, TokenType.Semicolon, ";", i + 1, i + 1));
                i++;
                line++;
                continue;
            }

            // Если текущий символ является оператором сложения (+)
            if (symbol == '+')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Addition, TokenType.Addition, ";", i + 1, i + 1));
                i++;
                line++;
                continue;
            }

            // Если текущий символ является оператором вычитания (-)
            if (symbol == '-')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Subtraction, TokenType.Subtraction, ";", i + 1, i + 1));
                i++;
                line++;
                continue;
            }

            // Если текущий символ является оператором вычитания (/)
            if (symbol == '/')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Division, TokenType.Division, ";", i + 1, i + 1));
                i++;
                line++;
                continue;
            }

            // Если текущий символ является звездочкой (*) - начало комментария или умножение
            if (symbol == '*')
            {
                string str = "\"";
                int start = i;
                bool isRightSymbol = false;
                i++;

                // Пока не найдем закрывающую звездочку или конец строки
                while (i < input.Length && input[i] != '\n' && input[i] != '*')
                {
                    str += input[i];
                    if (IsDigit(input[i]) || IsLetter(input[i])) { 
                        isRightSymbol = true;
                    }
                    i++;
                }

                // Если комментарий закончился на звездочку, то добавляем лексему в список
                if (i < input.Length && input[i] == '*')
                {
                    str += "\"";
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Comment, TokenType.Comment, str, start + 1, i));
                    line++;
                    i++;
                }
                else if (isRightSymbol) {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Multiplication, TokenType.Multiplication, str, start + 1, i));
                    line++;
                }
                else // Иначе комментарий был не закрыт - это ошибка
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Invalid, TokenType.Invalid, str, start + 1, i));
                    line++;
                }

                continue;
            }



            // Если текущий символ является цифрой, то это может быть число
            if (IsDigit(symbol))
            {
                string number = "";
                int start = i;

                // Пока текущий символ является цифрой, добавляем его к числу
                while (i < input.Length && IsDigit(input[i]))
                {
                    number += input[i];
                    i++;
                }

                // Добавляем лексему в список
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Integer, TokenType.Integer, number, start + 1, i));
                line++;

                continue;
            }

            // Если текущий символ является буквой, то это может быть ключевое слово или идентификатор
            if (IsLetter(symbol))
            {
                string word = "";
                int start = i;

                // Пока текущий символ является буквой или цифрой, добавляем его к слову
                while (i < input.Length && (IsLetter(input[i]) || IsDigit(input[i])))
                {
                    word += input[i];
                    i++;
                }

                // Если слово является ключевым, то добавляем лексему в список
                if (keywords.ContainsKey(word))
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)keywords[word], keywords[word], word, start + 1, i));
                    line++;
                }
                else // Иначе это идентификатор
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Identifier, TokenType.Identifier, word, start + 1, i));
                    line++;
                }

                continue;
            }

            // Недопустимый символ - добавляем лексему в список
            tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Invalid, TokenType.Invalid, symbol.ToString(), i + 1, i + 1));
            line++;
            i++;
        }

        return tokens;
    }
    public static string RunCompieler(string text)
    {
        var tokens = Tokenize(text);
        string result = "";
        foreach (var token in tokens)
        {
            result += $"{token.Item1,-20} код:{token.Item2,-20} {token.Item3,-20} {token.Item4,-20} c {token.Item5} по {token.Item6}\n";
        }
        return result;
    }
}
