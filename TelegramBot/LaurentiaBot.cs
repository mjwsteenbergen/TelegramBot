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
           
            Add(new TodoCommand(ts, tgs));
            Add(new PingCommand(tgs));
            Add(new QuoteCommand(tgs));
        }

        public void Add(Command com)
        {
            actionLib.Add(com.CommandName, com);
        }

        private async void MessageRecieved(TgMessages m, EventArgs e)
        {
            Command c;

            foreach (TgMessage message in m.Messages)
            {
                if (currentConv.TryGetValue(message.from.id, out c) && c != null)
                {
                    currentConv.Add(message.from.id, await c.Run(message.text, message));
                    continue;
                }

                Match match = Regex.Match(message.text, @"^/(\w+)");

                if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                {
                    currentConv[message.from.id] = await c.Run(message.text.Replace("/" + match.Groups[1].Value + " ", ""), message);
                }
            }
            
            foreach (TgInlineQuery inlineQuery in m.tgInlineQueries)
            {
                Match match = Regex.Match(inlineQuery.query, @"^(\w+)");

                if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                {
                    currentConv[inlineQuery.from.id] = await c.Run(inlineQuery.query.Replace(match.Groups[1].Value + " ", ""), inlineQuery);
                }
            }
            

            
        }
    }
}
