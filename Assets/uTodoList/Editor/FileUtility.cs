///-----------------------------------
/// uTodoList
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// ファイルのセーブなどを行うクラス
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// Todoデータを作成
        /// </summary>
        public static TodoData CreateTodoData()
        {
            var guid = AssetDatabase.FindAssets(TodoConfig.RootFolderName)[0];
            var rootDirectory = AssetDatabase.GUIDToAssetPath(guid);
            var directory = Path.Combine(rootDirectory, TodoConfig.SaveFolderRelativePath);
            if (string.IsNullOrEmpty(rootDirectory))
            {
                directory = "Assets";
            }

            var name = TodoConfig.DefaultNewTodoName + ".asset";
            var path = Path.Combine(directory, name);
            var instance = ScriptableObject.CreateInstance<TodoData>();
            ProjectWindowUtil.CreateAsset(instance, path);

            return instance;
        }
        
        /// <summary>
        /// Todoデータを強制的に作成
        /// </summary>
        public static TodoData CreateTodoDataImmediately()
        {
            var guid = AssetDatabase.FindAssets(TodoConfig.RootFolderName)[0];
            var rootDirectory = AssetDatabase.GUIDToAssetPath(guid);
            var directory = Path.Combine(rootDirectory, TodoConfig.SaveFolderRelativePath);
            if (string.IsNullOrEmpty(rootDirectory))
            {
                directory = "Assets";
            }

            var name = TodoConfig.DefaultNewTodoName + ".asset";
            var path = Path.Combine(directory, name);
            var instance = ScriptableObject.CreateInstance<TodoData>();
            AssetDatabase.CreateAsset(instance, path);
            Debug.Log("Create: " + path, instance);

            return instance;
        }

        /// <summary>
        /// タスクをテキストデータとして保存
        /// </summary>
        public static void SaveAsText(TodoData data)
        {
            DateTime dt = DateTime.Now;
            string defaultName = string.Empty;
            defaultName += data.name + "_";
            defaultName += dt.Year.ToString();
            defaultName += dt.Month.ToString();
            defaultName += dt.Day.ToString();
            defaultName += dt.Hour.ToString();
            defaultName += dt.Minute.ToString();
            defaultName += dt.Second.ToString();

            string directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(data));
            string fullpath = EditorUtility.SaveFilePanel("Todoリストの保存", directory, defaultName, "txt");
            if (fullpath.Length == 0) { return; }

            string path = "Assets" + fullpath.Substring(Application.dataPath.Length);
            string text = string.Empty;
            for (int i = 0; i < data.Tasks.Count; i++)
            {
                var task = data.Tasks[i];
                text += i.ToString() + ")\n";
                text += task.Text + "\n";
            }
            var asset = FileUtility.ExportText(text, path);
            Debug.Log("Create: " + asset.name + ".txt", asset);
        }

        /// <summary>
        ///　.txtファイル書き出し 
        /// </summary>
        private static UnityEngine.Object ExportText(string text, string path)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(Path.GetFullPath(path));
            sw = fi.CreateText();
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
            AssetDatabase.ImportAsset(path);

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
    }
}