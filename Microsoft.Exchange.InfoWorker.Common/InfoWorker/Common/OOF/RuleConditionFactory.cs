using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal static class RuleConditionFactory
	{
		public static Restriction CreateInternalSendersGroupCondition()
		{
			return RuleConditionFactory.NoCondition;
		}

		public static Restriction CreateKnownExternalSendersGroupCondition(string[] knownExternalSenders)
		{
			Restriction[] array = new Restriction[knownExternalSenders.Length];
			for (int i = 0; i < knownExternalSenders.Length; i++)
			{
				array[i] = Restriction.EQ(PropTag.SenderEmailAddress, knownExternalSenders[i]);
			}
			return Restriction.Or(array);
		}

		public static Restriction CreateAllExternalSendersGroupCondition()
		{
			return RuleConditionFactory.NoCondition;
		}

		private const string ExchangeAddressType = "EX";

		private static readonly Restriction ExchangeAddressTypeCondition = Restriction.EQ(PropTag.SenderAddrType, "EX");

		private static readonly Restriction NonExchangeAddressTypeCondition = Restriction.NE(PropTag.SenderAddrType, "EX");

		private static readonly Restriction NoCondition = null;
	}
}
