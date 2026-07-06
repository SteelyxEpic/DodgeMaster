using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class UIStuff : MonoBehaviour
{
    public static UIStuff ins;
    public Slider energybar;
    public GameObject cooldown;
    public GameObject cooldownprefab;
    public GameObject comboDisplay;
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
    public void comboDisplayTrigger(string text) {
        comboDisplay.SetActive(true);   
        comboDisplay.GetComponent<Animator>().SetTrigger("combo");
        comboDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }
    public IEnumerator Cooldown(float time) {
        GameObject cd = Instantiate(cooldownprefab, cooldown.transform);
        cd.GetComponent<Animator>().SetFloat("Speed", 1/time);
        yield return new WaitForSeconds(time);
        Destroy(cd);
    }
}
