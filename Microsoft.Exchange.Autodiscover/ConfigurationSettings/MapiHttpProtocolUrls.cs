using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net.MapiHttp;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class MapiHttpProtocolUrls
	{
		public Uri InternalBaseUrl { get; private set; }

		public Uri ExternalBaseUrl { get; private set; }

		public string MailboxId { get; private set; }

		public DateTime LastConfigurationTime { get; private set; }

		public bool TryGetProtocolUrl(ClientAccessType clientAccessType, MapiHttpProtocolUrls.Protocol mapiHttpProtocol, out Uri protocolUrl)
		{
			Uri uri = (clientAccessType == ClientAccessType.Internal) ? this.InternalBaseUrl : this.ExternalBaseUrl;
			if (uri != null)
			{
				if (mapiHttpProtocol == MapiHttpProtocolUrls.Protocol.Emsmdb)
				{
					protocolUrl = new Uri(MapiHttpEndpoints.GetMailboxUrl(uri.Host, this.MailboxId));
					return true;
				}
				if (mapiHttpProtocol == MapiHttpProtocolUrls.Protocol.Nspi)
				{
					protocolUrl = new Uri(MapiHttpEndpoints.GetAddressBookUrl(uri.Host, this.MailboxId));
					return true;
				}
			}
			protocolUrl = null;
			return false;
		}

		public bool HasUrls
		{
			get
			{
				return this.InternalBaseUrl != null || this.ExternalBaseUrl != null;
			}
		}

		public MapiHttpProtocolUrls(Uri internalUrl, Uri externalUrl, string mailboxId, DateTime lastConfigurationTime)
		{
			if (string.IsNullOrEmpty(mailboxId))
			{
				throw new ArgumentException("mailboxId is empty or null.", "mailboxId");
			}
			this.InternalBaseUrl = internalUrl;
			this.ExternalBaseUrl = externalUrl;
			this.MailboxId = mailboxId;
			this.LastConfigurationTime = lastConfigurationTime;
		}

		public enum Protocol
		{
			Nspi,
			Emsmdb
		}
	}
}
