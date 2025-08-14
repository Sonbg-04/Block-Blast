using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BlockBlast
{
    public class LoadingScene : MonoBehaviour
    {
        public Slider loading;

        void Start()
        {
            StartCoroutine(LoadRunning());
        }
        IEnumerator LoadRunning()
        {
            loading.value = 0;
            AsyncOperation async = SceneManager.LoadSceneAsync(Const.MAIN_MENU_SCENE);
            async.allowSceneActivation = false;

            float targetvalue = 0;

            while (async.progress < 0.9f)
            {
                targetvalue = async.progress;

                while (loading.value < targetvalue)
                {
                    loading.value += Time.deltaTime * 0.3f;
                    yield return null;
                }
            }    

            while (loading.value < 1f)
            {
                loading.value += Time.deltaTime * 0.5f;
                yield return null;
            }    

            yield return new WaitForSeconds(0.5f);
            async.allowSceneActivation = true;

        }    
    }
}
