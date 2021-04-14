using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JunkEmailConfiguration : BaseRow
	{
		public JunkEmailConfiguration(MailboxJunkEmailConfiguration mailboxJunkEmailConfiguration) : base(mailboxJunkEmailConfiguration)
		{
			this.MailboxJunkEmailConfiguration = mailboxJunkEmailConfiguration;
		}

		public MailboxJunkEmailConfiguration MailboxJunkEmailConfiguration { get; private set; }

		[DataMember]
		public string Enabled
		{
			get
			{
				return this.MailboxJunkEmailConfiguration.Enabled.ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ContactsTrusted
		{
			get
			{
				return this.MailboxJunkEmailConfiguration.ContactsTrusted;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool TrustedListsOnly
		{
			get
			{
				return this.MailboxJunkEmailConfiguration.TrustedListsOnly;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<string> TrustedSendersAndDomains
		{
			get
			{
				return this.MailboxJunkEmailConfiguration.TrustedSendersAndDomains;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<string> BlockedSendersAndDomains
		{
			get
			{
				return this.MailboxJunkEmailConfiguration.BlockedSendersAndDomains;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
