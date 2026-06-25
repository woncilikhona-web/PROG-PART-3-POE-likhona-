using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CyberLikh
{
    // ── Response engine (OOP separated) ─────────────────────────────────────
    public class RedAppResponseEngine
    {
        private readonly Random _random = new Random();

        private readonly Dictionary<string, List<string>> _randomResponses = new Dictionary<string, List<string>>
        {
            ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Always hover over links before clicking — the real URL often reveals a fake domain.",
                "Phishing emails create urgency like 'Your account will be closed!' — pause and verify before acting.",
                "Check the sender's email address carefully. Attackers use addresses like 'support@paypa1.com' to trick you.",
                "When in doubt, go directly to the official website instead of clicking any link in an email."
            },
            ["password"] = new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Avoid using personal details like birthdays or names.",
                "A strong password should be at least 12 characters with uppercase, lowercase, numbers, and symbols.",
                "Never reuse passwords across accounts — if one is stolen, all your accounts become vulnerable.",
                "Consider using a password manager like Bitwarden or 1Password to generate and store passwords safely.",
                "Enable Multi-Factor Authentication (MFA) on every account — it protects you even if your password is stolen."
            },
            ["scam"] = new List<string>
            {
                "Scams rely on urgency and panic. If someone pressures you to act fast — pause and verify first.",
                "Legitimate organisations will never ask for your PIN, password, or payment via gift cards.",
                "If something feels too good to be true, it almost always is. Trust your instincts.",
                "Romance scams and lottery scams are common. Never send money to someone you haven't met in person.",
                "Tech support scams are widespread — Microsoft and Apple will never call you unsolicited about your PC."
            },
            ["privacy"] = new List<string>
            {
                "Limit what personal information you share on social media — attackers harvest this data.",
                "Review app permissions regularly. Many apps collect far more data than they need.",
                "Use a VPN on public Wi-Fi to prevent eavesdropping on your data.",
                "Think carefully before posting anything online — once it's there, it's very hard to remove.",
                "Check your privacy settings on all platforms and restrict who can see your personal information."
            },
            ["malware"] = new List<string>
            {
                "Never download software from unknown or untrusted sources — it may contain hidden malware.",
                "Keep your antivirus software updated and run regular scans on your device.",
                "Ransomware can lock your files permanently. Back up your data regularly.",
                "Malware often arrives through email attachments — never open attachments from unknown senders.",
                "Keep your operating system updated — patches fix known vulnerabilities that malware exploits."
            },
            ["wifi"] = new List<string>
            {
                "Always use a VPN on public Wi-Fi — attackers can intercept your data on unsecured networks.",
                "Avoid logging into banking or sensitive accounts on public Wi-Fi networks.",
                "Make sure your home router uses WPA3 or WPA2 encryption with a strong unique password.",
                "Disable auto-connect on your device so it doesn't automatically join unknown networks.",
                "Use your phone's mobile hotspot instead of public Wi-Fi for sensitive tasks."
            },
            ["tip"] = new List<string>
            {
                "Always keep your software and operating system updated — patches fix security vulnerabilities.",
                "Use two-factor authentication on all important accounts for an extra layer of security.",
                "Back up your data regularly so you can recover from ransomware or hardware failure.",
                "Be cautious of unexpected emails, even from people you know — attackers spoof addresses.",
                "Log out of accounts on shared or public devices — don't leave sessions open.",
                "Check haveibeenpwned.com to see if your email has been in a known data breach.",
                "Use a password manager to generate and store strong unique passwords for every account."
            }
        };

        private readonly Dictionary<string, string> _sentimentResponses = new Dictionary<string, string>
        {
            ["worried"] = "It's completely understandable to feel worried. Cyber threats are real, but awareness is your strongest defence. ",
            ["scared"] = "Don't be scared — you're already doing the right thing by learning about this. ",
            ["nervous"] = "It's okay to feel nervous. Let me help you feel more confident about staying safe online. ",
            ["anxious"] = "I understand the anxiety around cybersecurity. Let's break it down into simple steps. ",
            ["frustrated"] = "I'm sorry you're feeling frustrated. Let's slow down and work through this together. ",
            ["annoyed"] = "I hear you — let's take a step back and I'll explain this more clearly. ",
            ["confused"] = "No worries at all — let me explain this as simply as possible. ",
            ["curious"] = "Great curiosity! That's the first step to staying safe online. ",
            ["interested"] = "Excellent! Staying interested in cybersecurity is one of the best habits you can have. "
        };

        private readonly Dictionary<string, string> _keywordTopicMap = new Dictionary<string, string>
        {
            ["phishing"] = "phishing",
            ["password"] = "password",
            ["scam"] = "scam",
            ["fraud"] = "scam",
            ["privacy"] = "privacy",
            ["personal data"] = "privacy",
            ["malware"] = "malware",
            ["virus"] = "malware",
            ["ransomware"] = "malware",
            ["spyware"] = "malware",
            ["wifi"] = "wifi",
            ["wi-fi"] = "wifi",
            ["network"] = "wifi",
            ["tip"] = "tip",
            ["tips"] = "tip",
            ["advice"] = "tip"
        };

        public string LastTopic { get; private set; } = "";

        public string GetResponse(string input, string userName, string favoriteTopic)
        {
            string raw = input.ToLower().Trim();
            string sentimentPrefix = DetectSentiment(raw);

            if (IsFollowUp(raw))
            {
                string topic = !string.IsNullOrEmpty(LastTopic) ? LastTopic : "tip";
                return sentimentPrefix + GetRandom(topic) + BuildSuffix(userName, favoriteTopic, topic);
            }

            foreach (var kvp in _keywordTopicMap)
            {
                if (raw.Contains(kvp.Key))
                {
                    LastTopic = kvp.Value;
                    return sentimentPrefix + GetRandom(kvp.Value) + BuildSuffix(userName, favoriteTopic, kvp.Value);
                }
            }

            if (!string.IsNullOrEmpty(favoriteTopic))
                return $"As someone interested in {favoriteTopic}, {userName}, here's something to consider: " + GetRandom("tip");

            return GetDefaultResponse(userName);
        }

        private string DetectSentiment(string input)
        {
            foreach (var kvp in _sentimentResponses)
                if (input.Contains(kvp.Key)) return kvp.Value;
            return "";
        }

        private bool IsFollowUp(string input)
        {
            string[] followUps = { "tell me more", "give me another", "more tips", "another tip",
                                   "explain more", "go on", "continue", "what else", "give me more",
                                   "more info", "more details" };
            foreach (string f in followUps)
                if (input.Contains(f)) return true;
            return false;
        }

        private string GetRandom(string topic)
        {
            if (_randomResponses.ContainsKey(topic))
            {
                var list = _randomResponses[topic];
                return list[new Random().Next(list.Count)];
            }
            return GetDefaultResponse("");
        }

        private string BuildSuffix(string userName, string favoriteTopic, string currentTopic)
        {
            if (!string.IsNullOrEmpty(favoriteTopic) && favoriteTopic == currentTopic)
                return $" Since this is your favourite topic, {userName}, would you like to go even deeper?";
            if (!string.IsNullOrEmpty(userName))
                return $" Feel free to ask me more, {userName}!";
            return "";
        }

        private string GetDefaultResponse(string userName)
        {
            string[] defaults = {
                "I'm not sure I understand. Can you try rephrasing?",
                "I didn't quite catch that. Try asking about passwords, phishing, scams, privacy, or malware.",
                "Hmm, I'm not sure about that one. Could you give me a bit more detail?",
                "I want to help! Try asking something like 'Give me a phishing tip' or 'Tell me about password safety'."
            };
            string response = defaults[new Random().Next(defaults.Length)];
            return string.IsNullOrEmpty(userName) ? response : $"{response} I'm here for you, {userName}!";
        }
    }

    // ── Main chat window ─────────────────────────────────────────────────────
    public partial class REDAPP : Window
    {
        private string userName = "";
        private string favoriteTopic = "";
        private int frustrationCount = 0;
        private string _awaitingReminderFor = "";
        private List<string> sessionHistory = new List<string>();
        private readonly RedAppResponseEngine _engine = new RedAppResponseEngine();

        private readonly string saveFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CyberLikhChats"
        );

        public REDAPP(string nameFromWelcome = "")
        {
            InitializeComponent();
            userName = nameFromWelcome;

            PlayGreeting();

            if (!string.IsNullOrEmpty(userName))
                AddMessage($"👋 Welcome, {userName}! I'm your cybersecurity assistant. Ask me about passwords, phishing, scams, privacy, malware, and more!", false);

            LoadSavedChats();
        }

        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("dado.wav");
                player.Load();
                player.Play();
            }
            catch { }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserMessage();

        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { ProcessUserMessage(); e.Handled = true; }
        }

        private async void ProcessUserMessage()
        {
            string userMessage = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            string lowerMsg = userMessage.ToLower();

            // ── NLP: Show activity log intent ──────────────────────────────
            if (lowerMsg.Contains("show activity log") || lowerMsg.Contains("what have you done")
                || lowerMsg.Contains("what did you do") || lowerMsg.Contains("show log")
                || lowerMsg.Contains("recent actions"))
            {
                AddMessage(userMessage, true);
                ChatInput.Clear();
                string logReply = AppState.Logger.GetRecentLog(10);
                if (AppState.Logger.GetCount() > 10)
                    logReply += "\n\n(Type 'show more' to see the full history)";
                AddMessage(logReply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {logReply}");
                return;
            }

            // ── NLP: Show more (full log) ──────────────────────────────────
            if (lowerMsg.Contains("show more") && AppState.Logger.GetCount() > 10)
            {
                AddMessage(userMessage, true);
                ChatInput.Clear();
                string fullLog = AppState.Logger.GetFullLog();
                AddMessage(fullLog, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {fullLog}");
                return;
            }

            // ── NLP: Start quiz intent ─────────────────────────────────────
            if (lowerMsg.Contains("start quiz") || lowerMsg.Contains("take quiz")
                || lowerMsg.Contains("test my knowledge") || lowerMsg.Contains("quiz me")
                || lowerMsg.Contains("play the game"))
            {
                AddMessage(userMessage, true);
                ChatInput.Clear();
                AppState.Logger.Log("Quiz started");
                string reply = "Starting the quiz now — good luck!";
                AddMessage(reply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {reply}");
                QuizArena quizWindow = new QuizArena();
                quizWindow.Show();
                return;
            }

            // ── NLP: Add task intent ───────────────────────────────────────
            if (lowerMsg.Contains("add task") || lowerMsg.Contains("add a task")
                || lowerMsg.Contains("create task") || lowerMsg.Contains("i need to")
                || lowerMsg.Contains("enable") || lowerMsg.Contains("set up"))
            {
                string taskTitle = ExtractTaskTitle(userMessage);
                AddMessage(userMessage, true);
                ChatInput.Clear();

                var taskManager = new TaskManager();
                taskManager.AddTask(taskTitle, $"Set up: {taskTitle}", "");
                AppState.Logger.Log($"Task added: '{taskTitle}' (no reminder set)");

                string reply = $"Task added: '{taskTitle}'. Would you like to set a reminder for this task?";
                AddMessage(reply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {reply}");
                _awaitingReminderFor = taskTitle;
                return;
            }

            // ── NLP: Set reminder intent (only right after a task was just added) ──
            if (!string.IsNullOrEmpty(_awaitingReminderFor) &&
                (lowerMsg.Contains("remind me") || lowerMsg.Contains("reminder")
                || lowerMsg.Contains("set a reminder") || lowerMsg.Contains("don't forget")
                || lowerMsg.StartsWith("yes")))
            {
                AddMessage(userMessage, true);
                ChatInput.Clear();

                string reminderText = ExtractReminderText(userMessage);
                AppState.Logger.Log($"Reminder set: '{_awaitingReminderFor}' - {reminderText}");

                string reply = $"Got it! I'll remind you: {reminderText}.";
                AddMessage(reply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {reply}");
                _awaitingReminderFor = "";
                return;
            }

            // Name capture
            if (lowerMsg.StartsWith("my name is"))
            {
                userName = lowerMsg.Replace("my name is", "").Trim();
                AddMessage(userMessage, true);
                ChatInput.Clear();
                string reply = $"Great to meet you, {userName}! I'll remember your name throughout our conversation.";
                AddMessage(reply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {reply}");
                return;
            }

            // Favourite topic capture
            if (lowerMsg.Contains("interested in") || lowerMsg.StartsWith("my favorite topic is"))
            {
                string topic = lowerMsg
                    .Replace("my favorite topic is", "")
                    .Replace("i'm interested in", "")
                    .Replace("i am interested in", "")
                    .Trim();
                favoriteTopic = topic;
                AddMessage(userMessage, true);
                ChatInput.Clear();
                string reply = $"Great! I'll remember that you're interested in {favoriteTopic}, {userName}. It's a crucial part of staying safe online. As someone interested in {favoriteTopic}, you might want to review the security settings on your accounts regularly.";
                AddMessage(reply, false);
                sessionHistory.Add($"User: {userMessage}");
                sessionHistory.Add($"Bot: {reply}");
                return;
            }

            // Frustration tracking
            if (lowerMsg.Contains("frustrated") || lowerMsg.Contains("annoyed"))
            {
                frustrationCount++;
                if (frustrationCount >= 2)
                {
                    AddMessage(userMessage, true);
                    ChatInput.Clear();
                    string reply = $"I hear you, {userName}. I'm sorry this has been frustrating. Let's slow right down — tell me exactly what's confusing you and I'll explain it step by step.";
                    AddMessage(reply, false);
                    sessionHistory.Add($"User: {userMessage}");
                    sessionHistory.Add($"Bot: {reply}");
                    return;
                }
            }

            AddMessage(userMessage, true);
            sessionHistory.Add($"User: {userMessage}");
            ChatInput.Clear();

            TypingIndicator.Visibility = Visibility.Visible;
            await Task.Delay(1200);
            TypingIndicator.Visibility = Visibility.Collapsed;

            string botResponse = _engine.GetResponse(userMessage, userName, favoriteTopic);

            if (!string.IsNullOrEmpty(_engine.LastTopic))
                AppState.Logger.Log($"Keyword matched: {_engine.LastTopic} - response delivered");

            AddMessage(botResponse, false);
            sessionHistory.Add($"Bot: {botResponse}");
        }

        private string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();
            string[] triggers = { "add a task to", "add task to", "add a task -", "add task -",
                                   "create task to", "i need to", "enable", "set up" };

            foreach (string trigger in triggers)
            {
                int idx = lower.IndexOf(trigger);
                if (idx >= 0)
                {
                    string remainder = input.Substring(idx + trigger.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(remainder))
                    {
                        remainder = remainder.TrimEnd('.', '!', '?');
                        return char.ToUpper(remainder[0]) + remainder.Substring(1);
                    }
                }
            }
            return "New task";
        }

        private string ExtractReminderText(string input)
        {
            string lower = input.ToLower();
            int idx = lower.IndexOf("remind me");
            if (idx >= 0)
            {
                string remainder = input.Substring(idx).Trim();
                return remainder.TrimEnd('.', '!', '?');
            }
            return "as requested";
        }

        private void AddMessage(string text, bool isUser)
        {
            Border bubble = new Border
            {
                Background = isUser ? Brushes.White : new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(12),
                Margin = new Thickness(5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                MaxWidth = 550
            };

            TextBlock txt = new TextBlock
            {
                Text = text,
                Foreground = isUser ? new SolidColorBrush(Color.FromRgb(220, 38, 38)) : Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16
            };

            bubble.Child = txt;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToBottom();
        }

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) { ChatInput.Text = btn.Tag.ToString(); ProcessUserMessage(); }
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SideMenu.Visibility == Visibility.Collapsed)
            {
                SideMenu.Visibility = Visibility.Visible;
                var anim = new ThicknessAnimation
                {
                    From = new Thickness(-250, 0, 0, 0),
                    To = new Thickness(0),
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };
                SideMenu.BeginAnimation(MarginProperty, anim);
            }
            else
            {
                var anim = new ThicknessAnimation
                {
                    From = new Thickness(0),
                    To = new Thickness(-250, 0, 0, 0),
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };
                anim.Completed += (s, args) =>
                {
                    SideMenu.Visibility = Visibility.Collapsed;
                    SideMenu.BeginAnimation(MarginProperty, null);
                };
                SideMenu.BeginAnimation(MarginProperty, anim);
            }
        }

        private void NewChat_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentChatToFile();
            ChatPanel.Children.Clear();
            sessionHistory.Clear();
            frustrationCount = 0;
            favoriteTopic = "";
            Welcome welcomeWindow = new Welcome();
            welcomeWindow.Show();
            this.Close();
        }

        private void SaveCurrentChatToFile()
        {
            if (sessionHistory.Count == 0) return;
            try
            {
                if (!Directory.Exists(saveFolderPath)) Directory.CreateDirectory(saveFolderPath);
                string fileName = $"Chat_{userName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
                var lines = new List<string>
                {
                    $"CyberLikh Chat — {DateTime.Now:dddd, dd MMMM yyyy HH:mm}",
                    $"User: {userName}",
                    new string('-', 40)
                };
                lines.AddRange(sessionHistory);
                File.WriteAllLines(Path.Combine(saveFolderPath, fileName), lines);
                LoadSavedChats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save chat: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadSavedChats()
        {
            SavedChatsList.Items.Clear();
            try
            {
                if (!Directory.Exists(saveFolderPath)) { AddNoChatsLabel(); return; }
                string[] files = Directory.GetFiles(saveFolderPath, "*.txt");
                if (files.Length == 0) { AddNoChatsLabel(); return; }
                Array.Sort(files); Array.Reverse(files);

                foreach (string file in files)
                {
                    string displayName = Path.GetFileNameWithoutExtension(file)
                        .Replace("Chat_", "").Replace("_", " ");

                    DockPanel row = new DockPanel { Margin = new Thickness(0, 2, 0, 2) };

                    Button deleteBtn = new Button
                    {
                        Content = "✕",
                        Width = 22,
                        Height = 22,
                        FontSize = 11,
                        Background = new SolidColorBrush(Color.FromRgb(120, 0, 0)),
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(0),
                        Cursor = Cursors.Hand,
                        ToolTip = "Delete this chat",
                        Tag = file
                    };
                    deleteBtn.Click += DeleteChat_Click;
                    DockPanel.SetDock(deleteBtn, Dock.Right);
                    row.Children.Add(deleteBtn);

                    Button nameBtn = new Button
                    {
                        Content = displayName,
                        Background = Brushes.Transparent,
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(0),
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Cursor = Cursors.Hand,
                        Tag = file,
                        Padding = new Thickness(2, 0, 4, 0)
                    };
                    nameBtn.Click += LoadChat_Click;
                    row.Children.Add(nameBtn);

                    SavedChatsList.Items.Add(row);
                }
            }
            catch { AddNoChatsLabel(); }
        }

        private void AddNoChatsLabel()
        {
            SavedChatsList.Items.Add(new TextBlock
            {
                Text = "No saved chats yet",
                Foreground = Brushes.LightGray,
                FontSize = 13,
                Margin = new Thickness(4)
            });
        }

        private void LoadChat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || !File.Exists(btn.Tag.ToString())) return;
            try
            {
                ChatPanel.Children.Clear();
                sessionHistory.Clear();
                foreach (string line in File.ReadAllLines(btn.Tag.ToString()))
                {
                    if (line.StartsWith("User: "))
                    {
                        string content = line.Substring("User: ".Length);
                        if (content == userName) continue;
                        AddMessage(content, true);
                        sessionHistory.Add(line);
                    }
                    else if (line.StartsWith("Bot: "))
                    {
                        AddMessage(line.Substring("Bot: ".Length), false);
                        sessionHistory.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load chat: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteChat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (MessageBox.Show("Are you sure you want to delete this chat?", "Delete Chat",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (File.Exists(btn.Tag.ToString())) File.Delete(btn.Tag.ToString());
                    LoadSavedChats();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not delete chat: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveChat_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentChatToFile();
            MessageBox.Show("Chat saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TaskHubButton_Click(object sender, RoutedEventArgs e)
        {
            TaskHub taskWindow = new TaskHub();
            taskWindow.Show();
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            QuizArena quizWindow = new QuizArena();
            quizWindow.Show();
            AppState.Logger.Log("Quiz started");
        }
    }
}