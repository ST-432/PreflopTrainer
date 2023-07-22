using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManagerSqript : MonoBehaviour
{
    // Start is called before the first frame update
    Range_Num Rangevalue;
    AggregationHandR AGHandRange;
    public static int s_rows = 13, s_cols = 13;
    public int SelectHandX = 0;
    public int SelectHandY = 0;
    Range_Num[,] HandRangeNum = new Range_Num[s_rows, s_cols];      //[Y,X]
    //ActionInfo_Text
    public Text TX_Heropos;
    public Text TX_Villainpos;
    public Text TX_XBet;
    public Text TX_RaiseFreq;
    public Text TX_CallFreq;
    public Text TX_FoldFreq;
    public Text TX_Answer;
    //TableText
    public GameObject Obj_player1_H;
    public GameObject Obj_player2;
    public GameObject Obj_player3;
    public GameObject Obj_player4;
    public GameObject Obj_player5;
    public GameObject Obj_player6;
    public GameObject Obj_Table;

    //ActionInfo_Button
    public Button BT_PlayAction_Raise;
    public Button BT_PlayAction_Call;
    public Button BT_PlayAction_Fold;
    public Button BT_NextProblem;
    //���C�����j���[�J�ڃ{�^��
    public Button BT_GoMainMenu;
    //�g���e�I�u�W�F�N�g
    public GameObject Obj_Freq_Num;
    public GameObject Obj_Cap_Freq;
    public GameObject Obj_PlayAction_Button;
    public GameObject Obj_BetogMoney;
    List<Tuple<int, int, int>> keysList;
    int HeroHandNum01 = 0;
    int HeroHandNum02 = 0;
    string HeroHandsuit01 = "";
    string HeroHandsuit02 = "";
    
    int Raisefreq = 0;
    int Callfreq = 0;
    int Foldfreq = 0;


    static int NumberofPlayer=6;
    GameObject[] playerList=new GameObject[NumberofPlayer];
    void Start()
    {
        AGHandRange = new AggregationHandR();
        if (AGHandRange.LoadJson_AggregationHandRange())
        {
            //�ǂݍ��߂��ꍇ
            keysList = new List<Tuple<int, int, int>>(AGHandRange.getRangeALL().Keys);
        }
        else
        {
            //�ǂݍ��߂Ȃ������ꍇ���C�����j���[�ɋ����J��
            Debug.Log("�n���h�����W��ǂݍ��߂Ȃ�����");
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        //for����find���g���ē���Ă���������
        playerList[0] = Obj_player1_H;
        playerList[1] = Obj_player2;
        playerList[2] = Obj_player3;
        playerList[3] = Obj_player4;
        playerList[4] = Obj_player5;
        playerList[5] = Obj_player6;

        Obj_Freq_Num.SetActive(false);
        Obj_Cap_Freq.SetActive(false);
        Obj_PlayAction_Button.SetActive(false);
        //NoActivePlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick_MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void OnClick_NextProblem()
    {
        Obj_PlayAction_Button.SetActive(true);
        Obj_Freq_Num.SetActive(false);
        Obj_Cap_Freq.SetActive(false);
        //Bet�z����������
        for (int i = 1; i < 7; i++)
        {
            Transform rstBet;
            rstBet = Obj_BetogMoney.transform.Find("Player" + i.ToString());
            Text Blindbet = rstBet.GetComponent<Text>();
            Blindbet.text = "";
        }
        //�V�`���G�[�V�����ύX����
        NoActivePlayer(0);
        int listCount = keysList.Count;
        if(listCount!=0)
        {
            //�����_���ɃL�[(�V�`���G�[�V������I��)
            Tuple<int, int, int> usekey = keysList[UnityEngine.Random.Range(0, listCount)];
            HandRangeNum = AGHandRange.SituationAggregation[usekey];
            //�����_���n���h��I�o
            while (true)
            {
                HeroHandNum01 = UnityEngine.Random.Range(0, 13);
                HeroHandNum02 = UnityEngine.Random.Range(0, 13);
                HeroHandsuit01 = Num_Suit(UnityEngine.Random.Range(0, 4));
                HeroHandsuit02 = Num_Suit(UnityEngine.Random.Range(0, 4));
                if(HeroHandNum01== HeroHandNum02&& HeroHandsuit01== HeroHandsuit02)
                {
                    //�Pdeck�Ȃ̂œ����n���h�͑��݂��Ȃ�
                    Debug.Log("����n���h�̂��߂�����x");
                }
                else
                {
                    break;
                }
            }
            if(HeroHandsuit01== HeroHandsuit02)
            {
                //�X�[�e�b�h�n���h�Ȃ̂�A<B�̂Ƃ�[A][B]
                
                if(HeroHandNum01> HeroHandNum02)
                {
                    Raisefreq = HandRangeNum[HeroHandNum02, HeroHandNum01].RaiseFrequency;
                    Callfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].CallFrequency;
                    Foldfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].FoldFrequency;
                }else
                {
                    Raisefreq = HandRangeNum[HeroHandNum01, HeroHandNum02].RaiseFrequency;
                    Callfreq = HandRangeNum[HeroHandNum01, HeroHandNum02].CallFrequency;
                    Foldfreq = HandRangeNum[HeroHandNum01, HeroHandNum02].FoldFrequency;
                }
                //�����}�[�N�������͒e���Ă���̂ōl�����Ȃ�
            }
            else
            {
                //�I�t�X�[�g�n���h�Ȃ̂�A<B�̂Ƃ�[B][A]
                if (HeroHandNum01 > HeroHandNum02)
                {
                    Raisefreq = HandRangeNum[HeroHandNum01, HeroHandNum02].RaiseFrequency;
                    Callfreq = HandRangeNum[HeroHandNum01, HeroHandNum02].CallFrequency;
                    Foldfreq = HandRangeNum[HeroHandNum01, HeroHandNum02].FoldFrequency;
                }
                else�@if(HeroHandNum01 < HeroHandNum02)
                {
                    Raisefreq = HandRangeNum[HeroHandNum02, HeroHandNum01].RaiseFrequency;
                    Callfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].CallFrequency;
                    Foldfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].FoldFrequency;
                }
                else
                {
                    //HeroHandNum01==HeroHandNum02�̃|�P�b�g�n���h�@��Ɠ��ɕς��Ȃ�
                    Raisefreq = HandRangeNum[HeroHandNum02, HeroHandNum01].RaiseFrequency;
                    Callfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].CallFrequency;
                    Foldfreq = HandRangeNum[HeroHandNum02, HeroHandNum01].FoldFrequency;
                }

            }
            //Situation�̕ύX
            TX_Heropos.text = Num_Position(usekey.Item1);
            TX_Villainpos.text = Num_Position(usekey.Item2);
            TX_XBet.text=Num_XBet(usekey.Item3)+"pot";

            //���ꂼ��̃|�W�V�����̕ύX
            Transform PositionName;
            int Start = usekey.Item1;
            for (int i=0;i< NumberofPlayer; i++)
            {
                PositionName = playerList[i].transform.Find("player_pos");
                if (PositionName != null)
                {
                    Text position = PositionName.GetComponent<Text>();
                    position.text = Num_Position(Start);
                    if(Start<6)
                    {
                        Start++;
                    }
                    else
                    {
                        Start = 1;
                    }
                }
            }
            //Hero��Villain��bet�z��ύX
            //��{�I��Hero�|�W�V������Villain�|�W�V�����̍���1�𑫂����ꏊ���A�N�e�B�u�ɂ������v���C���[open�̂Ƃ���Hero�ȊO��A�N�e�B�u
            int disp_V = 0;
            if (usekey.Item1< usekey.Item2)
            {
                disp_V = Mathf.Abs(usekey.Item2 - usekey.Item1);
            }
            else if(usekey.Item1 > usekey.Item2)
            {
                disp_V=6- Mathf.Abs(usekey.Item2 - usekey.Item1);
            }
            
            if(usekey.Item2==0)
            {
                disp_V = 0;
            }
            ActivePlayer(disp_V+1);
            //�V�`���G�[�V�������Ƃ�bet�z��\��
            if (((1 <= usekey.Item2 && usekey.Item2 <= 4) && (1 <= usekey.Item1 && usekey.Item1 <= 4)) || usekey.Item2 == 0)
            {
                Transform Bbet;
                Text textBlindbet;
                switch (usekey.Item3)
                {
                    case 1:
                        //open��bet���Ă���v���C���[��SB,BB�̂�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";    //BB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "0.5";    //SB
                        break;
                    case 2:
                        //3bet��Villain��2.5BBraise���s�������
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";    //BB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "0.5";    //SB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "2.5";    //Villain
                        break;
                    case 3:
                        //4bet��Hero2.5BB,Villain7.5BBraise���s�������
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";    //BB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "0.5";    //SB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "7.5";    //Villain

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());    //Hero��1�Œ�
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "2.5";  //Hero
                        break;
                    case 4:
                        //5bet��Hero7.5BB,Villain��20BBraise���s�������
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";    //BB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "0.5";    //SB

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "20";    //Villain

                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());    //Hero��1�Œ�
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "7.5";  //Hero
                        break;
                    default:
                        break;
                }
            }
            else if ((1 <= usekey.Item1 && usekey.Item1 <= 4) && (5 <= usekey.Item2 && usekey.Item2 <= 6)
                    || (5 <= usekey.Item1 && usekey.Item1 <= 6) && (1 <= usekey.Item2 && usekey.Item2 <= 4))
            {
                Transform Bbet;
                Text textBlindbet;
                //float coeSB = 0.5f;
                //float coeBB = 1f;
                
                switch (usekey.Item3)
                {
                    case 1:
                        //open��Villain=0�Ȃ͂��Ȃ̂ł����ɂ͓���Ȃ��͂�
                        //SBopen�̂� raise���Ă���v���C���[SB,BB�̂�
                        //Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());    //Hero��1�Œ�
                        //textBlindbet = Bbet.GetComponent<Text>();
                        //textBlindbet.text = coeSB.ToString();  //Hero�@(SB)

                        //Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        //textBlindbet = Bbet.GetComponent<Text>();
                        //textBlindbet.text = "1";    //VillainBB
                        break;
                    case 2:
                        //3bet��villain2.5BB,Hero��SBorBB�Ȃ̂�1or0.5BB�܂莩���̓u���C���h�m��
                        if(usekey.Item1==5)
                        {
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "0.5";  //Hero(6-5)
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "1";    //BB
                        }
                        else if(usekey.Item1 == 6)
                        {
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (6).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "0.5";  //HeroBB��player6��SB�m��
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "1";    //Hero(7-6)
                        }
                        //3bet��Villain��Hero�������̃|�W�V�����̂��Ƃ͂Ȃ��̂ōl�����Ȃ���OK
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "2.5";    //Villain
                        break;
                    case 3:
                        //4bet��Villain���u���C���h�m��
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "0.5";    //SB
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";    //BB
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());    //Hero��1�Œ�
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "2.5";  //Hero
                        //Villain���㏑��(SBorBB)
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "10";    //Villain
                        break;
                    case 4:
                        if (usekey.Item1 == 5)
                        {
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (6 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "0.5";  //Hero(6-5)
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "1";    //BB
                        }
                        else if (usekey.Item1 == 6)
                        {
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (6).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "0.5";  //HeroBB��player6��SB�m��
                            Bbet = Obj_BetogMoney.transform.Find("Player" + (7 - usekey.Item1).ToString());
                            textBlindbet = Bbet.GetComponent<Text>();
                            textBlindbet.text = "1";    //Hero(7-6)
                        }
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());    //Hero��1�Œ�
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "10";  //Hero
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (disp_V + 1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "22";    //Villain
                        break;
                    default:
                        break;
                }

            }
            else if(5<= usekey.Item1&&5<= usekey.Item2)
            {
                //Hero,Villain�ǂ�����u���C���h
                Transform Bbet;
                Text textBlindbet;
                switch (usekey.Item3)
                {
                    case 1:
                        //open��Villain=0�Ȃ͂��Ȃ̂ł����ɂ͓���Ȃ��͂�
                        break;
                    case 2:
                        //3bet��SBVillain�̏󋵂̂�
                        //�x�^�̂ق����킩��₷����������
                        //Hero(BB)player1�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "1";
                        //Villain(SB)player6�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "3";
                        break;
                    case 3:
                        //4bet��BBVillain�̏󋵂̂�
                        //Hero(SB)player1�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "3";
                        //Villain(SB)player2�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (2).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "10";
                        break;
                    case 4:
                        //5bet��SBVillain�̏󋵂̂�
                        //Hero(BB)player1�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (1).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "10";
                        //Villain(SB)player6�Œ�
                        Bbet = Obj_BetogMoney.transform.Find("Player" + (6).ToString());
                        textBlindbet = Bbet.GetComponent<Text>();
                        textBlindbet.text = "22";
                        break;
                    default:
                        break;
                }
            }




            //Hero�n���h�̕ύX
            Transform ObjHplayer = Obj_player1_H.transform.Find("card01");
            if(ObjHplayer != null)
            {
                Text[] info = ObjHplayer.GetComponentsInChildren<Text>();
                if(info!=null&&info.Length>=2)
                {
                    //Hero�n���h01�̐�����ύX
                    info[0].text = Num_num(HeroHandNum01);
                    //Hero�n���h02�̃X�[�g��ύX
                    info[1].text = HeroHandsuit01;
                    
                }
            }
            //Hero�n���h�̕ύX
            ObjHplayer = Obj_player1_H.transform.Find("card02");
            if (ObjHplayer != null)
            {
                Text[] info = ObjHplayer.GetComponentsInChildren<Text>();
                if (info != null && info.Length >= 2)
                {
                    //Hero�n���h01�̐�����ύX
                    info[0].text = Num_num(HeroHandNum02);
                    //Hero�n���h02�̃X�[�g��ύX
                    info[1].text = HeroHandsuit02;
                    
                }
            }


            
        }
    }
    public void Onclick_PlayActionButton(string Name)
    {
        Obj_PlayAction_Button.SetActive(false);
        Obj_Freq_Num.SetActive(true);
        Obj_Cap_Freq.SetActive(true);
        //�p�x��ύX
        TX_RaiseFreq.text= Raisefreq.ToString();
        TX_CallFreq.text = Callfreq.ToString();
        TX_FoldFreq.text = Foldfreq.ToString();
        if(Name=="R")
        {
            if(Raisefreq==0)
            {
                TX_Answer.text = "�s����";
            }
            else
            {
                TX_Answer.text = "����";
            }
            
        }else if(Name=="C")
        {
            if (Callfreq == 0)
            {
                TX_Answer.text = "�s����";
            }
            else
            {
                TX_Answer.text = "����";
            }
        }
        else if (Name=="F")
        {
            if (Foldfreq == 0)
            {
                TX_Answer.text = "�s����";
            }
            else
            {
                TX_Answer.text = "����";
            }
        }
        else
        {
            Debug.Log("ActionButton�\�����ʒl");
        }
    }
    public string Num_num(int num)
    {
        string ret;
        switch (num)
        {
            case 0:
                ret = "A";
                break;
            case 1:
                ret = "K";
                break;
            case 2:
                ret = "Q";
                break;
            case 3:
                ret = "J";
                break;
            case 4:
                ret = "T";
                break;
            case 5:
                ret = "9";
                break;
            case 6:
                ret = "8";
                break;
            case 7:
                ret = "7";
                break;
            case 8:
                ret = "6";
                break;
            case 9:
                ret = "5";
                break;
            case 10:
                ret = "4";
                break;
            case 11:
                ret = "3";
                break;
            case 12:
                ret = "2";
                break;
            default:
                ret = "";
                break;
        }
        return ret;
    }
    public string Num_Suit(int num)
    {
        string ret;
        switch (num)
        {
            case 0:
                ret= "S";
                break;
            case 1:
                ret = "C";
                break;
            case 2:
                ret = "H";
                break;
            case 3:
                ret = "D";
                break;
            default:
                ret = "";
                break;
        }
        return ret;
    }
    public string Num_Position(int num)
    {
        string ret;
        switch (num)
        {
            case 0:
                ret = "none";
                break;
            case 1:
                ret = "UTG";
                break;
            case 2:
                ret = "HJ";
                break;
            case 3:
                ret = "CO";
                break;
            case 4:
                ret = "BTN";
                break;
            case 5:
                ret = "SB";
                break;
            case 6:
                ret = "BB";
                break;
            default:
                ret = "";
                break;
        }
        return ret;
    }
    public string Num_XBet(int num)
    {
        string ret;
        switch (num)
        {
            case 0:
                ret = "none";
                break;
            case 1:
                ret = "Open";
                break;
            case 2:
                ret = "3bet";
                break;
            case 3:
                ret = "4bet";
                break;
            case 4:
                ret = "5bet";
                break;
            default:
                ret = "";
                break;
        }
        return ret;
    }
    public void NoActivePlayer(int Num)
    {
        //�����̔ԍ��ȊO�̃v���C���[�̃n���h���A�N�e�B�u�ɂ���
        Transform targetObject;
        switch (Num)
        {
            case 0:
                //���ׂĔ�\��
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 1:
                //Hero�ȊO��\��
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 2:
                //�v���C���[2��Hero�ȊO��\��
                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 3:
                //�v���C���[3��Hero�ȊO��\��
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 4:
                //�v���C���[4��Hero�ȊO��\��
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 5:
                //�v���C���[5��Hero�ȊO��\��
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            case 6:
                //�v���C���[6��Hero�ȊO��\��
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(false);

                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(false);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(false);
                break;
            default:
                break;

        }
        
    }
    public void ActivePlayer(int Num)
    {
        //����̃v���C���[��Hero��\��
        Transform targetObject;
        switch (Num)
        {
            case 0:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break; 
            case 1:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break;
            case 2:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player2.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player2.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break; 
            case 3:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player3.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player3.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break;
            case 4:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player4.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player4.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break;
            case 5:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player5.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player5.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break;
            case 6:
                targetObject = Obj_player1_H.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player1_H.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player6.transform.Find("card01");
                targetObject.gameObject.SetActive(true);
                targetObject = Obj_player6.transform.Find("card02");
                targetObject.gameObject.SetActive(true);
                break;
            default:
                break; 
        }
    }
}
