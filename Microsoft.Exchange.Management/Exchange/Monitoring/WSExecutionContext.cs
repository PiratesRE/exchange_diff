using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Monitoring
{
	internal class WSExecutionContext
	{
		public WSExecutionContext(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn)
		{
			this.instance = instance;
			this.mailboxFqdn = mailboxFqdn;
		}

		public TestCasConnectivity.TestCasConnectivityRunInstance Instance
		{
			get
			{
				return this.instance;
			}
		}

		public string MailboxFqdn
		{
			get
			{
				return this.mailboxFqdn;
			}
		}

		public ExchangeServiceBinding Esb
		{
			get
			{
				return this.esb;
			}
			set
			{
				this.esb = value;
			}
		}

		public string SyncState
		{
			get
			{
				return this.syncState;
			}
			set
			{
				this.syncState = value;
			}
		}

		public ItemIdType ItemId
		{
			get
			{
				return this.itemId;
			}
			set
			{
				this.itemId = value;
			}
		}

		public TimeSpan CreateItemLatency
		{
			get
			{
				return this.createItemLatency;
			}
			set
			{
				this.createItemLatency = value;
			}
		}

		public bool End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end = value;
			}
		}

		private TestCasConnectivity.TestCasConnectivityRunInstance instance;

		private readonly string mailboxFqdn;

		private ExchangeServiceBinding esb;

		private string syncState;

		private ItemIdType itemId;

		private bool end;

		private TimeSpan createItemLatency;
	}
}
