using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace CybersecurityChatbot
{
    public partial class Form1 : Form
    {
        private CybersecurityBot bot;
        private RichTextBox chatLog;
        private TextBox inputBox;
        private Button sendButton;
        private Label asciiArtLabel;
        private IContainer components = null;

        public Form1()
        {
            InitializeComponent();
            bot = new CybersecurityBot();
            PlayVoiceGreeting();
            ShowAsciiArt();
            bot.OnResponse += AppendBotMessage;
        }

        private void InitializeComponent()
        {
            this.chatLog = new RichTextBox();
            this.inputBox = new TextBox();
            this.sendButton = new Button();
            this.asciiArtLabel = new Label();
            this.SuspendLayout();

            // chatLog
            this.chatLog.Location = new Point(12, 100);
            this.chatLog.Size = new Size(560, 300);
            this.chatLog.ReadOnly = true;
            this.chatLog.BackColor = Color.LightbBlack;
            this.chatLog.ForeColor = Color.LightBlue;
            this.chatLog.Font = new Font("Consolas", 10);

            // inputBox
            this.inputBox.Location = new Point(12, 410);
            this.inputBox.Size = new Size(470, 20);
            this.inputBox.BackColor = Color.Black;
            this.inputBox.ForeColor = Color.White;
            this.inputBox.KeyDown += InputBox_KeyDown;

            // sendButton
            this.sendButton.Location = new Point(480, 408);
            this.sendButton.Size = new Size(96, 25);
            this.sendButton.Text = "Send";
            this.sendButton.BackColor = Color.DarkCyan;
            this.sendButton.ForeColor = Color.Black;
            this.sendButton.Click += SendButton_Click;

            // asciiArtLabel
            this.asciiArtLabel.Location = new Point(12, 12);
            this.asciiArtLabel.Size = new Size(560, 80);
            this.asciiArtLabel.Font = new Font("Consolas", 9);
            this.asciiArtLabel.ForeColor = Color.Cyan;
            this.asciiArtLabel.Text = "";

            // Form
            this.Text = "Cybersecurity Awareness Bot";
            this.BackColor = Color.DimGray;
            this.ClientSize = new Size(589, 450);
            this.Controls.Add(this.chatLog);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.asciiArtLabel);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("C:\\Users\\Student\\source\\repos\\Form1\\Form1\\greeting_1.wav");
                player.Play();
            }
            catch
            {
                AppendBotMessage("?? Voice file not found. Continuing in silence.");
            }
        }

private void ShowAsciiArt()
        {
            string art = @"
    ╔══════════════════════════════════════════════════╗
    ║  ██████╗██╗   ██╗██████╗ ███████╗██████╗         ║
    ║ ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗        ║
    ║ ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝        ║
    ║ ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗        ║
    ║ ╚██████╗   ██║   ██████╔╝███████╗██║  ██║        ║
    ║  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝        ║
    ║                                                  ║
    ║         🔐  HILTON'S SECURITY BOT  🔐           ║
    ║         ─────────────────────────────            ║
    ║            🛡️  Stay Safe Online!                 ║
    ╚══════════════════════════════════════════════════╝";
            asciiArtLabel.Text = art;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            ProcessUserInput();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessUserInput();
                e.SuppressKeyPress = true;
            }
        }

        private void ProcessUserInput()
        {
            string userMessage = inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            AppendUserMessage(userMessage);
            bot.ProcessInput(userMessage);
            inputBox.Clear();
        }

        private void AppendUserMessage(string message)
        {
            chatLog.SelectionColor = Color.Black;
            chatLog.AppendText($"You: {message}{Environment.NewLine}");
            chatLog.ScrollToCaret();
        }

        private void AppendBotMessage(string message)
        {
            chatLog.SelectionColor = Color.Blue;
            chatLog.AppendText($"Bot: {message}{Environment.NewLine}");
            chatLog.ScrollToCaret();
        }
    }
}
