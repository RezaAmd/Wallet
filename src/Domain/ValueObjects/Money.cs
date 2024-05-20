﻿using DigitalWallet.Domain.Common;
using System;
using System.Collections.Generic;

namespace DigitalWallet.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        #region Constructors

        public Money()
        {
            Value = 0;
        }
        public Money(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("Amout cannot be < 0");
            Value = amount;
        }
        public Money(long amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("Amout cannot be < 0");
            Value = amount;
        }
        public Money(ulong amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("Amout cannot be < 0");
            Value = amount;
        }

        #endregion

        public decimal Value { get; private set; }

        public Money Increase(decimal amount)
        {
            return Value + new Money(amount);
        }
        public Money Increase(Money amount)
        {
            return Value + amount;
        }
        public Money Decrease(decimal amount)
        {
            // Amount cannot grater than value.
            if (amount > Value)
                throw new ArgumentOutOfRangeException("Insuffience balance");
            return Value - new Money(amount);
        }
        public Money Decrease(Money amount)
        {
            // Amount cannot grater than value.
            if (amount.Value > Value)
                throw new ArgumentOutOfRangeException("Insuffience balance");
            return Value - amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        #region Operators
        protected static bool EqualOperator(decimal left, Money right)
        {
            if (left is 0 ^ right is null)
            {
                return false;
            }

            return left.Equals(right.Value!) != false;
        }
        protected static bool NotEqualOperator(decimal left, Money right)
        {
            return !(EqualOperator(left, right));
        }

        public static Money operator +(Money left, Money right) => new Money(left.Value + right.Value);
        public static Money operator -(Money left, Money right) => new Money(left.Value - right.Value);
        public static Money operator +(decimal left, Money right) => new Money(left + right.Value);
        public static Money operator -(decimal left, Money right) => new Money(left - right.Value);
        #endregion
    }
}