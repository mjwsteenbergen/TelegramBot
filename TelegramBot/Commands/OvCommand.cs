using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ApiLibs.Telegram;
using ApiLibs._9292;

namespace TelegramBot.Commands
{
    class OvCommand : Command
    {
        private TelegramService tgs;
        private readonly Journey _setJourney;

        public OvCommand(TelegramService tgs, Journey setJourney)
        {
            this.tgs = tgs;
            _setJourney = setJourney;
        }

        public OvCommand(TelegramService tgs)
        {
            this.tgs = tgs;
        }

        public override async Task<Command> Run(Message m)
        {
            if (_setJourney == null)
            {
                return await standardHandle(m);
            }

            if (m.text == "Yes")
            {
                await tgs.SendMessage(m.from.id, "Ok, I will keep you posted");

                Thread t = new Thread(async () =>
                {
                    foreach (Leg1 leg in _setJourney.legs)
                    {
                        

                        Stop getIn = leg.stops[0];

                        if (getIn.departure - TimeSpan.FromMinutes(1) > DateTime.Now && getIn.departure.HasValue)
                        {
                            Thread.Sleep(getIn.departure.Value - DateTime.Now - TimeSpan.FromMinutes(1));

                            await
                                tgs.SendMessage(m.from.id,
                                    "Your next train will leave at platform " + getIn.platform + " at " +
                                    getIn.departure.Value.ToString("t"));
                        }

                        
                        Stop getOut = leg.stops[leg.stops.Length - 1];

                        if (getOut.departure.Value - TimeSpan.FromMinutes(2) > DateTime.Now && getIn.departure.HasValue)
                        {
                            Thread.Sleep(getOut.departure.Value - DateTime.Now - TimeSpan.FromMinutes(2));
                            await tgs.SendMessage(m.from.id, "You need to get off at the next stop. This will be Station " + getOut.location.name);

                        }

                    }
                });
                t.Start();
            }

            return null;

        }

        private async Task<Command> standardHandle(Message m)
        {
            _9292Service ovSer = new _9292Service();

            string fromLocationString = m.text.Substring(4);

            Location fromLocation = (await ovSer.GetLocation(fromLocationString))[0];
            JourneyObject obj = await ovSer.GetJourney(fromLocation.id, (await ovSer.GetLocation("Rotterdam"))[0].id, byTrain: true);

            Journey myJourney = obj.journeys[0];

            await tgs.SendMessage(m.from.id, "Okay, your train leaves on " + myJourney.departure.ToString("t") + " from platform " + myJourney.legs[0].stops[0].platform);

            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup
            {
                keyboard =
                new[] {
                     new [] { new KeyboardButton { text = "Yes" } , new KeyboardButton { text = "No"} }
                },
                one_time_keyboard = true
            };

            await tgs.SendMessage(m.from.id, "Would you like to be notified of your stops?", replyMarkup: markup);
            return new OvCommand(tgs, myJourney);
        }

        public override string CommandName => @"/ov";
    }
}
