﻿/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

namespace InterReact
{
    public abstract class OperatorCondition : OrderCondition
    {
        protected abstract string Value { get; set; }
        public bool IsMore { get; set; }

        const string header = " is ";

        public override string ToString()
        {
            return header + (IsMore ? ">= " : "<= ") + Value;
        }

        public override bool Equals(object? obj)
        {
            var other = obj as OperatorCondition;

            if (other == null)
                return false;

            return base.Equals(obj)
                && Value.Equals(other.Value)
                && IsMore == other.IsMore;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Value.GetHashCode() + IsMore.GetHashCode();
        }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(header))
                return false;

            try
            {
                cond = cond.Replace(header, "");

                if (!cond.StartsWith(">=") && !cond.StartsWith("<="))
                    return false;

                IsMore = cond.StartsWith(">=");

                if (base.TryParse(cond.Substring(cond.LastIndexOf(" "))))
                    cond = cond.Substring(0, cond.LastIndexOf(" "));

                Value = cond.Substring(3);
            }
            catch
            {
                return false;
            }

            return true;
        }

        internal override void Deserialize(ResponseReader c)
        {
            base.Deserialize(c);
            IsMore = c.ReadBool();
            Value = c.ReadString();
        }
        internal override void Serialize(RequestMessage message)
        {
            base.Serialize(message);
            message.Write(IsMore, Value);
        }

    }
}
