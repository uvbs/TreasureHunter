﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamTrade.TradeOffer;

namespace TreasureHunter.Service.Message
{
    public class PaymentMessage
    {
        public string Token { get; set; }
        public TradeOffer Offer { get; set; }

        public double Price { get; set; }
        public double PaidAmmount { get; set; } = 0;
        public bool IsPaid = false;
    }
}