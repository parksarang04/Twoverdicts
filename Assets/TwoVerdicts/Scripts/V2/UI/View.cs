using TwoVerdicts.V2.Core;
using UnityEngine;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 모든 창의 공통 부모. 열려 있는 동안 플레이어 이동/조사를 막음.
    /// </summary>
    public abstract class View : MonoBehaviour
    {
        protected bool IsOpen { get; private set; }
        protected int OpenedFrame { get; private set; }

        protected static T CreateView<T>(Transform parent, string name) where T : View
        {
            RectTransform root = Ui.Stretch(name, parent);
            var view = root.gameObject.AddComponent<T>();
            view.Build();
            root.gameObject.SetActive(false);
            return view;
        }

        protected abstract void Build();

        protected void OpenView()
        {
            transform.SetAsLastSibling();
            OpenedFrame = Time.frameCount;
            if (IsOpen) return;
            IsOpen = true;
            gameObject.SetActive(true);
            InputLock.Push();
        }

        protected void CloseView()
        {
            if (!IsOpen) return;
            IsOpen = false;
            gameObject.SetActive(false);
            InputLock.Pop();
        }

        protected bool KeyReady => IsOpen && Time.frameCount > OpenedFrame;
    }
}
