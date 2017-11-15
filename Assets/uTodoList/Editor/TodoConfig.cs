///-----------------------------------
/// uTodoList
/// @ 2017 RNGTM(https://github.com/rngtm)
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

        [SerializeField, Header("ウィンドウを開いた時に選択されるデータ")] private TodoData defaultTodoData;
        [SerializeField, Header("ハイライトテクスチャ")] private Texture highlightTexture;
        [SerializeField, Header("ハイライトカラー")] private Color highlightTextColor;

        [SerializeField, Header("ヘッダー")] private Texture headerTexture;

        public TodoData DefaultTodoData { get { return this.defaultTodoData; } }
        public Texture HighlightTexture { get { return this.highlightTexture; } }
        public Color HighlightTextColor { get { return this.highlightTextColor; } }
        public Texture HeaderTexture { get { return this.headerTexture; } }
    }
}
