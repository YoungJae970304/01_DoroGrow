using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    //�̱���
    public static GameManager gm;
    public UIManager um;

    //���� �� Ȯ��
    string currentSceneName;

    //���η� ����
    public double doro_Exp;
    public double doro_ExpMax;
    public double doro_Level;
    public double doro_Sp;
    public double doro_Atk;
    public double doro_Atk_Total;   //�������� ������ ���� ������ ��
    public double doro_Hp;
    public double doro_HpMax;
    public double doro_Dodge;       //������ �ݿ��Ǵ� ȸ����
    public double doro_Dodge_Origin;    //�����Ǿ� �ִ� ȸ����
    public double doro_Accuracy;   //���η��� ���߷�
    public float speed; //�κ� ���η��� �̵��ӵ� (����)
    public float randX, randY;

    //�巹��ũ ����
    public double drake_Level;
    public double drake_Atk;
    public double drake_Hp;
    public double drake_HpMax;
    public double drake_Dodge;
    public int drake_Pet;

    //�ƽ��� ����
    public double maxwell_Level;
    public double maxwell_Atk;
    public double maxwell_Hp;
    public double maxwell_HpMax;
    public double maxwell_Dodge;
    public int maxwell_Pet;

    //���ö� ����
    public double laplace_Level;
    public double laplace_Atk;
    public double laplace_Hp;
    public double laplace_HpMax;
    public double laplace_Dodge;
    public int laplace_Pet;

    public double metis_DodgeTotal; //�巹��ũ, �ƽ���, ���ö��� ���� ȸ����

    //��ȭ ����
    public double mold; //���� ��ȭ

    //��Ÿ ����
    public double time; //���� �ð�
    bool isBattleEnded = false;

    //�� ����
    public GameObject prefabDrake;
    public GameObject prefabMaxwell;
    public GameObject prefabLaplace;

    public List<GameObject> spawnedPets = new List<GameObject>(); //������ ���� ���� ����Ʈ
    public List<Vector3> positionL = new List<Vector3>();
    public List<int> petTypeL = new List<int>();

    //public Dictionary<int, Vector3> positionPets = new Dictionary<int, Vector3>(); //�� ������ ������ ������ ����
    //public Dictionary<int, int> petInstanceToType = new Dictionary<int, int>();


    void Awake()
    {
        //�̱��� GameManager �ʱ�ȭ
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }

        //���� �� �ʱ�ȭ ( ���� ���� �̸��� currentSceneName ������ ���� )
        SceneManager.sceneLoaded += OnSceneLoaded;  //���� �ε�� �� OnSceneLoaded�Լ��� �����ϴ� �̺�Ʈ
        currentSceneName = SceneManager.GetActiveScene().name;

        //���η� ���� �ʱ�ȭ
        doro_Exp = 0;
        doro_ExpMax = 100;
        doro_Level = 1;
        doro_Sp = 5;
        doro_Atk = 10;
        doro_HpMax = 100;
        doro_Hp = doro_HpMax;
        doro_Dodge_Origin = 100f;
        doro_Dodge = doro_Dodge_Origin;
        doro_Accuracy = 100f;

        //�巹��ũ ���� �ʱ�ȭ
        drake_Level = 1;
        drake_Atk = 20 * drake_Level;
        drake_HpMax = 200 * drake_Level;
        drake_Hp = drake_HpMax;
        drake_Dodge = 150f + drake_Level * 15f;
        drake_Pet = 0;

        //�ƽ��� ���� �ʱ�ȭ
        maxwell_Level = 1;
        maxwell_Atk = 60 * maxwell_Level;
        maxwell_HpMax = 600 * maxwell_Level;
        maxwell_Hp = maxwell_HpMax;
        maxwell_Dodge = 300f + maxwell_Level * 30f;
        maxwell_Pet = 0;

        //���ö� ���� �ʱ�ȭ
        laplace_Level = 1;
        laplace_Atk = 180 * laplace_Level;
        laplace_HpMax = 1800 * laplace_Level;
        laplace_Hp = laplace_HpMax;
        laplace_Dodge = 500f + laplace_Level * 50f;
        laplace_Pet = 0;

        //��Ÿ ���� �ʱ�ȭ
        mold = 0;
        time = 60f;
    }

    void InitializeManagers()
    {
        //Awake���� �ٷ� �ᵵ �ǳ� �Ŵ����� �߰��� ���ɼ��� �־ ���� �Լ��� �и�
        um = new UIManager();
    }

    //��ũ��Ʈ�� ó�� Ȱ��ȭ �� �� ȣ��, currentSceneName�� awake�� start���� ����ϴ� ���������� ������Ʈ�� �ȵǰ�
    //update�� ������ڴ� ���� �ε�ɶ��� ���� ���� ã���� �Ǽ� ���ҽ��� �Ʊ�� OnSceneLoaded�� ���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        if (currentSceneName.Contains("Stage"))
        {
            DoroMetisStatUpdate();
            //������ �ʿ��� ���� �ʱ�ȭ
            doro_Hp = doro_HpMax;
            drake_Hp = drake_HpMax;
            maxwell_Hp = maxwell_HpMax;
            laplace_Hp = laplace_HpMax;
            time = 60;
            isBattleEnded = false;
        }

        else if (currentSceneName == "01_Robby")
        {
            //����Ǿ� �ִ� �� ������ �κ� �ٽ� �ҷ���
            RespawnPets();
        }
    }

    void Start()
    {
        // ���̺�� ������ ������ ������ �� �ε�����
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            Load();
        }
    }

    //////////////////////////////////////// Ŭ���� ���õ� �Լ� ///////////////////////////////////////////

    public void ClickEvent_DependingScene(GameObject doro, GameObject prefabExpUp, GameObject prefabLevelUp)
    {
        //UI�� �����ϰ� ȭ�� Ŭ���� ���� ���� ���� �ش��ϴ� �Լ� ����
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (currentSceneName == "01_Robby")
            {
                //Ŭ���� ��ġ�� ����ġ�� �����ϴ� �̹��� (������)�� ����
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(prefabExpUp, mousePos, Quaternion.identity);

                //���η� ���� �Լ� (����ġ����,������ ���)
                DoroGrowup();
            }
        }

        DoroLevelUp(doro, prefabLevelUp);
    }



    //////////////////////////////////////// �κ� ĳ���Ϳ� ���õ� �Լ��� ///////////////////////////////////////////

    void DoroGrowup()
    {
        // ���� ������ ���
        // ���η��� ����ġ�� �ø���.
        doro_Exp += 10 + (doro_Level * 0.5) + (30 * drake_Pet) + (70 * maxwell_Pet) + (300 * laplace_Pet);
    }

    //���η��� �������� ���õ� �Լ�
    public void DoroLevelUp(GameObject doro, GameObject prefabLevelUp)
    {
        if (doro_Exp >= doro_ExpMax)
        {
            //������ ��
            doro_Sp += doro_Level;          //sp����
            doro_Level++;                   //������
            doro_Exp -= doro_ExpMax;        //����ġ �ʱ�ȭ
            doro_ExpMax += 10 * doro_Level; //�������� �ʿ��� ����ġ �ִ�ġ ����

            // ���η��� ��ġ���� ������ ������ ����Ʈ�� ����
            Vector3 doroPos = doro.transform.position + Vector3.up * 1.5f;
            Instantiate(prefabLevelUp, doroPos, Quaternion.identity);
        }
    }

    //���η� �ִϸ��̼�
    public void StartDoroAction(GameObject doroGo)
    {
        StartCoroutine(DoroRandomAction(doroGo));
    }

    //Ȯ���� ���� �ִϸ��̼� ���
    IEnumerator DoroRandomAction(GameObject doroGo)
    {
        Animator anim = doroGo.GetComponent<Animator>();
        Rigidbody charac = doroGo.GetComponent<Rigidbody>();

        if (anim == null || charac == null)
        {
            yield break;
        }

        while (true)
        {
            if (anim == null || charac == null)
            {
                yield break;
            }

            //Ȯ��
            int probability = UnityEngine.Random.Range(1, 101);
            //�ִϸ��̼��� �����Ǵ� ������ �ð�
            int randSec = UnityEngine.Random.Range(3, 7);

            if (probability <= 40)
            {
                speed = 2f;
                RandMovCharac(doroGo, speed);
                anim.SetInteger("isAction", 1); //�޸���
            }
            else if (probability <= 80)
            {
                speed = 1f;
                RandMovCharac(doroGo, speed);
                anim.SetInteger("isAction", 2); //�ȱ�
            }
            else if (probability <= 100)
            {
                anim.SetInteger("isAction", 3); //���ڱ�
            }

            //���� �ð� ���� �׼��� ����
            yield return new WaitForSeconds(randSec);

            //null üũ�� ���ϸ� ���� �Ѿ �� MissingReferenceException�� ��
            if (anim != null && charac != null)   
            {
                //Idle���·� ��ȯ
                anim.SetInteger("isAction", 0);
                charac.velocity = Vector3.zero; //�ִϸ��̼��� ������ �ӵ��� 0����
            }
            //Idle�� ���������� 3�� ����
            yield return new WaitForSeconds(3f);
        }
    }

    //��Ƽ���� �ִϸ��̼�
    public void StartMetisAction(GameObject metisGo)
    {
        StartCoroutine(MetisRandomAction(metisGo));
    }

    //Ȯ���� ���� �ִϸ��̼� ���
    IEnumerator MetisRandomAction(GameObject metisGo)
    {
        Animator anim = metisGo.GetComponent<Animator>();
        Rigidbody charac = metisGo.GetComponent<Rigidbody>();

        while (true)
        {
            if (anim == null || charac == null)
            {
                yield break; // �޼��� ����
            }

            int probability = UnityEngine.Random.Range(1, 101);
            int randSec = UnityEngine.Random.Range(3, 7);

            if (probability <= 30)
            {
                anim.SetInteger("isWalk", 0); //Idle
            }
            else if (probability <= 100)
            {
                speed = 0.7f;
                RandMovCharac(metisGo, speed);
                anim.SetInteger("isWalk", 1); //������ �ȱ�
            }

            //���� �ð� ���� �׼��� ����
            yield return new WaitForSeconds(randSec);

            if (anim != null)
            {
                //Idle���·� ��ȯ
                anim.SetInteger("isWalk", 0);
            }
            if (charac != null)
            {
                charac.velocity = Vector3.zero; //�ִϸ��̼��� ������ �ӵ��� 0����
            }
            //Idle�� 3�� ���� ( �������� Idle�� �� ��� �� ���� �ð��� +3�ʸ�ŭ �� Idle
            yield return new WaitForSeconds(3f);
        }
    }

    //���ηհ� ��Ƽ�� ĳ���� ���� �̵�
    public void RandMovCharac(GameObject character, float speed)
    {
        if (character != null)
        {
            Rigidbody charac = character.GetComponent<Rigidbody>();

            do
            {
                randX = UnityEngine.Random.Range(-1f, 1f);  //-1���� 1������ �Ǽ�, 0�� ����
            } while (randX < 0.5 && randX > -0.5);               //�ӵ��� �ʹ� ������ �ٽ� �������� ����

            do
            {
                randY = UnityEngine.Random.Range(-1f, 1f);
            } while (randY < 0.5 && randY > -0.5);

            Vector3 mov = new Vector3(randX, randY, 0);

            charac.velocity = mov * speed;
        }
    }

    //�浹 �� ĳ���� �̵� ���� �ݴ�� , Trigger���� ���
    public void ChangeDirToTrigger(Collider other, GameObject charac)
    {
        Rigidbody characRig = charac.GetComponent<Rigidbody>();

        if (other.gameObject.CompareTag("HWall"))
        {
            Vector3 rig = characRig.velocity;
            rig.x *= -1;
            characRig.velocity = rig;
            randX *= -1;
        }

        else if (other.gameObject.CompareTag("VWall"))
        {
            Vector3 rig = characRig.velocity;
            rig.y *= -1;
            characRig.velocity = rig;
            randY *= -1;
        }
        /*
        else ĳ���ͳ��� �ε����� �� ( ���������� ���� )
        {
            Vector3 rig = -characRig.velocity;
            characRig.velocity = rig;
            randX *= -1;
            randY *= -1;
        }
        */
    }

    //���� ������ ���õ� �Լ�
    public void EndingStart(GameObject endingDoro)
    {
        StartCoroutine(EndSceneSetActive(endingDoro));
    }

    //���� ���İ��� �������� ���İ��� �ִ�ġ�� �Ǹ� ���� �ִϸ��̼��� ����
    IEnumerator EndSceneSetActive(GameObject endDoro)
    {
        endDoro.SetActive(true);

        Image srend = endDoro.GetComponent<Image>();
        Animator anim = endDoro.GetComponent<Animator>();

        anim.enabled = false;

        srend.color = new UnityEngine.Color(srend.color.r, srend.color.g, srend.color.b, 0);

        for (float i = 0; i < 1; i += 0.006f)
        {
            srend.color = new UnityEngine.Color(srend.color.r, srend.color.g, srend.color.b, i);
            if (srend.color.a >= 0.99f)
            {
                anim.enabled = true;
                break;
            }
            yield return null; // �����Ӹ��� null�� ��ȯ�� for���� �����Ӹ��� ����ǵ��� ���ִ� �߿��� �κ�
        }
        //�ִϸ��̼��� ����ǰ� 2.5���Ŀ� ����
        yield return new WaitForSeconds(2.5f);
        endDoro.SetActive(false);
    }


    //////////////////////////////////////// �������� ���� �Լ��� ///////////////////////////////////////////

    public void Attack_Metis(GameObject metis, GameObject dodgeTxt, GameObject doro)
    {
        //��� �ݺ��ؼ� �����ؾ��� -> �ڷ�ƾ? InvokeRepeating?
        //-> �� �Լ��� �ҷ��ö����� ������ ���ݽð����� �ʿ� -> �ڷ�ƾ ���
        StartCoroutine(Attack_RandTime(metis, dodgeTxt, doro));
    }

    //��Ƽ���� ���� �ð����� �ڵ����� ����
    IEnumerator Attack_RandTime(GameObject metis, GameObject dodgeTxt, GameObject doro)
    {
        Animator anim = metis.GetComponent<Animator>();

        while (true)
        {
            //�� �̵� �߻� �� anim�� doro�� �ı��Ǵ� �������� �ڷ�ƾ ����
            if (anim == null || doro == null)
            {
                yield break;
            }

            float randTim = UnityEngine.Random.Range(0.5f, 3f);
            int randDodge = UnityEngine.Random.Range(1, 1001);

            yield return new WaitForSeconds(randTim);
            if (anim != null)
            {
                anim.SetTrigger("isAttack");
            }

            //1���� 1000������ ���� ���ڰ� doro_Dodge���� ũ�ٸ� ���� ����, �۴ٸ� ���� ����
            if (doro_Dodge < randDodge)
            {
                yield return new WaitForSeconds(0.65f);
                if (doro == null)
                {
                    yield break;
                }
                if (metis.name == "Drake")
                {
                    doro_Hp -= drake_Atk;
                }
                else if (metis.name == "Maxwell")
                {
                    doro_Hp -= maxwell_Atk;
                }
                else if (metis.name == "Laplace")
                {
                    doro_Hp -= laplace_Atk;
                }
            }

            else
            {
                if (doro != null)
                {
                    Vector3 txtPos = doro.transform.position + Vector3.up * 1.2f;
                    yield return new WaitForSeconds(0.65f);
                    if (doro == null)
                    {
                        yield break;
                    }
                    Instantiate(dodgeTxt, txtPos, Quaternion.identity);
                }
            }
        }
    }

    //���콺 Ŭ���� ���� ��Ǹ� ������ �ϴ� �Լ�
    public void AttackMotion_Doro(GameObject doro)
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Animator anim = doro.GetComponent<Animator>();

            //Stage��� �̸��� ������ ���̶�� Ŭ�� �� ���η��� ����
            if (currentSceneName.Contains("Stage"))
            {
                if (anim != null)
                {
                    anim.SetBool("isAttack", true);
                    StartCoroutine(ChangeIsAttackFalse(anim));
                }
            }
        }
    }

    //���� �� bool�� false�� �ٲ� ���� ���?�� ����
    IEnumerator ChangeIsAttackFalse(Animator anim)
    {
        yield return new WaitForSeconds(0.5f);  //�����ϴ� 0.5�� ������ ���η��� ȸ������ 0�� ��
        if (anim != null)
        {
            anim.SetBool("isAttack", false);
        }
    }
    // �ڷ�ƾ�� ������� �ʰ� ȫ���þ��� �ִϸ��̼� ���� ���൵�� ���� ������ �� �� �ִ� anim.GetCurrentAnimatorStateInfo(0)�� ����ϴ� ����� ����

    //���� �� ���� �ݿ��Ǵ� �������� �����ϴ� �Լ�
    public void Atk_Doro(Collider col, GameObject missTxt, DoroFight df)
    {
        int randMiss = UnityEngine.Random.Range(1, 1001);

        if (df.isAttacking && col.gameObject.tag == "Metis")
        {
            if (col.name == "Drake")
            {
                metis_DodgeTotal = drake_Dodge - doro_Accuracy;

                if (metis_DodgeTotal < randMiss)
                {
                    drake_Hp -= doro_Atk_Total;
                }
                else
                {
                    Vector3 txtPos = col.transform.position + Vector3.up * 2f;
                    Instantiate(missTxt, txtPos, Quaternion.identity);
                }
            }

            else if (col.name == "Maxwell")
            {
                metis_DodgeTotal = maxwell_Dodge - doro_Accuracy;

                if (metis_DodgeTotal < randMiss)
                {
                    maxwell_Hp -= doro_Atk_Total;
                }
                else
                {
                    Vector3 txtPos = col.transform.position + Vector3.up * 2f;
                    Instantiate(missTxt, txtPos, Quaternion.identity);
                }
            }

            else if (col.name == "Laplace")
            {
                metis_DodgeTotal = laplace_Dodge - doro_Accuracy;

                if (metis_DodgeTotal < randMiss)
                {
                    laplace_Hp -= doro_Atk_Total;
                }
                else
                {
                    Vector3 txtPos = col.transform.position + Vector3.up * 2f;
                    Instantiate(missTxt, txtPos, Quaternion.identity);
                }
            }
        }
    }

    //���� ���� �Լ� / ���� ���� - �� ĳ������ hp�� 0�����϶�, �� hp�� 0�����϶�, �ð��� 0 �����϶�
    //EndBattle(���η��� hp�����̴�, ������ hp�����̴�, ���â, ���â ���� ��� �ؽ�Ʈ, ���â ���� ���� �ؽ�Ʈ)
    public void EndBattle(Slider doro, Slider metis, GameObject window, TextMeshProUGUI endTxt, TextMeshProUGUI moldtxt)
    {
        if (isBattleEnded) return;

        //���� ȹ�� Ȯ���� �����ϴ� ����
        int randResult = UnityEngine.Random.Range(1, 1001);

        if (doro.value <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "�й�", moldtxt, " X ");
            isBattleEnded = true;
        }

        else if (time <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "�ð� �ʰ�", moldtxt, " X ");
            isBattleEnded = true;
        }

        else if (metis.value <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "�¸�", moldtxt, " X ");
            if (metis.name == "DrakeHP")
            {
                if (randResult <= 300 + ( 10 * drake_Level ) )
                {
                    mold++;
                    um.End_UI_Window(window, endTxt, "�¸�", moldtxt, " 1 ");
                }
                drake_Level++;
            }

            else if (metis.name == "MaxwellHP")
            {
                if (randResult <= 500 + (10 * maxwell_Level))
                {
                    mold += 3;
                    um.End_UI_Window(window, endTxt, "�¸�", moldtxt, " 3 ");
                }
                maxwell_Level++;
            }

            else if (metis.name == "LaplaceHP")
            {
                if (randResult <= 700 + (10 * maxwell_Level))
                {
                    mold += 10;
                    um.End_UI_Window(window, endTxt, "�¸�", moldtxt, " 5 ");
                }
                laplace_Level++;
            }

            isBattleEnded = true;
        }
    }



    void DoroMetisStatUpdate()
    {
        //���η� ���� ����
        doro_Atk_Total = doro_Atk + (2 * drake_Pet) + (5 * maxwell_Pet) + (25 * laplace_Pet);

        //�巹��ũ ���� ����
        drake_Atk = 20 * drake_Level;
        drake_HpMax = 200 * drake_Level;
        drake_Dodge = 50f + drake_Level * 15f;

        //�ƽ��� ���� ����
        maxwell_Atk = 60 * maxwell_Level;
        maxwell_HpMax = 600 * maxwell_Level;
        maxwell_Dodge = 100f + maxwell_Level * 30f;

        //���ö� ���� ����
        laplace_Atk = 180 * laplace_Level;
        laplace_HpMax = 1800 * laplace_Level;
        laplace_Dodge = 300f + laplace_Level * 50f;
    }

    //���η��� ���� ȸ������ ����ϴ� �Լ�
    public void UpdateDoroDodge(GameObject metis)
    {
        if (metis.name == "Drake")
        {
            doro_Dodge = doro_Dodge_Origin / drake_Level;
        }
        else if (metis.name == "Maxwell")
        {
            doro_Dodge = doro_Dodge_Origin / maxwell_Level;
        }
        else if (metis.name == "Laplace")
        {
            doro_Dodge = doro_Dodge_Origin / laplace_Level;
        }
    }

    /////////////////////////////////////////// ���� �Լ��� //////////////////////////////////////////////

    // ���� �������� ������ ������� �������� �޼���
    GameObject GetPetPrefabFromType(int petType)
    {
        switch (petType)
        {
            case 1:
                return prefabDrake;
            case 2:
                return prefabMaxwell;
            case 3:
                return prefabLaplace;
            default:
                return null;
        }
    }

    //�� ���� �Լ�
    public void SpawnPet(Vector3 position, int petType)
    {
        // ���õ� �� ������ ���� �������� ����
        GameObject petPrefab = GetPetPrefabFromType(petType);

        if (petPrefab != null)
        {
            // ���� �����ϰ� ����Ʈ�� �߰�
            GameObject newPet = Instantiate(petPrefab, position, Quaternion.identity);
            spawnedPets.Add(newPet);

            // ���� ��ġ ������ ����
            positionL.Add(position);
            petTypeL.Add(petType);
        }
    }

    // �κ�� ���ƿ��� �� ���� ������ϴ� �޼���
    public void RespawnPets()
    {
        for (int i = 0; i < positionL.Count; i++)
        {
            int petType = petTypeL[i];
            Vector3 position = positionL[i];
            GameObject petPrefab = GetPetPrefabFromType(petType);

            if (petPrefab != null)
            {
                GameObject newPet = Instantiate(petPrefab, position, Quaternion.identity);
                if (newPet != null)
                {
                    spawnedPets.Add(newPet);
                }
            }
        }
    }

    //��ư �����̶� UIManager���� ������� ������ Instantiate�� ����ؾߵǴµ� UIManager�� MonoBehaviour�� ��� �̰����� ���
    public void Btn_BuyMetisPet(string toggleName)
    {
        float createdPosX = UnityEngine.Random.Range(-6.5f, 6.5f);
        float createdPosY = UnityEngine.Random.Range(-1f, 2f);
        Vector3 creatPos = new Vector3(createdPosX, createdPosY, 0);

        switch (toggleName)
        {
            // 1 = �巹��ũ / 2 = �ƽ��� / 3 = ���ö� //

            case "ToggleDrake":
                if (mold > 0)
                {
                    SpawnPet(creatPos, 1);

                    drake_Pet++;
                    mold = mold - 1;
                }
                break;

            case "ToggleMaxwell":
                if (mold >= 5)
                {
                    SpawnPet(creatPos, 2);

                    maxwell_Pet++;
                    mold = mold - 5;
                }
                break;

            case "ToggleLaplace":
                if (mold >= 25)
                {
                    SpawnPet(creatPos, 3);

                    laplace_Pet++;
                    mold = mold - 25;
                }
                break;

            default:

                break;
        }
    }

    ///////////////////////////////////// ������ ���̺�� �ε� ////////////////////////////////////

    //������ ����� �� �ڵ����� ���� �Ǵ� �̺�Ʈ
    private void OnApplicationQuit()
    {
        Save();
        //Reset();
    }

    public void Reset()
    {
        //��üȭ (������Ʈȭ)
        SaveData saveData;

        // ���� ������ �ε�
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string jsonS = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(jsonS);
        }
        else
        {
            saveData = new SaveData();
        }

        //���η� ���� �ʱ�ȭ
        doro_Exp = 0;
        saveData.doro_ExpMax = 100;
        saveData.doro_Level = 1;
        saveData.doro_Sp = 5;
        saveData.doro_Atk = 10;
        saveData.doro_HpMax = 100;
        saveData.doro_Dodge_Origin = 100f;
        saveData.doro_Accuracy = 100f;

        //�巹��ũ ���� �ʱ�ȭ
        saveData.drake_Level = 1;
        saveData.drake_Pet = 0;

        //�ƽ��� ���� �ʱ�ȭ
        saveData.maxwell_Level = 1;
        saveData.maxwell_Pet = 0;

        //���ö� ���� �ʱ�ȭ
        saveData.laplace_Level = 1;
        saveData.laplace_Pet = 0;

        //��Ÿ ���� �ʱ�ȭ
        mold = 0;
        time = 60f;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Save()
    {

        //��üȭ (������Ʈȭ)
        SaveData saveData;

        // ���� ������ �ε�
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string jsonS = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(jsonS);
        }
        else
        {
            saveData = new SaveData();
        }

        //�� ����
        saveData.doro_Exp = doro_Exp;
        saveData.doro_ExpMax = doro_ExpMax;
        saveData.doro_Level = doro_Level;
        saveData.doro_Sp = doro_Sp;
        saveData.doro_Atk = doro_Atk;
        saveData.doro_HpMax = doro_HpMax;
        saveData.doro_Dodge_Origin = doro_Dodge_Origin;
        saveData.doro_Accuracy = doro_Accuracy;

        saveData.drake_Level = drake_Level;
        saveData.drake_Atk = drake_Atk;
        saveData.drake_HpMax = drake_HpMax;
        saveData.drake_Dodge = drake_Dodge;
        saveData.drake_Pet = drake_Pet;

        saveData.maxwell_Level = maxwell_Level;
        saveData.maxwell_Atk = maxwell_Atk;
        saveData.maxwell_HpMax = maxwell_HpMax;
        saveData.maxwell_Dodge = maxwell_Dodge;
        saveData.maxwell_Pet = maxwell_Pet;

        saveData.laplace_Level = laplace_Level;
        saveData.laplace_Atk = laplace_Atk;
        saveData.laplace_HpMax = laplace_HpMax;
        saveData.laplace_Dodge = laplace_Dodge;
        saveData.laplace_Pet = laplace_Pet;

        saveData.mold = mold;

        saveData.petPositions.Clear();
        saveData.petTypes.Clear();

        /*
        foreach (var pet in spawnedPets)
        {
            if (pet != null && petInstanceToType.ContainsKey(pet.GetInstanceID()))  // pet�� �ı����� �ʾҴ��� Ȯ��
            {
                saveData.petPositions.Add(pet.transform.position);
                saveData.petTypes.Add(petInstanceToType[pet.GetInstanceID()]);
            }
        }
        */
        saveData.petPositions = positionL;
        saveData.petTypes = petTypeL;

        //������ ����
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            //������ ���� ���� ���� ����
            doro_Exp = saveData.doro_Exp;
            doro_ExpMax = saveData.doro_ExpMax;
            doro_Level = saveData.doro_Level;
            doro_Sp = saveData.doro_Sp;
            doro_Atk = saveData.doro_Atk;
            doro_HpMax = saveData.doro_HpMax;
            doro_Dodge_Origin = saveData.doro_Dodge_Origin;
            doro_Accuracy = saveData.doro_Accuracy;

            drake_Level = saveData.drake_Level;
            drake_Atk = saveData.drake_Atk;
            drake_HpMax = saveData.drake_HpMax;
            drake_Dodge = saveData.drake_Dodge;
            drake_Pet = saveData.drake_Pet;

            maxwell_Level = saveData.maxwell_Level;
            maxwell_Atk = saveData.maxwell_Atk;
            maxwell_HpMax = saveData.maxwell_HpMax;
            maxwell_Dodge = saveData.maxwell_Dodge;
            maxwell_Pet = saveData.maxwell_Pet;

            laplace_Level = saveData.laplace_Level;
            laplace_Atk = saveData.laplace_Atk;
            laplace_HpMax = saveData.laplace_HpMax;
            laplace_Dodge = saveData.laplace_Dodge;
            laplace_Pet = saveData.laplace_Pet;

            mold = saveData.mold;

            // ������ ������ �� ����
            foreach (var pet in spawnedPets)
            {
                Destroy(pet);
            }
            spawnedPets.Clear();
            //positionPets.Clear();
            //petInstanceToType.Clear();
            positionL.Clear();
            petTypeL.Clear();

            
            // ����� ��ġ�� ������ ���� �ٽ� ����
            for (int i = 0; i < saveData.petPositions.Count; i++)
            {
                //Vector3 position = saveData.petPositions[i];
                //int petType = saveData.petTypes[i];
                //SpawnPet(position, petType);
                positionL.Add(saveData.petPositions[i]);
                petTypeL.Add(saveData.petTypes[i]);
            }
            
        }
    }
}


[System.Serializable]
public class SaveData
{
    //������ ����
    //���η� ����
    public double doro_Exp;
    public double doro_ExpMax;          //�ִ� ����ġ      
    public double doro_Level;           //����
    public double doro_Sp;              //��ų ����Ʈ
    public double doro_Atk;             //���ݷ�
    public double doro_HpMax;           //�ִ� ü��
    public double doro_Dodge_Origin;    //ȸ����
    public double doro_Accuracy;        //���η��� ���߷�

    //�巹��ũ ����
    public double drake_Level;
    public double drake_Atk;
    public double drake_HpMax;
    public double drake_Dodge;
    public int drake_Pet;

    //�ƽ��� ����
    public double maxwell_Level;
    public double maxwell_Atk;
    public double maxwell_HpMax;
    public double maxwell_Dodge;
    public int maxwell_Pet;

    //���ö� ����
    public double laplace_Level;
    public double laplace_Atk;
    public double laplace_HpMax;
    public double laplace_Dodge;
    public int laplace_Pet;

    //���� ��ȭ
    public double mold;


    //��
    public List<int> petTypes = new List<int>();
    public List<Vector3> petPositions = new List<Vector3>();
}
