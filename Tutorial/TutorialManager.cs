using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;

public class TutorialManager:MonoBehaviour
{
    [System.Serializable]
    public enum TutorialType
    {
        Move,
        Attack,
        Item,
        Guage,
        Turn,
        Tutorial,
        Skill,
        Guage2,
        Attack2,
        Ailment1,
        Ailment2,
        Ailment3
    }

    [System.Serializable]
    public struct TutorialTexts
    {
        public TutorialType _type;
        public string _title;
        [MultiLineProperty(3)]
        public string[] _texts;
    }
    [SerializeField]GameObject _tutorialPanel,_tutorialButtons,_backButton;
    [SerializeField]TextMeshProUGUI _tutorialText,_tutorialTitle;
    [SerializeField]WeaponModel _tutorialWeapon;
    [SerializeField]ArmorModel _tutorialArmor;
    [SerializeField]SkillModel[] _skills;

    /// <summary>
    /// �`���[�g���A���p�̕���
    /// </summary>
    public WeaponModel GetWeapon()
    {
        return _tutorialWeapon;
    }
    /// <summary>
    /// �`���[�g���A���p�̏�
    /// </summary>
    public ArmorModel GetArmor()
    {
        return _tutorialArmor;
    }
    /// <summary>
    /// �`���[�g���A���p�̃X�L��
    /// </summary>
    public SkillModel[] GetSkills()
    {
        return _skills;
    }
    [SerializeField] TutorialTexts[] _tutorialTtexts;


    public void OnTutorialButtons()
    {
        _tutorialButtons.SetActive(true);
    }
    /// <summary>
    /// �`���[�g���A�����J�n����B
    /// </summary>
    public void StartTutorial()
    {
        //�`���[�g���A���̏���
        TutorialType[][] types = new TutorialType[][]
        {
            new TutorialType[] {TutorialType.Move,TutorialType.Item},
            new TutorialType[] {TutorialType.Attack,TutorialType.Attack2, TutorialType.Guage, TutorialType.Guage2, TutorialType.Turn },
            new TutorialType[] {TutorialType.Skill },
        };
        GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Subscribe(_ => SetTutorial(types[_ - 1]).Forget());
    }
    bool _wait = true;
    public int index = 0;

    /// <summary>
    ///�@�`���[�g���A����\������B
    /// </summary>
    async public UniTask SetTutorial(params TutorialType[] types)
    {
        _tutorialPanel.SetActive(true);
        //���[�v�����Ńy�[�W��i�߂�B
        for(int i=0; i< types.Length; i++)
        {
            index = 0;
            _backButton.SetActive(i != 0);
            //�`���[�g���A���̃e�L�X�g���擾
            var tutorial = _tutorialTtexts.FirstOrDefault(x => x._type == types[i]);
            //�e�L�X�g������
            WriteText(tutorial._title, tutorial._texts);
            _wait = true;
            while (_wait)
            {
                await UniTask.Yield();
            }
            i -= index;
        }
        _tutorialPanel.SetActive(false);
    }

    void WriteText(string title,params string[]texts)
    {
        _tutorialTitle.text = title;
        _tutorialText.text = "";
        StringBuilder sb = new StringBuilder();
        foreach (var t in texts)
        {
            sb.AppendLine(t);
            sb.AppendLine("");
        }
        _tutorialText.text = sb.ToString();
    }
    //�P�y�[�W�߂�
    public void OnBack()
    {
        index = 2;
        _wait = false;
    }
    //�P�y�[�W�i�߂�
    public void OnNext()
    {
        _wait = false;
    }
}
