using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIStuff : MonoBehaviour
{
    public static UIStuff ins;
    public Slider energybar;
    public GameObject cooldown;
    public GameObject cooldownprefab;
    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energybar.value = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator Cooldown(float time) {
        GameObject cd = Instantiate(cooldownprefab, cooldown.transform);
        cd.GetComponent<Animator>().SetFloat("Speed", 1/time);
        yield return new WaitForSeconds(time);
        Destroy(cd);
    }
}
