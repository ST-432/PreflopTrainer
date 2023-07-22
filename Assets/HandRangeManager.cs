using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class HandRangeManager : MonoBehaviour
{
    // Start is called before the first frame update
    Range_Num Rangevalue;
    AggregationHandR AggregationHandRange;

    public Material material;
    //設定値
    public int Leftmargin,Topmargin;
    //静的にした
    public static int s_rows=13, s_cols=13;
    public int SelectHandX = 0;
    public int SelectHandY = 0;
    //プレハブ取得時のオブジェクト
    public GameObject PreSDropDawn;
    public GameObject Handprefub;
    public Text pretex;
    public Button Buttonprefub;
    private Text Combination;

    public Transform canvas;
    //public InputField Afrequencyprefub;
    public Slider FSliderPrehub;

    //ローカルで保持しておく変数、オブジェクト
    //値の保存はシーン終了時またはレンジ要素クリック時
    //1シチュエーションにおけるレンジ全体
    GameObject[,] HandRangeObj = new GameObject[s_rows, s_cols];    //[Y,X]
    Range_Num[,] HandRangeNum = new Range_Num[s_rows, s_cols];      //[Y,X]
    //シチュエーションの集合
    //Dictionary<Tuple<int,int,int>, GameObject[,]> SituationAggregation = new Dictionary<Tuple<int, int, int>, GameObject[,]>();

    Tuple<int , int, int> KeyNum;
    //シチュエーション選択ドロップダウン
    public Dropdown Hdropdown;
    public Dropdown Vdropdown;
    public Dropdown Bdropdown;
    //アクション頻度を入力するinputfield
    //public InputField RaiseFre;
    //public InputField CallFre;
    //public InputField FoldFre;
    //利便性からスライダーに変更
    public Slider RaiseFSlider;
    public Slider CallFSlider;
    public Slider FoldFSlider;
    //保存ボタン
    public Button SaveFreq;
    public Button Go_mainmenu;
    //選択中のハンドのテキスト
    Text SelectHandText;
    //選択中のハンドの場所
    int localSelectHandX;   //横
    int localSelectHandY;   //縦
    //クリックされたハンドのオブジェクト
    GameObject clickedGameObject;
    //アクション頻度を表示するテキスト
    Text texRpar;
    Text texCpar;
    Text texFpar;
    Color enablecolor;
    Color ablecolor;
    void Start()
    {
        AggregationHandRange = new AggregationHandR();
        GameObject mainCamObj;
        Camera cam;
        enablecolor= new Color(0.3f, 0.3f, 0.3f, 1.0f);
        ablecolor = new Color(1.0f, 1.0f, 1.0f, 1.0f);



        string[] number = new string[]
        {
            "A","K","Q","J","T","9","8","7","6","5","4","3","2"
        };
        mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");
        cam = mainCamObj.GetComponent<Camera>();
        //カメラの左下座標を取得しワールド座標に変換
        Vector2 LeftBottom = cam.ViewportToWorldPoint(Vector2.zero);
        //Vector3 LeftTop = cam.ViewportToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        for (int Yvec = 0; Yvec < s_rows; Yvec++)
        {
            for (int Xvec = 0; Xvec < s_cols; Xvec++)
            {
                GameObject HandObj;
                HandObj = Instantiate(Handprefub);
                //位置の調整　左上から右に順番に
                Vector2 pos;
                pos.x = LeftBottom.x+ Xvec + Leftmargin;
                pos.y = (-LeftBottom.y)- Yvec - Topmargin;
                HandObj.transform.position = pos;
                string A,NumCom;
                A = "Hand";
                //A = "Hand" + col.ToString("0") + "," + row.ToString("0");
                if(Xvec>Yvec)   //スーテッド
                {
                    NumCom =  number[Yvec] +number[Xvec] + "s";
                }
                else if(Xvec<Yvec)  //オフスート
                {
                    NumCom = number[Xvec] + number[Yvec]+"o";
                }
                else　//ポケット
                {
                    NumCom = number[Yvec] + number[Xvec];
                }

                //A = "Hand" + number[Yvec] + "," + number[Xvec];
                HandObj.name = A+ NumCom;
                //Debug.Log(obj.name);
                Combination= HandObj.GetComponentInChildren<Text>();
                if (Combination != null)
                {
                    Combination.text = NumCom;
                }

                //全部白だと見にくいので色の変更
                if ((Xvec + Yvec) %2 == 0)
                {
                    HandObj.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f,1.0f);
                }
                else 
                {
                    HandObj.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f,0.8f, 1.0f);
                }
                InputHandClick InpHa = HandObj.GetComponent<InputHandClick>();
                InpHa.SelHandX = Xvec;
                InpHa. SelHandY = Yvec;
                HandRangeObj[Yvec, Xvec] = HandObj;

                //HandRangeObj[Yvec, Xvec].Select
            }
         
        }



        Dropdown ddtmp;
        Vector3 Setpos;
        Setpos = HandRangeObj[0, s_rows - 1].transform.position;
        Setpos.x = Setpos.x +2;
        //Heroのポジションを決定するドロップダウンの作成
        GameObject HeroSObj;
        HeroSObj = Instantiate(PreSDropDawn, Setpos, Quaternion.identity, canvas.transform);
        HeroSObj.transform.SetParent(canvas.transform, false);
        List<string> optionlist = new List<string>();
        ddtmp = HeroSObj.GetComponent<Dropdown>();
        if (ddtmp != null)
        {
            ddtmp.ClearOptions();
            optionlist.Add("none");
            optionlist.Add("UTG");
            optionlist.Add("HJ");
            optionlist.Add("CO");
            optionlist.Add("BTN");
            optionlist.Add("SB");
            optionlist.Add("BB");
            ddtmp.AddOptions(optionlist);
        }
        Hdropdown = ddtmp;
        Hdropdown.name = "HeroPosition";
        //Textの作成
        Setpos.x = Setpos.x + 3;
        Text texH = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texH.transform.position = Setpos;
        texH.text = "(Hero) Vs";

        
        //Villainのポジションを決定するドロップダウンの作成
        optionlist.Clear();
        Setpos.x = Setpos.x + 3;
        GameObject VillainSObj;
        VillainSObj = Instantiate(PreSDropDawn, Setpos, Quaternion.identity, canvas.transform);
        VillainSObj.transform.SetParent(canvas.transform, false);
        ddtmp = VillainSObj.GetComponent<Dropdown>();
        if (ddtmp != null)
        {
            ddtmp.ClearOptions();
            optionlist.Add("none");
            optionlist.Add("UTG");
            optionlist.Add("HJ");
            optionlist.Add("CO");
            optionlist.Add("BTN");
            optionlist.Add("SB");
            optionlist.Add("BB");
            ddtmp.AddOptions(optionlist);
        }
        Vdropdown = ddtmp;
        Vdropdown.name = "VillainPosition";
        //Textの作成
        Setpos.x = Setpos.x + 3;
        Text texV = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texV.transform.position = Setpos;
        texV.text = "Villain";

        //Villainのポジションを決定するドロップダウンの作成
        optionlist.Clear();
        Setpos.x = Setpos.x -6;
        Setpos.y = Setpos.y - 2;
        GameObject BetSObj;
        BetSObj = Instantiate(PreSDropDawn, Setpos, Quaternion.identity, canvas.transform);
        BetSObj.transform.SetParent(canvas.transform, false);
        ddtmp = BetSObj.GetComponent<Dropdown>();
        if (ddtmp != null)
        {
            ddtmp.ClearOptions();
            optionlist.Add("none");
            optionlist.Add("Open");
            optionlist.Add("3bet");
            optionlist.Add("4bet");
            optionlist.Add("5bet");
            ddtmp.AddOptions(optionlist);
        }
        Bdropdown = ddtmp;
        Bdropdown.name = "Xbet";
        //Textの作成
        Setpos.x = Setpos.x + 3;
        Text texB = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texB.transform.position = Setpos;
        texB.text = "Bet Pot";

        //先にAddListnerすると作るときに値の変更を検知してしまうのでここで
        Hdropdown.onValueChanged.AddListener((value) => { DropDownchange(Hdropdown.name, value); });
        Vdropdown.onValueChanged.AddListener((value) => { DropDownchange(Vdropdown.name, value); });
        Bdropdown.onValueChanged.AddListener((value) => { DropDownchange(Bdropdown.name, value); });
        // Update is called once per frame

        //アクション頻度入力欄作成
        //Raise頻度
        Setpos.x = Setpos.x - 3;
        Setpos.y = Setpos.y - 4;
        Text texRF = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texRF.transform.position = Setpos;
        texRF.text = "Raise";
        Slider FreObj;
        Setpos.x = Setpos.x +3;
        FreObj = Instantiate(FSliderPrehub, Setpos, Quaternion.identity, canvas.transform);
        FreObj.transform.SetParent(canvas.transform, false);
        RaiseFSlider = FreObj;
        RaiseFSlider.name = "RaiseFrequency";
        Setpos.x = Setpos.x + 4;
        texRpar = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texRpar.transform.position = Setpos;
        texRpar.text = "%";

        //call頻度
        Setpos.x = Setpos.x - 7;
        Setpos.y = Setpos.y - 2;
        Text texCF = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texCF.transform.position = Setpos;
        texCF.text = "Call";
        Setpos.x = Setpos.x + 3;
        FreObj = Instantiate(FSliderPrehub, Setpos, Quaternion.identity, canvas.transform);
        FreObj.transform.SetParent(canvas.transform, false);
        CallFSlider = FreObj;
        CallFSlider.name = "CallFrequency";
        Setpos.x = Setpos.x + 4;
        texCpar = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texCpar.transform.position = Setpos;
        texCpar.text = "%";

        //fold頻度
        Setpos.x = Setpos.x - 7;
        Setpos.y = Setpos.y - 2;
        Text texFF = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texFF.transform.position = Setpos;
        texFF.text = "Fold";
        Setpos.x = Setpos.x + 3;
        FreObj = Instantiate(FSliderPrehub, Setpos, Quaternion.identity, canvas.transform);
        //FreObj = Instantiate(Afrequencyprefub);
        FreObj.transform.SetParent(canvas.transform, false);
        FoldFSlider = FreObj;
        FoldFSlider.name = "FoldFrequency";
        Setpos.x = Setpos.x + 4;
        texFpar = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texFpar.transform.position = Setpos;
        texFpar.text = "%";


        //ValueChangedだと入力中に入る
        //RaiseFre.onEndEdit.AddListener(InputFieldchange("引数","A"));
        //RaiseFre.onEndEdit.AddListener(() => { InputFieldchange("引数","a"); });
        RaiseFSlider.onValueChanged.AddListener((value) => { InputFieldchange(RaiseFSlider,texRpar, value); });
        CallFSlider.onValueChanged.AddListener((value) => { InputFieldchange(CallFSlider,texCpar, value); });
        FoldFSlider.onValueChanged.AddListener((value) => { InputFieldchange(FoldFSlider,texFpar, value); });

        //保存ボタンの作成
        Setpos.x = Setpos.x - 7;
        Setpos.y = Setpos.y - 2;
        Button BT = Instantiate(Buttonprefub, Setpos, Quaternion.identity, canvas.transform);
        BT.transform.position = Setpos;
        SaveFreq = BT;
        SaveFreq.name = "SaveButton";
        SaveFreq.onClick.AddListener(ClickSave);

        //メインメニュー遷移ボタンの作成
        Setpos.x = Setpos.x +6;
        Button BTmm = Instantiate(Buttonprefub, Setpos, Quaternion.identity, canvas.transform);
        BTmm.transform.position = Setpos;
        Go_mainmenu = BTmm;
        Go_mainmenu.name = "Go_MainMenu";
        //横幅だけプレハブの2倍に
        RectTransform Go_main_rt = Go_mainmenu.GetComponent<RectTransform>();
        float originalWidth= Go_main_rt.rect.width;
        float originalHeight = Go_main_rt.rect.height;
        float newWidth = originalWidth * 2f;
        Go_main_rt.sizeDelta=new Vector2 (newWidth, originalHeight);
        Text MainmenuBTCap= Go_mainmenu.GetComponentInChildren<Text>();
        MainmenuBTCap.text = "メインメニュー";
        Go_mainmenu.onClick.AddListener(ClickGo_MainMenu);

        Setpos.x = Setpos.x - 6;
        //選択ハンドの表示欄
        Setpos = Hdropdown.transform.position;
        Setpos.x = Setpos.x + 3;
        Setpos.y = Setpos.y - 4;
        Text texHra = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texHra.transform.position = Setpos;
        texHra.text = "";
        SelectHandText = texHra;

        //--------------------------------------------------------------------------------------
        //初期値セット、読み込めた場合は読み込み値
        if (AggregationHandRange.LoadJson_AggregationHandRange())
        {
            //読み込めた場合
        }
        else
        {
            //読み込めなかった場合
            
        }
        //読み込めた場合も読み込めなかった場合も最初はドロップダウンが全てnoneからなのでHandRangeNumは初期値でok
        ResetHandRangeNum();
        ClickHandNum(localSelectHandY, localSelectHandX);
        RaiseFSlider.enabled = false;
        CallFSlider.enabled = false;
        FoldFSlider.enabled = false;
        changeable(false, RaiseFSlider);
        changeable(false,CallFSlider);
        changeable(false,FoldFSlider);

        //--------------------------------------------------------------------------------------


    }
    private void OnDisable()
    {
        //終了時処理
        AggregationHandRange.SaveJson_AggregationHandRange();
    }
    void Update()
    {
        //ハンド選択時マウスクリックの場所からどのハンドを選択したか検知
        if (Input.GetMouseButtonDown(0))
        {

            clickedGameObject = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

            if (hit2d)
            {
                clickedGameObject = hit2d.transform.gameObject;
                InputHandClick InpHa = clickedGameObject.GetComponent<InputHandClick>();
                ClickHandNum(InpHa.SelHandX, InpHa.SelHandY);
            }
        }
    }
    public void DropDownchange(string changedropdawn,int i)
    {

        //シチュエーションを変えたときにHandRangeNumをクリアして読み込みをしないといけない
        bool checkSit = true;
        if(i==0)
        {
            checkSit = false;
        }
        if (changedropdawn== Hdropdown.name)
        {
            //Heroのポジションを選ぶドロップダウンがが変更された時
            Dropdown ddtmp;
            ddtmp = Vdropdown.GetComponent<Dropdown>();
            //betシチュエーションがopenではなくVillainポジションがnoneではなくVillainと同じポジションが指定されてしまった時
            if (Vdropdown.value== Hdropdown.value && Vdropdown.value!=0 &&Bdropdown.value!=1)
            {
                //noneじゃなくて選んでいるポジションが被ってしまった時noneに戻す
                Hdropdown.value = 0;
                Debug.Log("同じ値を選んでいる");
                checkSit = false;
            }
            //
            if(Bdropdown.value!=1&& Bdropdown.value != 0)
            {
                if (Hdropdown.value < Vdropdown.value)
                {
                    //HeroがVillainよりも前のポジションの時
                    if (Bdropdown.value == 2 || Bdropdown.value == 4)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("存在しないシチュエーション3bet or 5bet");
                        checkSit = false;
                    }
                }
                else if (Hdropdown.value > Vdropdown.value)
                {
                    if (Bdropdown.value == 3)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("存在しないシチュエーションopen or 4bet");
                        checkSit = false;
                    }
                }
            }
            //BBのopenをさせない
            if(Hdropdown.value ==6&&Bdropdown.value==1)
            {
                Hdropdown.value = 0;
                checkSit = false;
            }
            //betシチュエーションを再設定
            //Bdropdown.value = 0;
        }
        else if(changedropdawn == Vdropdown.name)
        {
            //Villainのポジションを選ぶドロップダウンがが変更された時
            if (Hdropdown.value == Vdropdown.value && Hdropdown.value != 0)
            {
                //noneじゃなくて選んでいるポジションが被ってしまった時noneに戻す
                Vdropdown.value = 0;
                Debug.Log("同じ値を選んでいる");
                checkSit = false;
            }
            if (Bdropdown.value != 1&&Bdropdown.value != 0)
            {
                if (Hdropdown.value < Vdropdown.value)
                {
                    //HeroがVillainよりも前のポジションの時
                    if (Bdropdown.value == 2 || Bdropdown.value == 4)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("存在しないシチュエーション3bet or 5bet");
                        checkSit = false;
                    }
                }
                else if (Hdropdown.value > Vdropdown.value)
                {
                    if (Bdropdown.value == 3)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("存在しないシチュエーション4bet");
                        checkSit = false;
                    }
                }
            }

            //betシチュエーションを再設定
            //Bdropdown.value = 0;
        }
        else if(changedropdawn == Bdropdown.name)
        {
            //bet回数が変更された時
            if(Bdropdown.value ==1)
            {
                //openレンジなのでvillainは無効に
                Vdropdown.value = 0;
                Vdropdown.enabled = false;
                changeable(false, Vdropdown);

                //BBopenはないので強制的にnoneに
                if (Hdropdown.value==6)
                {
                    Hdropdown.value = 0;
                    checkSit = false;
                }
            }
            else
            {
                Vdropdown.enabled = true;
                changeable(true, Vdropdown);
            }
            //HeroのポジションがVillainより前かあとかでopen,4bet or 3bet,5betに場合分け
            //IP,OOPで分けるとBTN以降がめんどくさくなるので早いor遅い
            //open,4bet しかないポジション関係の時
            if (Hdropdown.value<Vdropdown.value)
            {
                //HeroがVillainよりも前のポジションの時
                //3bet,5betは存在しない
                if(Bdropdown.value ==2||Bdropdown.value ==4)
                {
                    Bdropdown.value = 0;
                    Debug.Log("存在しないシチュエーション3bet or 5bet");
                    checkSit = false;
                }
            }
            //3bet,5bet しかないポジション関係の時
            else if (Hdropdown.value > Vdropdown.value)
            {
                //open,4betは存在しない
                //openのときは絶対にHeroposition>Villainposition(0)になるため条件づけしない
                if (Bdropdown.value == 3)
                {
                    Bdropdown.value = 0;
                    Debug.Log("存在しないシチュエーションopen or 4bet");
                    checkSit = false;
                }
                //
                else if (Vdropdown.value == 0 && Bdropdown.value != 1)    //Villainpositionがnoneのときはbetシチュエーションがopen以外弾く
                {
                    Bdropdown.value = 0;
                    Debug.Log("存在しないシチュエーションVillain(none)で3bet or 5bet");
                    checkSit = false;
                }
            }

        }
        else
        {
            checkSit=false;
        }
        //いらないかも
        if (Hdropdown.value == 0 || Bdropdown.value == 0)
        {
            checkSit = false;
        }
        if (checkSit)
        {
            //レンジの変更処理
            //betシチュエーションがopenのときはVillainは関係ないのでkeyを0に
            int Villainkey;
            if(Bdropdown.value==1)
            {
                Villainkey = 0;
            }
            else
            {
                Villainkey = Vdropdown.value;
            }
            KeyNum = new Tuple<int, int, int>(Hdropdown.value, Villainkey, Bdropdown.value);
            if(AggregationHandRange.getRangeOne(KeyNum,ref HandRangeNum))
            {
                //keyが見つかったため保存値であるrangeに変更
                Debug.Log("保存値のセット");
            }
            else
            {
                //キーが見つからなかったため初期値をセット
                ResetHandRangeNum();
                Debug.Log("初期値のセット");
            }
            Debug.Log("シチュエーション変更のためレンジをセット");

            RaiseFSlider.enabled = true;
            CallFSlider.enabled = true;
            FoldFSlider.enabled = true;
            changeable(true, RaiseFSlider);
            changeable(true, CallFSlider);
            changeable(true, FoldFSlider);
            //初期値のセット
            RaiseFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("R");
            CallFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("C");
            FoldFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("F");
            InputFieldchange(RaiseFSlider, texRpar, RaiseFSlider.value);
            InputFieldchange(CallFSlider, texCpar, CallFSlider.value);
            InputFieldchange(FoldFSlider, texFpar, FoldFSlider.value);
        }
        else
        {

            Debug.Log("セットされているレンジ変更なし");
            //正しいシチュエーションじゃないので初期値にセット
            ResetHandRangeNum();
            SelectHandText.text = HandRangeObj[0, 0].name;
            localSelectHandX = 0;
            localSelectHandY = 0;
            RaiseFSlider.value = HandRangeNum[0, 0].getFreq("R");
            CallFSlider.value = HandRangeNum[0, 0].getFreq("C");
            FoldFSlider.value = HandRangeNum[0, 0].getFreq("F");
            InputFieldchange(RaiseFSlider, texRpar, RaiseFSlider.value);
            InputFieldchange(CallFSlider, texCpar, CallFSlider.value);
            InputFieldchange(FoldFSlider, texFpar, FoldFSlider.value);
            //スライダーバーを動かないように
            RaiseFSlider.enabled = false;
            CallFSlider.enabled = false;
            FoldFSlider.enabled = false;
            changeable(false, RaiseFSlider);
            changeable(false, CallFSlider);
            changeable(false, FoldFSlider);
        }

    }

    public void InputFieldchange(Slider Sl ,Text freqval, float val)
    {
        //スライダーを5刻みにする処理
        if ((val) % 5 == 0)
        {
            freqval.text = val.ToString() + "%";
            //Sl.value = val;
        }
        else
        {

            freqval.text = (val-(val%5)).ToString() + "%";
            Sl.value = (val - (val % 5));
        }

    }
    //保存ボタンの押下時の処理
    public void ClickSave()
    {
        if (Hdropdown.value == 0 || Bdropdown.value == 0)
        {
            //ポップアップを表示したい
            Debug.Log("シチュエーションが選択されていない");
        }
        else if(Vdropdown.value == 0&& Bdropdown.value != 1)
        {
            //Vdropdawn==0のときはopenがあるのでOpenじゃないときは弾く
            Debug.Log("シチュエーションが選択されていない");
        }
        else
        {
            if (RaiseFSlider.value + CallFSlider.value + FoldFSlider.value == 100)
            {
                KeyNum = new Tuple<int, int, int>(Hdropdown.value, Vdropdown.value, Bdropdown.value);
                Rangevalue = new Range_Num();
                Rangevalue.SetFreq("R", (int)RaiseFSlider.value);
                Rangevalue.SetFreq("C", (int)CallFSlider.value);
                Rangevalue.SetFreq("F", (int)FoldFSlider.value);

                HandRangeNum[localSelectHandY,localSelectHandX]= Rangevalue;

                if(AggregationHandRange.SetRange(KeyNum, HandRangeNum))
                {
                    Debug.Log("更新");
                }
                else
                {
                    Debug.Log("新規代入");
                }
                
            }
            else
            {
                //ポップアップを表示したい
                Debug.Log("頻度の合計が100になっていない");

            }
        }
    }

    public void ClickGo_MainMenu()
    {
        AggregationHandRange.SaveJson_AggregationHandRange();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void ClickHandNum(int X ,int Y)
    {
        //選択ハンドの表示
        SelectHandText.text = HandRangeObj[Y,X].name;
        localSelectHandX = X;
        localSelectHandY=Y;
        RaiseFSlider.value = HandRangeNum[Y, X].getFreq("R");
        CallFSlider.value = HandRangeNum[Y, X].getFreq("C");
        FoldFSlider.value = HandRangeNum[Y, X].getFreq("F");
        InputFieldchange(RaiseFSlider, texRpar, RaiseFSlider.value);
        InputFieldchange(CallFSlider, texCpar, CallFSlider.value);
        InputFieldchange(FoldFSlider, texFpar, FoldFSlider.value);
        //HandRangeObj
    }
    public void ResetHandRangeNum()
    {
        for(int i=0;i<s_rows;i++)
        {
            for(int j=0;j<s_cols;j++)
            {
                HandRangeNum[i,j]= new Range_Num();
            }

        }
    }
    public void changeable(bool va ,Slider slider)
    {
        Image background = slider.transform.Find("Background").GetComponent<Image>();
        Image fill = slider.fillRect.GetComponent<Image>();
        Image handle = slider.handleRect.GetComponent<Image>();
        if(va ==true)
        {
            background.color = ablecolor;
            fill.color = ablecolor;
            handle.color = ablecolor;
        }
        else
        {
            background.color = enablecolor;
            fill.color = enablecolor;
            handle.color = enablecolor;
        }
    }
    public void changeable(bool va, Dropdown dropdawn)
    {
        Image objI = dropdawn.transform.GetComponent<Image>();
        if(va==true)
        {
            objI.color = ablecolor;
        }
        else
        {
            objI.color = enablecolor;
        }
    }
}
