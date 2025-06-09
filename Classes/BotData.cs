// References
// https://chatgpt.com
// https://gemini.google.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ST10445832_PROG6221_PoE.Classes
{
    public class BotData
    {
        // all questions and answers
        public Dictionary<string, string> QnA = new Dictionary<string, string>();
        // Main topics
        public List<string> Keywords = new List<string> { "password", "scam", "privacy", "virus", "malware", "phishing" };
        // several tips for each of the main topics
        public Dictionary<string, List<string>> Tips = new Dictionary<string, List<string>>();
        // responses to add after answers
        public List<string> FollowUps;
        // responses to add before answers
        public List<List<string>> Openers = new List<List<string>>();
        // words used to gague sentiment
        public List<string> SentimentWords = new List<string>();
        // outputs for first interaction on entering chat
        public List<string> FirstMessageEndings = new List<string>()
        {
            "What will it be this time?",
            "How can I help?",
            "What would you like to know?",
            "What do you want to know about?",
            "What would you like to ask me?"
        };

        public string UserName;

        // Value represents index in SentimentWords list
        public enum Sentiment
        {
            WORRIED = 0,
            CURIOUS = 1,
            HAPPY = 2,
            FRUSTRATED = 3,
            CONFUSED = 4,
            NEUTRAL = 5
        };
        // path for task storage
        private string _tasksDataPath = "Tasks.xml";
        public ObservableCollection<TaskReminder> Tasks { get; set; }

        // Quiz Fields
        private List<MultipleChoiceQuestion> _multipleChoiceQuestions;
        private List<BoolQuestion> _boolQuestions;
        public int QuestionCounter = 0;
        public int CorrectAnswers = 0;
        public List<Object> RoundQuestions;

        private Random _rand = new Random();

        //=========================================================//
        // Default constructor
        public BotData(string userName)
        {
            UserName = userName;
            InitialiseQnA();
            InitialiseTips();
            InitialiseOpeners();
            InitialiseFollowUps();
            InitialiseSentimentWords();
            InitialiseQuizQuestions();
            Tasks = new ObservableCollection<TaskReminder>();
            LoadTasks();
        }


        //=========================================================//
        // Loads tasks from xml file
        private void LoadTasks()
        {
            if (File.Exists(_tasksDataPath))
            {
                try
                {
                    // Gemini
                    XmlRootAttribute rootAttribute = new XmlRootAttribute("Tasks");
                    rootAttribute.Namespace = "";
                    XmlSerializer xmlSerial = new XmlSerializer(typeof(TasksStore), rootAttribute);
                    using (FileStream fs = new FileStream(_tasksDataPath, FileMode.Open))
                    {
                        var tasksStore = (TasksStore)xmlSerial.Deserialize(fs);
                        if (tasksStore != null && tasksStore.Tasks.Count > 0)
                        {
                            foreach (var task in tasksStore.Tasks)
                            {
                                Tasks.Add(task);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("Tasks file not found");
            }
        }

        //===============================================//
        // saves items from Tasks list an xml file
        public void UpdateTasks()
        {
            TasksStore newTasksStore = new TasksStore();
            foreach (var task in Tasks)
            {
                newTasksStore.Tasks.Add(task);
            }
            try
            {
                XmlRootAttribute rootAttribute = new XmlRootAttribute("Tasks");
                rootAttribute.Namespace = "";
                XmlSerializer xmlSerial = new XmlSerializer(typeof(TasksStore), rootAttribute);
                using (FileStream fs = new FileStream(_tasksDataPath, FileMode.Create))
                {
                    xmlSerial.Serialize(fs, newTasksStore);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        //=========================================================//
        // General question-answer pairs
        private void InitialiseQnA()
        {
            // Default response
            QnA.Add("", $"I'm sorry {UserName}, I don't understand your question. Try to rephrase it or ask me something else.");

            // Assistance
            QnA.Add("Help", "What do you want help with? You can ask me about anything cybersecurity related!");
            QnA.Add("What can I ask you?", "You can ask me any cybersecurity related question and I will do my best to answer you.");
            QnA.Add("What can I ask you about?", "You can ask me any cybersecurity related question and I will do my best to answer you.");

            // Conversation
            QnA.Add("What is your name?", "My name is SecWiz!");
            QnA.Add("How are you?", $"I am well, thank you. I'm always ready to answer your questions! Ask away {UserName}!");
            QnA.Add("What's your purpose?", "I'm here to educate users on matters relating to cyber security and online safety.");
            QnA.Add("Thank you", $"My pleasure, {UserName}. Is there anything else you would like to know?");
            QnA.Add("Yes", "Excellent! I'm happy to hear I could assist you.");
            QnA.Add("I'm interested in passwords", "Great! Would you like help creating strong passwords or managing them securely?");
            QnA.Add("I am interested in passwords", "Awesome — passwords are your first line of defense. Want to know how to make them stronger or use a password manager?");
            QnA.Add("I'm interested in scams", "Good thinking — scams are getting more convincing. Want to learn how to spot them or what to do if you fall for one?");
            QnA.Add("I am interested in scams", "Staying alert is smart. Are you more curious about online scams, phone scams, or how to protect yourself?");
            QnA.Add("I'm interested in privacy", "Excellent — online privacy matters more than ever. Want to review how to control who sees your data or reduce tracking?");
            QnA.Add("I am interested in privacy", "Privacy is a big topic. Would you like tips on keeping your information safe on social media, apps, or your browser?");
            QnA.Add("I'm interested in viruses", "Viruses can seriously damage your system. Want to learn how to prevent infections or how to remove one if it happens?");
            QnA.Add("I am interested in viruses", "Smart move — digital viruses are sneaky. Are you looking for protection tools or signs your device might be infected?");
            QnA.Add("I'm interested in malware", "Malware can steal data or take over your system. Would you like tips on avoiding it or spotting signs you’re infected?");
            QnA.Add("I am interested in malware", "There are many kinds of malware. Want to explore how it works, or how antivirus and anti-malware tools help?");
            QnA.Add("I'm interested in phishing", "Phishing attacks can be hard to spot. Want to learn how to recognize fake messages or what to do if you click one?");
            QnA.Add("I am interested in phishing", "Great — avoiding phishing is key to staying safe. Are you curious about how phishing works or how to defend against it?");

            // CHATGPT
            // Malware
            QnA.Add("What is malware?", "Malware is malicious software designed to harm computer systems.");
            QnA.Add("What are common types of malware?", "Common types include viruses, worms, trojans, ransomware, spyware, and adware.");
            QnA.Add("How does malware get onto my computer?", "Malware can be downloaded through malicious websites, email attachments, software from untrusted sources, or USB drives.");
            QnA.Add("What are signs my device might be infected with malware?", "Slow performance, frequent crashes, unexpected pop-ups, unknown programs launching, or unusual network activity.");
            QnA.Add("What can I do to be safe from malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("Can malware infect smartphones too?", "Yes, smartphones can be infected through malicious apps, unsecured Wi-Fi networks, and phishing links—especially on Android devices.");
            QnA.Add("What is ransomware and how does it work?", "Ransomware locks or encrypts your files and demands payment (usually in cryptocurrency) to restore access.");
            QnA.Add("How do I remove malware?", "Run a full scan using reputable anti-malware software, disconnect from the internet, and follow the tool's removal steps. In severe cases, professional help may be needed.");
            QnA.Add("Can malware hide from antivirus software?", "Yes, some malware uses stealth techniques like rootkits or code obfuscation to avoid detection.");
            QnA.Add("What is a Trojan horse in cybersecurity?", "A Trojan appears to be legitimate software but secretly performs malicious actions once installed.");
            QnA.Add("Is adware considered malware?", "Yes, adware is a type of malware that displays unwanted ads and can track user behavior.");
            QnA.Add("Can malware affect smart home devices?", "Yes, malware can target IoT devices like smart cameras or thermostats if they're not properly secured.");
            QnA.Add("What’s the difference between spyware and malware?", "Spyware is a type of malware specifically designed to secretly monitor and collect user information.");

            // Virus
            QnA.Add("How can I be safe frome viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("Is antivirus software enough to keep me safe?", "Antivirus helps, but it's not foolproof. You should also practice good security habits and keep your system updated.");
            QnA.Add("Do I need antivirus on my phone?", "Yes, especially on Android devices, as they can be vulnerable to malicious apps and websites.");
            QnA.Add("What is a virus?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("What are viruses?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("How do computer viruses spread?", "They often spread through infected email attachments, malicious downloads, removable media, or compromised websites.");
            QnA.Add("What’s the difference between a virus and malware?", "A virus is a type of malware that replicates by attaching to files. Malware is the broader category of any malicious software.");
            QnA.Add("What are some common symptoms of a computer virus?", "Slow performance, frequent crashes, unexpected pop-ups, and unknown programs starting automatically.");
            QnA.Add("Can a virus infect my files in the cloud?", "While cloud providers have strong protections, infected files uploaded from your device can potentially carry threats.");
            QnA.Add("Do viruses still spread through USB drives?", "Yes, USB drives remain a common way for viruses to spread between devices, especially if autorun is enabled.");
            QnA.Add("What’s a macro virus?", "A macro virus is written in the same language as macros in office software like Word or Excel, and activates when you open the file.");
            QnA.Add("Can a virus cause hardware damage?", "It's rare, but some viruses (like CIH) were designed to overwrite firmware, potentially harming hardware.");

            QnA.Add("good anti-virus software", "Bitdefender and Malwarebytes are two popular choices. They offer both paid and free plans.");
            QnA.Add("good anti-malware software", "Bitdefender and Malwarebytes are two popular choices. They offer both paid and free plans.");
            QnA.Add("How does a firewall protect my device?", "It filters network traffic to block unauthorized access and alert you of suspicious activity.");
            QnA.Add("Can I use both a firewall and antivirus?", "Yes, they serve different purposes and work best together to protect your system.");
            QnA.Add("What is a false positive in antivirus software?", "It’s when the antivirus mistakenly identifies a safe file as a threat.");
            QnA.Add("Should I use the built-in Windows Defender?", "Yes, it’s much improved and provides solid baseline protection. Pair it with safe browsing habits.");
            QnA.Add("Do I need antivirus on a Mac?", "Yes, Macs can still be infected by malware or exploited via vulnerabilities and phishing attacks.");

            // Password
            QnA.Add("How can I create a strong password?", "A strong password should be long, unique, and include a mix of uppercase and lowercase letters, numbers, and symbols.");
            QnA.Add("What is a strong password?", "A strong password is long, complex, and includes a mix of letters, numbers, and symbols.");
            QnA.Add("What is a secure password policy?", "It’s a set of rules for creating and maintaining strong passwords.");
            QnA.Add("Why should you avoid reusing passwords?", "Reusing passwords increases risk if one account is compromised.");
            QnA.Add("What is a password manager and how does it work?", "A password manager stores your login credentials securely and can auto-fill them. It encrypts your data and is protected by a master password.");
            QnA.Add("How do hackers crack passwords?", "They use methods like brute force attacks, dictionary attacks, and phishing to guess or steal passwords.");
            QnA.Add("What is password entropy?", "Password entropy measures how unpredictable a password is. Higher entropy means better security.");
            QnA.Add("Is using the same password for multiple accounts risky?", "Yes, if one account is breached, attackers can try the same password on others — this is called credential stuffing.");
            QnA.Add("What makes a password strong?", "A strong password is long (12+ characters), includes a mix of letters, numbers, and symbols, and avoids dictionary words.");
            QnA.Add("Should I write down my passwords?", "It’s better to use a password manager, but if you must write them down, store them in a very secure place.");
            QnA.Add("How often should I change my passwords?", "Change them immediately after a breach, and consider regular updates for sensitive accounts every 6–12 months.");

            // VPN
            QnA.Add("What is a VPN?", "VPN stands for Virtual Private Network. It's a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");
            QnA.Add("What is a virtual private network?", "A virtual private network, or VPN, is a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");
            QnA.Add("Why is it important to use a VPN?", "A VPN encrypts your internet traffic and can help protect your privacy and security, especially on public Wi-Fi.");
            QnA.Add("What is a VPN and how does it help protect privacy?", "A VPN (Virtual Private Network) encrypts your internet connection, hiding your data and browsing activity from third parties.");
            QnA.Add("Do I really need a VPN?", "If you care about privacy—especially on public networks—then yes, it’s a good idea.");
            QnA.Add("How does a VPN work?", "A VPN works by routing your internet traffic through an encrypted tunnel to a server operated by the VPN service. This hides your IP address and encrypts your data.");
            QnA.Add("What are the benefits of using a VPN?", "The main benefits of using a VPN include enhanced privacy, increased security on public Wi-Fi, bypassing geo-restrictions, and hiding your IP address.");
            QnA.Add("Can a VPN make me completely anonymous?", "While a VPN significantly enhances your privacy, it doesn't make you completely anonymous. Your VPN provider still knows your real IP address, and other tracking methods can still be in play.");
            QnA.Add("Is a VPN legal to use?", "Using a VPN is legal in most countries. However, some countries have restricted or banned their use, and using a VPN for illegal activities remains illegal everywhere.");
            QnA.Add("What kind of data does a VPN protect?", "A VPN primarily protects your IP address, Browse history, location, and other data sent over the internet from being intercepted or monitored by third parties.");
            QnA.Add("Can I use a VPN on multiple devices?", "Many VPN providers allow you to use their service on multiple devices simultaneously, typically with a set limit on the number of connections.");
            QnA.Add("Does a VPN slow down my internet speed?", "A VPN can sometimes slow down your internet speed due to the encryption process and the extra distance your data travels. However, the impact is often minimal, especially with a good VPN service.");
            QnA.Add("What is a 'no-log' VPN policy?", "A 'no-log' VPN policy means that the VPN provider does not store any logs of your online activity, such as Browse history, connection times, or IP addresses, further enhancing your privacy.");
            QnA.Add("Is a free VPN safe to use?", "Free VPNs often come with limitations like slower speeds, data caps, and can sometimes compromise your privacy by logging and selling your data or displaying ads. Paid VPNs generally offer better security and performance.");
            QnA.Add("Is a VPN the same as a proxy server?", "No, VPNs encrypt all your traffic, while proxies only route specific app data and usually lack encryption.");
            QnA.Add("Can a VPN prevent malware infections?", "No, a VPN secures your connection but doesn’t protect you from downloading malicious files. Use antivirus too.");
            QnA.Add("Do VPNs slow down internet speed?", "Sometimes, yes. Your speed can be reduced due to encryption overhead or server distance.");
            QnA.Add("Should I leave my VPN on all the time?", "Yes, keeping it on ensures your data stays encrypted and your IP address remains hidden.");
            QnA.Add("Can I use a free VPN safely?", "Be cautious. Many free VPNs log your data or inject ads. Use a trusted provider with a no-logs policy.");

            // Scam
            QnA.Add("What is a scam?", "A scam is a deceptive scheme or trick used to cheat someone out of money, personal information, or other valuables.");
            QnA.Add("How can I identify a scam?", "Look for red flags like urgent requests for money or personal information, offers that seem too good to be true, pressure to act immediately, and unusual payment methods (e.g., gift cards, wire transfers). Scammers often impersonate legitimate organizations.");
            QnA.Add("What are common types of scams?", "Common scams include phishing (fake emails/messages to steal info), tech support scams, fake lottery or prize winnings, romance scams, and impersonation scams (e.g., pretending to be from the government or a known business).");
            QnA.Add("How do I protect myself from scams?", "Be skeptical of unsolicited communications asking for personal details or money. Verify requests independently by contacting the organization through official channels. Don't click on suspicious links or download unknown attachments. Use strong, unique passwords and enable two-factor authentication where possible.");
            QnA.Add("What should I do if I've been scammed?", "If you've been scammed, report it to the relevant authorities (e.g., police, fraud reporting agencies). Change any compromised passwords immediately, contact your bank if financial information was shared, and monitor your accounts for suspicious activity.");
            QnA.Add("Are online reviews always trustworthy?", "No, online reviews can be faked. Look for patterns in reviews, check multiple sources, and be wary of overly positive or overly negative reviews, especially if they are vague or use similar language.");

            // Privacy
            QnA.Add("What is online privacy?", "Online privacy refers to your ability to control what information you share about yourself online and who can access it. It involves managing your digital footprint and protecting your personal data from unauthorized access or use.");
            QnA.Add("Why is online privacy important?", "Protecting your online privacy helps prevent identity theft, financial loss, unwanted marketing, and personal harassment. It allows you to use the internet safely and maintain control over your personal information.");
            QnA.Add("How can I protect my online privacy?", "Use strong, unique passwords for different accounts, enable two-factor authentication, be cautious about sharing personal information online, review app permissions, use a VPN, and regularly check and adjust your privacy settings on social media and other online services.");
            QnA.Add("What are cookies and how do they affect my privacy?", "Cookies are small text files websites store on your browser to remember your preferences, login details, or track your Browse activity. While some are essential for website function, others can be used for targeted advertising. You can manage cookies through your browser settings.");
            QnA.Add("Should I read privacy policies?", "Yes, reading privacy policies helps you understand how websites and services collect, use, and share your data. While often long, look for key sections on data collection, sharing with third parties, and your rights.");
            QnA.Add("How can I limit tracking online?", "You can limit online tracking by using browser settings to block third-party cookies, using private Browse modes, installing tracker-blocking extensions, and considering privacy-focused search engines and browsers.");

            // General
            QnA.Add("That email looked real—how was I supposed to know it was fake?", "Phishing emails often look legit, but small details like the sender's email address, weird grammar, or urgent demands can give them away. When in doubt, don’t click.");
            QnA.Add("Is it okay to open links if I don’t click on anything else?", "Even just clicking a suspicious link can be risky. It’s best to avoid them unless you're sure they’re safe.");
            QnA.Add("Do I really need different passwords for every site?", "Yep—it’s a pain, but using the same password everywhere is super risky. A password manager can help keep things organized.");
            QnA.Add("I forgot my password again! Is it bad to keep it written down?", "Writing it down isn't ideal, but if you must, keep it in a secure place. Better yet, use a password manager.");
            QnA.Add("Is it really that easy to get hacked?", "Unfortunately, yes—especially if your guard’s down. Even one wrong click can be all it takes.");
            QnA.Add("I'm not tech-savvy. How do I know if something's a threat?", "You don’t have to be an expert—just stay skeptical of unexpected emails, weird links, and too-good-to-be-true offers. When unsure, ask or do a quick search.");
            QnA.Add("if I get ransomware, all my stuff is gone?", "Unless you have a backup, pretty much. That’s why backing up your files is super important.");
            QnA.Add("Can I just unplug my computer if I think I got a virus?", "Actually, yes! Turning off the internet or shutting it down can help stop malware from spreading further.");
            QnA.Add("Is it fine to check my bank account on airport Wi-Fi?", "It’s not recommended. Public Wi-Fi can be sketchy. If you must, use a VPN to be safer.");
            QnA.Add("Why would anyone want to hack me?", "Hackers don’t always target people specifically. Sometimes it’s just about grabbing whatever info they can get.");
            QnA.Add("What’s one thing I can do today to be safer online?", "Start by enabling two-factor authentication wherever you can. It’s a simple move with a big impact.");
            QnA.Add("That message said I won a prize—should I believe it?", "Probably not. If you didn’t enter a contest, it’s most likely a scam. Don’t click anything or give out info.");
            QnA.Add("Someone just DMed me asking for money—what should I do?", "Be cautious. Scammers often pretend to be friends or family. Double-check with the person directly if you're unsure.");
            QnA.Add("How do I know if a website is safe to use?", "Look for 'https://' in the address bar and a padlock icon. If the site looks off or asks for weird info, trust your gut and leave.");
            QnA.Add("Do I really need to update my phone all the time?", "Yes! Updates fix security holes that hackers can exploit. It’s worth the few minutes it takes.");
            QnA.Add("Is it bad to download apps from outside the App Store?", "It can be. Apps from unknown sources might carry malware. Stick to official stores when possible.");
            QnA.Add("My computer’s acting weird—should I be worried?", "Possibly. It could be malware or just a glitch. Run a scan with your antivirus to be safe.");
            QnA.Add("Is it okay to post vacation pics while I’m away?", "Better to wait until you’re back. Posting in real-time can let people know your home is empty.");
            QnA.Add("Can people really find me just from my social media?", "Yep. Even small details can give away your location, habits, or identity. Be mindful of what you share.");
            QnA.Add("Should I accept friend requests from strangers?", "Probably not. Some accounts are fake and just trying to collect your info or scam you.");
            QnA.Add("Do I need to log out of accounts every time?", "It’s a good habit, especially on shared or public devices. It keeps your info safer.");
            QnA.Add("How often should I change my password?", "Every few months is a good rule. Or immediately if you think an account has been compromised.");
            QnA.Add("Is clicking 'Remember Me' on websites a bad idea?", "It's convenient, but it can be risky on public or shared devices. Only use it on your personal, secure devices.");
            QnA.Add("Is cybersecurity just for people who work in tech?", "Nope! Everyone who uses the internet should care. Hackers go after regular users all the time.");
            QnA.Add("What’s the worst that could happen if I get hacked?", "You could lose money, have your identity stolen, or your private info leaked. It’s more common than you’d think.");
            QnA.Add("What is a digital footprint?", "A digital footprint is the trail of data you leave behind when using the internet.");
            QnA.Add("What is cyberbullying?", "Cyberbullying is using digital communication to harass, threaten, or humiliate others.");
            QnA.Add("Can social media put me at risk?", "Yes. Sharing personal info, location, or habits can expose you to identity theft or scams.");
            QnA.Add("What should I do if I get a strange message on social media?", "Don’t click on any links. Report and block the sender. Let your contacts know in case they’re affected too.");
            QnA.Add("Are smart home devices safe?", "They can be, but make sure they’re updated regularly and protected with strong passwords. Don’t ignore those security settings.");
            QnA.Add("What is two-factor authentication and why is it important?", "2FA requires both a password and a second factor like a code sent to your phone. It helps stop hackers even if they get your password.");
            QnA.Add("What are some signs a website might not be safe?", "No HTTPS, pop-up ads, odd domain names, poor design, and requests for sensitive info are all red flags.");
            QnA.Add("What is social engineering?", "It’s manipulating people into giving up confidential information—like phishing, pretexting, or baiting.");
            QnA.Add("Is cybersecurity only a concern for businesses?", "No, individuals are targeted daily via phishing, scams, and malware. Everyone needs basic protection.");
            QnA.Add("What is multifactor authentication (MFA)?", "MFA uses two or more ways to verify your identity: something you know, have, or are (like fingerprints).");
            QnA.Add("Why should software be kept updated?", "Updates fix security vulnerabilities that attackers can exploit.");
            QnA.Add("What are zero-day attacks?", "These exploit unknown or unpatched vulnerabilities—software makers haven’t had time to fix them yet.");
            QnA.Add("What is two-factor authentication?", "Two-factor authentication (2FA) adds an extra layer of security by requiring a second form of verification, such as a code sent to your phone.");
            QnA.Add("What is end-to-end encryption?", "End-to-end encryption ensures that only the sender and recipient can read the message, preventing even the service provider from accessing it.");
            QnA.Add("What is the dark web?", "The dark web is a hidden part of the internet accessible only with special software, often used for anonymous activities, some of which may be illegal.");
            QnA.Add("Why is public Wi-Fi considered risky?", "Public Wi-Fi is insecure and can be exploited by hackers to intercept your data or launch man-in-the-middle attacks.");
            QnA.Add("What is a security patch?", "A security patch is a software update that fixes known vulnerabilities to protect against exploitation.");
            QnA.Add("What does 'zero trust' mean in cybersecurity?", "'Zero trust' is a security model where no user or device is automatically trusted, even if inside the network.");
            QnA.Add("What is a digital certificate?", "A digital certificate verifies the identity of a website or device and helps enable encrypted communications.");
            QnA.Add("How can I recognize a secure website?", "Look for 'https://' in the URL and a padlock icon in the address bar. These indicate secure, encrypted connections.");
            QnA.Add("What is phishing-as-a-service?", "Phishing-as-a-service is when cybercriminals offer phishing kits or services for rent, making it easier for others to run scams.");
            QnA.Add("What is a cyber hygiene checklist?", "Cyber hygiene includes regular actions like updating software, using strong passwords, and reviewing account activity to stay secure.");
            QnA.Add("What is multi-layered security?", "Multi-layered security involves using different types of defenses (firewalls, antivirus, 2FA, etc.) to protect against various threats.");
            QnA.Add("What is the principle of least privilege?", "It means users and applications should only have the access necessary to perform their tasks—nothing more.");
            QnA.Add("Why is it important to back up data?", "Backups protect against data loss due to malware, hardware failure, or accidental deletion, allowing recovery when needed.");
            QnA.Add("What are insider threats?", "Insider threats are security risks that originate from employees or other trusted individuals within an organization.");
            QnA.Add("How can I stay informed about cybersecurity threats?", "Follow trusted security blogs, set alerts for breach news, or subscribe to newsletters from cybersecurity organizations like CISA or NCSC.");
            QnA.Add("How can I create a strong password??", "Use at least 12 characters with a mix of upper- and lowercase letters, numbers, and symbols. Avoid common words or patterns.");
            QnA.Add("Should I use the same password for multiple accounts?", "No. Reusing passwords makes all your accounts vulnerable if one is compromised.");
            QnA.Add("What's a password manager and should I use one?", "A password manager stores and encrypts your passwords so you don’t have to remember them. Yes, it’s highly recommended.");
            QnA.Add("How often should I change my passwords??", "Change passwords when there's a suspected breach or every 6–12 months for sensitive accounts.");
            QnA.Add("What is a passphrase and is it better than a password?", "A passphrase is a longer sequence of words that's easier to remember and harder to crack, often more secure than short passwords.");
            QnA.Add("How can I check if my password has been exposed?", "Use sites like 'Have I Been Pwned' to see if your email or password appears in known data breaches.");
            QnA.Add("What should I do if I think my password was stolen?", "Change it immediately, enable 2FA, and monitor your accounts for suspicious activity.");
            QnA.Add("How can I limit what data apps collect about me?", "Review app permissions and only grant access to what's necessary. Disable location tracking when not needed.");
            QnA.Add("Should I use a VPN?", "Yes, a VPN helps protect your privacy by encrypting your internet traffic and hiding your IP address.");
            QnA.Add("How can I stop websites from tracking me?", "Use privacy-focused browsers, clear cookies regularly, and consider browser extensions that block trackers.");
            QnA.Add("Is private browsing really private?", "It prevents local history and cookie storage, but your ISP or websites can still track you.");
            QnA.Add("How do I know what data companies have on me?", "Check your account privacy settings. Many services offer data export or access tools under GDPR or CCPA.");
            QnA.Add("Can my phone camera or mic be used to spy on me?", "Yes, in rare cases. Always monitor app permissions and use hardware covers if you're concerned.");
            QnA.Add("What should I do before selling or donating my phone?", "Factory reset your device and remove any linked accounts. Back up and wipe all personal data.");
            QnA.Add("How can I remember complex passwords without writing them down?", "Use a password manager to store them securely, or create memorable passphrases made of unrelated but vivid words.");
            QnA.Add("Why is using a unique password for every account important?", "If one site gets breached, reused passwords let attackers access your other accounts easily.");
            QnA.Add("How can I check if my passwords are still secure over time?", "Regularly run your emails through breach-checking tools like Have I Been Pwned, and update exposed passwords immediately.");
            QnA.Add("How do password managers keep my data safe?", "They encrypt your data locally, so only you can unlock it with a master password or biometric login.");
            QnA.Add("Why shouldn't I use personal information in my passwords?", "Names, birthdays, and favorite things are easy to guess or find through social media and phishing.");
            QnA.Add("How can I set up two-factor authentication (2FA) to protect my logins?", "Enable 2FA in your account settings and use an app like Authy or Google Authenticator for added security.");
            QnA.Add("Why is a longer password usually safer than a short one?", "Longer passwords increase the number of possible combinations, making brute-force attacks exponentially harder.");
            QnA.Add("How can I control who sees my personal information online?", "Review privacy settings on social media and apps, limit profile visibility, and avoid oversharing publicly.");
            QnA.Add("Why is using public Wi-Fi risky without a VPN?", "Because public networks can be monitored or spoofed, letting attackers intercept your data unless it's encrypted.");
            QnA.Add("How do browser extensions help improve my privacy?", "Extensions like uBlock Origin or Privacy Badger block trackers and ads that collect your data.");
            QnA.Add("Why should I review app permissions regularly?", "Apps may request access to data they don’t need — like location or mic — and over time this can compromise your privacy.");
            QnA.Add("How can I limit how much companies track me online?", "Use browsers with tracking protection, reject non-essential cookies, and regularly delete browsing data.");
            QnA.Add("How does disabling location services improve my privacy?", "It prevents apps and advertisers from building a profile based on your movements and routines.");
            QnA.Add("Why should I think before posting photos online?", "Photos can reveal location, home layouts, and personal habits, even unintentionally.");
            QnA.Add("How can I recognize a scam before falling for it?", "Be skeptical of urgent requests, unfamiliar links, or messages asking for personal info. Verify the source before responding.");
            QnA.Add("Why do scammers impersonate companies or people I know?", "They use familiarity to lower your guard and make you act quickly — always double-check with the real person or company.");
            QnA.Add("How can I protect myself from investment scams?", "Research thoroughly, avoid promises of guaranteed returns, and never send money to unknown or unregulated sources.");
            QnA.Add("Why do scams often create a sense of urgency?", "Rushing you reduces your ability to think critically and increases the chance you’ll act without verifying.");
            QnA.Add("How do scammers use QR codes or fake websites to trick people?", "They link to malicious sites that look real — always preview a link and scan only codes from trusted sources.");
            QnA.Add("How can I report a scam and help protect others?", "Report it to local fraud authorities, the FTC (in the US), or online platforms — this helps stop the scam from spreading.");

            // Online Safety
            QnA.Add("What threats exist online?", "There are many threats to be aware of online. Some of the most prevalent are:\nData Breaches\nMalware\nViruses\nPhishing\nRomance Scams\nIdentity Theft.\nYou can ask me about them if you want to learn more!");
            QnA.Add("How can I stay safe online?", "Think before clicking, use strong passwords, keep software updated, be wary of sharing personal information, and use security software.");
            QnA.Add("What can I do to be safe online?", "Think before clicking, use strong passwords, keep software updated, be wary of sharing personal information, and use security software.");

            // Data Breaches & Personal Information
            QnA.Add("What is a data breach?", "A data breach is the unauthorized access, disclosure, or loss of sensitive information.");
            QnA.Add("Am I safe from data breaches?", "You can't be completely safe from data breaches. However, being careful about who you give information to online can help reduce your risk in the event of data breaches.");
            QnA.Add("What should I do if my data has been breached?", "Change your passwords, monitor accounts for suspicious activity, enable two-factor authentication, and consider credit monitoring services.");
            QnA.Add("How can I know if I've been affected by a data breach?", "Check websites like HaveIBeenPwned.com or watch for breach notifications from companies you use.");
            QnA.Add("How do I know if my data is exposed?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("Can I check if my data is safe?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("Have I been exposed online?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("What if my data has leaked?", "Change your passwords and keep an eye out for suspicious activity on any of your online accounts. Consider alerting your bank.");
            QnA.Add("What kind of data is usually stolen in a data breach?", "Often email addresses, passwords, credit card details, and personal identifiers like Social Security numbers are targeted.");
            QnA.Add("Can I recover from a data breach?", "Yes, but it takes action: change credentials, enable 2FA, monitor accounts, and possibly freeze credit.");
            QnA.Add("How do data breaches usually happen?", "They occur through hacking, phishing, poor security, stolen devices, or insider threats.");
            QnA.Add("Should I change all my passwords after a breach?", "At least change passwords for the affected service and any accounts where that password was reused.");
            QnA.Add("How can I monitor if my data has been leaked?", "Use services like HaveIBeenPwned.com to check for your email in known breaches.");
            QnA.Add("What is a credential stuffing attack?", "Hackers use leaked credentials from one site to try logging into other accounts.");
            QnA.Add("What is the role of encryption in preventing breaches?", "Encryption ensures that even if data is stolen, it’s unreadable without the decryption key.");

            // Phishing & Scams
            QnA.Add("What is a phishing attack?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
            QnA.Add("What are phising attacks?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
            QnA.Add("What is phishing?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
            QnA.Add("How can I recognize a phishing email?", "Look for suspicious sender addresses, generic greetings, grammatical errors, urgent language, and unexpected attachments or links.");
            QnA.Add("What should I do if I clicked on a phishing link?", "Disconnect from the internet, run a malware scan, change your passwords, and report the incident to your IT department or email provider.");
            QnA.Add("Are phishing attacks only done via email?", "No, phishing can occur via text messages (smishing), phone calls (vishing), and even social media messages.");
            QnA.Add("What is spear phishing?", "Spear phishing is a targeted phishing attack aimed at a specific individual or organization.");
            QnA.Add("What is whaling?", "Whaling is a phishing attack targeting high-profile individuals like executives.");
            QnA.Add("What is a romance scam?", "A romance scam is a deceptive online relationship used to manipulate someone into sending money or personal information.");
            QnA.Add("What are romance scams?", "A romance scam is a deceptive online relationship used to manipulate someone into sending money or personal information.");
            QnA.Add("What should I do if I receive a suspicious email?", "Do not click on any links or open attachments. Mark it as spam or phishing and delete it.");
            QnA.Add("What should you do if you receive a suspicious email?", "Do not click on links or open attachments. Report it to your IT or security team.");
            QnA.Add("What is smishing?", "Smishing is phishing via SMS or text message.");
            QnA.Add("What is vishing?", "Vishing is voice phishing conducted over the phone.");
            QnA.Add("Are phishing emails always obvious?", "Not anymore. Many are highly sophisticated and can mimic real organizations very convincingly.");
            QnA.Add("What is spear phishing", "Spear phishing is a targeted scam that uses personalized info to trick specific individuals or companies.");
            QnA.Add("What should I do if I clicked on a phishing link", "Disconnect from the internet, run a malware scan, and change your passwords immediately.");
            QnA.Add("Can phishing happen via social media?", "Yes, attackers may use fake profiles or links in messages and posts to trick users into revealing info.");
            QnA.Add("What are common phishing red flags?", "Generic greetings, urgent language, suspicious links, poor grammar, and requests for personal data.");

            // Identity Theft
            QnA.Add("What is identity theft?", "Identity theft is the fraudulent use of someone else's personal information for financial gain.");
            QnA.Add("How can someone steal my identity?", "Someone can steal your identity through various methods like phishing, data breaches, physical theft of documents, and malware");
            QnA.Add("What can I do if someone has stolen my identity?", "Report to relevant authorities banks/police, freeze bank cards, change compromised account passwords, monitor financial accounts closely.");
            QnA.Add("What can I do if someone is using my identity?", "Report to relevant authorities banks/police, freeze bank cards, change compromised account passwords, monitor financial accounts closely.");
            QnA.Add("How do I know if my identity has been stolen?", "Watch for unfamiliar charges, changes to credit scores, or notifications about accounts you didn’t open.");
            QnA.Add("How can I prevent identity theft?", "Use strong passwords, avoid oversharing personal info, shred sensitive documents, and monitor your credit reports regularly.");
            QnA.Add("Can identity theft happen from just an email address?", "Alone, an email address isn’t enough, but combined with leaked personal info, it can help fuel attacks.");
            QnA.Add("What is synthetic identity theft?", "It’s when criminals combine real and fake data to create new identities and commit fraud.");
            QnA.Add("How can I report identity theft?", "In the U.S., visit IdentityTheft.gov. In other countries, contact your national consumer protection agency.");
            QnA.Add("Can someone steal my identity with just my name and birthday?", "It’s a starting point, but usually more data is needed to commit full identity theft.");
            QnA.Add("How long can identity theft effects last?", "Victims can deal with consequences for years—credit damage, legal trouble, and financial loss.");

            // Security Tools and Techniques
            QnA.Add("What is a firewall?", "A firewall is a security system that monitors and controls incoming and outgoing network traffic.");
            QnA.Add("What is antivirus software?", "Antivirus software detects, prevents, and removes malicious software from your computer.");
            QnA.Add("What is two-factor authentication", "Two-factor authentication adds an extra layer of security by requiring a second verification step 1  beyond your password.");
            QnA.Add("What is two-factor authentication (2FA)?", "2FA is an extra layer of security requiring not only a password but also a second form of verification.");
            QnA.Add("What is multi-factor authentication?", "It is a security system that requires more than one method of verification to access data.");
            QnA.Add("What is a password manager?", "It’s a tool that helps users create, store, and manage secure passwords.");
            QnA.Add("What is patch management?", "Patch management is the process of updating software to fix vulnerabilities.");
            QnA.Add("Why is updating software important?", "Updates patch security vulnerabilities and help protect against new threats.");

            // Public Wi-Fi & Network Security
            QnA.Add("What are the risks of using public Wi-Fi?", "Public Wi-Fi can be less secure, making your data vulnerable to interception.");
            QnA.Add("Why is public Wi-Fi risky?", "Public Wi-Fi networks are often unsecured and can expose your data to hackers.");
            QnA.Add("What is network segmentation?", "Network segmentation divides a network into smaller parts to improve security.");
            QnA.Add("What is BYOD?", "Bring Your Own Device (BYOD) allows employees to use personal devices for work.");
            QnA.Add("What is endpoint security?", "Endpoint security protects network-connected devices like computers and smartphones.");
            QnA.Add("Is using a VPN on public Wi-Fi enough?", "It greatly reduces risk, but it’s also wise to avoid entering sensitive information on public networks when possible.");
            QnA.Add("Can I use a VPN for streaming or bypassing censorship?", "Yes, many people use VPNs to access geo-restricted content or browse freely in countries with internet censorship.");

            // Technical Threats & Advanced Topics
            QnA.Add("What is ransomware?", "Ransomware is malware that encrypts your data and demands payment for the decryption key.");
            QnA.Add("How does ransomware infect a system?", "It often spreads through phishing emails, malicious attachments, software vulnerabilities, or compromised websites.");
            QnA.Add("Should I pay the ransom if my files are encrypted?", "Authorities advise against paying, as it doesn't guarantee file recovery and encourages future attacks. Focus on backups and recovery instead.");
            QnA.Add("What is spyware?", "Spyware is software that secretly gathers information about a person or organization without their knowledge.");
            QnA.Add("What is adware?", "Adware is software that automatically displays or downloads advertising material.");
            QnA.Add("What is a botnet?", "A botnet is a network of infected devices controlled by a cybercriminal.");
            QnA.Add("What is a zero-day vulnerability?", "A zero-day is a software flaw unknown to the vendor, leaving systems exposed to attacks.");
            QnA.Add("What is social engineering", "Social engineering is manipulating people into revealing confidential information.");
            QnA.Add("What is a brute-force attack?", "A brute-force attack tries all possible combinations to crack passwords.");
            QnA.Add("What is a denial-of-service (DoS) attack?", "A DoS attack overwhelms a system, making it unavailable to users.");
            QnA.Add("What is a distributed denial-of-service (DDoS) attack?", "A DDoS attack uses multiple systems to flood a target and disrupt service.");

            // Cybersecurity Concepts & Frameworks
            QnA.Add("What is cybersecurity?", "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.");
            QnA.Add("What is information security?", "Information security involves protecting data from unauthorized access or alterations.");
            QnA.Add("What is the CIA triad?", "The CIA triad stands for Confidentiality, Integrity, and Availability, core principles of cybersecurity.");
            QnA.Add("What is risk management?", "Risk management is identifying and reducing potential threats to information assets.");
            QnA.Add("What is the principle of least privilege", "It means users should have only the access necessary to do their job.");
            QnA.Add("What is threat modeling?", "Threat modeling is identifying and addressing potential threats to a system.");
            QnA.Add("What is penetration testing?", "Penetration testing simulates cyberattacks to find security weaknesses.");
            QnA.Add("What is encryption?", "Encryption is converting data into a code to prevent unauthorized access.");
            QnA.Add("What is hashing?", "Hashing is converting data into a fixed-size string, usually for security or integrity checking.");
            QnA.Add("What is digital forensics?", "Digital forensics involves investigating and recovering data from digital devices.");
            QnA.Add("What is the difference between a virus and a worm?", "A virus needs a host to spread; a worm can spread on its own.");
            QnA.Add("What are the main areas of cybersecurity?", "Key areas include network security, application security, information security, operational security, and disaster recovery.");
            QnA.Add("Who is responsible for cybersecurity?", "Everyone plays a role—from individuals practicing safe behavior to organizations implementing policies and protections.");

            // Privacy, Data Protection & Compliance
            QnA.Add("What is personal data?", "Personal data is information that can identify a person, such as name, ID number, or email address.");
            QnA.Add("What is GDPR?", "The General Data Protection Regulation (GDPR) is a European law that protects personal data and privacy.");
            QnA.Add("What is POPIA?", "The Protection of Personal Information Act (POPIA) is South African legislation protecting personal information.");
            QnA.Add("What is the POPI Act?", "The Protection of Personal Information Act (POPIA) is South African legislation protecting personal information.");
            QnA.Add("How can I protect my privacy online?", "Limit what you share, use privacy settings, avoid oversharing on social media, and consider using VPNs and private browsers.");
            QnA.Add("Is using incognito mode enough for privacy?", "Incognito mode prevents local history tracking but doesn't hide your activity from websites, ISPs, or employers.");

            // Incident Response & Reporting
            QnA.Add("What should I do if I think I’ve been hacked?", "Change passwords immediately, disconnect from the internet, run antivirus scans, and report it to your IT or security team.");
            QnA.Add("What should I do after a data breach?", "Change passwords, monitor accounts, notify affected parties, and follow your organization’s incident response plan.");
            QnA.Add("How should I report a cybercrime?", "Contact your local law enforcement or cybercrime unit. In South Africa, contact the Cybercrime Division of the SAPS.");
            QnA.Add("Who do I contact if I’ve been scammed online?", "Report to your bank and contact the Cybercrime Division of the SAPS. You can also report on the Cybersecurity Hub (www.cybersecurityhub.gov.za).");

            // Cybersecurity Culture & Awareness
            QnA.Add("Why is cybersecurity awareness important?", "It helps people recognize threats, avoid risky behaviors, and respond appropriately to incidents.");
            QnA.Add("What is cyber hygiene?", "Cyber hygiene refers to practices that help maintain good digital security, like regular updates and strong passwords.");
            QnA.Add("Why should I care about cybersecurity?", "Everyone is a potential target. Good cybersecurity helps protect your identity, money, and personal data.");
            QnA.Add("How can a cyberattack impact me personally?", "It can lead to identity theft, financial loss, privacy invasion, or damage to your digital reputation.");
            QnA.Add("Is cybersecurity only important for businesses?", "No, individuals are frequent targets too. Personal devices, accounts, and data can all be compromised.");

            // Kids & Online Safety
            QnA.Add("How can I keep my children safe online?", "Use parental controls, talk about internet safety, monitor online activity, and encourage open conversations.");
            QnA.Add("What should kids know about internet safety?", "They should know not to share personal info, to ask for help if something feels wrong, and to be respectful online.");
            QnA.Add("How can I teach my kids about cybersecurity?", "Use games, videos, and apps designed for online safety education. Lead by example.");
            QnA.Add("What is two-factor authentication and why is it important", "It adds an extra layer of security by requiring a second form of verification, such as a text message or authentication app.");
            QnA.Add("Are public Wi-Fi networks safe to use?", "Public Wi-Fi can be risky. Use a VPN or avoid accessing sensitive accounts when connected to them.");
            QnA.Add("What kind of parental controls should I use?", "Use tools that filter content, monitor activity, limit screen time, and block unknown contacts.");
            QnA.Add("How do I talk to my kids about online dangers?", "Be open and age-appropriate. Explain risks like talking to strangers, sharing personal info, and cyberbullying.");
            QnA.Add("What apps are safest for kids?", "Apps with strong privacy controls, educational value, and no chat features (unless moderated) are generally safer.");
            QnA.Add("How can I tell if my child is being cyberbullied?", "Watch for signs like anxiety, secrecy about online activity, or sudden changes in behavior.");
            QnA.Add("Should kids use social media?", "Only if age-appropriate and supervised. Monitor who they connect with and teach them about online risks.");
            QnA.Add("How do I block inappropriate content?", "Use parental controls on devices, browsers, and home Wi-Fi to filter adult or unsafe content.");
            QnA.Add("What is a digital footprint and why does it matter?", "It’s the trail of data kids leave online. Teaching good habits early helps prevent privacy issues later.");

            // Safe Browsing
            QnA.Add("What is safe browsing?", "Safe browsing means using the internet cautiously by avoiding suspicious links, ensuring websites are secure, and keeping software up to date.");
            QnA.Add("How can I identify a phishing website?", "Look for spelling errors, suspicious URLs, lack of HTTPS, and requests for personal information.");
            QnA.Add("Should I click on pop-up ads or unknown links?", "No, avoid clicking on pop-ups or unknown links as they may contain malware or lead to phishing sites.");
            QnA.Add("Why is it important to keep my browser updated?", "Browser updates fix security vulnerabilities and help protect against the latest online threats.");
            QnA.Add("Can I use public Wi-Fi for work-related tasks?", "Avoid using public Wi-Fi for sensitive tasks unless you're connected through a secure VPN.");
            QnA.Add("What should I do if I accidentally visit a suspicious site?", "Close the site immediately, clear your browser cache, and report the incident to your IT department.");
            QnA.Add("Is it safe to save passwords in my browser?", "It's better to use a trusted password manager; browser-stored passwords can be vulnerable.");
            QnA.Add("What is HTTPS and why does it matter?", "HTTPS encrypts your connection to a website, helping protect your data from interception.");
            QnA.Add("How do I know if a website is secure?", "Look for HTTPS in the URL and a padlock icon next to the address bar.");

            // Workplace Security
            QnA.Add("What is a security policy?", "A security policy outlines rules and procedures for protecting digital and physical assets.");
            QnA.Add("What is acceptable use?", "Acceptable use refers to policies that define how employees can use company systems and data.");
            QnA.Add("Why should I lock my computer when away?", "Locking your screen helps prevent unauthorized access to your data.");
            QnA.Add("How do I protect my data at work?", "Lock your screen, use strong passwords, follow policies, and report suspicious activity.");
            //END CHATGPT

            // EXIT (return to main menu)
            //QnA.Add("No more questions", "Thanks for chatting!");
            //QnA.Add("No thanks", "Thanks for chatting!");
            //QnA.Add("No", "Thanks for chatting!");
            //QnA.Add("Goodbye", "Thanks for chatting!");
            //QnA.Add("Good bye", "Thanks for chatting!");
            //QnA.Add("Bye", "Thanks for chatting!");
            //QnA.Add("Back", "Thanks for chatting!");
            //QnA.Add("Exit", "Thanks for chatting!");
            //QnA.Add("Quit", "Thanks for chatting!");
            //QnA.Add("Main menu", "Thanks for chatting!");
        }


        //=========================================================//
        // Tips for when a tip is asked for.
        // ** Tips written by Copilot **
        private void InitialiseTips()
        {
            Tips.Add("password tip",
                new List<string>
                {
                    "Use a unique password for every account.",
                    "Do not reuse passwords. Reusing passwords can compromise multiple accounts if a single password is leaked somewhere.",
                    "Change your passwords regularly, especially if you suspect an account may have been compromised.",
                    "Use a password manager to store your credentials.",
                    "Create strong passwords with a mix of letters, numbers, and symbols.",
                    "Avoid using personal information such as your name, birthdate, or common words in your passwords.",
                    "Avoid using personal information in your passwords."
                });

            Tips.Add("phishing tip",
                new List<string>
                {
                    "Never click on suspicious links in emails or texts.",
                    "Verify sender addresses before responding to emails.",
                    "Be cautious of urgent requests for personal information.",
                    "Report phishing attempts to your IT/security team.",
                    "Be cautious of unsolicited emails or messages, especially those asking for personal information.",
                    "Be careful about what you post on social media; avoid sharing sensitive personal information.",
                    "Check website URLs for HTTPS and a padlock icon before entering sensitive information.",
                    "Enable two-factor authentication (2FA) on your important accounts for extra security."
                });

            Tips.Add("malware tip",
                new List<string>
                {
                    "Never download software or attachments from unknown or untrusted sources.",
                    "Keep your antivirus and anti-malware software up to date.",
                    "Avoid clicking on pop-up ads or suspicious links.",
                    "Use real-time protection features in your security software.",
                    "Be cautious when using USB drives or external media—scan them before use.",
                    "Regularly back up your data to an offline or cloud source.",
                    "Do not disable built-in security features like Windows Defender or firewall without a good reason.",
                    "Educate yourself and others about common malware tactics like trojans, ransomware, and spyware."
                });

            Tips.Add("scam tip",
                new List<string>
                {
                    "Be skeptical of deals that sound too good to be true.",
                    "Verify the identity of anyone asking for money or personal information.",
                    "Don't trust caller ID—scammers can spoof numbers.",
                    "Avoid wiring money or sending gift cards to strangers.",
                    "Report scams to your local authorities or consumer protection agency.",
                    "Use multi-factor authentication to reduce risk even if credentials are stolen.",
                    "Check URLs carefully before entering sensitive information."
                });

            Tips.Add("privacy tip",
                new List<string>
                {
                    "Review app permissions and limit data access where possible.",
                    "Use encrypted messaging services for private communication.",
                    "Avoid oversharing personal details on social media.",
                    "Regularly review and update privacy settings on all online accounts.",
                    "Use browser extensions that block trackers and third-party cookies.",
                    "Use a VPN on public networks to obscure your internet activity.",
                    "Delete unused accounts and limit data collected by services."
                });

            Tips.Add("virus tip",
                new List<string>
                {
                    "Install reputable antivirus software and keep it up to date.",
                    "Do not open email attachments unless you trust the source.",
                    "Be cautious when downloading files from the internet or peer-to-peer networks.",
                    "Avoid pirated software, which is often bundled with malware or viruses.",
                    "Regularly scan your system for threats.",
                    "Keep your operating system and software patched with the latest updates.",
                    "Use email filters and security software to detect malicious content before it reaches you."
                });
        }


        //=========================================================//
        // Responses to be used as follow-ups to bot answers
        private void InitialiseFollowUps()
        {
            // Follow-ups
            FollowUps = new List<string>
            {
                "Is there anything else you want to know?",
                "Can I help you with anything else?",
                "What else would you like to know?"
            };
        }


        //=========================================================//
        // Topic specific follow-ups
        public List<string> GetFollowUps(string topic)
        {
            topic = Pluralise(topic);

            return new List<string>
            {
                $"Do you have any more questions related to {topic}?",
                $"Would you like more tips or information on {topic}?",
                $"Is there a specific part of {topic} you want to know about?",
                $"Can I help clarify anything else regarding {topic}?",
                $"Is there another cybersecurity area you’d like to explore besides {topic}?"
            };
        }


        //=========================================================//
        // Responses to preceed bot answers, based on detected
        // sentiment
        // ** answers written by Copilot **
        private void InitialiseOpeners()
        {
            Openers.AddRange(new[]
            {
                // Worried
                new List<string>
                {
                    "I understand this can be concerning, but I'm here to help.",
                    "It's normal to feel uneasy about this. Let's see what we can do.",
                    "Don't worry, I'll do my best to guide you.",
                    "I can help you work through this. Let's take it step by step."
                },
                // Curious
                new List<string>
                {
                    "That's an interesting question! Let's explore it together.",
                    "I'm glad you're curious! Here's what I found:",
                    "Curiosity is key to learning—let's see what we can discover.",
                    "Great curiosity! Let me help you find the answer."
                },
                // Happy
                new List<string>
                {
                    "I'm glad you're excited! Let's dive in.",
                    "Great to see your enthusiasm! Here's what I found:",
                    "Love the positive energy! Let me help you with that.",
                    "Awesome question! Let's take a look together."
                },
                // Frustrated
                new List<string>
                {
                    "I know this can be frustrating, but let's work through it together.",
                    "Don't worry, we'll figure this out step by step.",
                    "It's okay to feel frustrated—I'm here to help you.",
                    "Let's tackle this problem together and find a solution."
                },
                // Confused
                new List<string>
                {
                    "No worries, that's a great question!",
                    "I understand—that can definitely be confusing.",
                    "Let's break it down together.",
                    "Good catch! That’s not always clear at first.",
                    "You're not the only one who's asked that—happy to explain.",
                    "It can seem tricky at first, but I’ll make it simple."
                },
                // Neutral
                new List<string>
                {
                    "Let me see if I can help with that.",
                    "Here's what I found for you:",
                    "Let's take a look at your question.",
                    "I'll do my best to answer:"
                }
            });
        }


        //=========================================================//
        // Openers for when a user states an interest
        public string GetInterestOpener(string interest)
        {
            interest = Pluralise(interest);

            switch (interest)
            {
                case "malware":
                    return "I see, you're interested in malware. An interest in malware means you likely already know the basics.";
                case "phishing":
                    return "I see, you're interested in phishing. Phishing is a useful interest to have.";
                case "privacy":
                    return "I see, you're interested in privacy. Privacy is one of the most important cybersecurity fields for personal security.";
                case "viruses":
                    return "I see, you;re interested in viruses. You probably also know something about malware then.";
                default:
                    return $"I see, you're interested in {interest}. That's a great field to be interested in.";
            }
        }


        //=========================================================//
        // Recall reponses for when a user interest appear later
        public List<string> RecallInterest(string interest)
        {
            interest = Pluralise(interest);
            return new List<string>
            {
                $"Ah, I remember you mentioned being interested in {interest}. That curiosity really sets you apart!",
                $"Oh yes, you brought up {interest} earlier. You're definitely on the right track—most people don't even think to ask about it!",
                $"You said before that {interest} was something you wanted to learn more about. That kind of initiative is impressive.",
                $"Interesting! Since {interest} is something you’ve looked into, you’re already ahead of the game.",
                $"That fits perfectly—{interest} is something you’ve expressed interest in. Great consistency in your thinking!",
                $"That connects nicely with your interest in {interest}. You're doing a great job tying everything together.",
                $"I see you’re sticking with your focus on {interest}. That’s a strong way to build deeper expertise.",
                $"Since you've shown interest in {interest}, this will make even more sense to you."
            };
        }


        //=========================================================//
        // Words to base sentiment detection off
        // ** Words suggested by Copilot **
        private void InitialiseSentimentWords()
        {
            SentimentWords.AddRange(new[]
            {
                // Worried
                "worried worry concerned anxious nervous uneasy fear scared afraid panic dread stressed tense insecure restless unsure doubt nervousness overwhelmed",
                // Curious
                "curious wondering interested confused question why how what learn learning explore exploring investigate searching search keen eager fascinated",
                // Happy
                "happy joy excited glad cheerful satisfied good love awesome great fun yay thrilled thrilled great amazing positive smiling",
                // Frustrated
                "frustrated annoyed mad angry upset tired stuck blocked hate hatefull irritated exhausted pissed confused overwhelmed helpless sick",
                // Confused
                "confused lost unsure unclear stuck wrong huh weird mistake forgot"
            });
        }


        //=========================================================//
        // converts a topic from singular to plural form
        private string Pluralise(string topic)
        {
            switch (topic)
            {
                case "malware":
                    topic += "";
                    break;
                case "phishing":
                    topic += "";
                    break;
                case "privacy":
                    topic += "";
                    break;
                case "virus":
                    topic += "es";
                    break;
                default:
                    topic += "s";
                    break;
            }
            return topic;
        }


        //========================================================//
        // Populates the quiz questions list
        private void InitialiseQuizQuestions()
        {
            _multipleChoiceQuestions = new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion
                {
                    Question = "What does the acronym 'VPN' stand for?",
                    Choices = new List<string> { "Virtual Private Network", "Virtual Protected Network", "Verified Private Network", "Variable Proxy Node" },
                    Answer = "A",
                    Explanation = "A VPN encrypts your internet traffic and routes it through a secure server to protect your identity and data."
                },
                new MultipleChoiceQuestion
                {
                    Question = "Which of the following is considered a strong password?",
                    Choices = new List<string> { "12345678", "password", "P@ssw0rd#2024", "qwerty" },
                    Answer = "C",
                    Explanation = "Strong passwords use a mix of upper/lowercase letters, numbers, and special characters to increase complexity."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What type of attack involves tricking users into giving up personal information?",
                    Choices = new List < string > { "Phishing", "DDoS", "Brute force", "Spoofing" },
                    Answer = "A",
                    Explanation = "Phishing deceives users with fake emails or sites to steal credentials or financial data."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is the purpose of a firewall?",
                    Choices = new List < string > { "Encrypt data", "Prevent unauthorized access", "Back up data", "Monitor employee activity" },
                    Answer = "B",
                    Explanation = "Firewalls monitor and control network traffic, allowing or blocking traffic based on security rules."
                },
                new MultipleChoiceQuestion
                {
                    Question = "Which is a form of malware that locks you out of your files and demands payment?",
                    Choices = new List < string >    { "Spyware", "Ransomware", "Trojan", "Adware" },
                    Answer = "B",
                    Explanation = "Ransomware encrypts your files and demands payment (usually in cryptocurrency) to restore access."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What does HTTPS indicate in a website URL?",
                    Choices = new List < string > { "The site is hosted on a cloud server", "The site is secure", "The site is fast", "The site is blocked" },
                    Answer = "B",
                    Explanation = "HTTPS uses SSL/TLS encryption, ensuring that data between your browser and the website is protected."
                },
                new MultipleChoiceQuestion
                {
                    Question = "Which one is NOT a type of multi-factor authentication?",
                    Choices = new List < string > { "Password", "Security token", "Fingerprint", "Firewall" },
                    Answer = "D",
                    Explanation = "Firewalls are network security tools, not a method of identity verification like MFA."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is social engineering in cybersecurity?",
                    Choices = new List < string > { "Hardware manipulation", "Manipulating people to gain access", "Network routing", "Using software exploits" },
                    Answer = "B",
                    Explanation = "Social engineering relies on human error or trust rather than technical exploits to gain unauthorized access."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is a zero-day vulnerability?",
                    Choices = new List < string > { "A fully patched system", "A known bug", "An unknown and unpatched flaw", "A software update" },
                    Answer = "C",
                    Explanation = "Zero-days are security holes that are unknown to vendors and exploited before they’re patched."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is a brute-force attack?",
                    Choices = new List < string > { "Trying many passwords until one works", "Destroying hardware", "Flooding a network", "Hijacking a browser" },
                    Answer = "A",
                    Explanation = "It involves systematically guessing passwords until the correct one is found."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What does 'DDoS' stand for?",
                    Choices = new List < string > { "Distributed Denial of Service", "Data Distribution over Systems", "Direct Data over Servers", "Dynamic Defense of Security" },
                    Answer = "A",
                    Explanation = "A DDoS attack overwhelms a system with traffic from multiple sources to make it unavailable."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is the main role of an antivirus program?",
                    Choices = new List < string > { "Back up data", "Prevent hacking", "Detect and remove malware", "Encrypt files" },
                    Answer = "C",
                    Explanation = "Antivirus software scans systems for malicious files and helps remove them."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is two-factor authentication?",
                    Choices = new List < string > { "Logging in from two devices", "Using two passwords", "Using two different types of credentials", "Scanning your computer twice" },
                    Answer = "C",
                    Explanation = "2FA requires something you know (password) and something you have (token) or are (biometric)."
                },
                new MultipleChoiceQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Choices = new List < string > { "A personal message from a friend", "Generic greetings and urgent language", "Correct grammar and spelling", "From a known business" },
                    Answer = "B",
                    Explanation = "Phishing emails often use vague salutations and scare tactics to trick recipients into clicking links."
                }
            };

            _boolQuestions = new List<BoolQuestion>
            {
                new BoolQuestion
                {
                    Question = "Using 'password' as your password is secure.",
                    Answer = false,
                    Explanation = "'password' is one of the most common and weakest passwords. It can be easily guessed or cracked."
                },
                new BoolQuestion
                {
                    Question = "Antivirus software can help detect and remove malware.",
                    Answer = true,
                    Explanation = "Antivirus programs are designed to identify, block, and remove malware from your system."
                },
                new BoolQuestion
                {
                    Question = "Phishing attacks can only occur through email.",
                    Answer = false,
                    Explanation = "Phishing can happen via email, text messages, phone calls, or even social media messages."
                },
                new BoolQuestion
                {
                    Question = "A firewall can help block unauthorized access to your network.",
                    Answer = true,
                    Explanation = "Firewalls act as a barrier between your network and potential threats from outside sources."
                },
                new BoolQuestion
                {
                    Question = "It's safe to use public Wi-Fi for online banking without a VPN.",
                    Answer = false,
                    Explanation = "Public Wi-Fi networks are insecure, and without a VPN, your data can be intercepted by attackers."
                },
                new BoolQuestion
                {
                    Question = "Ransomware can encrypt your files and demand payment.",
                    Answer = true,
                    Explanation = "Ransomware locks your files with encryption and demands payment to provide the decryption key."
                },
                new BoolQuestion
                {
                    Question = "Two-factor authentication adds an extra layer of security.",
                    Answer = true,
                    Explanation = "2FA adds a second form of identity verification, making it harder for attackers to access your account."
                },
                new BoolQuestion
                {
                    Question = "HTTPS is less secure than HTTP.",
                    Answer = false,
                    Explanation = "HTTPS is more secure than HTTP because it encrypts the data sent between your browser and the server."
                },
                new BoolQuestion
                {
                    Question = "It's okay to reuse the same password across multiple sites.",
                    Answer = false,
                    Explanation = "Reusing passwords makes you vulnerable to credential stuffing attacks if one site is compromised."
                },
                new BoolQuestion
                {
                    Question = "Software updates can fix security vulnerabilities.",
                    Answer = true,
                    Explanation = "Updates often include patches for known security flaws, protecting your system from exploits."
                }
            };
        }


        //================================================//
        // Populates RoundQuestions with five random
        // multiple choice and five random true/false 
        // questions
        public void SetRoundQuestions()
        {
            // get 5 random questions of each type
            var multipleChoice = _multipleChoiceQuestions.OrderBy(q => _rand.Next()).Take(5);
            var trueFalse = _boolQuestions.OrderBy(q =>  _rand.Next()).Take(5);
            var roundQuestions = new List<object>();

            foreach(var q in multipleChoice)
            {
                roundQuestions.Add(q);
            }

            foreach (var q in trueFalse)
            {
                roundQuestions.Add(q);
            }
            // shuffle questions
            RoundQuestions = roundQuestions.OrderBy(q => _rand.Next()).Take(10).ToList();
        }


        //================================================//
        // Returns the score (%) of the latest quiz round
        public double GetRoundScore()
        {
            return CorrectAnswers / (RoundQuestions.Count / 100);
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\