using System;
using System.Collections.Generic;

public static class Utils
{
    public static string ListToString(List<char> input)
    {
        string result = "";
        for (int i = 0; i < input.Count; i++)
        {
            result += input[i];
        }

        return result;
    }

    public static long GetLong(string input, int startIndex)
    {
        input = input[startIndex..];
        input = "" + input[14] + input[15] + input[12] + input[13] + input[10] + input[11] + input[8] + input[9] + input[6] + input[7] + input[4] + input[5] + input[2] + input[3] + input[0] + input[1];
        input = input.ToUpper();
        long result = long.Parse(input, System.Globalization.NumberStyles.AllowHexSpecifier);
        return result;
    }

    public static string GetString(long input)
    {
        string result = Convert.ToString(input, 16);
        result = result.ToLower();
        while (result.Length < 16)
        {
            result = result.Insert(0, "0");
        }
        result = "" + result[14] + result[15] + result[12] + result[13] + result[10] + result[11] + result[8] + result[9] + result[6] + result[7] + result[4] + result[5] + result[2] + result[3] + result[0] + result[1];

        return result;
    }
}
