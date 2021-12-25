using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    [SerializeField] TutorialManager _tutorialManager;
    [SerializeField] TutorialManager.TutorialType[] types;

    public void ShowTutorial()
    {
        _tutorialManager.SetTutorial(types).Forget();
    }
}
