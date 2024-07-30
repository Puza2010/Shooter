using System;
using System.Collections.Generic;
using System.Linq;

namespace Playniax.Ignition
{
    public class EngineBehaviour : IgnitionBehaviour
    {
        public static Action postInit;

        public int order;

        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public override void Awake()
        {
            base.Awake();

            _Init();
            _PostInit();

        }

        protected void Update()
        {
            _Update();
        }

        void _Init()
        {
            _Sort();

            if (_main == this)
            {
                for (int i = 0; i < _list.Count(); i++)
                    if (_list.ElementAt(i).enabled) _list.ElementAt(i).OnStart();
            }
        }

        void _PostInit()
        {
            if (postInit != null)
            {
                postInit.Invoke();

                postInit = null;
            }
        }

        void _Sort()
        {
            if (_main == null && enabled) _main = this;

            if (_main == this && enabled)
            {
                _list = FindObjectsOfType<EngineBehaviour>().OfType<EngineBehaviour>();

                _list = _list.OrderBy(w => w.order);
            }
        }

        void _Update()
        {
            _Sort();

            if (_main == this)
            {
                for (int i = 0; i < _list.Count(); i++)
                    if (_list.ElementAt(i).enabled) _list.ElementAt(i).OnUpdate();
            }
        }

        static IEnumerable<EngineBehaviour> _list;
        static EngineBehaviour _main;
    }
}