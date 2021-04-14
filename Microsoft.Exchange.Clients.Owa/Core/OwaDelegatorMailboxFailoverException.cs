using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaDelegatorMailboxFailoverException : OwaPermanentException
	{
		public string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		public OwaDelegatorMailboxFailoverException(string mailboxOwnerLegacyDN, Exception innerException) : base(null, innerException)
		{
			this.mailboxOwnerLegacyDN = mailboxOwnerLegacyDN;
		}

		private string mailboxOwnerLegacyDN;
	}
}
