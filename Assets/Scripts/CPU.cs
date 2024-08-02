using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CPU : MonoBehaviour
{
    [Header("PC")]
    public short PC;

    [Header("Register")]
    public long rax;
    public long rcx;
    public long rdx;
    public long rbx;
    public long rsp;
    public long rbp;
    public long rsi;
    public long rdi;
    public long r8;
    public long r9;
    public long r10;
    public long r11;
    public long r12;
    public long r13;
    public long r14;

    [Header("Memory")]
    public List<char> Memory = new();

    [Header("CC")]
    public bool ZF = false;
    public bool SF = false;
    public bool OF = false;

    [Header("Status Conditions")]
    public short STAT = 1;

    public List<string> BinaryMIS = new();
    
    public Dictionary<int, long> MemoryOutputs = new();

    public List<string> ReadMIS(string input)
    {
        string fileUrl = Application.streamingAssetsPath + "/test/" + input + ".yo";
        StreamReader streamReader = File.OpenText(fileUrl);
        string readData = streamReader.ReadToEnd();
        streamReader.Close();

        char[] separator = { '|', '\n' };
        string[] spiltReadData = readData.Split(separator);
        List<string> result = new();
        for (int i = 0; i < spiltReadData.Length; i += 2)
        {
            if (spiltReadData[i].Length == 28)
            {
                if (spiltReadData[i][7] != ' ')
                {
                    result.Add(spiltReadData[i]);
                }
            }
        }

        return result;
    }

    public List<char> EncodeMIS(List<string> input)
    {
        List<char> result = new();
        char[] separator = { ':' };
        foreach (string statement in input)
        {
            string[] spiltReadData = statement.Split(separator);

            string temp = spiltReadData[1].Trim();
            int mem = 2 * int.Parse(spiltReadData[0][2..], System.Globalization.NumberStyles.AllowHexSpecifier);
            for (int j = result.Count; j < mem; j++)
            {
                result.Add('0');
            }
            for (int i = 0; i < temp.Length; i++)
            {
                result.Add(temp[i]);
            }
        }
        for (int k = result.Count; k < 1024; k++)
        {
            result.Add('0');
        }
        return result;
    }

    public void Initialize()
    {
        PC = 0;

        rax = 0;
        rcx = 0;
        rdx = 0;
        rbx = 0;
        rsp = 0;
        rbp = 0;
        rsi = 0;
        rdi = 0;
        r8 = 0;
        r9 = 0;
        r10 = 0;
        r11 = 0;
        r12 = 0;
        r13 = 0;
        r14 = 0;

        Memory = new();

        ZF = true;
        SF = false;
        OF = false;

        STAT = 1;

        BinaryMIS = new();
    }

    public long GetRegister(char registerIndex)
    {
        return registerIndex switch
        {
            '0' => rax,
            '1' => rcx,
            '2' => rdx,
            '3' => rbx,
            '4' => rsp,
            '5' => rbp,
            '6' => rsi,
            '7' => rdi,
            '8' => r8,
            '9' => r9,
            'a' => r10,
            'b' => r11,
            'c' => r12,
            'd' => r13,
            'e' => r14,
            _ => STAT = 3,
        };
    }

    public void SetRegister(char registerIndex, long value)
    {
        switch (registerIndex)
        {
            case '0':
                rax = value;
                break;
            case '1':
                rcx = value;
                break;
            case '2':
                rdx = value;
                break;
            case '3':
                rbx = value;
                break;
            case '4':
                rsp = value;
                break;
            case '5':
                rbp = value;
                break;
            case '6':
                rsi = value;
                break;
            case '7':
                rdi = value;
                break;
            case '8':
                r8 = value;
                break;
            case '9':
                r9 = value;
                break;
            case 'a':
                r10 = value;
                break;
            case 'b':
                r11 = value;
                break;
            case 'c':
                r12 = value;
                break;
            case 'd':
                r13 = value;
                break;
            case 'e':
                r14 = value;
                break;
            default:
                STAT = 3;
                break;
        }
    }

    public void SetConditionCodes(long value1, long value2)
    {
        if (value1 == value2)
        {
            ZF = true;
        }
        else
        {
            ZF = false;
        }

        if (value1 < value2)
        {
            SF = true;
        }
        else
        {
            SF = false;
        }

        if (value2 < 0)
        {
            if (value1 > long.MaxValue + value2)
            {
                OF = true;
            }
            else
            {
                OF = false;
            }
        }
        else if (value2 > 0)
        {
            if (value1 < long.MinValue + value2)
            {
                OF = true;
            }
            else
            {
                OF = false;
            }
        }
        else
        {
            OF = false;
        }
    }

    public void SetConditionCodes(long value)
    {
        if (value < 0)
        {
            ZF = false;
            SF = true;
        }
        else if (value == 0)
        {
            ZF = true;
            SF = false;
        }
        else
        {
            ZF = false;
            SF = false;
        }

        OF = false;
    }
}
