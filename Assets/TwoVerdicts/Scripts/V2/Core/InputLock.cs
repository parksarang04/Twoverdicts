using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 창(대화, 인터뷰 등)이 열려 있으면 이동/조사 입력을 막음.
    /// 창이 닫힌 그 프레임의 키 입력이 바로 조사로 이어지지 않게 한 프레임 쉰다.
    /// </summary>
    public static class InputLock
    {
        private static int count;
        private static int changedFrame;

        public static bool IsFree => count == 0 && Time.frameCount > changedFrame;

        public static void Push()
        {
            count++;
            changedFrame = Time.frameCount;
        }

        public static void Pop()
        {
            count = Mathf.Max(0, count - 1);
            changedFrame = Time.frameCount;
        }

        public static void Reset()
        {
            count = 0;
            changedFrame = Time.frameCount;
        }

        public static bool Pressed(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].wasPressedThisFrame;
        }

        public static bool Held(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].isPressed;
        }

        public static bool AdvancePressed()
        {
            return Pressed(Key.E) || Pressed(Key.Space) || Pressed(Key.Enter);
        }
    }
}
