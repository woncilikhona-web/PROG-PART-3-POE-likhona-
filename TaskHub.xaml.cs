using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberLikh
{
    public partial class TaskHub : Window
    {
        private readonly TaskManager _taskManager;

        public TaskHub()
        {
            InitializeComponent();
            _taskManager = new TaskManager();
            RefreshTaskList();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleInput.Text.Trim();
            string description = DescriptionInput.Text.Trim();
            string reminder = ReminderInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            _taskManager.AddTask(title, description, reminder);

            TitleInput.Text = "";
            DescriptionInput.Text = "";
            ReminderInput.Text = "";

            RefreshTaskList();
        }

        private void RefreshTaskList()
        {
            TaskListContainer.Items.Clear();
            var tasks = _taskManager.GetAllTasks();

            foreach (var task in tasks)
            {
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(69, 10, 10)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                    BorderThickness = new Thickness(0, 0, 0, 3),
                    Padding = new Thickness(14),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = (task.IsComplete ? "✔ " : "▢ ") + task.Title,
                    Foreground = Brushes.White,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextDecorations = task.IsComplete ? TextDecorations.Strikethrough : null
                });

                if (!string.IsNullOrWhiteSpace(task.Description))
                    stack.Children.Add(new TextBlock
                    {
                        Text = task.Description,
                        Foreground = new SolidColorBrush(Color.FromRgb(252, 165, 165)),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 4, 0, 0)
                    });

                if (!string.IsNullOrWhiteSpace(task.Reminder))
                    stack.Children.Add(new TextBlock
                    {
                        Text = "⏰ " + task.Reminder,
                        Foreground = new SolidColorBrush(Color.FromRgb(248, 113, 113)),
                        FontSize = 12,
                        Margin = new Thickness(0, 4, 0, 0)
                    });

                var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

                if (!task.IsComplete)
                {
                    var completeBtn = new Button
                    {
                        Content = "MARK DONE",
                        Padding = new Thickness(10, 4, 10, 4),
                        Margin = new Thickness(0, 0, 8, 0),
                        Background = new SolidColorBrush(Color.FromRgb(34, 139, 34)),
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(0),
                        Cursor = System.Windows.Input.Cursors.Hand
                    };
                    completeBtn.Click += (s, e) => { _taskManager.MarkAsComplete(task.Id); RefreshTaskList(); };
                    btnRow.Children.Add(completeBtn);
                }

                var deleteBtn = new Button
                {
                    Content = "DELETE",
                    Padding = new Thickness(10, 4, 10, 4),
                    Background = new SolidColorBrush(Color.FromRgb(120, 0, 0)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                deleteBtn.Click += (s, e) => { _taskManager.DeleteTask(task.Id); RefreshTaskList(); };
                btnRow.Children.Add(deleteBtn);

                stack.Children.Add(btnRow);
                card.Child = stack;
                TaskListContainer.Items.Add(card);
            }
        }
    }
}