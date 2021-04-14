using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Server.LocStrings
{
	internal static class Strings
	{
		public static LocalizedString FailedToAcquireBudget(string windowsIdentity, string principal)
		{
			return new LocalizedString("FailedToAcquireBudget", Strings.ResourceManager, new object[]
			{
				windowsIdentity,
				principal
			});
		}

		public static LocalizedString OperationCancelled(string command)
		{
			return new LocalizedString("OperationCancelled", Strings.ResourceManager, new object[]
			{
				command
			});
		}

		public static LocalizedString ServiceCommandTransientExceptionMessage(string message)
		{
			return new LocalizedString("ServiceCommandTransientExceptionMessage", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ServiceBusy(string command)
		{
			return new LocalizedString("ServiceBusy", Strings.ResourceManager, new object[]
			{
				command
			});
		}

		public static LocalizedString ServiceCommandExceptionMessage(string message)
		{
			return new LocalizedString("ServiceCommandExceptionMessage", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.PushNotifications.Server.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			FailedToAcquireBudget,
			OperationCancelled,
			ServiceCommandTransientExceptionMessage,
			ServiceBusy,
			ServiceCommandExceptionMessage
		}
	}
}
