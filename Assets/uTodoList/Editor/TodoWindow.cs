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

    /// <summary>
    /// Todoの管理を行うクラス
    /// </summary>
    public class TodoWindow : EditorWindow
    {
        /// <summary>
        /// Label領域の大きさ
        /// </summary>
        private const float LabelWidth = 52f;

        /// <summary>
        /// ボタンの大きさ
        /// </summary>
        private const float ButtonWidth = 46f;

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

        /// <summary>
        /// ExportのGUIを表示するかどうか
        /// </summary>
        [SerializeField] private bool isOpenExports = false;

        private static bool _needReloadData = false;

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/uTodoList")]
        static void Open()
        {
            var window = GetWindow<TodoWindow>();
            window.titleContent.text = "uTodoList";
        }

        /// <summary>
        /// アセットのロード時に呼ばれる
        /// </summary> 
        [DidReloadScripts]
        [InitializeOnLoadMethodAttribute]
        public static void OnLoadAssets()
        {
            _needReloadData = true;
        }

        /// <summary>
        /// Inspector描画処理
        /// </summary>
        private void OnGUI()
        {
            if (_needReloadData)
            {
                _needReloadData = false;
                this.ReloadDatas();
            }

            if (this.todoDatas == null)
            {
                this.ReloadDatas();
            }

            if (this.currentData == null)
            {
                this.ReloadDatas();
            }

            if (this.taskList == null)
            {
                this.RebuildList();
            }
            if (this.taskList == null) { return; }

            if (Event.current.keyCode == KeyCode.Escape)
            {
                GUI.FocusControl("");
                this.Repaint();
            }


            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            GUILayout.Space(2f);

            this.TodoSelectionGUI();

            // リストの表示
            this.taskList.DoLayoutList();
            GUILayout.Space(-this.taskList.footerHeight);


            GUILayout.Space(3f);

            // エクスポートGUIの表示
            this.ExportGUI();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Todo選択GUI
        /// </summary>
        private void TodoSelectionGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("TodoData", GUILayout.Width(LabelWidth + 16f));

            var popupList = this.todoDataNames.Concat(new[] { "", "New..." }).ToArray();
            int index = EditorGUILayout.Popup(this.currentTodoDataIndex, popupList);
            if (EditorGUI.EndChangeCheck())
            {
                if (index < this.todoDatas.Length)
                {
                    this.SelectDataByIndex(index);
                }
                else
                {
                    // Todoデータ新規作成
                    var newData = FileUtility.CreateTodoData();
                    this.ReloadDatas();
                    this.SelectDataByData(newData);
                }
            }

            if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(ButtonWidth)))
            {
                EditorGUIUtility.PingObject(this.currentData);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Todoを選択
        /// </summary>
        private void SelectDataByData(TodoData data)
        {
            var match = this.todoDatas
            .Select((d, i) => new { Data = d, Index = i })
            .FirstOrDefault(item => item.Data.name == data.name);

            if (match != null)
            {
                SelectDataByIndex(match.Index);
            }
        }

        /// <summary>
        /// Todoを選択
        /// </summary>
        private void SelectDataByIndex(int dataIndex)
        {
            this.currentTodoDataIndex = dataIndex;
            EditorUtility.SetDirty(this.currentData);
            this.currentData = this.todoDatas[this.currentTodoDataIndex];
            this.currentDataName = this.currentData.name;
            this.RebuildList();
        }

        /// <summary>
        /// Todoデータの再読み見込み
        /// </summary>
        private void ReloadDatas()
        {
            var datas = DataLoader.LoadDatas().Where(data => data != null).ToArray();
            this.todoDatas = datas;

            if (datas == null || datas.Length == 0)
            {
                Debug.LogError("data not found");
                FileUtility.CreateTodoDataImmediately();
                return;
            }

            this.todoDataNames = datas.Select(data => data.name).ToArray();

            if (this.currentData == null)
            {
                this.currentTodoDataIndex = 0;
                var defaultData = DataLoader.LoadConfig().DefaultTodoData ?? DataLoader.LoadDatas().FirstOrDefault();

                var selectData = datas
                .Select((data, i) => new { Index = i, Data = data })
                .FirstOrDefault(item => item.Data.name == defaultData.name);

                if (selectData == null || selectData.Data == null)
                {
                    this.currentTodoDataIndex = 0;
                }
                else
                {
                    this.currentTodoDataIndex = selectData.Index;
                }
            }
            else
            {
                var selectData = datas
                .Select((data, i) => new { Index = i, Data = data })
                .FirstOrDefault(item => item.Data.name == this.currentDataName);

                if (selectData == null || selectData.Data == null)
                {
                    this.currentTodoDataIndex = 0;
                }
                else
                {
                    this.currentTodoDataIndex = selectData.Index;
                }
            }

            this.currentData = this.todoDatas[this.currentTodoDataIndex];
            this.currentDataName = this.currentData.name;
            this.RebuildList();
        }

        /// <summary>
        /// ReorderableListの再作成
        /// </summary>
        private void RebuildList()
        {
            this.taskList = CreateReorderableList(this.currentData, Repaint);
        }

        /// <summary>
        /// ReorderableListの作成
        /// </summary>
        static ReorderableList CreateReorderableList(TodoData data, System.Action repaintAction)
        {
            // テキストデータ
            var list = new ReorderableList(data.Tasks, typeof(string));

            // ヘッダ
            Rect headerRect = default(Rect);
            list.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Tasks");
                headerRect = rect;
            };

            // フッター描画
            list.drawFooterCallback = (rect) =>
            {
                rect.y = headerRect.y + 3;
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            // テキストの行数を取得
            System.Func<int, int> textCount = (index) =>
            {
                return data.Tasks[index].Text.Count(c => c == '\n') + 1;
            };

            // 要素の高さ
            list.elementHeightCallback = (index) =>
            {
                int count = textCount(index);
                return list.elementHeight + (list.elementHeight - 8f) * (count - 1);
            };

            // 背景の描画
            var texture = DataLoader.LoadConfig().HighlightTexture;
            list.drawElementBackgroundCallback += (rect, index, isActive, isFocused) =>
            {
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, list.draggable);
                if (index == 0)
                {
                    GUI.DrawTexture(rect, texture);
                }
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                rect.height -= 5;

                var labelRect = new Rect(rect);
                labelRect.width = LabelWidth;

                var textRect = new Rect(rect);
                textRect.x += labelRect.width + 5;
                textRect.width -= labelRect.width;
                textRect.width -= 5;

                EditorGUI.LabelField(labelRect, string.Format("Task {0}", index));

                EditorGUI.BeginChangeCheck();
                string text = EditorGUI.TextArea(textRect, data.Tasks[index].Text);

                if (EditorGUI.EndChangeCheck())
                {
                    data.Tasks[index].Text = text;
                    EditorUtility.SetDirty(data);
                }
            };

            return list;
        }

        /// <summary>
        /// エクスポート用のGUIの表示
        /// </summary>
        private void ExportGUI()
        {
            this.isOpenExports = Foldout("Export", this.isOpenExports);
            if (this.isOpenExports)
            {
                GUILayout.Space(-3f);

                EditorGUI.indentLevel++;
                this.Label("Todoリストをエクスポートします");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20f);
                if (GUILayout.Button(".txt"))
                {
                    FileUtility.SaveAsText(this.currentData);
                }
                GUILayout.Space(12f);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;

                GUILayout.Space(1f);
            }
        }

        /// <summary>
        /// 折り畳みをかっこよく表示
        /// </summary>
        private static bool Foldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }

        /// <summary>
        /// 区切り線を表示
        /// </summary>
        private void Line(float size)
        {
            float indent = 8f;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent);
            GUILayout.Box("", GUILayout.Width(this.position.width - indent * 2f), GUILayout.Height(size));
            EditorGUILayout.EndHorizontal();
        }

        private void Label(string label)
        {
            Line(1f);
            EditorGUILayout.LabelField(label);
            Line(1f);
        }
    }
}