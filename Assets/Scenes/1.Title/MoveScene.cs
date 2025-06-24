using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title
{
    public class MoveScene : MonoBehaviour
    {
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
            SceneManager.LoadScene("Home");
        }
    }
}

