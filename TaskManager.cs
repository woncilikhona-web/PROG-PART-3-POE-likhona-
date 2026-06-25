using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberLikh
{
    public class TaskManager
    {
        private readonly TaskStorageHelper _storage;
        private List<CyberTask> _tasks;

        public TaskManager()
        {
            _storage = new TaskStorageHelper();
            _tasks = _storage.LoadTasks();
        }

        public string AddTask(string title, string description, string reminder)
        {
            int nextId = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;

            var newTask = new CyberTask
            {
                Id = nextId,
                Title = title,
                Description = description,
                Reminder = reminder,
                IsComplete = false,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            _tasks.Add(newTask);
            _storage.SaveTasks(_tasks);

            return $"Task added: '{title}'";
        }

        public List<CyberTask> GetAllTasks()
        {
            return _tasks;
        }

        public void MarkAsComplete(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsComplete = true;
                _storage.SaveTasks(_tasks);
            }
        }

        public void DeleteTask(int id)
        {
            _tasks.RemoveAll(t => t.Id == id);
            _storage.SaveTasks(_tasks);
        }
    }
}
