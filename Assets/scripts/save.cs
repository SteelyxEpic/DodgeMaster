// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// save
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class save : MonoBehaviour
{

	public static save ins;

	public savedata activesave;

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
        load();
	}


	public void saves()
	{
		string persistentDataPath = Application.persistentDataPath;
		Debug.Log(persistentDataPath);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(savedata));
		FileStream fileStream = new FileStream(persistentDataPath + "/" + activesave.savename + ".save", FileMode.Create);
		xmlSerializer.Serialize(fileStream, activesave);
		fileStream.Close();
		Debug.Log("saved on " + Time.time);
	}

	public void load()
	{
		string persistentDataPath = Application.persistentDataPath;
		if (File.Exists(persistentDataPath + "/" + activesave.savename + ".save"))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(savedata));
			FileStream fileStream = new FileStream(persistentDataPath + "/" + activesave.savename + ".save", FileMode.Open);
			activesave = xmlSerializer.Deserialize(fileStream) as savedata;
			fileStream.Close();
            Debug.Log("done load");
		}
	}

	public void delete()
	{
		string persistentDataPath = Application.persistentDataPath;
		if (File.Exists(persistentDataPath + "/" + activesave.savename + ".save"))
		{
			File.Delete(persistentDataPath + "/" + activesave.savename + ".save");
			Debug.Log("delete");
		}
	}
}


[System.Serializable]
public class savedata
{
	public string savename = "save2";
    public float healthpoints = 100f;
    public float speed = 5f;
    public float speedmulti = 2f;
    public float jumpstrength = 5f;
    public float dmg = 1.5f;
	public float staminause = 15f;
	
public float dashSpeed = 20f;
public float dashCooldown = 0.5f;

}