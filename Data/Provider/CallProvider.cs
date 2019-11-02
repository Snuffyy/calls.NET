using System.Collections.Generic;
using System.Linq;
using ite4160.Models;
using System;
using static ite4160.Models.Call;

namespace ite4160.Data.Provider
{
    public class CallProvider
    {
        private PhoneNumberProvider phoneNumberGenerator = new PhoneNumberProvider();

        private Call Call(CallType type)
        {
            return new Call()
            {
                Caller = phoneNumberGenerator.Number(),
                Receiver = phoneNumberGenerator.NullableNumber(type),
                Type = type
            };
        }

        public IEnumerable<Call> Calls(int total, CallType type)
        {
            return Enumerable
            .Range(0, total)
            .Select(o => Call(type));
        }

        private class PhoneNumberProvider
        {
            private Random random = new Random();
            private int[] firstDigits = { 3, 5, 8 };

            public string Number()
            {
                int firstDigit = firstDigits[random.Next(0, firstDigits.Length)];
                return Convert.ToInt32($"{firstDigit}{random.Next(10000, 100000)}").ToString();
            }

            public string NullableNumber(CallType type)
            {
                if (type == CallType.NonDialled) return null;

                return Number();
            }
        }
    }
}