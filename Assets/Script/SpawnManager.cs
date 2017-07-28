using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnManager : MonoBehaviour {

    //TODO::특정라운드 부터는 새몬스터도 같이소환하기<<먼훗날?
    //TODO::대상이 죽어버리면 멈추거나 GameOver 시키기 <<안할지도?

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float nextSpawnTime; //한방에 소환하면 쓸모없을거같음
    }
    public Wave[] waves;
    Wave currentWave;
    int currentWaveNumber;
    float waveSpawnTime;

    int enemiesAlive;
    int enemiesToSpawn;

    public float restTime;//외부에서 참조 할꺼

    bool isEnabled;

    public Enemy[] enemys;

    public GameObject skipButton;

    public float maxSpawn = 10;//왜있는지 까먹음 << 최대소환수 여기서 1이라도 줄면 다시 생성시작하는식

//    Status playerLive;//플레이어,성의생존여부
//    Status houseLive;


    [Header("X축 범위")]
    public float x1Min;
    public float x1Max;

    [Header("X축 범위2")]
    public float x2Min;
    public float x2Max;

    [Header("Y축 범위")]
    public float yMin;
    public float yMax;
    //이렇게두고 짝수번째면 왼쪽 홀수번째면 오른쪽 이렇게 y축은 공유하는거일터이니 추가 안함
    public void Skip()
    {
        //다잡으면 스킵이 보이도록만들거임
        restTime = Time.time;

        skipButton.SetActive(false);
    }
    public void WaveSet()
    {
        GameManager.inst.InBattle = true;
        currentWaveNumber++;
        
        if(currentWaveNumber - 1 < waves.Length)
        {
                currentWave = waves[currentWaveNumber - 1];
                enemiesToSpawn = currentWave.enemyCount;
                enemiesAlive = enemiesToSpawn;
        }

    }
    void EnemyPowerUp(Enemy enemy,int num)
    {
        enemy.gold += 2 +(uint)num;
        int val = Random.Range(0, num);
        enemy.virtualStatus.max_Hp += val;
        enemy.virtualStatus.hp = enemy.virtualStatus.max_Hp;
        val = Random.Range(0, num);
        enemy.virtualStatus.maxAtk += val;
        val = Random.Range(0, num);
        enemy.virtualStatus.minAtk += val;
        val = Random.Range(0, num);//랜덤 넥스트? 그런거 있던거같음
        enemy.virtualStatus.def += val;
    }
    void EnemyDead()
    {
        enemiesAlive--;
       
        if(enemiesAlive <= 0)
        {
            skipButton.SetActive(true);

            restTime = Time.time + 20;
            GameManager.inst.InBattle = false;
//            tempTime = Time.time;
        }
    }
    void Spawn()
    {
        //        float delay = 1f;
        //       float timer = 0f;
        //특정애니메이션 같은건 아직 생각없음
        //여기서 랜덤한 위치를 구하게함
        float x;
        if (enemiesToSpawn % 2 == 0)
            x = Random.Range(x1Min, x1Max);
        else
            x = Random.Range(x2Min, x2Max);
        
        float y = Random.Range(yMin,yMax);
        int cur = Random.Range(0, Mathf.Min(currentWaveNumber / 3,enemys.Length));//레벨/3 or 적의 최대수중 낮은값을고름 이렇게하면 enemys가 부족해서 오류터질일이 없음

        Enemy spawnEnemy = Instantiate(enemys[cur],new Vector3(x,0,y)+ Vector3.up,Quaternion.identity) as Enemy;
        EnemyPowerUp(spawnEnemy, currentWaveNumber );
        spawnEnemy.Death += EnemyDead;
        //yield return null; //ㅁㄹ


    }
    // Use this for initialization
    void Start () {
        Random.InitState((int)System.DateTime.Now.Ticks);
        isEnabled = true;
        if(skipButton == null)
        skipButton = GameObject.Find("Canvas").transform.Find("Skip").gameObject;
        restTime = Time.time + 20;
        GameManager.inst.InBattle = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (enemiesToSpawn > 0 && Time.time > waveSpawnTime)
            {
                waveSpawnTime = Time.time + currentWave.nextSpawnTime;
                enemiesToSpawn--;
                //      StartCoroutine(Spawn());
                Spawn();
                //웨이브스폰타임을 늘림으로써 일정간격을할것
            }
            else if (restTime <= Time.time && enemiesAlive <= 0)
            {
                skipButton.SetActive(false);

                GameManager.inst.waveStartText.text = currentWaveNumber + 1 + " 웨이브 시작!";

                GameManager.inst.AnimationWaveStart();
                WaveSet();
            }


        }
    }
}
