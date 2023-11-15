using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DKA
{
    private enum State
    {
        Start,  // Начальное состояние
        q1,     // Состояние после первого 'a'
        q2,     // Состояние после 'aa'
        q3,     // Состояние после 'b'
        q4,     // Состояние после 'ab'
        q5,     // Состояние перед 'ca'
        Final,  // Конечное состояние
        Reject  // Состояние отклонения
    }

    private State currentState;

    public bool CheckExpression(string input)
    {
        currentState = State.Start; // Устанавливаем начальное состояние

        foreach (char symbol in input)
        {
            if (!ProcessSymbol(symbol))
                return false;
        }

        // Проверяем, находимся ли в конечном состоянии или в одном из правильных
        return currentState == State.Final || currentState == State.q2;
    }

    private bool ProcessSymbol(char symbol)
    {
        // В зависимости от текущего состояния осуществляем переходы
        switch (currentState)
        {
            // Начальное состояние
            case State.Start:
                // Если встречаем 'a', переходим в состояние q1, иначе отклоняем строку
                if (symbol == 'a')
                    currentState = State.q1;
                else
                    currentState = State.Reject;
                break;

            // Состояние после первого 'a'
            case State.q1:
                // Если встречаем еще одно 'a', переходим в состояние q2, иначе тоже ошибка(у нас минимум две буквы a в начале)
                if (symbol == 'a')
                    currentState = State.q2;
                else
                    currentState = State.Reject;
                break;

            // Состояние после 'aa' или же после итерации ab
            case State.q2:
                // Если встречаем 'b', переходим в q3, иначе отклоняем
                if (symbol == 'b')
                    currentState = State.q3;
                else
                    currentState = State.Reject;
                break;

            // Состояние после 'b'
            case State.q3:
                // Если встречаем 'a', переходим в q2, если 'c', переходим в q4, иначе отклоняем
                if (symbol == 'a')
                    currentState = State.q4;
                else if (symbol == 'c')
                    currentState = State.q5;
                else
                    currentState = State.Reject;
                break;

            // Состояние после 'ab'
            case State.q4:
                // Если встречаем 'b', переходим в q3, иначе переходим в состояние отклонения
                if (symbol == 'b')
                    currentState = State.q3;
                else
                    currentState = State.Reject;
                break;

            // Состояние после 'c'
            case State.q5:
                // Если встречаем 'a', тогда достигаем финального состояния, в противном случае переходим в состояние отклонения
                if (symbol == 'a')
                    currentState = State.Final;
                else
                    currentState = State.Reject;
                break;

            // Состояние отклонения
            case State.Reject:
                // Отклоняем любой символ, если находимся в состоянии отклонения
                return false;

            // Финальное состояние
            case State.Final:
                // Если после достижения финального состояния есть еще символы - переходим в состояние отклонения
                currentState = State.Reject;
                break;

            default:
                // В случае неожиданного состояния выбрасываем исключение
                throw new ArgumentOutOfRangeException();
        }

        // Успешно обработали символ
        return true;
    }


    public static string RunCompieler(string text)
    {
        DKA dka = new DKA();
        bool accepted = dka.CheckExpression(text);
        Console.WriteLine(accepted);
        return accepted ? "Проверку прошла" : "Проверку не прошла";
    }
}

