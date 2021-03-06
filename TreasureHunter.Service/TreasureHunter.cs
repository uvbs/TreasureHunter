﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Akka.Util.Internal;
using log4net;
using TreasureHunter.Bot;
using TreasureHunter.Contract.AkkaMessageObject;
using TreasureHunter.SteamTrade;
using TreasureHunter.DataAccess;
using TreasureHunter.Transaction;

namespace TreasureHunter.Service
{
    public class TreasureHunter
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(TreasureHunter));
        private static ActorSystem _system;
        public void Init()
        {
            var config = LoadConfig();
            if (config == null)
            {
                Log.Error("No config json, program exit");
                return;
            }
            var disableJsonWarning = ConfigurationFactory.ParseString(@"
                    akka {
                            suppress-json-serializer-warning = on
                         }");
            _system = ActorSystem.Create("TreasureHunter", disableJsonWarning);
            var actors = new List<IActorRef>();          
            var valuationActor = _system.ActorOf(ValuationActor.Props(), "Valuation");
            var commander = _system.ActorOf(CommandActor.Props(actors), "Commander");
            var dataAccessActor = _system.ActorOf(DataAccessActor.Props(actors), "DataAccess");
            var paymentActor = _system.ActorOf(PaymentActor.Props(dataAccessActor, actors), "Payment");           
            var routees = config.Bots.Select(bot => _system.ActorOf(BotActor.Props(bot, config.ApiKey, (x, y) => new CustomUserHandler(x, y), valuationActor, commander, dataAccessActor), bot.DisplayName)).ToList();
            actors.AddRange(routees);
            actors.Add(paymentActor);
            actors.Add(valuationActor);
            do
            {
                Console.Write("botmgr > ");
                string inputText = Console.ReadLine();

                if (!string.IsNullOrEmpty(inputText))
                    commander.Tell(new UserCommandMessage()
                    {
                        Text = inputText
                    });

            } while (true);
        }

        private static Configuration LoadConfig()
        {
            Configuration configObject = null;
            if (!System.IO.File.Exists("settings.json"))
            {
                Console.WriteLine("No settings.json file found.");
                return null;
            }
            try
            {
                configObject = Configuration.LoadConfiguration("settings.json");
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                // handle basic json formatting screwups
                Console.WriteLine("settings.json file is corrupt or improperly formatted.");
                return configObject;
            }
            return configObject;
        }
    }
}
