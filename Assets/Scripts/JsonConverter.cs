using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonConverter
{
    public List<JsonCPUData> jsonCPUDatas = new();

    public void WriteJson(CPU CPUData)
    {
        JsonCPUData jsonCPUData = new()
        {
            PC = CPUData.PC,

            REG = new()
            {
                rax = CPUData.rax,
                rcx = CPUData.rcx,
                rdx = CPUData.rdx,
                rbx = CPUData.rbx,
                rsp = CPUData.rsp,
                rbp = CPUData.rbp,
                rsi = CPUData.rsi,
                rdi = CPUData.rdi,
                r8 = CPUData.r8,
                r9 = CPUData.r9,
                r10 = CPUData.r10,
                r11 = CPUData.r11,
                r12 = CPUData.r12,
                r13 = CPUData.r13,
                r14 = CPUData.r14,
            },

            MEM = GetMemoryOutputs(CPUData.Memory),

            CC = new()
            {
                ZF = CPUData.ZF ? 1 : 0,
                SF = CPUData.SF ? 1 : 0,
                OF = CPUData.OF ? 1 : 0
            },

            STAT = CPUData.STAT
        };

        jsonCPUDatas.Add(jsonCPUData);
    }

    public void SaveJson(string name)
    {
        string path = Application.streamingAssetsPath + "/answer/" + name + ".json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string json = JsonMapper.ToJson(jsonCPUDatas);
        StreamWriter streamWriter = new(path);
        streamWriter.Write(json);
        streamWriter.Close();
        streamWriter.Dispose();
    }

    public Dictionary<int, long> GetMemoryOutputs(List<char> input)
    {
        Dictionary<int, long> result = new();

        for (int i = 0; i < input.Count; i += 16)
        {
            long tempData = Utils.GetLong(Utils.ListToString(input.GetRange(i, 16)), 0);
            if (tempData == 0)
            {
                continue;
            }

            result.Add(i / 2, tempData);
        }

        return result;
    }
}

public class JsonCPUData
{
    public int PC;

    public REGData REG;

    public Dictionary<int, long> MEM = new();

    public CCData CC;

    public int STAT = 1;
}

public class REGData
{
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
}

public class CCData
{
    public int ZF;
    public int SF;
    public int OF;
}