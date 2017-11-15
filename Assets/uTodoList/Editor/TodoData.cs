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
        #region Variables
        [SerializeField] private List<TaskData> tasks = new List<TaskData>();
        #endregion Variables

        #region Properties
        /// <summary>
        /// Todoテキスト
        /// </summary>
        public List<TaskData> Tasks { get { return this.tasks; } }
        #endregion Properties

        [System.SerializableAttribute]
        public class TaskData
        {
            public string Text = string.Empty;
        }
    }
}
