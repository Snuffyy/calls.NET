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
                Caller = phoneNumberGenerator.Number(type).Value,
                Receiver = phoneNumberGenerator.Number(type),
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

            public int? Number(CallType type)
            {
                if (type == CallType.NonDialled) return null;

                int firstDigit = firstDigits[random.Next(0, firstDigits.Length)];
                return Convert.ToInt32($"{firstDigit}{random.Next(10000, 100000)}");
            }
        }
    }
}