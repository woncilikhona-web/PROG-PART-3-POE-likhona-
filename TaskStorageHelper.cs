using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CyberLikh
{
    public class TaskStorageHelper
    {
        private readonly string _filePath = "tasks.json";

        public List<CyberTask> LoadTasks()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<CyberTask>();

                string json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<CyberTask>();

                return JsonConvert.DeserializeObject<List<CyberTask>>(json) ?? new List<CyberTask>();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading tasks: " + ex.Message);
                return new List<CyberTask>();
            }
        }

        public void SaveTasks(List<CyberTask> tasks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error saving tasks: " + ex.Message);
            }
        }
    }
}