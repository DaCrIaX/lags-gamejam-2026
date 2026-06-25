using UnityEngine;

namespace UnityEngine.SceneManagement
{
    public class QuitGame : MonoBehaviour
    {
       public void Quit()
		{
			Debug.Log ("Quit");
			Application.Quit();
		}
    }
}
