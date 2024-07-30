using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    public class GraphicRendererGroup : MonoBehaviour
    {
        public float contrast = 1;

        void Awake()
        {
            _Init();
        }

        void Start()
        {
            _Init();
        }

        void Update()
        {
            _Update();
        }

        void _GetGraphs()
        {
            Graphic[] graphs = GetComponentsInChildren<Graphic>();

            for (int i = 0; i < graphs.Length; i++)
            {
                if (Contains(graphs[i]) == false)
                {
                    _graphs = ArrayHelpers.Add(_graphs, graphs[i]);
                    _color = ArrayHelpers.Add(_color, graphs[i].color);
                }
            }

            bool Contains(Graphic spriteRenderer)
            {
                for (int i = 0; i < _graphs.Length; i++)
                    if (_graphs[i] == spriteRenderer) return true;

                return false;
            }
        }

        void _Init()
        {
            _GetGraphs();

            for (int i = 0; i < _graphs.Length; i++)
            {
                if (_graphs[i] == null) continue;

                Color color = _color[i] * contrast;

                //color.r *= contrast;
                //color.g *= contrast;
                //color.b *= contrast;

                _graphs[i].color = color;
            }
        }

        void _Update()
        {
            _GetGraphs();

            for (int i = 0; i < _graphs.Length; i++)
            {
                if (_graphs[i] == null) continue;

                Color color = _color[i] * contrast;

                //color.r *= contrast;
                //color.g *= contrast;
                //color.b *= contrast;

                _graphs[i].color = color;
            }
        }

        Color[] _color = new Color[0];
        Graphic[] _graphs = new Graphic[0];
    }
}