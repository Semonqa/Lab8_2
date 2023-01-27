using System;
using System.CodeDom;
using System.IO;
using System.Linq;

class Program
{
    public static string[] arrayOfData = File.ReadAllLines("file.txt");
    static void Main()
    {
        int startSum = Convert.ToInt32(arrayOfData[0]);

        string[] changedArray = ChangeArray(arrayOfData);

        string[] dataAndTime = new string[changedArray.Length];
        string[] sum = new string[changedArray.Length];
        string[] typeOfOperation = new string[changedArray.Length];
        long[] time = new long[changedArray.Length];

        dataAndTime = AddTime(changedArray);
        sum = AddSum(changedArray);
        typeOfOperation = AddOperation(changedArray);
        time = AddTimeLong(changedArray);

        string[][] timeSumOperation = new string[changedArray.Length][];

        for (int i = 0; i < changedArray.Length; i++)
        {
            timeSumOperation[i] = new string[3];
            string[] operation = changedArray[i].Replace(" | ", "|").Split('|');

            for (int j = 0; j < operation.Length; j++)
            {
                timeSumOperation[i][j] = operation[j];
            }
        }

        string[][] sorted = Sort(time, timeSumOperation);
        bool correct = CheckCorrect(sorted, startSum);

        if (correct == true) CheckDataAndTime(sorted, startSum);
        else Console.WriteLine("Файл некорректен");
    }

    private static string[] ChangeArray(string[] sum)
    {
        string[] sumOutStart = new string[sum.Length - 1];
        for (int i = 0; i < sumOutStart.Length; i++) sumOutStart[i] = sum[i + 1];

        return sumOutStart;
    }

    private static string[] AddTime(string[] changedArray)
    {
        string[] time = new string[changedArray.Length];
        for (int i = 0; i < changedArray.Length; i++)
        {
            string line = changedArray[i];
            line = line.Substring(0, 16);
            line = line.Replace("-", "");
            line = line.Replace(" ", "");
            line = line.Replace(":", "");
            time[i] = line;
        }

        return time;
    }

    private static long[] AddTimeLong(string[] changedArray)
    {
        long[] time = new long[changedArray.Length];

        for (int i = 0; i < changedArray.Length; i++)
        {
            string line = changedArray[i];
            line = line.Substring(0, 16);
            line = line.Replace("-", "");
            line = line.Replace(" ", "");
            line = line.Replace(":", "");
            time[i] = Convert.ToInt64(line);
        }

        return time;
    }

    private static string[] AddSum(string[] changedArray)
    {
        string[] sumLast = new string[changedArray.Length];

        for (int i = 0; i < changedArray.Length; i++)
        {
            string sum = changedArray[i];
            sum = sum.Substring(18);
            int index = sum.IndexOf('|');

            if (index != -1)
            {
                sum = sum.Substring(0, index - 1);
                sumLast[i] = sum;
            }

            else sumLast[i] = "";
        }

        return sumLast;
    }

    private static string[] AddOperation(string[] changedArray)
    {
        string[] operationLast = new string[changedArray.Length];

        for (int i = 0; i < changedArray.Length; i++)
        {
            string operation = changedArray[i];
            operation = operation.Substring(18);
            int index = operation.IndexOf('|');

            if (index != -1)
            {
                operation = operation.Substring(index + 2);
                operationLast[i] = operation;
            }

            else operationLast[i] = "revert";
        }

        return operationLast;
    }

    private static string[][] Sort(long[] time, string[][] sort)
    {
        int[] array = new int[time.Length];

        for (int i = 0; i < time.Length; i++)
        {
            int index = Array.IndexOf(time, time.Min());
            array[i] = index;
            time[index] = 999999999999;
        }

        string[][] sorted = new string[sort.Length][];

        for (int i = 0; i < array.Length; i++)
        {
            int k = array[i];
            sorted[i] = new string[sort[k].Length];
            sorted[i] = sort[k];
        }

        return sorted;

    }

    private static bool CheckCorrect(string[][] sort, int startSum)
    {
        for (int i = 0; i < sort.Length; i++)
        {
            if (sort[i][1] == "revert")
            {
                if (sort[i - 1][1] == "revert" || i == 0) return false;
                else
                {
                    if (sort[i - 1][2] == "in") startSum -= Convert.ToInt32(sort[i - 1][1]);

                    else if (sort[i - 1][2] == "out") startSum += Convert.ToInt32(sort[i - 1][1]);
                }
            }

            else
            {
                if (sort[i][2] == "in") startSum += Convert.ToInt32(sort[i][1]);

                else startSum -= Convert.ToInt32(sort[i][1]);

                if (startSum < 0) return false;
            }
        }

        return true;
    }

    private static void CheckDataAndTime(string[][] sorted, int startSum)
    {
        Console.WriteLine("Введите дату и время в формате: гггг-мм-дд чч:мм");
        Console.WriteLine("Если хотите узнать остаток средств на счету нажмите Enter");
        string input = Console.ReadLine();

        if (input == "") Console.WriteLine(CheckFinishSum(sorted, startSum));
        else
        {
            string line = input;
            line = line.Substring(0, 16);
            line = line.Replace("-", "");
            line = line.Replace(" ", "");
            line = line.Replace(":", "");
            long time = Convert.ToInt64(line);
            Console.WriteLine(CheckSumAtTime(time, sorted, startSum));
        }
    }

    private static int CheckFinishSum(string[][] sorted, int startSum)
    {
        for (int i = 0; i < sorted.Length; i++)
        {
            if (sorted[i][1] == "revert")
            {
                if (sorted[i - 1][2] == "in") startSum -= Convert.ToInt32(sorted[i - 1][1]);

                else if (sorted[i - 1][2] == "out") startSum += Convert.ToInt32(sorted[i - 1][1]);
            }
            else
            {
                if (sorted[i][2] == "in") startSum += Convert.ToInt32(sorted[i][1]);

                else startSum -= Convert.ToInt32(sorted[i][1]);
            }
        }

        return startSum;
    }

    private static int CheckSumAtTime(long time, string[][] sorted, int startSum)
    {
        for (int i = 0; i < sorted.Length; i++)
        {
            long allTime = ConvertTimeToLong(sorted[i][0]);

            if (allTime <= time)
            {
                if (sorted[i][1] == "revert")
                {
                    if (sorted[i - 1][2] == "in") startSum -= Convert.ToInt32(sorted[i - 1][i]);
                    else if (sorted[i - 1][2] == "out") startSum += Convert.ToInt32(sorted[i - 1][1]);
                }

                else
                {
                    if (sorted[i][2] == "in") startSum += Convert.ToInt32(sorted[i][1]);

                    else startSum -= Convert.ToInt32(sorted[i][1]);
                }
            }
            else return startSum;
        }
        return startSum;
    }

    private static long ConvertTimeToLong(string time)
    {
        string line = time;
        line = line.Substring(0, 16);
        line = line.Replace("-", "");
        line = line.Replace(" ", "");
        line = line.Replace(":", "");
        long timeToLong = Convert.ToInt64(line);
        return timeToLong;
    }
}