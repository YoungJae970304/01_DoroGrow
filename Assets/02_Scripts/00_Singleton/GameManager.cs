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
    //싱글톤
    public static GameManager gm;
    public UIManager um;

    //현재 씬 확인
    string currentSceneName;

    //도로롱 변수
    public double doro_Exp;
    public double doro_ExpMax;
    public double doro_Level;
    public double doro_Sp;
    public double doro_Atk;
    public double doro_Atk_Total;   //최종으로 적에게 들어가는 데미지 합
    public double doro_Hp;
    public double doro_HpMax;
    public double doro_Dodge;       //전투중 반영되는 회피율
    public double doro_Dodge_Origin;    //고정되어 있는 회피율
    public double doro_Accuracy;   //도로롱의 명중률
    public float speed; //로비 도로롱의 이동속도 (랜덤)
    public float randX, randY;

    //드레이크 변수
    public double drake_Level;
    public double drake_Atk;
    public double drake_Hp;
    public double drake_HpMax;
    public double drake_Dodge;
    public int drake_Pet;

    //맥스웰 변수
    public double maxwell_Level;
    public double maxwell_Atk;
    public double maxwell_Hp;
    public double maxwell_HpMax;
    public double maxwell_Dodge;
    public int maxwell_Pet;

    //라플라스 변수
    public double laplace_Level;
    public double laplace_Atk;
    public double laplace_Hp;
    public double laplace_HpMax;
    public double laplace_Dodge;
    public int laplace_Pet;

    public double metis_DodgeTotal; //드레이크, 맥스웰, 라플라스의 최종 회피율

    //재화 변수
    public double mold; //상점 재화

    //기타 변수
    public double time; //남은 시간
    bool isBattleEnded = false;

    //펫 변수
    public GameObject prefabDrake;
    public GameObject prefabMaxwell;
    public GameObject prefabLaplace;

    public List<GameObject> spawnedPets = new List<GameObject>(); //생성된 펫을 담을 리스트
    public List<Vector3> positionL = new List<Vector3>();
    public List<int> petTypeL = new List<int>();

    //public Dictionary<int, Vector3> positionPets = new Dictionary<int, Vector3>(); //펫 정보를 저장할 데이터 구조
    //public Dictionary<int, int> petInstanceToType = new Dictionary<int, int>();


    void Awake()
    {
        //싱글톤 GameManager 초기화
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

        //현재 씬 초기화 ( 현재 씬의 이름을 currentSceneName 변수에 저장 )
        SceneManager.sceneLoaded += OnSceneLoaded;  //씬이 로드될 때 OnSceneLoaded함수를 실행하는 이벤트
        currentSceneName = SceneManager.GetActiveScene().name;

        //도로롱 변수 초기화
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

        //드레이크 변수 초기화
        drake_Level = 1;
        drake_Atk = 20 * drake_Level;
        drake_HpMax = 200 * drake_Level;
        drake_Hp = drake_HpMax;
        drake_Dodge = 150f + drake_Level * 15f;
        drake_Pet = 0;

        //맥스웰 변수 초기화
        maxwell_Level = 1;
        maxwell_Atk = 60 * maxwell_Level;
        maxwell_HpMax = 600 * maxwell_Level;
        maxwell_Hp = maxwell_HpMax;
        maxwell_Dodge = 300f + maxwell_Level * 30f;
        maxwell_Pet = 0;

        //라플라스 변수 초기화
        laplace_Level = 1;
        laplace_Atk = 180 * laplace_Level;
        laplace_HpMax = 1800 * laplace_Level;
        laplace_Hp = laplace_HpMax;
        laplace_Dodge = 500f + laplace_Level * 50f;
        laplace_Pet = 0;

        //기타 변수 초기화
        mold = 0;
        time = 60f;
    }

    void InitializeManagers()
    {
        //Awake에서 바로 써도 되나 매니저가 추가될 가능성도 있어서 따로 함수로 분리
        um = new UIManager();
    }

    //스크립트가 처음 활성화 될 때 호출, currentSceneName을 awake나 start에서 사용하니 지속적으로 업데이트가 안되고
    //update에 사용하자니 씬이 로드될때만 현재 씬을 찾으면 되서 리소스가 아까워 OnSceneLoaded를 사용
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        if (currentSceneName.Contains("Stage"))
        {
            DoroMetisStatUpdate();
            //전투에 필요한 변수 초기화
            doro_Hp = doro_HpMax;
            drake_Hp = drake_HpMax;
            maxwell_Hp = maxwell_HpMax;
            laplace_Hp = laplace_HpMax;
            time = 60;
            isBattleEnded = false;
        }

        else if (currentSceneName == "01_Robby")
        {
            //저장되어 있던 펫 정보를 로비에 다시 불러옴
            RespawnPets();
        }
    }

    void Start()
    {
        // 세이브된 정보를 게임이 시작할 때 로드해줌
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            Load();
        }
    }

    //////////////////////////////////////// 클릭과 관련된 함수 ///////////////////////////////////////////

    public void ClickEvent_DependingScene(GameObject doro, GameObject prefabExpUp, GameObject prefabLevelUp)
    {
        //UI를 제외하고 화면 클릭시 현재 씬에 따라 해당하는 함수 실행
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (currentSceneName == "01_Robby")
            {
                //클릭한 위치에 경험치가 증가하는 이미지 (프리팹)이 생성
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(prefabExpUp, mousePos, Quaternion.identity);

                //도로롱 성장 함수 (경험치증가,레벨업 등등)
                DoroGrowup();
            }
        }

        DoroLevelUp(doro, prefabLevelUp);
    }



    //////////////////////////////////////// 로비 캐릭터와 관련된 함수들 ///////////////////////////////////////////

    void DoroGrowup()
    {
        // 레벨 디자인 요소
        // 도로롱의 경험치를 늘린다.
        doro_Exp += 10 + (doro_Level * 0.5) + (30 * drake_Pet) + (70 * maxwell_Pet) + (300 * laplace_Pet);
    }

    //도로롱의 레벨업에 관련된 함수
    public void DoroLevelUp(GameObject doro, GameObject prefabLevelUp)
    {
        if (doro_Exp >= doro_ExpMax)
        {
            //레벨업 시
            doro_Sp += doro_Level;          //sp증가
            doro_Level++;                   //레벨업
            doro_Exp -= doro_ExpMax;        //경험치 초기화
            doro_ExpMax += 10 * doro_Level; //레벨업에 필요한 경험치 최대치 증가

            // 도로롱의 위치값을 참조해 레벨업 이펙트를 생성
            Vector3 doroPos = doro.transform.position + Vector3.up * 1.5f;
            Instantiate(prefabLevelUp, doroPos, Quaternion.identity);
        }
    }

    //도로롱 애니메이션
    public void StartDoroAction(GameObject doroGo)
    {
        StartCoroutine(DoroRandomAction(doroGo));
    }

    //확률에 따라 애니메이션 재생
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

            //확률
            int probability = UnityEngine.Random.Range(1, 101);
            //애니메이션이 유지되는 랜덤한 시간
            int randSec = UnityEngine.Random.Range(3, 7);

            if (probability <= 40)
            {
                speed = 2f;
                RandMovCharac(doroGo, speed);
                anim.SetInteger("isAction", 1); //달리기
            }
            else if (probability <= 80)
            {
                speed = 1f;
                RandMovCharac(doroGo, speed);
                anim.SetInteger("isAction", 2); //걷기
            }
            else if (probability <= 100)
            {
                anim.SetInteger("isAction", 3); //잠자기
            }

            //일정 시간 동안 액션을 유지
            yield return new WaitForSeconds(randSec);

            //null 체크를 안하면 씬이 넘어갈 때 MissingReferenceException가 뜸
            if (anim != null && charac != null)   
            {
                //Idle상태로 전환
                anim.SetInteger("isAction", 0);
                charac.velocity = Vector3.zero; //애니메이션이 끝나면 속도를 0으로
            }
            //Idle은 고정적으로 3초 유지
            yield return new WaitForSeconds(3f);
        }
    }

    //메티스의 애니메이션
    public void StartMetisAction(GameObject metisGo)
    {
        StartCoroutine(MetisRandomAction(metisGo));
    }

    //확률에 따라 애니메이션 재생
    IEnumerator MetisRandomAction(GameObject metisGo)
    {
        Animator anim = metisGo.GetComponent<Animator>();
        Rigidbody charac = metisGo.GetComponent<Rigidbody>();

        while (true)
        {
            if (anim == null || charac == null)
            {
                yield break; // 메서드 종료
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
                anim.SetInteger("isWalk", 1); //느리게 걷기
            }

            //일정 시간 동안 액션을 유지
            yield return new WaitForSeconds(randSec);

            if (anim != null)
            {
                //Idle상태로 전환
                anim.SetInteger("isWalk", 0);
            }
            if (charac != null)
            {
                charac.velocity = Vector3.zero; //애니메이션이 끝나면 속도를 0으로
            }
            //Idle은 3초 유지 ( 랜덤으로 Idle이 될 경우 총 랜덤 시간에 +3초만큼 더 Idle
            yield return new WaitForSeconds(3f);
        }
    }

    //도로롱과 메티스 캐릭터 랜덤 이동
    public void RandMovCharac(GameObject character, float speed)
    {
        if (character != null)
        {
            Rigidbody charac = character.GetComponent<Rigidbody>();

            do
            {
                randX = UnityEngine.Random.Range(-1f, 1f);  //-1부터 1까지의 실수, 0도 포함
            } while (randX < 0.5 && randX > -0.5);               //속도가 너무 낮으면 다시 랜덤값을 생성

            do
            {
                randY = UnityEngine.Random.Range(-1f, 1f);
            } while (randY < 0.5 && randY > -0.5);

            Vector3 mov = new Vector3(randX, randY, 0);

            charac.velocity = mov * speed;
        }
    }

    //충돌 시 캐릭터 이동 방향 반대로 , Trigger에서 사용
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
        else 캐릭터끼리 부딪혔을 때 ( 난잡해져서 삭제 )
        {
            Vector3 rig = -characRig.velocity;
            characRig.velocity = rig;
            randX *= -1;
            randY *= -1;
        }
        */
    }

    //히든 엔딩에 관련됨 함수
    public void EndingStart(GameObject endingDoro)
    {
        StartCoroutine(EndSceneSetActive(endingDoro));
    }

    //점점 알파값이 높아지고 알파값이 최대치가 되면 엔딩 애니메이션을 실행
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
            yield return null; // 프레임마다 null을 반환해 for문이 프레임마다 실행되도록 해주는 중요한 부분
        }
        //애니메이션이 실행되고 2.5초후에 종료
        yield return new WaitForSeconds(2.5f);
        endDoro.SetActive(false);
    }


    //////////////////////////////////////// 스테이지 관련 함수들 ///////////////////////////////////////////

    public void Attack_Metis(GameObject metis, GameObject dodgeTxt, GameObject doro)
    {
        //계속 반복해서 공격해야함 -> 코루틴? InvokeRepeating?
        //-> 이 함수를 불러올때마다 랜덤한 공격시간설정 필요 -> 코루틴 사용
        StartCoroutine(Attack_RandTime(metis, dodgeTxt, doro));
    }

    //메티스가 랜덤 시간마다 자동으로 공격
    IEnumerator Attack_RandTime(GameObject metis, GameObject dodgeTxt, GameObject doro)
    {
        Animator anim = metis.GetComponent<Animator>();

        while (true)
        {
            //씬 이동 발생 시 anim과 doro가 파괴되니 실행중인 코루틴 중지
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

            //1부터 1000까지의 랜덤 숫자가 doro_Dodge보다 크다면 공격 성공, 작다면 공격 실패
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

    //마우스 클릭시 공격 모션만 나오게 하는 함수
    public void AttackMotion_Doro(GameObject doro)
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Animator anim = doro.GetComponent<Animator>();

            //Stage라는 이름을 포함한 씬이라면 클릭 시 도로롱이 공격
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

    //공격 후 bool을 false로 바꿔 공격 모션?을 종료
    IEnumerator ChangeIsAttackFalse(Animator anim)
    {
        yield return new WaitForSeconds(0.5f);  //공격하는 0.5초 동안은 도로롱의 회피율이 0이 됨
        if (anim != null)
        {
            anim.SetBool("isAttack", false);
        }
    }
    // 코루틴을 사용하지 않고 홍준택씨의 애니메이션 동작 진행도에 따라 조건을 줄 수 있는 anim.GetCurrentAnimatorStateInfo(0)을 사용하는 방법도 있음

    //공격 후 실제 반영되는 정보들을 관리하는 함수
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

    //전투 종료 함수 / 조건 설정 - 내 캐릭터의 hp가 0이하일때, 적 hp가 0이하일때, 시간이 0 이하일때
    //EndBattle(도로롱의 hp슬라이더, 보스의 hp슬라이더, 결과창, 결과창 내의 결과 텍스트, 결과창 내의 몰드 텍스트)
    public void EndBattle(Slider doro, Slider metis, GameObject window, TextMeshProUGUI endTxt, TextMeshProUGUI moldtxt)
    {
        if (isBattleEnded) return;

        //몰드 획득 확률에 관여하는 변수
        int randResult = UnityEngine.Random.Range(1, 1001);

        if (doro.value <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "패배", moldtxt, " X ");
            isBattleEnded = true;
        }

        else if (time <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "시간 초과", moldtxt, " X ");
            isBattleEnded = true;
        }

        else if (metis.value <= 0)
        {
            Time.timeScale = 0;
            um.End_UI_Window(window, endTxt, "승리", moldtxt, " X ");
            if (metis.name == "DrakeHP")
            {
                if (randResult <= 300 + ( 10 * drake_Level ) )
                {
                    mold++;
                    um.End_UI_Window(window, endTxt, "승리", moldtxt, " 1 ");
                }
                drake_Level++;
            }

            else if (metis.name == "MaxwellHP")
            {
                if (randResult <= 500 + (10 * maxwell_Level))
                {
                    mold += 3;
                    um.End_UI_Window(window, endTxt, "승리", moldtxt, " 3 ");
                }
                maxwell_Level++;
            }

            else if (metis.name == "LaplaceHP")
            {
                if (randResult <= 700 + (10 * maxwell_Level))
                {
                    mold += 10;
                    um.End_UI_Window(window, endTxt, "승리", moldtxt, " 5 ");
                }
                laplace_Level++;
            }

            isBattleEnded = true;
        }
    }



    void DoroMetisStatUpdate()
    {
        //도로롱 스탯 갱신
        doro_Atk_Total = doro_Atk + (2 * drake_Pet) + (5 * maxwell_Pet) + (25 * laplace_Pet);

        //드레이크 스탯 갱신
        drake_Atk = 20 * drake_Level;
        drake_HpMax = 200 * drake_Level;
        drake_Dodge = 50f + drake_Level * 15f;

        //맥스웰 스탯 갱신
        maxwell_Atk = 60 * maxwell_Level;
        maxwell_HpMax = 600 * maxwell_Level;
        maxwell_Dodge = 100f + maxwell_Level * 30f;

        //라플라스 스탯 갱신
        laplace_Atk = 180 * laplace_Level;
        laplace_HpMax = 1800 * laplace_Level;
        laplace_Dodge = 300f + laplace_Level * 50f;
    }

    //도로롱의 현재 회피율을 담당하는 함수
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

    /////////////////////////////////////////// 예외 함수들 //////////////////////////////////////////////

    // 펫의 프리팹을 종류를 기반으로 가져오는 메서드
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

    //펫 생성 함수
    public void SpawnPet(Vector3 position, int petType)
    {
        // 선택된 펫 종류에 따라 프리팹을 선택
        GameObject petPrefab = GetPetPrefabFromType(petType);

        if (petPrefab != null)
        {
            // 펫을 생성하고 리스트에 추가
            GameObject newPet = Instantiate(petPrefab, position, Quaternion.identity);
            spawnedPets.Add(newPet);

            // 펫의 위치 정보를 저장
            positionL.Add(position);
            petTypeL.Add(petType);
        }
    }

    // 로비로 돌아왔을 때 펫을 재생성하는 메서드
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

    //버튼 동작이라 UIManager에서 만드려고 했으나 Instantiate를 사용해야되는데 UIManager는 MonoBehaviour가 없어서 이곳에서 사용
    public void Btn_BuyMetisPet(string toggleName)
    {
        float createdPosX = UnityEngine.Random.Range(-6.5f, 6.5f);
        float createdPosY = UnityEngine.Random.Range(-1f, 2f);
        Vector3 creatPos = new Vector3(createdPosX, createdPosY, 0);

        switch (toggleName)
        {
            // 1 = 드레이크 / 2 = 맥스웰 / 3 = 라플라스 //

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

    ///////////////////////////////////// 데이터 세이브와 로드 ////////////////////////////////////

    //게임이 종료될 때 자동으로 실행 되는 이벤트
    private void OnApplicationQuit()
    {
        Save();
        //Reset();
    }

    public void Reset()
    {
        //객체화 (오브젝트화)
        SaveData saveData;

        // 기존 데이터 로드
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

        //도로롱 변수 초기화
        doro_Exp = 0;
        saveData.doro_ExpMax = 100;
        saveData.doro_Level = 1;
        saveData.doro_Sp = 5;
        saveData.doro_Atk = 10;
        saveData.doro_HpMax = 100;
        saveData.doro_Dodge_Origin = 100f;
        saveData.doro_Accuracy = 100f;

        //드레이크 변수 초기화
        saveData.drake_Level = 1;
        saveData.drake_Pet = 0;

        //맥스웰 변수 초기화
        saveData.maxwell_Level = 1;
        saveData.maxwell_Pet = 0;

        //라플라스 변수 초기화
        saveData.laplace_Level = 1;
        saveData.laplace_Pet = 0;

        //기타 변수 초기화
        mold = 0;
        time = 60f;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Save()
    {

        //객체화 (오브젝트화)
        SaveData saveData;

        // 기존 데이터 로드
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

        //값 저장
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
            if (pet != null && petInstanceToType.ContainsKey(pet.GetInstanceID()))  // pet이 파괴되지 않았는지 확인
            {
                saveData.petPositions.Add(pet.transform.position);
                saveData.petTypes.Add(petInstanceToType[pet.GetInstanceID()]);
            }
        }
        */
        saveData.petPositions = positionL;
        saveData.petTypes = petTypeL;

        //데이터 저장
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

            //저장한 값을 현재 값에 대입
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

            // 이전에 생성된 펫 삭제
            foreach (var pet in spawnedPets)
            {
                Destroy(pet);
            }
            spawnedPets.Clear();
            //positionPets.Clear();
            //petInstanceToType.Clear();
            positionL.Clear();
            petTypeL.Clear();

            
            // 저장된 위치와 종류로 펫을 다시 생성
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
    //저장할 변수
    //도로롱 변수
    public double doro_Exp;
    public double doro_ExpMax;          //최대 경험치      
    public double doro_Level;           //레벨
    public double doro_Sp;              //스킬 포인트
    public double doro_Atk;             //공격력
    public double doro_HpMax;           //최대 체력
    public double doro_Dodge_Origin;    //회피율
    public double doro_Accuracy;        //도로롱의 명중률

    //드레이크 변수
    public double drake_Level;
    public double drake_Atk;
    public double drake_HpMax;
    public double drake_Dodge;
    public int drake_Pet;

    //맥스웰 변수
    public double maxwell_Level;
    public double maxwell_Atk;
    public double maxwell_HpMax;
    public double maxwell_Dodge;
    public int maxwell_Pet;

    //라플라스 변수
    public double laplace_Level;
    public double laplace_Atk;
    public double laplace_HpMax;
    public double laplace_Dodge;
    public int laplace_Pet;

    //상점 재화
    public double mold;


    //펫
    public List<int> petTypes = new List<int>();
    public List<Vector3> petPositions = new List<Vector3>();
}
