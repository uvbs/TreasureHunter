﻿using System;

namespace TreasureHunter.Contract.TransactionObjects
{
    public class PaymentNotificationMessage
    {
        public Guid PaymentId { get; private set; }

        public PaymentNotificationMessage(Guid id)
        {
            PaymentId = id;
        }
    }
}