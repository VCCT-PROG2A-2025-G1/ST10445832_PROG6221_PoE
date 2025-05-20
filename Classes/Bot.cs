// References
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp


using System.Collections.Generic;
using FuzzySharp;

namespace ST10445832_PROG6221_PoE
{
    internal class Bot
    {
        Dictionary<string, string> QnA = new Dictionary<string, string>();
        string UserName;

        //=========================================================//
        // Default Constructor
        public Bot(string username)
        {
            UserName = username;
            InitialiseQnA();
        }


        //=========================================================//
        // Populate the QnA dictionary with questions and answers
        private void InitialiseQnA()
        {
            // QnA.Add("", "");
            // DEFAULT
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

            // CHATGPT
            QnA.Add("That email looked real—how was I supposed to know it was fake?", "Phishing emails often look legit, but small details like the sender's email address, weird grammar, or urgent demands can give them away. When in doubt, don’t click.");
            QnA.Add("Is it okay to open links if I don’t click on anything else?", "Even just clicking a suspicious link can be risky. It’s best to avoid them unless you're sure they’re safe.");
            QnA.Add("Do I really need different passwords for every site?", "Yep—it’s a pain, but using the same password everywhere is super risky. A password manager can help keep things organized.");
            QnA.Add("I forgot my password again! Is it bad to keep it written down?", "Writing it down isn't ideal, but if you must, keep it in a secure place. Better yet, use a password manager.");
            QnA.Add("Is it really that easy to get hacked?", "Unfortunately, yes—especially if your guard’s down. Even one wrong click can be all it takes.");
            QnA.Add("I'm not tech-savvy. How do I know if something's a threat?", "You don’t have to be an expert—just stay skeptical of unexpected emails, weird links, and too-good-to-be-true offers. When unsure, ask or do a quick search.");
            QnA.Add("if I get ransomware, all my stuff is gone?", "Unless you have a backup, pretty much. That’s why backing up your files is super important.");
            QnA.Add("Can I just unplug my computer if I think I got a virus?", "Actually, yes! Turning off the internet or shutting it down can help stop malware from spreading further.");
            QnA.Add("Is it fine to check my bank account on airport Wi-Fi?", "It’s not recommended. Public Wi-Fi can be sketchy. If you must, use a VPN to be safer.");
            QnA.Add("Do I really need a VPN?", "If you care about privacy—especially on public networks—then yes, it’s a good idea.");
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
            QnA.Add("How often should I change my passwords?", "Every few months is a good rule. Or immediately if you think an account has been compromised.");
            QnA.Add("Is clicking 'Remember Me' on websites a bad idea?", "It's convenient, but it can be risky on public or shared devices. Only use it on your personal, secure devices.");
            QnA.Add("Is cybersecurity just for people who work in tech?", "Nope! Everyone who uses the internet should care. Hackers go after regular users all the time.");
            QnA.Add("What’s the worst that could happen if I get hacked?", "You could lose money, have your identity stolen, or your private info leaked. It’s more common than you’d think.");
            QnA.Add("Are smart home devices safe?", "They can be, but make sure they’re updated regularly and protected with strong passwords. Don’t ignore those security settings.");

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

            // Malware, Viruses & Threats
            QnA.Add("What is malware?", "Malware is malicious software designed to harm computer systems.");
            QnA.Add("What are common types of malware?", "Common types include viruses, worms, trojans, ransomware, spyware, and adware.");
            QnA.Add("How does malware get onto my computer?", "Malware can be downloaded through malicious websites, email attachments, software from untrusted sources, or USB drives.");
            QnA.Add("What are signs my device might be infected with malware?", "Slow performance, frequent crashes, unexpected pop-ups, unknown programs launching, or unusual network activity.");
            QnA.Add("What can I do to be safe from malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("What is a virus?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("What are viruses?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("How can I be safe frome viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("Is antivirus software enough to keep me safe?", "Antivirus helps, but it's not foolproof. You should also practice good security habits and keep your system updated.");
            QnA.Add("Do I need antivirus on my phone?", "Yes, especially on Android devices, as they can be vulnerable to malicious apps and websites.");


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

            // Identity Theft
            QnA.Add("What is identity theft?", "Identity theft is the fraudulent use of someone else's personal information for financial gain.");
            QnA.Add("How can someone steal my identity?", "Someone can steal your identity through various methods like phishing, data breaches, physical theft of documents, and malware");
            QnA.Add("What can I do if someone has stolen my identity?", "Report to relevant authorities banks/police, freeze bank cards, change compromised account passwords, monitor financial accounts closely.");
            QnA.Add("What can I do if someone is using my identity?", "Report to relevant authorities banks/police, freeze bank cards, change compromised account passwords, monitor financial accounts closely.");

            // Security Tools and Techniques
            QnA.Add("What is a VPN?", "VPN stands for Virtual Private Network. It's a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");
            QnA.Add("What is a virtual private network?", "A virtual private network, or VPN, is a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");
            QnA.Add("Why is it important to use a VPN?", "A VPN encrypts your internet traffic and can help protect your privacy and security, especially on public Wi-Fi.");
            QnA.Add("What is a VPN and how does it help protect privacy?", "A VPN (Virtual Private Network) encrypts your internet connection, hiding your data and browsing activity from third parties.");
            QnA.Add("What is a firewall?", "A firewall is a security system that monitors and controls incoming and outgoing network traffic.");
            QnA.Add("What is antivirus software?", "Antivirus software detects, prevents, and removes malicious software from your computer.");
            QnA.Add("What is two-factor authentication?", "Two-factor authentication adds an extra layer of security by requiring a second verification step 1  beyond your password.");
            QnA.Add("What is two-factor authentication (2FA)?", "2FA is an extra layer of security requiring not only a password but also a second form of verification.");
            QnA.Add("What is multi-factor authentication?", "It is a security system that requires more than one method of verification to access data.");
            QnA.Add("What is a password manager?", "It’s a tool that helps users create, store, and manage secure passwords.");
            QnA.Add("What is patch management?", "Patch management is the process of updating software to fix vulnerabilities.");
            QnA.Add("Why is updating software important?", "Updates patch security vulnerabilities and help protect against new threats.");

            // Passwords & Authentication
            QnA.Add("How can I create a strong password?", "A strong password should be long, unique, and include a mix of uppercase and lowercase letters, numbers, and symbols.");
            QnA.Add("What is a strong password?", "A strong password is long, complex, and includes a mix of letters, numbers, and symbols.");
            QnA.Add("What is a secure password policy?", "It’s a set of rules for creating and maintaining strong passwords.");
            QnA.Add("Why should you avoid reusing passwords?", "Reusing passwords increases risk if one account is compromised.");

            // Public Wi-Fi & Network Security
            QnA.Add("What are the risks of using public Wi-Fi?", "Public Wi-Fi can be less secure, making your data vulnerable to interception.");
            QnA.Add("Why is public Wi-Fi risky?", "Public Wi-Fi networks are often unsecured and can expose your data to hackers.");
            QnA.Add("What is network segmentation?", "Network segmentation divides a network into smaller parts to improve security.");
            QnA.Add("What is BYOD?", "Bring Your Own Device (BYOD) allows employees to use personal devices for work.");
            QnA.Add("What is endpoint security?", "Endpoint security protects network-connected devices like computers and smartphones.");

            // Technical Threats & Advanced Topics
            QnA.Add("What is ransomware?", "Ransomware is malware that encrypts your data and demands payment for the decryption key.");
            QnA.Add("How does ransomware infect a system?", "It often spreads through phishing emails, malicious attachments, software vulnerabilities, or compromised websites.");
            QnA.Add("Should I pay the ransom if my files are encrypted?", "Authorities advise against paying, as it doesn't guarantee file recovery and encourages future attacks. Focus on backups and recovery instead.");
            QnA.Add("What is spyware?", "Spyware is software that secretly gathers information about a person or organization without their knowledge.");
            QnA.Add("What is adware?", "Adware is software that automatically displays or downloads advertising material.");
            QnA.Add("What is a botnet?", "A botnet is a network of infected devices controlled by a cybercriminal.");
            QnA.Add("What is a zero-day vulnerability?", "A zero-day is a software flaw unknown to the vendor, leaving systems exposed to attacks.");
            QnA.Add("What is social engineering?", "Social engineering is manipulating people into revealing confidential information.");
            QnA.Add("What is a brute-force attack?", "A brute-force attack tries all possible combinations to crack passwords.");
            QnA.Add("What is a denial-of-service (DoS) attack?", "A DoS attack overwhelms a system, making it unavailable to users.");
            QnA.Add("What is a distributed denial-of-service (DDoS) attack?", "A DDoS attack uses multiple systems to flood a target and disrupt service.");

            // Cybersecurity Concepts & Frameworks
            QnA.Add("What is cybersecurity?", "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.");
            QnA.Add("What is information security?", "Information security involves protecting data from unauthorized access or alterations.");
            QnA.Add("What is the CIA triad?", "The CIA triad stands for Confidentiality, Integrity, and Availability, core principles of cybersecurity.");
            QnA.Add("What is risk management?", "Risk management is identifying and reducing potential threats to information assets.");
            QnA.Add("What is the principle of least privilege?", "It means users should have only the access necessary to do their job.");
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
            QnA.Add("What is two-factor authentication and why is it important?", "It adds an extra layer of security by requiring a second form of verification, such as a text message or authentication app.");
            QnA.Add("Are public Wi-Fi networks safe to use?", "Public Wi-Fi can be risky. Use a VPN or avoid accessing sensitive accounts when connected to them.");


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

            // Miscellaneous
            QnA.Add("What is a digital footprint?", "A digital footprint is the trail of data you leave behind when using the internet.");
            QnA.Add("What is cyberbullying?", "Cyberbullying is using digital communication to harass, threaten, or humiliate others.");
            QnA.Add("Can social media put me at risk?", "Yes. Sharing personal info, location, or habits can expose you to identity theft or scams.");
            QnA.Add("What should I do if I get a strange message on social media?", "Don’t click on any links. Report and block the sender. Let your contacts know in case they’re affected too.");

            //END CHATGPT


            // EXIT (return to main menu)
            QnA.Add("No more questions", "Thanks for chatting!");
            QnA.Add("No", "Thanks for chatting!");
            QnA.Add("No thanks", "Thanks for chatting!");
            QnA.Add("Goodbye", "Thanks for chatting!");
            QnA.Add("Good bye", "Thanks for chatting!");
            QnA.Add("Bye", "Thanks for chatting!");
            QnA.Add("Back", "Thanks for chatting!");
            QnA.Add("Exit", "Thanks for chatting!");
            QnA.Add("Main menu", "Thanks for chatting!");
        }


        //=========================================================//
        // Use the FuzzySharp package to return an answer based
        // on a similarity score, comparing the user question to
        // the questions in the QnA dictionary
        public string AnswerQuestion(string userQuestion)
        {
            // best match score
            int fuzzMax = 0;
            // best match question
            string bestMatch = "";
            foreach (string question in QnA.Keys)
            {
                // compare the question to questions which have answers, ignoring the order of the words in the question
                var score = Fuzz.TokenSortRatio(userQuestion.ToLower(), question.ToLower());
                // only provide a non-default answer if the similarity score is higher than 60
                if (score > 60 && score > fuzzMax)
                {
                    fuzzMax = score;
                    bestMatch = question;
                }
            }
            // return the most relevant answer
            return QnA[bestMatch];
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\