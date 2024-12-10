using UnityEngine;

namespace ApplicationScripts.Manager
{
    public class Application_Manager : MonoBehaviour
    {


        void Awake()
        {
            Debug.Log($"FixedDeltaTime set to {Time.fixedDeltaTime} seconds.");
            Debug.Log($"Application.persistentDataPath = {Application.persistentDataPath} (Recorded Reference Paths Stored Location)");
        }


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnQuit()
        { 
            Application.Quit();
        }

    }
}
