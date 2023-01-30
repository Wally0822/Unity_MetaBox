using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawner : ObjectPool<Thief>
{
    [SerializeField] Thief thiefPref = null;

    List<ThiefData> ThiefDatas = null;
    List<int> wantedlist = new List<int>();

    public override Thief CreatePool()
    {
        if (thiefPref == null) thiefPref = Resources.Load<Thief>(nameof(Thief));
        return thiefPref;
    }

    private void Awake()
    {
        GameManager.Instance.spawnThief = Spawn;
        PoolInit();
    }
    
    public void Spawn(StageData stage)
    {
        ThiefDatas = DataManager.Instance.FindThiefDatasByThiefGroup(stage.thiefGroup);
        wantedlist.Clear();

        int wantedCount = stage.wantedCount;
        bool wanted = true;

        for (int i = 0; i < stage.thiefCount; i++)
        {
            if (wantedCount <= 0) wanted = false; 
            int random = Random.Range(0, ThiefDatas.Count);
            Thief thief = Get();
            thief.transform.position = new Vector3(Random.Range(-5f, 8f), Random.Range(-3f, 3f), 0);
            if (thief.transform.parent == null) thief.transform.parent = this.transform;
            thief.Setting(wanted, ThiefDatas[random].id, ThiefDatas[random].moveSpeed, ThiefDatas[random].moveTime);
            if (wanted)
            {
                wantedlist.Add(ThiefDatas[random].id);
            }
            thief.callbackArrest = Release;
            ThiefDatas.RemoveAt(random);
            wantedCount--;
        }
        UIManager.Instance.WantedListSetting(wantedlist);
    }
}
