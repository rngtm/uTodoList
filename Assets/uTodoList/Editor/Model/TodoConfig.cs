///-----------------------------------
/// uTodoList
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using UnityEngine;

    /// <summary>
    /// 設定
    /// </summary>
    public class TodoConfig : ScriptableObject
    {
        /// <summary>
        /// uTodoListのルートフォルダ名
        /// </summary>
        public const string RootFolderName = "uTodoList";

        /// <summary>
        /// Todoデータの保存先の相対パス
        /// </summary>
        public const string SaveFolderRelativePath = "Data/TodoData";

        /// <summary>
        /// 新規Todoデータのデフォルトの名前
        /// </summary>
        public const string DefaultNewTodoName = "NewTodoData";

        [Header("ウィンドウを開いた時に選択されるデータ")]
        [SerializeField] private TodoData defaultTodoData;

        /// <summary>
        /// ウィンドウを開いた時に選択されるデータ
        /// </summary>
        public TodoData DefaultTodoData  { get { return this.defaultTodoData; } }

    }
}
