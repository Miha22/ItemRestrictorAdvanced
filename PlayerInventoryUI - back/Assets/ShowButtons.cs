using UnityEngine;
using UnityEngine.UI;

public class ShowButtons : MonoBehaviour {
    // Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowChildButtons()
	{
        //GameObject[] objects = FindObjectsOfType<GameObject>(); //finds all (active/inactive) objects
        //foreach (var obj in objects)
        //{
        //    Debug.Log(obj.name);
        //}
        //Debug.Log("-------------"); 
        if(gameObject.GetComponent<Text>().text != "+")
        {
            foreach (Transform child in transform) //finds child (active/inactive) objects
                child.gameObject.SetActive((child.gameObject.active == true) ? false : true);
        }
        else
        {
            Debug.Log("GameObjects:");
            foreach (Transform child in transform.parent.parent)
            {
                if (child.gameObject.name == "AddItem")
                    child.gameObject.SetActive(true);
            }

        }
    }
    public void ReadTextOnClick()
    {
        Text textObj = gameObject.GetComponent<Text>();
        Debug.Log(textObj.text);
    }
    public void SetBlue()
    {
        Image image = gameObject.GetComponentInParent<Image>();
    }
}
