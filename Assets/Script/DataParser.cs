using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class JsonDataParser {

    public static string GetString(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? config[key].ToString() : "";
    }

    public static int GetInt(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? int.Parse(config[key].ToString()) : 0;
    }

    public static float GetFloat(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? float.Parse(config[key].ToString()) : 0f;
    }

	public static float GetFloatRound(JsonObject config, string key, int digits)
	{
		return (float)Math.Round((decimal)GetFloat(config, key), digits, MidpointRounding.AwayFromZero);
	}

    public static bool GetIntBool(JsonObject config, string key)
    {
        return GetInt(config, key) == 0 ? false : true;
    }

    public static bool GetBool(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? bool.Parse(config[key].ToString()) : false;
    }

    public static JsonObject GetJsonObject(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? (JsonObject)config[key] : new JsonObject();
    }

    public static JsonArray GetJsonArray(JsonObject config, string key)
    {
        return config.ContainsKey(key) ? (JsonArray)config[key] : new JsonArray();
    }

    public static string[] GetStringArray(JsonObject config, string key)
    {
        return config.ContainsKey(key) && config[key].ToString() != "0" ? config[key].ToString().Split(";"[0]) : new string[0];
    }
}

public class DialogDataPaser
{
    //格式
    //1;2;3...
    public static List<int> IntList(string strParser)
    {
        List<int> myList = new List<int>();

        string[] arrStrings = strParser.Split(';');
        for (int i = 0; i < arrStrings.Length; ++i )
        {
            int nTest = 0;
            if (int.TryParse(arrStrings[i], out nTest))
            {
                myList.Add(nTest);
            }
        }

        return myList;
    }

    //格式
    //100101;100102;100103....
    public static List<string> StringList(string strParser)
    {
        List<string> myList = new List<string>();

        string[] arrStrings = strParser.Split(';');
        for (int i = 0; i < arrStrings.Length; ++i)
        {
            myList.Add(arrStrings[i]);
        }

        return myList;
    }

    //格式
    //100101,1;100102,2;100103,3.....
    public static Dictionary<string, int> StringIntDic(string strParser)
    {
        Dictionary<string, int> myList = new Dictionary<string, int>();

        string[] arrStrings = strParser.Split(';');
        for (int i = 0; i < arrStrings.Length; ++i)
        {
            string[] oneString = arrStrings[i].Split(',');
            if (oneString.Length != 2)
            {
                continue;
            }

            int nTestValue;
            if (!int.TryParse(oneString[1], out nTestValue))
            {
                continue;
            }

            myList.Add(oneString[0], nTestValue);
            
        }

        return myList;
    }

    //格式
    //1,2;1,3;2,4....
    public static List<int[]> Int2ArrayList(string strParser)
    {
        List<int[]> myList = new List<int[]>();

        string[] arrStrings = strParser.Split(';');
        for (int i = 0; i < arrStrings.Length; ++i)
        {
            string[] oneString = arrStrings[i].Split(',');
            int nTestOne;
            int nTestTwo;

            if (!int.TryParse(oneString[0], out nTestOne) || !int.TryParse(oneString[1], out nTestTwo))
            {
                continue;
            }

            int[] oneArray = {nTestOne,nTestTwo};
            myList.Add(oneArray);
    
        }

        return myList;
    }
}