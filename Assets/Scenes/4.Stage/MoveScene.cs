using UnityEngine;
using UnityEngine.SceneManagement;

namespace StageSelect
{
    public class MoveScene : MonoBehaviour
    {
        public string SceneName;
        //// Start is called once before the first execution of Update after the MonoBehaviour is created
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        public void ChangeScene()
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}

