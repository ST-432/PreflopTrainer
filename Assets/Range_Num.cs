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
    //�R�s�[�R���X�g���N�^
    public Range_Num(Range_Num copyRN)
    {
        RaiseFrequency = copyRN.RaiseFrequency;
        CallFrequency = copyRN.CallFrequency;
        FoldFrequency = copyRN.FoldFrequency;
    }
    //�p�x�̃Z�b�^�[
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
    //�p�x�̃Q�b�^�[
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
//�n���h�����W�̏W��
public class AggregationHandR
{
    public Dictionary<Tuple<int, int, int>, Range_Num[,]> SituationAggregation = new Dictionary<Tuple<int, int, int>, Range_Num[,]>();
    public AggregationHandR()
    {

    }
    public bool SetRange(Tuple<int, int, int> key, Range_Num[,] Hrange)
    {
        //������Range_Num[,]�̃R�s�[���쐬(�Q�ƌ^�̂���)
        Range_Num[,] instantcopyrange = copyRange(Hrange);
        //�L�[���L��Ƃ��͍X�V�A����ȊO�͍쐬
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
    //���ׂẴV�`���G�[�V�����̃����W���擾
    public Dictionary<Tuple<int, int, int>, Range_Num[,]> getRangeALL()
    {
        return SituationAggregation;
    }
    //1�̃V�`���G�[�V�����̃����W���擾
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
    //json�t�@�C���ւ̃Z�[�u����
    public void SaveJson_AggregationHandRange()
    {
        // JSON��ۑ�����t�@�C���p�X
        //C:/Users/81803/AppData/LocalLow/DefaultCompany/PreflopTrainer\data.json
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");
        // Dictionary��JSON�ɕϊ�
        //string json = JsonConvert.SerializeObject(SituationAggregation, Newtonsoft.Json.Formatting.Indented);
        string json = JsonConvert.SerializeObject(SituationAggregation, Formatting.Indented, new TupleInt3Converter());

        // JSON�t�@�C���ɏ�������
        File.WriteAllText(filePath, json);
    }
    //json�t�@�C������̃��[�h����
    public bool LoadJson_AggregationHandRange()
    {
        // JSON��ۑ�����t�@�C���p�X
        //C:/Users/81803/AppData/LocalLow/DefaultCompany/PreflopTrainer\data.json
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");
        // JSON�t�@�C������ǂݍ���
        if(File.Exists(filePath))
        {
            string loadedJson = File.ReadAllText(filePath);

            // JSON���f�V���A���C�Y����Dictionary�ɕ���
            //SituationAggregation = JsonConvert.DeserializeObject<Dictionary<Tuple<int, int, int>, Range_Num[,]>>(loadedJson);
            SituationAggregation = JsonConvert.DeserializeObject<Dictionary<Tuple<int, int, int>, Range_Num[,]>>(loadedJson, new TupleInt3Converter());

            return true;
        }
        else
        {
            Debug.Log("json�t�@�C���͌�����Ȃ�����");
            return false;
        }
    }
}
//Divtionary�̃L�[��Tuple<int,int,int>�^�ɂ��Ă��邽�ߕ������ɃG���[���N����B���̂��߃J�X�^���V���A���C�U�N���X���쐬
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

//            // �}���`�f�B�����V�����z���2�����z��ɕϊ�
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


//Divtionary�̃L�[��Tuple<int,int,int>�^�ɂ��Ă��邽�ߕ������ɃG���[���N����B���̂��߃J�X�^���V���A���C�U�N���X���쐬
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

            // �}���`�f�B�����V�����z���2�����z��ɕϊ�
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

        // JSON����f�[�^��ǂݍ��݁A�z��𕜌�����
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