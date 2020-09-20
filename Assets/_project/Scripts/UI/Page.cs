using System;
using UnityEngine;

namespace DashTerritory
{
    public class Page : MonoBehaviour
    {
        public GameObject firstSelected;

        public GameObject[] elements;

        private void Awake()
        {
            HidePage();
        }

        public GameObject OpenPage()
        {
            foreach (var element in elements)
            {
                if (!element) continue;
                element.SetActive(true);
            }

            return firstSelected;
        }

        public void HidePage()
        {
            foreach (var element in elements)
            {
                if (!element) continue;
                element.SetActive(false);
            }
        }
    }
}
