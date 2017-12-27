using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunny_TK.Utils
{

    public class AnimatorBlend : MonoBehaviour
    {

        public Animator animator;

        private bool isRunning = false;
        [SerializeField]
        private int index = 0;
        private float t = 0f;

        [SerializeField]
        bool run = false;
        public bool IsRunning
        {
            get { return IsRunning; }
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (run)
            {
                isRunning = true;
                if (t < 1f)
                {
                    t += Time.deltaTime;
                    animator.SetFloat("Blend", Mathf.PingPong((1f - index) + t, 1f));
                }
                else
                {
                    animator.SetFloat("Blend", Mathf.PingPong(index, 1f));
                    isRunning = false;
                    run = false;
                    t = 0f;
                }
            }
        }

        //IEnumerator AnimationRun(float index)
        //{
        //    isRunning = true;
        //    while (t < 1f)
        //    {
        //        yield return new WaitForEndOfFrame();
        //        t += Time.unscaledDeltaTime;
        //        animator.SetFloat("Blend", Mathf.PingPong((1f - index) + t, 1f));
        //    }
        //    animator.SetFloat("Blend", Mathf.PingPong(index, 1f));
        //    isRunning = false;
        //}

    }
}

