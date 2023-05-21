using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
//webで開く用
using UnityEngine.Networking;
using System.IO;
//casle用
public class ReadJson2 : MonoBehaviour
{
    //time.deltatimeを使うための変数
    float timer;

    //phpとの接続
    public string ResultText;    //結果を格納するテキスト

    public bool cntFlag = false;

    //UI用
    public GameObject RetryButton;
    public GameObject NextStage;

    //クリアマーク
    public GameObject Good;
    public GameObject Hanamaru;
    
    //webの場所
    private string url = "https://www.google.com/?hl=ja";

    //サウンドエフェクト用
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip walkSound;
    public AudioClip jumpSound;

    AudioSource audioSource;

    //アイテム取得用
    public GameObject[] blocks;
    public string game;

    //アニメーション用
    //アニメーションフラグ
    private const string key_isRun = "walk";
    private const string key_isJump = "jump";

    [Serializable]
    public class InputJson
    {
        public string[] process;
    }

    //number-------------------------------------------------------
    //元json
    [Serializable]
    public class Number
    {
        public string[] Numberjson;
    }

    public Rigidbody rb;
    public float speed = 1f;
    
    public int cnt;//動く回数格納する変数
    public int numberCnt;//クリアの最小個数
    public int StageNum;//プレイヤーの積んだ積み木の数
    //周期をカウントする変数
    public float walkCnt;

    public string timeFlug;

    public string controlPlyer;
   
    float v;

    //レバー操作用変数
    public string lever;

    //キャラクター前身方向操作のフラグ
    private string advance;

    //回転中かどうか
    bool coroutineBool = false;

    public string a = "aaaaaaaaaaaaa";
    public string b = "bbbbbbbbbbbbbb";

    public string Path = Application.streamingAssetsPath + "/process.json";
    //baseFilePath = Application.streamingAssetsPath + "/" + fileName;
    //public string CopyPath = Application.persistentDataPath + "/process.json";
    //File.Copy(Path, filePath);

    public string result;

    //元json
    public string numberresult;

    //テスト
    public InputJson inputJson;

    //ここだよーーーーーーーーーーーーーーーーーーーーーーーーー
    public Number number;
    public int ArrayControl;
    private Animator animator;

    private IEnumerator textLoad()
    {
        string phpPath = "https://halproducts.main.jp/online/testdb_unity.php";　//オンライン用
        WWW wwwphp = new WWW(phpPath);
        yield return wwwphp;
        ResultText = wwwphp.text;
        Debug.Log(wwwphp.text + "125行目");

        string filepath = Application.streamingAssetsPath + "/" +ResultText + ".json";
        Debug.Log(filepath + "128行目");
       
        if (filepath.Contains("://") || filepath.Contains(":///"))
        {
            WWW www = new WWW(filepath);
            yield return www;
            result = www.text;
            print(result + a + "134行目");
            InputJson inputJson = JsonUtility.FromJson<InputJson>(result);
            foreach (string p in inputJson.process)
            {
                cnt++;
            }
            ArrayControl = cnt - 1;

            print(result + b);
            Debug.Log("145行目");
        }
        else
        {
            result = File.ReadAllText(filepath);
            print(result);
            InputJson inputJson = JsonUtility.FromJson<InputJson>(result);
            foreach (string p in inputJson.process)
            {
                cnt++;
            }
            ArrayControl = cnt - 1;
        }

        //元の配列読み込み
        string numberfilepath = Application.streamingAssetsPath + "/" + ResultText + "ohs.json";
        if (numberfilepath.Contains("://") || numberfilepath.Contains(":///"))
        {
            WWW wwwNum = new WWW(numberfilepath);
            yield return wwwNum;
            numberresult = wwwNum.text;
            Debug.Log(wwwNum.text);
            print(numberresult + a);
            Number number = JsonUtility.FromJson<Number>(numberresult);
            foreach (string n in number.Numberjson)
            {
                numberCnt++;
                print(numberCnt + "積み木の積まれた数を確認します");
            }
            //ArrayControl = cnt - 1;
            //print(result + b);
        }
        else
        {
            numberresult = File.ReadAllText(numberfilepath);
            print(result);
            Debug.Log("muzuine22222");
            Number number = JsonUtility.FromJson<Number>(numberresult);
            foreach (string n in number.Numberjson)
            {
                numberCnt++;
                print(numberCnt + "積み木の積まれた数を確認します");
            }
            //ArrayControl = cnt - 1;
        }
    }

    //コルーチン関数を定義
    private IEnumerator Corou1() //コルーチン関数の名前
    {
        //コルーチンの内容
        Debug.Log("スタート");
        yield return new WaitForSeconds(1.0f);
        this.animator.SetBool(key_isRun, true);

        if (advance == "right")
        {
            rb.velocity = new Vector3(0, 0, -3.5f);
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 3.5f);
        }

        audioSource.PlayOneShot(walkSound);
        Debug.Log(cnt + "211行目");
        cnt--;
        ArrayControl--;
        Debug.Log("スタートから2秒後");
    }

    private IEnumerator Corou2() //コルーチン関数の名前
    {
        //コルーチンの内容
        Debug.Log("スタート");
        yield return new WaitForSeconds(1.0f);
        this.animator.SetBool(key_isJump, true);
        
        if (advance == "right")
        {
            rb.velocity = new Vector3(0, 5f, -2.0f);
        }
        else
        {
            rb.velocity = new Vector3(0, 5f, 2f);
        }
        audioSource.PlayOneShot(jumpSound);
        Debug.Log(cnt + "233行目");
        cnt--;
        ArrayControl--;
        Debug.Log("スタートから2秒後");
    }

    private IEnumerator Corou3() //コルーチン関数の名前
    {
        //コルーチンの内容
        Debug.Log("ターンスタート");
        //回転中ではない場合は実行 
        if (!coroutineBool)
        {
            coroutineBool = true;
            StartCoroutine("RightMove");
        }
        yield return new WaitForSeconds(1.0f);//多分これではできない
        Debug.Log(cnt + "250行目");
        cnt--;
        ArrayControl--;
        advance = "left"; 
        Debug.Log("ターン終了");
    }

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(textLoad());
    }

    private void Start()
    {
        //仮置き
        /*numberCnt =12;*/
        StageNum = 12;

        advance = "right";
        this.animator = GetComponent<Animator>();
        
        //UI用
        //canvasの各UIの非活性化
        RetryButton.SetActive(false);
        Good.SetActive(false);
        Hanamaru.SetActive(false);
        NextStage.SetActive(false);

        //サウンドエフェクト用
        audioSource = GetComponent<AudioSource>();

        //アイテム取得用
        game = "play";

        rb = GetComponent<Rigidbody>();
        Debug.Log(rb);

        cnt = 0;
        walkCnt = 0;

        InputJson inputJson = JsonUtility.FromJson<InputJson>(result);

        foreach (string p in inputJson.process)
        {
            cnt++;
        }
        ArrayControl = cnt - 1;
        Debug.Log(cnt);
        Debug.Log(ArrayControl);
        Debug.Log(inputJson);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void FixedUpdate()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        if (walkCnt == 150)
        {
            if(cnt != 0)//(ArrayControl >= 0 && cnt > 0 )
            {
                InputJson inputJson = JsonUtility.FromJson<InputJson>(result);
          
                Debug.Log(inputJson.process[ArrayControl]);
                if (inputJson.process[ArrayControl] == "walk")
                {
                    StartCoroutine("Corou1");
                }
                if (inputJson.process[ArrayControl] == "jump")
                {
                    StartCoroutine("Corou2");
                }
                if (inputJson.process[ArrayControl] == "turn")
                {
                    StartCoroutine("Corou3");
                }
            }
            else
            {
                cntFlag = true;
                Debug.Log("配列の中身がなくなりました");
            }
            walkCnt = 0;
        }
        else
        {
            walkCnt = walkCnt + 1;
        }


        
        //gameコントローラー
        if (game == "stop")
        {
            cnt = 0;
            game = "stop";
            Debug.Log(game);
        }


        if (itemAllClear())
        {
            game = "clear";
            NextStage.SetActive(true);
            Debug.Log(game);
            cnt = 0;
            if (numberCnt >= StageNum) //プレイヤーの積んだ積み木が最小個数の時 
            {
                Hanamaru.SetActive(true);
            }
            else if (numberCnt < StageNum) //プレイヤーの積んだ積み木が最適解でない時
            {
                Good.SetActive(true);
            }
        }
        else if (cntFlag == true && !itemAllClear())
        {
            game = "over";
            Debug.Log(game);
            RetryButton.SetActive(true);
        }
       
        this.animator.SetBool(key_isJump, false);
        this.animator.SetBool(key_isRun, false);
    }

    private bool itemAllClear()
    {
        foreach (GameObject b in blocks)
        {
            if (b != null)
            {
                return false;
            }
        }
        return true;
    }
    //音鳴らすよう
    //当たってはいけないものにぶつかった時
    private void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(sound1);
        if (other.gameObject.name == "GameOverObj")
        {
            game = "stop";
        }

        if (other.gameObject.name == "DownFloor")
        {
            lever = "leverOn";
        }

        if (other.gameObject.tag == "enemy")
        {
            audioSource.PlayOneShot(sound2);
            cnt = 0;
        }

        if (other.gameObject.tag == "item")
        {
            audioSource.PlayOneShot(sound1);
        }

        if (other.gameObject.name == "JugeBox")
        {
            if (cnt != 0)//音符がすべて消えずにゴールについた時の処理
            {
                cnt = 0;
            }
        }
    }

    //web開く
    private void OpenWeb()
    {
        var uri = new System.Uri("https://www.google.com/");
        Application.OpenURL(uri.AbsoluteUri);
    }

    //右にゆっくり回転して180°でストップ
    IEnumerator RightMove()
    {
        for (int turn = 0; turn < 180; turn++)
        {
            transform.Rotate(0, 1, 0);
            yield return new WaitForSeconds(0.001f);
        }
        
    }


}


