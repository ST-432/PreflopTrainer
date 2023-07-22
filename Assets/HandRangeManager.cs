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
    //�ݒ�l
    public int Leftmargin,Topmargin;
    //�ÓI�ɂ���
    public static int s_rows=13, s_cols=13;
    public int SelectHandX = 0;
    public int SelectHandY = 0;
    //�v���n�u�擾���̃I�u�W�F�N�g
    public GameObject PreSDropDawn;
    public GameObject Handprefub;
    public Text pretex;
    public Button Buttonprefub;
    private Text Combination;

    public Transform canvas;
    //public InputField Afrequencyprefub;
    public Slider FSliderPrehub;

    //���[�J���ŕێ����Ă����ϐ��A�I�u�W�F�N�g
    //�l�̕ۑ��̓V�[���I�����܂��̓����W�v�f�N���b�N��
    //1�V�`���G�[�V�����ɂ����郌���W�S��
    GameObject[,] HandRangeObj = new GameObject[s_rows, s_cols];    //[Y,X]
    Range_Num[,] HandRangeNum = new Range_Num[s_rows, s_cols];      //[Y,X]
    //�V�`���G�[�V�����̏W��
    //Dictionary<Tuple<int,int,int>, GameObject[,]> SituationAggregation = new Dictionary<Tuple<int, int, int>, GameObject[,]>();

    Tuple<int , int, int> KeyNum;
    //�V�`���G�[�V�����I���h���b�v�_�E��
    public Dropdown Hdropdown;
    public Dropdown Vdropdown;
    public Dropdown Bdropdown;
    //�A�N�V�����p�x����͂���inputfield
    //public InputField RaiseFre;
    //public InputField CallFre;
    //public InputField FoldFre;
    //���֐�����X���C�_�[�ɕύX
    public Slider RaiseFSlider;
    public Slider CallFSlider;
    public Slider FoldFSlider;
    //�ۑ��{�^��
    public Button SaveFreq;
    public Button Go_mainmenu;
    //�I�𒆂̃n���h�̃e�L�X�g
    Text SelectHandText;
    //�I�𒆂̃n���h�̏ꏊ
    int localSelectHandX;   //��
    int localSelectHandY;   //�c
    //�N���b�N���ꂽ�n���h�̃I�u�W�F�N�g
    GameObject clickedGameObject;
    //�A�N�V�����p�x��\������e�L�X�g
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
        //�J�����̍������W���擾�����[���h���W�ɕϊ�
        Vector2 LeftBottom = cam.ViewportToWorldPoint(Vector2.zero);
        //Vector3 LeftTop = cam.ViewportToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        for (int Yvec = 0; Yvec < s_rows; Yvec++)
        {
            for (int Xvec = 0; Xvec < s_cols; Xvec++)
            {
                GameObject HandObj;
                HandObj = Instantiate(Handprefub);
                //�ʒu�̒����@���ォ��E�ɏ��Ԃ�
                Vector2 pos;
                pos.x = LeftBottom.x+ Xvec + Leftmargin;
                pos.y = (-LeftBottom.y)- Yvec - Topmargin;
                HandObj.transform.position = pos;
                string A,NumCom;
                A = "Hand";
                //A = "Hand" + col.ToString("0") + "," + row.ToString("0");
                if(Xvec>Yvec)   //�X�[�e�b�h
                {
                    NumCom =  number[Yvec] +number[Xvec] + "s";
                }
                else if(Xvec<Yvec)  //�I�t�X�[�g
                {
                    NumCom = number[Xvec] + number[Yvec]+"o";
                }
                else�@//�|�P�b�g
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

                //�S�������ƌ��ɂ����̂ŐF�̕ύX
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
        //Hero�̃|�W�V���������肷��h���b�v�_�E���̍쐬
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
        //Text�̍쐬
        Setpos.x = Setpos.x + 3;
        Text texH = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texH.transform.position = Setpos;
        texH.text = "(Hero) Vs";

        
        //Villain�̃|�W�V���������肷��h���b�v�_�E���̍쐬
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
        //Text�̍쐬
        Setpos.x = Setpos.x + 3;
        Text texV = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texV.transform.position = Setpos;
        texV.text = "Villain";

        //Villain�̃|�W�V���������肷��h���b�v�_�E���̍쐬
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
        //Text�̍쐬
        Setpos.x = Setpos.x + 3;
        Text texB = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texB.transform.position = Setpos;
        texB.text = "Bet Pot";

        //���AddListner����ƍ��Ƃ��ɒl�̕ύX�����m���Ă��܂��̂ł�����
        Hdropdown.onValueChanged.AddListener((value) => { DropDownchange(Hdropdown.name, value); });
        Vdropdown.onValueChanged.AddListener((value) => { DropDownchange(Vdropdown.name, value); });
        Bdropdown.onValueChanged.AddListener((value) => { DropDownchange(Bdropdown.name, value); });
        // Update is called once per frame

        //�A�N�V�����p�x���͗��쐬
        //Raise�p�x
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

        //call�p�x
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

        //fold�p�x
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


        //ValueChanged���Ɠ��͒��ɓ���
        //RaiseFre.onEndEdit.AddListener(InputFieldchange("����","A"));
        //RaiseFre.onEndEdit.AddListener(() => { InputFieldchange("����","a"); });
        RaiseFSlider.onValueChanged.AddListener((value) => { InputFieldchange(RaiseFSlider,texRpar, value); });
        CallFSlider.onValueChanged.AddListener((value) => { InputFieldchange(CallFSlider,texCpar, value); });
        FoldFSlider.onValueChanged.AddListener((value) => { InputFieldchange(FoldFSlider,texFpar, value); });

        //�ۑ��{�^���̍쐬
        Setpos.x = Setpos.x - 7;
        Setpos.y = Setpos.y - 2;
        Button BT = Instantiate(Buttonprefub, Setpos, Quaternion.identity, canvas.transform);
        BT.transform.position = Setpos;
        SaveFreq = BT;
        SaveFreq.name = "SaveButton";
        SaveFreq.onClick.AddListener(ClickSave);

        //���C�����j���[�J�ڃ{�^���̍쐬
        Setpos.x = Setpos.x +6;
        Button BTmm = Instantiate(Buttonprefub, Setpos, Quaternion.identity, canvas.transform);
        BTmm.transform.position = Setpos;
        Go_mainmenu = BTmm;
        Go_mainmenu.name = "Go_MainMenu";
        //���������v���n�u��2�{��
        RectTransform Go_main_rt = Go_mainmenu.GetComponent<RectTransform>();
        float originalWidth= Go_main_rt.rect.width;
        float originalHeight = Go_main_rt.rect.height;
        float newWidth = originalWidth * 2f;
        Go_main_rt.sizeDelta=new Vector2 (newWidth, originalHeight);
        Text MainmenuBTCap= Go_mainmenu.GetComponentInChildren<Text>();
        MainmenuBTCap.text = "���C�����j���[";
        Go_mainmenu.onClick.AddListener(ClickGo_MainMenu);

        Setpos.x = Setpos.x - 6;
        //�I���n���h�̕\����
        Setpos = Hdropdown.transform.position;
        Setpos.x = Setpos.x + 3;
        Setpos.y = Setpos.y - 4;
        Text texHra = Instantiate(pretex, Setpos, Quaternion.identity, canvas.transform);
        texHra.transform.position = Setpos;
        texHra.text = "";
        SelectHandText = texHra;

        //--------------------------------------------------------------------------------------
        //�����l�Z�b�g�A�ǂݍ��߂��ꍇ�͓ǂݍ��ݒl
        if (AggregationHandRange.LoadJson_AggregationHandRange())
        {
            //�ǂݍ��߂��ꍇ
        }
        else
        {
            //�ǂݍ��߂Ȃ������ꍇ
            
        }
        //�ǂݍ��߂��ꍇ���ǂݍ��߂Ȃ������ꍇ���ŏ��̓h���b�v�_�E�����S��none����Ȃ̂�HandRangeNum�͏����l��ok
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
        //�I��������
        AggregationHandRange.SaveJson_AggregationHandRange();
    }
    void Update()
    {
        //�n���h�I�����}�E�X�N���b�N�̏ꏊ����ǂ̃n���h��I�����������m
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

        //�V�`���G�[�V������ς����Ƃ���HandRangeNum���N���A���ēǂݍ��݂����Ȃ��Ƃ����Ȃ�
        bool checkSit = true;
        if(i==0)
        {
            checkSit = false;
        }
        if (changedropdawn== Hdropdown.name)
        {
            //Hero�̃|�W�V������I�ԃh���b�v�_�E�������ύX���ꂽ��
            Dropdown ddtmp;
            ddtmp = Vdropdown.GetComponent<Dropdown>();
            //bet�V�`���G�[�V������open�ł͂Ȃ�Villain�|�W�V������none�ł͂Ȃ�Villain�Ɠ����|�W�V�������w�肳��Ă��܂�����
            if (Vdropdown.value== Hdropdown.value && Vdropdown.value!=0 &&Bdropdown.value!=1)
            {
                //none����Ȃ��đI��ł���|�W�V����������Ă��܂�����none�ɖ߂�
                Hdropdown.value = 0;
                Debug.Log("�����l��I��ł���");
                checkSit = false;
            }
            //
            if(Bdropdown.value!=1&& Bdropdown.value != 0)
            {
                if (Hdropdown.value < Vdropdown.value)
                {
                    //Hero��Villain�����O�̃|�W�V�����̎�
                    if (Bdropdown.value == 2 || Bdropdown.value == 4)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("���݂��Ȃ��V�`���G�[�V����3bet or 5bet");
                        checkSit = false;
                    }
                }
                else if (Hdropdown.value > Vdropdown.value)
                {
                    if (Bdropdown.value == 3)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("���݂��Ȃ��V�`���G�[�V����open or 4bet");
                        checkSit = false;
                    }
                }
            }
            //BB��open�������Ȃ�
            if(Hdropdown.value ==6&&Bdropdown.value==1)
            {
                Hdropdown.value = 0;
                checkSit = false;
            }
            //bet�V�`���G�[�V�������Đݒ�
            //Bdropdown.value = 0;
        }
        else if(changedropdawn == Vdropdown.name)
        {
            //Villain�̃|�W�V������I�ԃh���b�v�_�E�������ύX���ꂽ��
            if (Hdropdown.value == Vdropdown.value && Hdropdown.value != 0)
            {
                //none����Ȃ��đI��ł���|�W�V����������Ă��܂�����none�ɖ߂�
                Vdropdown.value = 0;
                Debug.Log("�����l��I��ł���");
                checkSit = false;
            }
            if (Bdropdown.value != 1&&Bdropdown.value != 0)
            {
                if (Hdropdown.value < Vdropdown.value)
                {
                    //Hero��Villain�����O�̃|�W�V�����̎�
                    if (Bdropdown.value == 2 || Bdropdown.value == 4)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("���݂��Ȃ��V�`���G�[�V����3bet or 5bet");
                        checkSit = false;
                    }
                }
                else if (Hdropdown.value > Vdropdown.value)
                {
                    if (Bdropdown.value == 3)
                    {
                        Bdropdown.value = 0;
                        Debug.Log("���݂��Ȃ��V�`���G�[�V����4bet");
                        checkSit = false;
                    }
                }
            }

            //bet�V�`���G�[�V�������Đݒ�
            //Bdropdown.value = 0;
        }
        else if(changedropdawn == Bdropdown.name)
        {
            //bet�񐔂��ύX���ꂽ��
            if(Bdropdown.value ==1)
            {
                //open�����W�Ȃ̂�villain�͖�����
                Vdropdown.value = 0;
                Vdropdown.enabled = false;
                changeable(false, Vdropdown);

                //BBopen�͂Ȃ��̂ŋ����I��none��
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
            //Hero�̃|�W�V������Villain���O�����Ƃ���open,4bet or 3bet,5bet�ɏꍇ����
            //IP,OOP�ŕ������BTN�ȍ~���߂�ǂ������Ȃ�̂ő���or�x��
            //open,4bet �����Ȃ��|�W�V�����֌W�̎�
            if (Hdropdown.value<Vdropdown.value)
            {
                //Hero��Villain�����O�̃|�W�V�����̎�
                //3bet,5bet�͑��݂��Ȃ�
                if(Bdropdown.value ==2||Bdropdown.value ==4)
                {
                    Bdropdown.value = 0;
                    Debug.Log("���݂��Ȃ��V�`���G�[�V����3bet or 5bet");
                    checkSit = false;
                }
            }
            //3bet,5bet �����Ȃ��|�W�V�����֌W�̎�
            else if (Hdropdown.value > Vdropdown.value)
            {
                //open,4bet�͑��݂��Ȃ�
                //open�̂Ƃ��͐�΂�Heroposition>Villainposition(0)�ɂȂ邽�ߏ����Â����Ȃ�
                if (Bdropdown.value == 3)
                {
                    Bdropdown.value = 0;
                    Debug.Log("���݂��Ȃ��V�`���G�[�V����open or 4bet");
                    checkSit = false;
                }
                //
                else if (Vdropdown.value == 0 && Bdropdown.value != 1)    //Villainposition��none�̂Ƃ���bet�V�`���G�[�V������open�ȊO�e��
                {
                    Bdropdown.value = 0;
                    Debug.Log("���݂��Ȃ��V�`���G�[�V����Villain(none)��3bet or 5bet");
                    checkSit = false;
                }
            }

        }
        else
        {
            checkSit=false;
        }
        //����Ȃ�����
        if (Hdropdown.value == 0 || Bdropdown.value == 0)
        {
            checkSit = false;
        }
        if (checkSit)
        {
            //�����W�̕ύX����
            //bet�V�`���G�[�V������open�̂Ƃ���Villain�͊֌W�Ȃ��̂�key��0��
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
                //key�������������ߕۑ��l�ł���range�ɕύX
                Debug.Log("�ۑ��l�̃Z�b�g");
            }
            else
            {
                //�L�[��������Ȃ��������ߏ����l���Z�b�g
                ResetHandRangeNum();
                Debug.Log("�����l�̃Z�b�g");
            }
            Debug.Log("�V�`���G�[�V�����ύX�̂��߃����W���Z�b�g");

            RaiseFSlider.enabled = true;
            CallFSlider.enabled = true;
            FoldFSlider.enabled = true;
            changeable(true, RaiseFSlider);
            changeable(true, CallFSlider);
            changeable(true, FoldFSlider);
            //�����l�̃Z�b�g
            RaiseFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("R");
            CallFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("C");
            FoldFSlider.value = HandRangeNum[localSelectHandY, localSelectHandX].getFreq("F");
            InputFieldchange(RaiseFSlider, texRpar, RaiseFSlider.value);
            InputFieldchange(CallFSlider, texCpar, CallFSlider.value);
            InputFieldchange(FoldFSlider, texFpar, FoldFSlider.value);
        }
        else
        {

            Debug.Log("�Z�b�g����Ă��郌���W�ύX�Ȃ�");
            //�������V�`���G�[�V��������Ȃ��̂ŏ����l�ɃZ�b�g
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
            //�X���C�_�[�o�[�𓮂��Ȃ��悤��
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
        //�X���C�_�[��5���݂ɂ��鏈��
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
    //�ۑ��{�^���̉������̏���
    public void ClickSave()
    {
        if (Hdropdown.value == 0 || Bdropdown.value == 0)
        {
            //�|�b�v�A�b�v��\��������
            Debug.Log("�V�`���G�[�V�������I������Ă��Ȃ�");
        }
        else if(Vdropdown.value == 0&& Bdropdown.value != 1)
        {
            //Vdropdawn==0�̂Ƃ���open������̂�Open����Ȃ��Ƃ��͒e��
            Debug.Log("�V�`���G�[�V�������I������Ă��Ȃ�");
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
                    Debug.Log("�X�V");
                }
                else
                {
                    Debug.Log("�V�K���");
                }
                
            }
            else
            {
                //�|�b�v�A�b�v��\��������
                Debug.Log("�p�x�̍��v��100�ɂȂ��Ă��Ȃ�");

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
        //�I���n���h�̕\��
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
