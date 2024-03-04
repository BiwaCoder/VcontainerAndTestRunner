using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : IGacha
{
    //ガチャのテーブル、1-10にあわせて　アイテムを設定するDictionary
    private Dictionary<int, string> gachaTable = new Dictionary<int, string>()
    {
        {1, "Rアイテム1"},
        {2, "Rアイテム2"},
        {3, "$アイテム3"},
        {4, "Rアイテム4"},
        {5, "Rアイテム5"},
        {6, "R アイテム6"},
        {7, "SRアイテム7"},
        {8, "SR アイテム8"},
        {9, "SR アイテム9"},
        {10, "SSR アイテム10"}
    };
    
    //ガチャを引いて文字列を返す
    public string DrawGacha()
    {
        int gachaNumber = Random.Range(1, 11);
        return gachaTable[gachaNumber];
    }
    
    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}