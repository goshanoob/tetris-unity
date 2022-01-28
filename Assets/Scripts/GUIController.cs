using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel;
    void Update()
    {
        scoreLabel.text = Time.realtimeSinceStartup.ToString();
    }
    public void OnRestart()
    {
        Debug.Log("Restart Clicked");
    }

    public void OnFirstModeChanged()
    {
        Debug.Log("First Mode Changed");
    }

    public void OnSecondModeChanged()
    {
        Debug.Log("Second Mode Changed");
    }
}
