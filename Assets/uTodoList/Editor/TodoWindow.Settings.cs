///-----------------------------------
/// uTodoList
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEditor.Callbacks;
    using UnityEngine;

    public partial class TodoWindow : EditorWindow
    {
        /// <summary>
        /// Label領域の大きさ
        /// </summary>
        private const float LabelWidth = 18f;

        /// <summary>
        /// ボタンの大きさ
        /// </summary>
        private const float ButtonWidth = 46f;
    }
}