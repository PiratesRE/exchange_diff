using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal static class Strings
	{
		public static LocalizedString ItemNotFound(TransportMessageId messageId)
		{
			return new LocalizedString("ItemNotFound", Strings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString FailedToRemove(TransportMessageId messageId)
		{
			return new LocalizedString("FailedToRemove", Strings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString InvalidMessageStateTransition(TransportMessageId messageId, MessageDepotItemState currentState, MessageDepotItemState nextState)
		{
			return new LocalizedString("InvalidMessageStateTransition", Strings.ResourceManager, new object[]
			{
				messageId,
				currentState,
				nextState
			});
		}

		public static LocalizedString InvalidMessageStateForRemove(TransportMessageId messageId, MessageDepotItemState currentState)
		{
			return new LocalizedString("InvalidMessageStateForRemove", Strings.ResourceManager, new object[]
			{
				messageId,
				currentState
			});
		}

		public static LocalizedString AcquireTokenMatchFail(TransportMessageId messageId)
		{
			return new LocalizedString("AcquireTokenMatchFail", Strings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString DuplicateItemFound(TransportMessageId messageId)
		{
			return new LocalizedString("DuplicateItemFound", Strings.ResourceManager, new object[]
			{
				messageId
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Storage.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ItemNotFound,
			FailedToRemove,
			InvalidMessageStateTransition,
			InvalidMessageStateForRemove,
			AcquireTokenMatchFail,
			DuplicateItemFound
		}
	}
}
