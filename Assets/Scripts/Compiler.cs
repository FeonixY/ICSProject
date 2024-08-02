using System;
using UnityEngine;

public class Compiler : MonoBehaviour
{
    public string[] MISName = 
        {
        "abs-asum-cmov",
        "abs-asum-jmp",
        "asum",
        "asumi",
        "asumr",
        "cjr",
        "j-cc",
        "poptest",
        "prog1",
        "prog2",
        "prog3",
        "prog4",
        "prog5",
        "prog6",
        "prog7",
        "prog8",
        "prog9",
        "prog10",
        "pushquestion",
        "pushtest",
        "ret-hazard"
        };
    public string CurrentMISName;
    public CPU CPUData;
    public UIManager UIManager;
    private JsonConverter JsonConverter = new();

    private void Start()
    {
        OnSelectMIS(0);
    }

    public void OnSelectMIS(int input)
    {
        CurrentMISName = MISName[input];

        CPUData.Initialize();

        CPUData.BinaryMIS = CPUData.ReadMIS(CurrentMISName);

        CPUData.Memory = CPUData.EncodeMIS(CPUData.BinaryMIS);

        UIManager.UpdateUI(CPUData);
    }

    public void OnButtonAClicked()
    {
        if (CPUData.STAT == 1)
        {
            CompileMIS(CPUData);
            JsonConverter.WriteJson(CPUData);
            if (CPUData.STAT != 1)
            {
                JsonConverter.SaveJson(CurrentMISName);
            }
            UIManager.UpdateUI(CPUData);
        }
    }
    public void OnButtonBClicked()
    {
        while (CPUData.STAT == 1)
        {
            CompileMIS(CPUData);
            JsonConverter.WriteJson(CPUData);
            if (CPUData.STAT != 1)
            {
                JsonConverter.SaveJson(CurrentMISName);
            }
            UIManager.UpdateUI(CPUData);
        }
    }

    public void CompileMIS(CPU input)
    {
        switch (input.Memory[2 * input.PC])
        {
            case '0':
                Halt();
                break;
            case '1':
                Nop();
                break;
            case '2':
                char fn2 = input.Memory[2 * input.PC + 1];
                char rA2 = input.Memory[2 * input.PC + 2];
                char rB2 = input.Memory[2 * input.PC + 3];
                CmovXX(fn2, rA2, rB2);
                break;
            case '3':
                char rB3 = input.Memory[2 * input.PC + 3];
                long V3 = Utils.GetLong(Utils.ListToString(input.Memory), 2 * input.PC + 4);
                Irmovq(rB3, V3);
                break;
            case '4':
                char rA4 = input.Memory[2 * input.PC + 2];
                char rB4 = input.Memory[2 * input.PC + 3];
                long D4 = Utils.GetLong(Utils.ListToString(input.Memory), 2 * input.PC + 4);
                Rmmovq(rA4, rB4, D4);
                break;
            case '5':
                char rA5 = input.Memory[2 * input.PC + 2];
                char rB5 = input.Memory[2 * input.PC + 3];
                long D5 = Utils.GetLong(Utils.ListToString(input.Memory), 2 * input.PC + 4);
                Mrmovq(rA5, rB5, D5);
                break;
            case '6':
                char fn6 = input.Memory[2 * input.PC + 1];
                char rA6 = input.Memory[2 * input.PC + 2];
                char rB6 = input.Memory[2 * input.PC + 3];
                OPq(fn6, rA6, rB6);
                break;
            case '7':
                char fn7 = input.Memory[2 * input.PC + 1];
                long dest7 = Utils.GetLong(Utils.ListToString(input.Memory), 2 * input.PC + 2);
                JXX(fn7, dest7);
                break;
            case '8':
                long dest8 = Utils.GetLong(Utils.ListToString(input.Memory), 2 * input.PC + 2);
                Call(dest8);
                break;
            case '9':
                Ret();
                break;
            case 'a':
                char rAa = input.Memory[2 * input.PC + 2];
                Pushq(rAa);
                break;
            case 'b':
                char rAb = input.Memory[2 * input.PC + 2];
                Popq(rAb);
                break;
            default:
                input.STAT = 4;
                break;
        }
    }

    public void Halt()
    {
        CPUData.STAT = 2;
    }

    public void Nop()
    {
        CPUData.PC += 1;
    }

    public void CmovXX(char fn, char rA, char rB)
    {
        switch (fn)
        {
            //rrmovq:Move Unconditionally
            case '0':
                CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                break;
            //cmovle:Move When Less or Equal
            case '1':
                if ((CPUData.SF ^ CPUData.OF) | CPUData.ZF)
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            //cmovl:Move When Less
            case '2':
                if (CPUData.SF ^ CPUData.OF)
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            //cmove:Move When Equal
            case '3':
                if (CPUData.ZF)
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            //cmovne:Move When Not Equal
            case '4':
                if (!CPUData.ZF)
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            //cmovge:Move When Greater or Equal
            case '5':
                if (!(CPUData.SF ^ CPUData.OF))
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            //cmovg:Move When Greater
            case '6':
                if (!(CPUData.SF ^ CPUData.OF) & !CPUData.ZF)
                {
                    CPUData.SetRegister(rB, CPUData.GetRegister(rA));
                }
                break;
            default:
                CPUData.STAT = 4;
                break;
        }

        CPUData.PC += 2;
    }

    public void Irmovq(char rB, long V)
    {
        CPUData.SetRegister(rB, V);
        CPUData.PC += 10;
    }

    public void Rmmovq(char rA, char rB, long D)
    {
        string temp = Utils.GetString(CPUData.GetRegister(rA));
        for (int i = 0; i < 16; i++)
        {
            CPUData.Memory[2 * Convert.ToInt32(D + CPUData.GetRegister(rB)) + i] = temp[i];
        }
        
        CPUData.PC += 10;
    }

    public void Mrmovq(char rA, char rB, long D)
    {
        CPUData.SetRegister(rA, Utils.GetLong(Utils.ListToString(CPUData.Memory), 2 * Convert.ToInt32(D + CPUData.GetRegister(rB))));
        CPUData.PC += 10;
    }

    public void OPq(char fn, char rA, char rB)
    {
        switch (fn)
        {
            //addq:Add
            case '0':
                CPUData.SetConditionCodes(CPUData.GetRegister(rA), -CPUData.GetRegister(rB));
                CPUData.SetRegister(rB, CPUData.GetRegister(rA) + CPUData.GetRegister(rB));
                break;
            //subq:Subtract (rA from rB)
            case '1':
                CPUData.SetConditionCodes(CPUData.GetRegister(rB), CPUData.GetRegister(rA));
                CPUData.SetRegister(rB, CPUData.GetRegister(rB) - CPUData.GetRegister(rA));
                break;
            //andq:And
            case '2':
                CPUData.SetConditionCodes(CPUData.GetRegister(rA) & CPUData.GetRegister(rB));
                CPUData.SetRegister(rB, CPUData.GetRegister(rA) & CPUData.GetRegister(rB));
                break;
            //xorq:Exclusive-Or
            case '3':
                CPUData.SetConditionCodes(CPUData.GetRegister(rA) ^ CPUData.GetRegister(rB));
                CPUData.SetRegister(rB, CPUData.GetRegister(rA) ^ CPUData.GetRegister(rB));
                break;
            default:
                CPUData.STAT = 4;
                break;
        }

        CPUData.PC += 2;
    }

    public void JXX(char fn, long dest)
    {
        switch (fn)
        {
            //jmp:Jump Unconditionally
            case '0':
                CPUData.PC = Convert.ToInt16(dest);
                break;
            //jle:Jump When Less or Equal
            case '1':
                if ((CPUData.SF ^ CPUData.OF) | CPUData.ZF)
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            //jl:Jump When Less
            case '2':
                if (CPUData.SF ^ CPUData.OF)
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            //je:Jump When Equal
            case '3':
                if (CPUData.ZF)
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            //jne:Jump When Not Equal
            case '4':
                if (!CPUData.ZF)
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            //jge:Jump When Greater or Equal
            case '5':
                if (!(CPUData.SF ^ CPUData.OF))
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            //jg:Jump When Greater
            case '6':
                if (!(CPUData.SF ^ CPUData.OF) & !CPUData.ZF)
                {
                    CPUData.PC = Convert.ToInt16(dest);
                }
                else
                {
                    CPUData.PC += 9;
                }
                break;
            default:
                CPUData.STAT = 2;
                break;
        }
    }

    public void Call(long dest)
    {
        CPUData.rsp -= 8;
        if (CPUData.rsp < 0)
        {
            CPUData.STAT = 3;
            return;
        }
        string temp = Utils.GetString(Convert.ToInt64(CPUData.PC + 9));
        for (int i = 0; i < 16; i++)
        {
            CPUData.Memory[Convert.ToInt32(2 * CPUData.rsp + i)] = temp[i];
        }

        JXX('0', dest);
    }

    public void Ret()
    {
        CPUData.PC = Convert.ToInt16(Utils.GetLong(Utils.ListToString(CPUData.Memory), Convert.ToInt32(2 * CPUData.rsp)));
        CPUData.rsp += 8;
    }

    public void Pushq(char rA)
    {
        string temp = Utils.GetString(CPUData.GetRegister(rA));
        CPUData.rsp -= 8;
        if (CPUData.rsp < 0)
        {
            CPUData.STAT = 3;
            return;
        }
        for (int i = 0; i < 16; i++)
        {
            CPUData.Memory[Convert.ToInt32(2 * CPUData.rsp + i)] = temp[i];
        }

        CPUData.PC += 2;
    }


    public void Popq(char rA)
    {
        long temp = CPUData.rsp;
        CPUData.rsp += 8;
        CPUData.SetRegister(rA, Utils.GetLong(Utils.ListToString(CPUData.Memory), Convert.ToInt32(2 * temp)));
        CPUData.PC += 2;
    }
}
