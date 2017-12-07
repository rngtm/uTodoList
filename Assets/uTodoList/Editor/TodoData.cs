///-----------------------------------
/// uTodoList
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace uTodoList
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Todo通知表示用のデータ
    /// </summary>
    [SerializableAttribute]
    public class TodoData : ScriptableObject
    {
        [SerializeField] private List<TaskData> tasks = new List<TaskData>();

        /// <summary>
        /// Todoテキスト
        /// </summary>
        public List<TaskData> Tasks { get { return this.tasks; } }

        [System.SerializableAttribute]
        public class TaskData
        {
            public bool IsDone = false;
            public string Text = string.Empty;

            public TaskData(string text = "")
            {
                this.Text = text;
            }
        }
    }
}
