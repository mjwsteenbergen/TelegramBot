using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            LaurentiaBot bot = new LaurentiaBot(tgs, todoist);
            Task.Run(async () =>
            {
                await tgs.SendMessage("newnottakenname", "TelegramBot is online");
            }).Wait();
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
            Add(new OvCommand(tgs));
        }

        private void Add(Command com)
        {
            actionLib.Add(com.CommandName, com);
        }

        private async void MessageRecieved(Message m, EventArgs e)
        {
            Command c;
            if (currentConv.TryGetValue(m.from.id, out c) && c != null)
            {
                currentConv.Add(m.contact.user_id, await c.Run(m));
                return;
            }

            Match match = Regex.Match(m.text, @"^/\w+");

            if (match.Success && actionLib.TryGetValue(match.Groups[0].Value, out c))
            {
                currentConv[m.from.id] = await c.Run(m);
            }
        }
    }
}
