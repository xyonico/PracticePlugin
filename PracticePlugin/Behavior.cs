using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace PracticePlugin
{
    public class Behavior : MonoBehaviour
    {

        void Update()
        {
            if (Plugin._uiElementsCreator == null || UIElementsCreator.SongSeeker == null) return;
            UIElementsCreator.SongSeeker.OnUpdate();
        }
    }
}
