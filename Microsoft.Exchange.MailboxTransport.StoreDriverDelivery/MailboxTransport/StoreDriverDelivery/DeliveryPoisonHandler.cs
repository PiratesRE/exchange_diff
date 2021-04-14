using System;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryPoisonHandler : PoisonHandler<DeliveryPoisonContext>
	{
		public DeliveryPoisonHandler(TimeSpan poisonEntryExpiryWindow, int maxPoisonEntries) : base("Delivery", poisonEntryExpiryWindow, maxPoisonEntries)
		{
		}

		public override void SavePoisonContext()
		{
			try
			{
				base.SavePoisonContext();
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_PoisonMessageSaveFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
		}

		public override void MarkPoisonMessageHandled(string poisonId)
		{
			try
			{
				base.MarkPoisonMessageHandled(poisonId);
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_PoisonMessageMarkFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
			}
		}

		public override void Load()
		{
			try
			{
				base.Load();
			}
			catch (UnauthorizedAccessException ex)
			{
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_PoisonMessageLoadFailedRegistryAccessDenied, null, new object[]
				{
					ex.Message
				});
				throw new TransportComponentLoadFailedException(Strings.PoisonMessageRegistryAccessFailed, ex);
			}
		}

		protected override bool IsMessagePoison(CrashProperties crashProperties)
		{
			return !this.IsExpired(crashProperties) && base.IsMessagePoison(crashProperties);
		}

		private const string DeliveryRegistrySuffix = "Delivery";
	}
}
