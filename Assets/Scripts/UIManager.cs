using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("PC")]
    public TMP_Text PC;

    [Header("MIS")]
    public TMP_Text MIS;

    [Header("Register")]
    public TMP_Text rax;
    public TMP_Text rcx;
    public TMP_Text rdx;
    public TMP_Text rbx;
    public TMP_Text rsp;
    public TMP_Text rbp;
    public TMP_Text rsi;
    public TMP_Text rdi;
    public TMP_Text r8;
    public TMP_Text r9;
    public TMP_Text r10;
    public TMP_Text r11;
    public TMP_Text r12;
    public TMP_Text r13;
    public TMP_Text r14;

    [Header("Memory")]
    public GameObject MemoryContent;
    public GameObject MemoryPrefab;

    [Header("CC")]
    public Toggle ZF;
    public Toggle SF;
    public Toggle OF;

    [Header("Status Conditions")]
    public TMP_Text STAT;
    public Sprite NormalSprite;
    public Sprite HaltSprite;
    public Sprite ErrorSprite;

    public void UpdateUI(CPU CPU)
    {
        PC.text = CPU.PC.ToString();

        MIS.text = SetMIS(CPU.PC, CPU.Memory);

        rax.text = CPU.rax.ToString();
        rcx.text = CPU.rcx.ToString();
        rdx.text = CPU.rdx.ToString();
        rbx.text = CPU.rbx.ToString();
        rsp.text = CPU.rsp.ToString();
        rbp.text = CPU.rbp.ToString();
        rsi.text = CPU.rsi.ToString();
        rdi.text = CPU.rdi.ToString();
        r8.text = CPU.r8.ToString();
        r9.text = CPU.r9.ToString();
        r10.text = CPU.r10.ToString();
        r11.text = CPU.r11.ToString();
        r12.text = CPU.r12.ToString();
        r13.text = CPU.r13.ToString();
        r14.text = CPU.r14.ToString();

        SetMemoryUI(CPU.Memory);

        ZF.isOn = CPU.ZF;
        SF.isOn = CPU.SF;
        OF.isOn = CPU.OF;

        STAT.text = SetStatusUIText(CPU.STAT);
        STAT.transform.parent.Find("Status Image").GetComponent<Image>().sprite = SetStatusUIImage(CPU.STAT);
    }

    private string SetStatusUIText(short input)
    {
        return input switch
        {
            1 => "1 AOK",
            2 => "2 HLT",
            3 => "3 ADR",
            4 => "4 INS",
            _ => "? WTF",
        };
    }

    private Sprite SetStatusUIImage(short input)
    {
        return input switch
        {
            1 => NormalSprite,
            2 => HaltSprite,
            3 => ErrorSprite,
            4 => ErrorSprite,
            _ => ErrorSprite,
        };
    }

    private void SetMemoryUI(List<char> input)
    {
        if (MemoryContent.transform.childCount > 0)
        {
            for (int i = 0; i < MemoryContent.transform.childCount; i++)
            {
                Destroy(MemoryContent.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < input.Count; i += 16)
        {
            long tempData = Utils.GetLong(Utils.ListToString(input.GetRange(i, 16)), 0);
            if (tempData == 0)
            {
                continue;
            }

            GameObject instance = Instantiate(MemoryPrefab, MemoryContent.transform);
            instance.transform.Find("Address").GetComponent<TMP_Text>().text = (i / 2).ToString();
            instance.transform.Find("Value").GetComponent<TMP_Text>().text = tempData.ToString();
        }
    }

    private string SetMIS(short PC, List<char> memory)
    {
        return memory[2 * PC] switch
        {
            '0' => Utils.ListToString(memory.GetRange(2 * PC, 2)),
            '1' => Utils.ListToString(memory.GetRange(2 * PC, 2)),
            '2' => Utils.ListToString(memory.GetRange(2 * PC, 4)),
            '3' => Utils.ListToString(memory.GetRange(2 * PC, 20)),
            '4' => Utils.ListToString(memory.GetRange(2 * PC, 20)),
            '5' => Utils.ListToString(memory.GetRange(2 * PC, 20)),
            '6' => Utils.ListToString(memory.GetRange(2 * PC, 4)),
            '7' => Utils.ListToString(memory.GetRange(2 * PC, 18)),
            '8' => Utils.ListToString(memory.GetRange(2 * PC, 18)),
            '9' => Utils.ListToString(memory.GetRange(2 * PC, 2)),
            'a' => Utils.ListToString(memory.GetRange(2 * PC, 4)),
            'b' => Utils.ListToString(memory.GetRange(2 * PC, 4)),
            _ => "WTF"
        };
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}

