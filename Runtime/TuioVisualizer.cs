using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltraCombos
{
    public class TuioVisualizer : MonoBehaviour
    {
        [SerializeField]
        Texture2D texture;

        RectTransform template;
        Dictionary<int, RectTransform> inputs;

        [SerializeField, Header("Debug")]
        bool mode = false;

        private void Start()
        {
            var go = new GameObject("template");
            template = go.AddComponent<RectTransform>();
            template.SetParent(transform);
            var img = go.AddComponent<RawImage>();
            img.texture = texture;
            go.SetActive(false);

            inputs = new Dictionary<int, RectTransform>();
        }

        private void Update()
        {
            foreach (var id in inputs.Keys)
            {
                inputs[id].gameObject.SetActive(false);
            }

            if (mode == false)
                return;

            if (EventSystem.current == null || EventSystem.current.currentInputModule == null)
                return;

            BaseInput input = EventSystem.current.currentInputModule.input;

            if (input.GetMouseButton(0))
            {
                UpdateInput(-1, input.mousePosition);
            }

            for (int i = 0; i < input.touchCount; i++)
            {
                Touch t = input.GetTouch(i);
                UpdateInput(i, t.position);
            }
        }

        private void UpdateInput(int id, Vector2 position)
        {
            if (inputs.ContainsKey(id) == false)
            {
                var input = Instantiate(template, transform);
                input.name = $"Input - {id}";
                inputs.Add(id, input);
            }
            
            var rt = transform.root.GetComponentInChildren<Canvas>().transform as RectTransform;
            position = position / Camera.main.pixelRect.size * rt.sizeDelta;

            inputs[id].anchoredPosition = position;
            inputs[id].gameObject.SetActive(true);
        }
    }

}

