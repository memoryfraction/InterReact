﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Stringification;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// A connectable observable which continually emits IAccountUpdate objects for all accounts.
        /// Account update objects include AccountValue, AccountPortfolio, AccountTime and AccountUpdateEnd.
        /// AccountUpdateEnd is emitted after the values for each account have been emitted.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() the value returned from Connect() to disconnect from the source, release all subscriptions and clear the cache.
        /// </summary>
        public IObservable<IAccountUpdate> CreateAccountUpdatesObservable() =>
            Response.ToObservableContinuous<IAccountUpdate>(
                () => Request.RequestAccountUpdates(start: true),
                () => Request.RequestAccountUpdates(start: false));

        // The key identifies unique items to be cached and specifies the order.
        public static string AccountUpdatesCacheKey(IAccountUpdate v)
        {
            if (v is AccountUpdateTime)
                return "!"; // top

            if (v is PortfolioValue ap)
                return $"{ap.Account} (1) {(ap.Contract == null ? "" : ap.Contract.Stringify(includeTypeName: false))}";

            if (v is AccountValue av)
                return $"{av.Account} (2) {av.Key}:{av.Currency}";

            // AccountUpdateEnd is last and indicates that the initial values for the partcular account have been emitted.
            return $"{((AccountUpdateEnd)v).Account} (3)";
        }
    }
}
