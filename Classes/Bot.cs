// References
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp


using System.Collections.Generic;
using FuzzySharp;

namespace ST10445832_PROG6221_Part1
{
    internal class Bot
    {
        Dictionary<string, string> QnA = new Dictionary<string, string>();

        //=========================================================//
        // Default Constructor
        public Bot()
        {
            InitialiseQnA();
        }


        //=========================================================//
        // Populate the QnA dictionary with questions and answers
        private void InitialiseQnA()
        {
            // QnA.Add("", "");
            // DEFAULT
            QnA.Add("", "I'm sorry, I don't understand your question. Try to rephrase it or ask me something else.");

            // Assistance
            QnA.Add("Help", "What do you want help with? You can ask me about anything cybersecurity related!");
            QnA.Add("What can I ask you?", "You can ask me any cybersecurity related question and I will do my best to answer you.");
            QnA.Add("What can I ask you about?", "You can ask me any cybersecurity related question and I will do my best to answer you.");

            // Conversation
            QnA.Add("What is your name?", "My name is SecWiz!");
            QnA.Add("How are you?", "I am well, thank you. I'm always ready to answer your questions! Ask away!");
            QnA.Add("What's your purpose?", "I'm here to educate users on matters relating to cyber security and online safety.");
            QnA.Add("Thank you", "My pleasure. Is there anything else you would like to know?");


            // CHATGPT
            // Online Safety
            QnA.Add("What threats exist online?", "There are many threats to be aware of online. Some of the most prevalent are:\nData Breaches\nMalware\nViruses\nPhishing\nRomance Scams\nIdentity Theft.\nYou can ask me about them if you want to learn more!");
            QnA.Add("How can I stay safe online?", "Think before clicking, use strong passwords, keep software updated, be wary of sharing personal information, and use security software.");
            QnA.Add("What can I do to be safe online?", "Think before clicking, use strong passwords, keep software updated, be wary of sharing personal information, and use security software.");

            // Data Breaches & Personal Information
            QnA.Add("What is a data breach?", "A data breach is the unauthorized access, disclosure, or loss of sensitive information.");
            QnA.Add("Am I safe from data breaches?", "You can't be completely safe from data breaches. However, being careful about who you give information to online can help reduce your risk in the event of data breaches.");
            QnA.Add("How do I know if my data is exposed?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("Can I check if my data is safe?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("Have I been exposed online?", "Using a website like https://haveibeenpwned.com/ can help you know if your email or password has been compromised.");
            QnA.Add("What if my data has leaked?", "Change your passwords and keep an eye out for suspicious activity on any of your online accounts. Consider alerting your bank.");

            // Malware, Viruses & Threats
            QnA.Add("What is malware?", "Malware is malicious software designed to harm computer systems.");
            QnA.Add("What can I do to be safe from malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against malware?", "Install anti-malware software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("What is a virus?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("What are viruses?", "A computer virus is a type of malware that replicates and spreads by attaching itself to other programs.");
            QnA.Add("How can I be safe frome viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");
            QnA.Add("How do I defend against viruses?", "Install anti-virus software on your devices and schedule regular scans. Be careful of suspicious links and avoid clicking on them.");

            // Phishing & Scams
            QnA.Add("What is a phishing attack?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
            QnA.Add("What are phising attacks?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
            QnA.Add("What is phishing?", "A phishing attack is a deceptive attempt to steal sensitive information by impersonating a trustworthy entity.");
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

            // Privacy, Data Protection & Compliance
            QnA.Add("What is personal data?", "Personal data is information that can identify a person, such as name, ID number, or email address.");
            QnA.Add("What is GDPR?", "The General Data Protection Regulation (GDPR) is a European law that protects personal data and privacy.");
            QnA.Add("What is POPIA?", "The Protection of Personal Information Act (POPIA) is South African legislation protecting personal information.");
            QnA.Add("How can I protect my privacy online?", "Limit what you share, use privacy settings, avoid oversharing on social media, and consider using VPNs and private browsers.");

            // Incident Response & Reporting
            QnA.Add("What should I do if I think I’ve been hacked?", "Change passwords immediately, disconnect from the internet, run antivirus scans, and report it to your IT or security team.");
            QnA.Add("What should I do after a data breach?", "Change passwords, monitor accounts, notify affected parties, and follow your organization’s incident response plan.");
            QnA.Add("How should I report a cybercrime?", "Contact your local law enforcement or cybercrime unit. In South Africa, contact the Cybercrime Division of the SAPS.");
            QnA.Add("Who do I contact if I’ve been scammed online?", "Report to your bank and contact the Cybercrime Division of the SAPS. You can also report on the Cybersecurity Hub (www.cybersecurityhub.gov.za).");

            // Cybersecurity Culture & Awareness
            QnA.Add("Why is cybersecurity awareness important?", "It helps people recognize threats, avoid risky behaviors, and respond appropriately to incidents.");
            QnA.Add("What is cyber hygiene?", "Cyber hygiene refers to practices that help maintain good digital security, like regular updates and strong passwords.");
            QnA.Add("Why should I care about cybersecurity?", "Everyone is a potential target. Good cybersecurity helps protect your identity, money, and personal data.");

            // Kids & Online Safety
            QnA.Add("How can I keep my children safe online?", "Use parental controls, talk about internet safety, monitor online activity, and encourage open conversations.");
            QnA.Add("What should kids know about internet safety?", "They should know not to share personal info, to ask for help if something feels wrong, and to be respectful online.");
            QnA.Add("How can I teach my kids about cybersecurity?", "Use games, videos, and apps designed for online safety education. Lead by example.");

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
            QnA.Add("Bye", "Thanks for chatting!");
            QnA.Add("Back", "Thanks for chatting!");
            QnA.Add("Exit", "Thanks for chatting!");
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