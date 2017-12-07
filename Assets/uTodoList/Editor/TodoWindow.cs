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
    /// Todo管理のウィンドウ
    /// </summary>
    public partial class TodoWindow : EditorWindow
    {
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

            if (Event.current.keyCode == KeyCode.Escape)
            {
                GUI.FocusControl("");
                this.Repaint();
            }

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            GUILayout.Space(2f);

            this.ShowTodoSelectionGUI();
            this.ShowTaskList();
            this.ShowDeleteGUI();
            this.ShowExportGUI();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 完了タスクの削除ボタン
        /// </summary>
        private void TodoRemoveButton()
        {
            // 完了タスクの削除ボタン
            if (CustomUI.Button("完了タスク 削除", new Color(1f, 0.25f, 0.1f)))
            {
                this.currentData.Tasks.RemoveAll(t => t.IsDone);
            }
        }

        /// <summary>
        /// Todo選択GUI
        /// </summary>
        private void ShowTodoSelectionGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            // EditorGUILayout.LabelField("TodoData", GUILayout.Width(LabelWidth + 16f));
            EditorGUILayout.LabelField("TodoData", GUILayout.Width(60f));

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
        /// タスク一覧の表示
        /// </summary>
        public void ShowTaskList()
        {
            this.taskList.DoLayoutList();
            GUILayout.Space(-this.taskList.footerHeight);
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
                .Where(data => data != null)
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
            this.taskList = CreateReorderableList(this.currentData, Repaint, (task) => true);
        }

        /// <summary>
        /// ReorderableListの作成
        /// </summary>
        static private ReorderableList CreateReorderableList(TodoData data, System.Action repaintAction, System.Func<TodoData.TaskData, bool> showElementCallback)
        {
            var list = new ReorderableList(data.Tasks, typeof(string));

            list.displayRemove = false;

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

            list.onRemoveCallback = (l) =>
            {
                Debug.LogFormat("Remove: {0}", data.Tasks[l.index].Text);
            };

            // テキストの行数を取得
            System.Func<int, int> textCount = (index) =>
            {
                return data.Tasks[index].Text.Count(c => c == '\n') + 1;
            };

            // 要素の高さ
            list.elementHeightCallback = (index) =>
            {
                if (index >= data.Tasks.Count) { return 0f; }
                if (!showElementCallback(data.Tasks[index])) { return 0; }

                int count = textCount(index);
                return list.elementHeight + (list.elementHeight - 8f) * (count - 1);
            };

            // 背景の描画
            var texture = DataLoader.LoadConfig().HighlightTexture;
            bool isDrawTexture = false;
            list.drawElementBackgroundCallback += (rect, index, isActive, isFocused) =>
            {
                if (index < 0 || index >= data.Tasks.Count) { return; }

                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, list.draggable);
                if (index == 0)
                {
                    isDrawTexture = false; // リセット
                }

                if (!showElementCallback(data.Tasks[index])) { return; }

                if (!isDrawTexture)
                {
                    GUI.DrawTexture(rect, texture);
                    isDrawTexture = true;
                }
            };

            list.onAddCallback = (l) =>
            {
                data.Tasks.Insert(0, new TodoData.TaskData("NEW"));
                EditorUtility.SetDirty(data);
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                if (index >= data.Tasks.Count) { return; }
                if (!showElementCallback(data.Tasks[index])) { return; }

                rect.y += 2;
                rect.height -= 5;

                var labelRect = new Rect(rect);
                labelRect.width = LabelWidth;

                var textRect = new Rect(rect);
                textRect.x += labelRect.width + 5;
                textRect.width -= labelRect.width;
                textRect.width -= 5;

                // EditorGUI.LabelField(labelRect, string.Format("Task {0}", index));

                EditorGUI.BeginChangeCheck();
                string text = EditorGUI.TextArea(textRect, data.Tasks[index].Text);

                data.Tasks[index].IsDone = EditorGUI.Toggle(labelRect, data.Tasks[index].IsDone);

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
        private void ShowExportGUI()
        {
            this.isOpenExports = CustomUI.Foldout("Export", this.isOpenExports);
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

        private void ShowDeleteGUI()
        {
            this.isOpenDelete = CustomUI.Foldout("Delete", this.isOpenDelete);
            if (this.isOpenDelete)
            {
                // 完了タスクの削除ボタン
                this.TodoRemoveButton();
            }
            GUILayout.Space(3f);
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