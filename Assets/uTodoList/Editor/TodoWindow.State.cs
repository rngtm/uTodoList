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
        /// 現在選択中のTodoのインデックス
        /// </summary>
        [SerializeField] private int currentTodoDataIndex = 0;

        /// <summary>
        /// 現在選択中のTodo
        /// </summary>
        [SerializeField] private TodoData currentData = null;

        /// <summary>
        /// 現在選択中のTodoの名前
        /// </summary>
        [SerializeField] private string currentDataName = string.Empty;

        /// <summary>
        /// Todoデータ一覧
        /// </summary>
        private TodoData[] todoDatas;

        /// <summary>
        /// Todoデータ名
        /// </summary>
        private string[] todoDataNames;

        /// <summary>
        /// タスク表示用のReorderableList
        /// </summary>
        private ReorderableList taskList;

        /// <summary>
        /// スクロール位置
        /// </summary>
        private Vector2 scrollPosition = new Vector2(0f, 0f);

        [SerializeField] private bool isOpenDelete = false;

        /// <summary>
        /// ExportのGUIを表示するかどうか
        /// </summary>
        [SerializeField] private bool isOpenExports = false;

        private static bool _needReloadData = false;
    }
}