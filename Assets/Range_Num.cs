using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class Range_Num
{
    //private int RaiseFrequency;
    //private int CallFrequency;
    //private int FoldFrequency;
    public int RaiseFrequency;
    public int CallFrequency;
    public int FoldFrequency;
    public Range_Num()
    {
        RaiseFrequency = 0;
        CallFrequency = 0;
        FoldFrequency = 100;

    }
    //コピーコンストラクタ
    public Range_Num(Range_Num copyRN)
    {
        RaiseFrequency = copyRN.RaiseFrequency;
        CallFrequency = copyRN.CallFrequency;
        FoldFrequency = copyRN.FoldFrequency;
    }
    //頻度のセッター
    public bool SetFreq(string Si,int val)
    {
        if(Si=="R")
        {
            RaiseFrequency = val;
            return true;
        }
        else if(Si=="C")
        {
            CallFrequency = val;
            return true;
        }
        else if(Si=="F")
        {
            FoldFrequency = val;
            return true;
        }
        else
        {
            return false;
        }
    }
    //頻度のゲッター
    public int getFreq(string Si)
    {
        if (Si == "R")
        {
            return RaiseFrequency;
        }
        else if (Si == "C")
        {
            return CallFrequency;
        }
        else if (Si == "F")
        {
            return FoldFrequency;
        }
        else
        {
            return 101;
        }
    }

}
//ハンドレンジの集合
public class AggregationHandR
{
    public Dictionary<Tuple<int, int, int>, Range_Num[,]> SituationAggregation = new Dictionary<Tuple<int, int, int>, Range_Num[,]>();
    public AggregationHandR()
    {

    }
    public bool SetRange(Tuple<int, int, int> key, Range_Num[,] Hrange)
    {
        //引数のRange_Num[,]のコピーを作成(参照型のため)
        Range_Num[,] instantcopyrange = copyRange(Hrange);
        //キーが有るときは更新、それ以外は作成
        if (SituationAggregation.ContainsKey(key))
        {
            SituationAggregation[key] = instantcopyrange;
            return true;
        }
        else
        {
            SituationAggregation.Add(key, instantcopyrange);
            return false;
        }
        
    }
    //すべてのシチュエーションのレンジを取得
    public Dictionary<Tuple<int, int, int>, Range_Num[,]> getRangeALL()
    {
        return SituationAggregation;
    }
    //1つのシチュエーションのレンジを取得
    public bool getRangeOne(Tuple<int, int, int> key, ref Range_Num[,] range)
    {
        if(SituationAggregation.ContainsKey(key))
        {
            range = copyRange(SituationAggregation[key]);
            
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public Range_Num[,] copyRange(Range_Num[,] range)
    {
        Range_Num[,] instantcopyrange = new Range_Num[13, 13];
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                instantcopyrange[i, j] = new Range_Num(range[i, j]);
            }
        }
        return instantcopyrange;
    }
    //jsonファイルへのセーブ処理
    public void SaveJson_AggregationHandRange()
    {
        // JSONを保存するファイルパス
        //C:/Users/81803/AppData/LocalLow/DefaultCompany/PreflopTrainer\data.json
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");
        // DictionaryをJSONに変換
        //string json = JsonConvert.SerializeObject(SituationAggregation, Newtonsoft.Json.Formatting.Indented);
        string json = JsonConvert.SerializeObject(SituationAggregation, Formatting.Indented, new TupleInt3Converter());

        // JSONファイルに書き込み
        File.WriteAllText(filePath, json);
    }
    //jsonファイルからのロード処理
    public bool LoadJson_AggregationHandRange()
    {
        // JSONを保存するファイルパス
        //C:/Users/81803/AppData/LocalLow/DefaultCompany/PreflopTrainer\data.json
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");
        // JSONファイルから読み込み
        if(File.Exists(filePath))
        {
            string loadedJson = File.ReadAllText(filePath);

            // JSONをデシリアライズしてDictionaryに復元
            //SituationAggregation = JsonConvert.DeserializeObject<Dictionary<Tuple<int, int, int>, Range_Num[,]>>(loadedJson);
            SituationAggregation = JsonConvert.DeserializeObject<Dictionary<Tuple<int, int, int>, Range_Num[,]>>(loadedJson, new TupleInt3Converter());

            return true;
        }
        else
        {
            Debug.Log("jsonファイルは見つからなかった");
            return false;
        }
    }
}
//DivtionaryのキーをTuple<int,int,int>型にしているため復元時にエラーが起きる。そのためカスタムシリアライザクラスを作成
//[System.Serializable]
//public class TupleInt3Converter : JsonConverter<Tuple<int, int, int>>
//{
//    public override void WriteJson(JsonWriter writer, Tuple<int, int, int> value, JsonSerializer serializer)
//    {
//        serializer.Serialize(writer, new int[] { value.Item1, value.Item2, value.Item3 });
//    }

//    public override Tuple<int, int, int> ReadJson(JsonReader reader, System.Type objectType, Tuple<int, int, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
//    {
//        int[] array = serializer.Deserialize<int[]>(reader);
//        return new Tuple<int, int, int>(array[0], array[1], array[2]);
//    }
//}

//public class TupleInt3Converter : JsonConverter<Dictionary<Tuple<int, int, int>, Range_Num[,]>>
//{
//    public override void WriteJson(JsonWriter writer, Dictionary<Tuple<int, int, int>, Range_Num[,]> value, JsonSerializer serializer)
//    {
//        writer.WriteStartObject();

//        foreach (var kvp in value)
//        {
//            Tuple<int, int, int> key = kvp.Key;
//            Range_Num[,] dataArray = kvp.Value;

//            writer.WritePropertyName($"{key.Item1},{key.Item2},{key.Item3}");

//            // マルチディメンション配列を2次元配列に変換
//            int rows = dataArray.GetLength(0);
//            int cols = dataArray.GetLength(1);
//            Range_Num[][] jaggedArray = new Range_Num[rows][];

//            for (int i = 0; i < rows; i++)
//            {
//                jaggedArray[i] = new Range_Num[cols];
//                for (int j = 0; j < cols; j++)
//                {
//                    jaggedArray[i][j] = dataArray[i, j];
//                }
//            }

//            serializer.Serialize(writer, jaggedArray);
//        }

//        writer.WriteEndObject();
//    }

//    public override Dictionary<Tuple<int, int, int>, Range_Num[,]> ReadJson(JsonReader reader, Type objectType, Dictionary<Tuple<int, int, int>, Range_Num[,]> existingValue, bool hasExistingValue, JsonSerializer serializer)
//    {
//        throw new NotImplementedException("Deserialization is not supported for this custom converter.");
//    }
//}


//DivtionaryのキーをTuple<int,int,int>型にしているため復元時にエラーが起きる。そのためカスタムシリアライザクラスを作成
public class TupleInt3Converter : JsonConverter<Dictionary<Tuple<int, int, int>, Range_Num[,]>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<Tuple<int, int, int>, Range_Num[,]> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            Tuple<int, int, int> key = kvp.Key;
            Range_Num[,] dataArray = kvp.Value;

            writer.WritePropertyName($"{key.Item1},{key.Item2},{key.Item3}");

            // マルチディメンション配列を2次元配列に変換
            int rows = dataArray.GetLength(0);
            int cols = dataArray.GetLength(1);
            Range_Num[][] jaggedArray = new Range_Num[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new Range_Num[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = dataArray[i, j];
                }
            }

            serializer.Serialize(writer, jaggedArray);
        }

        writer.WriteEndObject();
    }

    public override Dictionary<Tuple<int, int, int>, Range_Num[,]> ReadJson(JsonReader reader, Type objectType, Dictionary<Tuple<int, int, int>, Range_Num[,]> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        Dictionary<Tuple<int, int, int>, Range_Num[,]> result = new Dictionary<Tuple<int, int, int>, Range_Num[,]>();

        // JSONからデータを読み込み、配列を復元する
        var jsonObject = serializer.Deserialize<Dictionary<string, Range_Num[][]>>(reader);

        foreach (var kvp in jsonObject)
        {
            string[] keys = kvp.Key.Split(',');
            int key1 = int.Parse(keys[0]);
            int key2 = int.Parse(keys[1]);
            int key3 = int.Parse(keys[2]);

            Range_Num[,] dataArray = new Range_Num[kvp.Value.Length, kvp.Value[0].Length];
            for (int i = 0; i < kvp.Value.Length; i++)
            {
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    dataArray[i, j] = kvp.Value[i][j];
                }
            }

            result.Add(new Tuple<int, int, int>(key1, key2, key3), dataArray);
        }

        return result;
    }
}