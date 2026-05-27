using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class CybersecurityBot
    {
        // Events for GUI interaction
        public event Action<string> OnResponse;
        public event Action<string> RequestUserInput; // optional for asking name

        // Memory
        private string userName = null;
        private string favoriteTopic = null;

        // Conversation flow (last topic & tip index for "tell me more")
        private string lastTopic = null;
        private int lastTipIndex = -1;

        // Keyword response mapping – each key maps to a list of possible responses
        private Dictionary<string, List<string>> responseMap;

        // Sentiment keywords and corresponding empathetic responses
        private Dictionary<string, string> sentimentResponses;

        // Random generator
        private Random rand = new Random();

        public CybersecurityBot()
        {
            InitializeResponseMap();
            InitializeSentimentMap();
        }

        private void InitializeResponseMap()
        {
            responseMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "Use strong, unique passwords for each account. Consider a password manager.",
                    "Avoid using personal info like birthdays or pet names in passwords.",
                    "Enable two-factor authentication wherever possible – it adds an extra layer of security."
                },
                ["phishing"] = new List<string>
                {
                    "Never click on suspicious links in emails. Always verify the sender's address.",
                    "Phishing emails often create urgency. Take a moment to double-check before acting.",
                    "If an email asks for personal info, contact the company directly using a known phone number or website."
                },
                ["privacy"] = new List<string>
                {
                    "Review your social media privacy settings regularly. Limit what you share publicly.",
                    "Use a VPN on public Wi‑Fi to encrypt your connection.",
                    "Be careful what you post online – once it's out, it's hard to take back."
                },
                ["scam"] = new List<string>
                {
                    "Scammers impersonate trusted organisations. Hang up and call back on official numbers.",
                    "If an offer sounds too good to be true, it probably is a scam.",
                    "Never send money or gift cards to someone you met online without verification."
                },
                ["2fa"] = new List<string>
                {
                    "Two‑factor authentication adds a second step using your phone or an authenticator app.",
                    "Always enable 2FA on email, banking, and social media accounts."
                },
                ["update"] = new List<string>
                {
                    "Keep your operating system and apps updated. Patches fix security holes.",
                    "Enable automatic updates whenever possible to avoid missing critical fixes."
                },
                ["safe browsing"] = new List<string>
                {
                    "Check for 'https://' and the padlock icon before entering personal data.",
                    "Avoid downloading files from untrusted websites."
                }
            };
        }

        private void InitializeSentimentMap()
        {
            sentimentResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["worried"] = "I understand your concern. Let me give you a practical tip to ease your mind: ",
                ["frustrated"] = "It can be frustrating, but staying calm helps you make better security decisions. Here's something useful: ",
                ["curious"] = "It's great that you're curious! That's the first step to staying safe online. ",
                ["scared"] = "It's normal to feel scared – scammers are persuasive. Let me help you feel more in control: ",
                ["thankful"] = "You're welcome! Keeping you safe online makes me happy. "
            };
        }

        public void ProcessInput(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // 1. Sentiment detection – adjust response tone
            string sentimentTip = DetectSentiment(lowerInput);
            if (sentimentTip != null)
            {
                OnResponse?.Invoke(sentimentTip);
                // Continue to normal keyword response after empathy
            }

            // 2. Memory: ask for name if not known (first interaction)
            if (string.IsNullOrEmpty(userName))
            {
                AskName(userInput);
                return; // wait for name
            }

            // 3. Conversation flow: "tell me more", "another tip", "explain more"
            if (IsRequestForMore(lowerInput))
            {
                ProvideMoreInfo();
                return;
            }

            // 4. General keyword recognition
            bool matched = false;
            foreach (var kvp in responseMap)
            {
                if (lowerInput.Contains(kvp.Key))
                {
                    lastTopic = kvp.Key;
                    string response = GetRandomResponse(kvp.Value);
                    OnResponse?.Invoke(response);
                    matched = true;
                    break;
                }
            }

            // 5. If no keyword matched, fallback with error handling
            if (!matched)
            {
                OnResponse?.Invoke("I'm not sure I understand. Can you try rephrasing? Ask me about passwords, phishing, privacy, scams, 2FA, updates, or safe browsing.");
            }
        }

        private void AskName(string userInput)
        {
            if (string.IsNullOrEmpty(userName))
            {
                // The first message might already contain a name
                if (!string.IsNullOrWhiteSpace(userInput) && !userInput.ToLower().Contains("my name is"))
                {
                    OnResponse?.Invoke("Hello! I'm your cybersecurity assistant. What's your name?");
                
                }
                else
                {
                    // Extract name from "My name is hilton"
                    string[] parts = userInput.Split(new[] { "my name is" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                        userName = parts[1].Trim();
                    else
                        userName = userInput.Trim();

                    if (!string.IsNullOrEmpty(userName))
                    {
                        OnResponse?.Invoke($"Nice to meet you, {userName}! I'll remember you. What cybersecurity topic interests you most? (e.g., passwords, phishing, privacy)");
                    }
                }
            }
            else if (string.IsNullOrEmpty(favoriteTopic))
            {
                // User is answering favourite topic
                favoriteTopic = userInput;
                OnResponse?.Invoke($"Great! As someone interested in {favoriteTopic}, I'll tailor tips for you.");
            }
        }

        private bool IsRequestForMore(string input)
        {
            return input.Contains("tell me more") ||
                   input.Contains("another tip") ||
                   input.Contains("explain more") ||
                   input.Contains("more info");
        }

        private void ProvideMoreInfo()
        {
            if (string.IsNullOrEmpty(lastTopic))
            {
                OnResponse?.Invoke("I don't have a current topic. Ask me something first, then I can give you more tips.");
                return;
            }

            if (responseMap.ContainsKey(lastTopic))
            {
                var tips = responseMap[lastTopic];
                // Rotate to a different tip (not the same as last time if possible)
                int newIndex = rand.Next(tips.Count);
                if (tips.Count > 1 && newIndex == lastTipIndex)
                    newIndex = (newIndex + 1) % tips.Count;
                lastTipIndex = newIndex;
                OnResponse?.Invoke(tips[newIndex]);
            }
            else
            {
                OnResponse?.Invoke("I don't have additional tips on that right now. Try asking about another topic.");
            }
        }

        private string GetRandomResponse(List<string> responses)
        {
            int index = rand.Next(responses.Count);
            lastTipIndex = index; // remember for "more"
            return responses[index];
        }

        private string DetectSentiment(string input)
        {
            foreach (var sentiment in sentimentResponses)
            {
                if (input.Contains(sentiment.Key))
                {
                    return sentiment.Value;
                }
            }
            return null;
        }
    }
}