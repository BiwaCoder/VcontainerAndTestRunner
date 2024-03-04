using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaDummy : IGacha
{
    public string DrawGacha()
    {
        return "ダミーアイテム1";
    }
    
    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}
