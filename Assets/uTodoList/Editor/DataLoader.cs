///-----------------------------------
/// uTodoList
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using UnityEditor;
    using System.Linq;

    /// <summary>
    /// データのロードを行うクラス
    /// </summary>
    public class DataLoader : AssetPostprocessor
    {
        /// <summary>
        /// アセットのインポート完了時に呼ばれる
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            TodoWindow.OnLoadAssets();
        }

        /// <summary>
        /// Todoデータのロード
        /// </summary>
        public static TodoData[] LoadDatas()
        {
            return (TodoData[])AssetDatabase.FindAssets("t:ScriptableObject")
           .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
           .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(TodoData)))
           .Where(obj => obj != null)
           .Where(obj => obj.GetType() == typeof(TodoData))
           .Select(obj => (TodoData)obj)
           .ToArray();
        }

        /// <summary>
        /// 設定データの読み込み
        /// </summary>
        public static TodoConfig LoadConfig()
        {
            return AssetDatabase.FindAssets("t:ScriptableObject")
           .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
           .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(TodoConfig)))
           .Where(obj => obj != null)
           .Where(obj => obj.GetType() == typeof(TodoConfig))
           .Select(obj => (TodoConfig)obj)
           .FirstOrDefault();
        }
    }
}
