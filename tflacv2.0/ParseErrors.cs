using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ErrorType
{
    IdFirstSymb, // Первый символ - буква
    WaitedIdentificator,
    WaitedIdOrExp,
    WaitedEqual,
    InvalidSymbol,
    EqualMiss,
    InvalidToken // Некорректный токен
}

namespace tflacv2._0
{
    public class ParseErrors
    {
        private string message;
        private static string tokenValue;
        private int id { get; }
        private int tokenStartPosition { get; }
        private static int lastId { get; set; }

        public static void SetLastId() 
        { 
            lastId = 0; 
        }

        public ParseErrors(ErrorType errorType, int tokenPosition)
        {
            if (lastId == 0)
                id = lastId;
            else
                id = ++lastId;
            tokenStartPosition = tokenPosition;
            message = GeneratingErrorMessage(errorType);

        }

        public ParseErrors(string msg, int tokenStartPosition, string value)
        {
            id = ++lastId;
            message = msg;
            this.tokenStartPosition = tokenStartPosition;
            tokenValue = value;
        }

        public ParseErrors(string msg, int tokenStartPosition)
        {
            id = ++lastId;
            message = msg;
            this.tokenStartPosition = tokenStartPosition;
        }

        private string GeneratingErrorMessage(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.IdFirstSymb:
                    return "Идентификатор должен начинаться с буквы";
                case ErrorType.WaitedIdentificator:
                    return "Ожидался идентификатор";
                case ErrorType.InvalidSymbol:
                    return "Неизвестный символ";
                case ErrorType.WaitedIdOrExp:
                    return "Ожидался идентификатор или выражение";
                case ErrorType.EqualMiss:
                    return "Пропущен оператор присваивания";
                case ErrorType.WaitedEqual:
                    return "Ожидался оператор присваивания";
                case ErrorType.InvalidToken:
                    return "Некорректный токен";
                default:
                    return "";
            }
        }

        public static string ErrorsOut()
        {
            int i = 0;
            List<ParseErrors> errors = Parser.GetErrors();
            string errorsMessage = "";
            while (i < errors.Count)
            {
                if (tokenValue == null)
                    errorsMessage += $"{errors[i].id}. Ошибка в позиции {errors[i].tokenStartPosition + 1}: {errors[i].message}\n";
                else
                    errorsMessage += $"{errors[i].id}. Ошибка в позиции {errors[i].tokenStartPosition + 1}: {errors[i].message}\n";
                i++;
            }
            return errorsMessage;
        }

    }
}
