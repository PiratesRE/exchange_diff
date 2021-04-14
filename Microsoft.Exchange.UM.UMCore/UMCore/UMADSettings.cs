using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMADSettings
	{
		public abstract ProtocolConnectionSettings SIPAccessService { get; }

		public abstract UMStartupMode UMStartupMode { get; }

		public abstract string UMCertificateThumbprint { get; }

		public abstract string Fqdn { get; }

		public abstract UMSmartHost ExternalServiceFqdn { get; }

		public abstract IPAddressFamily IPAddressFamily { get; }

		public abstract bool IPAddressFamilyConfigurable { get; }

		public abstract string UMPodRedirectTemplate { get; }

		public abstract string UMForwardingAddressTemplate { get; }

		public abstract int SipTcpListeningPort { get; }

		public abstract int SipTlsListeningPort { get; }

		public abstract ADObjectId Id { get; }

		public abstract bool IsSIPAccessServiceNeeded { get; }

		internal abstract void SubscribeToADNotifications(ADNotificationEvent notificationHandler);

		internal abstract UMADSettings RefreshFromAD();

		protected static void ExecuteADOperation(Action function)
		{
			try
			{
				function();
			}
			catch (Exception ex)
			{
				if (ex is ADOperationException || ex is ADTransientException)
				{
					throw new ExchangeServerNotFoundException(ex.Message, ex);
				}
				throw;
			}
		}
	}
}
