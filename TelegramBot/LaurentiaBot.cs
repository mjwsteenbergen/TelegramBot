using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
﻿using System.Threading;
﻿using System.Threading.Tasks;
using ApiLibs.General;
using ApiLibs.Telegram;
using ApiLibs.Todoist;
using TelegramBot.Commands;

namespace TelegramBot
{
    public class LaurentiaBot
    {
        private static readonly string ApplicationDataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "TelegramBot" + Path.DirectorySeparatorChar;

        Dictionary<string, Command> actionLib;
        private Dictionary<int, Command> currentConv;

        public static void Main(string[] args)
        {
            Passwords passwords = Passwords.ReadPasswords(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "Laurentia" + Path.DirectorySeparatorChar);

            TelegramService tgs = new TelegramService(passwords.Telegram_token, ApplicationDataPath);
            TodoistService todoist = new TodoistService(passwords.TodoistKey, passwords.TodoistUserAgent);

            Task.Run(async () =>
            {
                await tgs.SendMessage(13173126, "TelegramBot is online");
            }).Wait();
            LaurentiaBot bot = new LaurentiaBot(tgs, todoist);
            while (true)
            {
                Thread.Sleep(int.MaxValue);
            }
            Console.ReadLine();


        }

        public LaurentiaBot(TelegramService tgs, TodoistService ts)
        {
            tgs.MessageRecieved += MessageRecieved;
            tgs.LookForMessages();
            currentConv = new Dictionary<int, Command>();
            actionLib = new Dictionary<string, Command>();
           
            Add(new TodoCommand(ts));
            Add(new PingCommand(tgs));
            Add(new QuoteCommand(tgs));
        }

        public void Add(Command com)
        {
            actionLib.Add(com.CommandName, com);
        }

        private async void MessageRecieved(Message m, EventArgs e)
        {
            Command c;
            if (currentConv.TryGetValue(m.from.id, out c) && c != null)
            {
                currentConv.Add(m.contact.user_id, await c.Run(m.text?? m.query, m));
                return;
            }

            if (m.isMessage)
            {
                Match match = Regex.Match(m.text, @"^/(\w+)");

                if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                {
                    currentConv[m.from.id] = await c.Run(m.text.Replace("/" + match.Groups[1].Value + " ", ""), m);
                }
            }
            else
            {
                Match match = Regex.Match(m.query, @"^(\w+)");

                if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                {
                    currentConv[m.from.id] = await c.Run(m.query.Replace(match.Groups[1].Value + " ", ""), m);
                }
            }
            

            
        }
    }
}
