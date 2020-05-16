using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class TextAction : FSMAction
    {
        private string textToShow;
        private float duration;
        private float cachedDuration;
        private string finishEvent;

        public TextAction (FSMState owner, Character aiController) : base(owner, aiController)
        {

        }
        public void Init(string textToShow, float duration, string finishEvent)
        {
            this.textToShow = textToShow;
            this.duration = duration;
            this.cachedDuration = duration;
            this.finishEvent = finishEvent;
            Debug.Log(textToShow);
        }

        public override void OnEnter()
        {
            if(duration <= 0)
            {
                Finish();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            duration = Time.deltaTime;
            if(duration <= 0)
            {
                Finish();
                return;
            }
        }
        public void Finish()
        {
            if(!string.IsNullOrEmpty(finishEvent))
            {
                GetOwner().SendEvent(finishEvent);
            }
            //Debug.Log(textToShow);
            duration = cachedDuration;
        }
    }
}

