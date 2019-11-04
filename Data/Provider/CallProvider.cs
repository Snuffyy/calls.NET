using System.Collections.Generic;
using System.Linq;
using ite4160.Models;
using System;

namespace ite4160.Data.Provider
{
    public class CallProvider
    {
        private PhoneNumberProvider phoneNumberGenerator = new PhoneNumberProvider();

        private Call Call(CallType type)
        {
            return new Call()
            {
                Caller = phoneNumberGenerator.Number(type, false, true),
                Receiver = phoneNumberGenerator.Number(type, true, false),
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
            private int[] _numberBase = { 3, 5, 8 };
            private IList<string> Existing { get; } = new List<string>();

            public string Number(CallType type, bool isNullable, bool canRepeat)
            {
                int numberBase = _numberBase[random.Next(0, _numberBase.Length)];

                if (type == CallType.NonDialled && isNullable) return null;

                var number = canRepeat && random.Next(0, 3) == 0 && Existing.Count != 0
                    ? Existing[random.Next(Existing.Count() - 1)]
                    : Convert.ToInt32($"{numberBase}{random.Next(10000, 100000)}").ToString();

                Existing.Add(number);

                return number;
            }
        }
    }
}