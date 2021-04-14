using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal static class ClientAPIStrings
	{
		static ClientAPIStrings()
		{
			ClientAPIStrings.stringIDs.Add(3277329186U, "BrokerStatus_UnknownError");
			ClientAPIStrings.stringIDs.Add(3160820891U, "CallbackAlreadyRegistered");
			ClientAPIStrings.stringIDs.Add(197742799U, "BrokerStatus_Cancelled");
		}

		public static LocalizedString BrokerStatus_UnknownError
		{
			get
			{
				return new LocalizedString("BrokerStatus_UnknownError", ClientAPIStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallbackAlreadyRegistered
		{
			get
			{
				return new LocalizedString("CallbackAlreadyRegistered", ClientAPIStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidBrokerSubscriptionOnLoadException(string storeId, string mailbox)
		{
			return new LocalizedString("InvalidBrokerSubscriptionOnLoadException", ClientAPIStrings.ResourceManager, new object[]
			{
				storeId,
				mailbox
			});
		}

		public static LocalizedString BrokerStatus_Cancelled
		{
			get
			{
				return new LocalizedString("BrokerStatus_Cancelled", ClientAPIStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(ClientAPIStrings.IDs key)
		{
			return new LocalizedString(ClientAPIStrings.stringIDs[(uint)key], ClientAPIStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Notifications.Broker.Strings", typeof(ClientAPIStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			BrokerStatus_UnknownError = 3277329186U,
			CallbackAlreadyRegistered = 3160820891U,
			BrokerStatus_Cancelled = 197742799U
		}

		private enum ParamIDs
		{
			InvalidBrokerSubscriptionOnLoadException
		}
	}
}
