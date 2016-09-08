using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using ApiLibs;
using ApiLibs.Telegram;
using Chronic;

namespace TelegramBot.Commands
{
    public class RemindMeCommand : Command
    {
        private readonly TelegramService _tgs;
        private readonly IStorage _store;

        public RemindMeCommand(TelegramService tgs, IStorage store)
        {
            _tgs = tgs;
            _store = store;
            t = new Timer(1000);
            t.Elapsed += CheckForElapse;
        }

        private async void CheckForElapse(object sender, ElapsedEventArgs e)
        {
            DateTime current = DateTime.Now;

            List<Reminder> reminders = await _store.Read<List<Reminder>>("TelegramBot/Reminders");

            foreach (Reminder reminder in reminders)
            {
                if (DateTime.Compare(reminder.Date, current) < 0)
                {
                    _tgs.SendMessage(reminder.Message.chat.id, "Hi, you wanted to be reminded of something",
                        ParseMode.None, true, reminder.Message.message_id);
                }
            }

        }

        public static Timer t;

        public override void Run(Message m)
        {
            string messageText = m.text.Replace(CommandName, "");

            Match match = Regex.Match(messageText, "([^\"]+)\"([^\"]+)\"(.*)");
            string date = match.Groups[1].Value;
            string message = match.Groups[2].Value;
            string extra = match.Groups[3].Value;

            DateTime time = GetDate(date);

        }

        public DateTime GetDate(string date)
        {
            Parser p = new Parser();
            return p.Parse(date).ToTime();
        }

        public override string CommandName => "/remindme";
    }

    public class Reminder
    {
        public DateTime Date;
        public Message Message;
        public string RemindMessage;
    }
}
