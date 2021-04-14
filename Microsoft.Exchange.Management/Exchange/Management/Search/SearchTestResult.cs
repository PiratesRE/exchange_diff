using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Search
{
	[Serializable]
	public class SearchTestResult : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SearchTestResult.schema;
			}
		}

		public string Mailbox
		{
			get
			{
				return (string)this[SearchTestResultSchema.Mailbox];
			}
			set
			{
				this[SearchTestResultSchema.Mailbox] = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[SearchTestResultSchema.MailboxGuid];
			}
			set
			{
				this[SearchTestResultSchema.MailboxGuid] = value;
			}
		}

		public string UserLegacyExchangeDN
		{
			get
			{
				return (string)this[SearchTestResultSchema.UserLegacyExchangeDN];
			}
			set
			{
				this[SearchTestResultSchema.UserLegacyExchangeDN] = value;
			}
		}

		public string Database
		{
			get
			{
				return (string)this[SearchTestResultSchema.Database];
			}
			set
			{
				this[SearchTestResultSchema.Database] = value;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return (Guid)this[SearchTestResultSchema.DatabaseGuid];
			}
			set
			{
				this[SearchTestResultSchema.DatabaseGuid] = value;
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return (Guid)this[SearchTestResultSchema.ServerGuid];
			}
			set
			{
				this[SearchTestResultSchema.ServerGuid] = value;
			}
		}

		public bool ResultFound
		{
			get
			{
				return (bool)this[SearchTestResultSchema.ResultFound];
			}
			set
			{
				this[SearchTestResultSchema.ResultFound] = value;
			}
		}

		public double SearchTimeInSeconds
		{
			get
			{
				return double.Parse((string)this[SearchTestResultSchema.SearchTimeInSeconds]);
			}
			set
			{
				this[SearchTestResultSchema.SearchTimeInSeconds] = value.ToString();
			}
		}

		public List<MonitoringEvent> DetailEvents
		{
			get
			{
				return (List<MonitoringEvent>)this[SearchTestResultSchema.DetailEvents];
			}
			set
			{
				this[SearchTestResultSchema.DetailEvents] = value;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[SearchTestResultSchema.Server];
			}
			set
			{
				this[SearchTestResultSchema.Server] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[SearchTestResultSchema.Error];
			}
			set
			{
				this[SearchTestResultSchema.Error] = value;
			}
		}

		public uint DocumentId
		{
			get
			{
				return this.documentId;
			}
			set
			{
				this.documentId = value;
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
			set
			{
				this.entryId = value;
			}
		}

		public static SearchTestResult DefaultSearchTestResult
		{
			get
			{
				return new SearchTestResult();
			}
		}

		public SearchTestResult() : base(new SimpleProviderPropertyBag())
		{
			this.Reset();
		}

		internal void Reset()
		{
			this[SearchTestResultSchema.ResultFound] = false;
			this[SearchTestResultSchema.SearchTimeInSeconds] = "0";
			this[SearchTestResultSchema.Mailbox] = null;
			this[SearchTestResultSchema.UserLegacyExchangeDN] = null;
			this[SearchTestResultSchema.Database] = null;
			this[SearchTestResultSchema.Server] = null;
			this[SearchTestResultSchema.Error] = null;
			this[SearchTestResultSchema.MailboxGuid] = Guid.Empty;
			this[SearchTestResultSchema.DatabaseGuid] = Guid.Empty;
			this[SearchTestResultSchema.ServerGuid] = Guid.Empty;
			this[SearchTestResultSchema.DetailEvents] = new List<MonitoringEvent>(1);
			this.resultTimeout = false;
			this.documentId = 0U;
		}

		internal void SetResult(bool bResult, double SearchTimeInSeconds)
		{
			this[SearchTestResultSchema.ResultFound] = bResult;
			this[SearchTestResultSchema.SearchTimeInSeconds] = SearchTimeInSeconds.ToString();
		}

		internal void SetErrorTestResult(EventId eventId, LocalizedString strMessage)
		{
			this.SetResult(false, -1.0);
			this.Error = strMessage;
			this.DetailEvents.Add(new MonitoringEvent("MSExchange Monitoring ExchangeSearch", (int)eventId, EventTypeEnumeration.Error, strMessage, this.Database));
		}

		internal void SetErrorTestResult(EventId eventId, string strMessage)
		{
			this.SetResult(false, -1.0);
			MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring ExchangeSearch", (int)eventId, EventTypeEnumeration.Error, strMessage, this.Database);
			this.Error = strMessage;
			this.DetailEvents.Add(item);
		}

		internal void SetErrorTestResultWithTestThreadTimeOut()
		{
			this.SetResult(false, 0.0);
			this.resultTimeout = true;
			MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring ExchangeSearch", 1020, EventTypeEnumeration.Error, Strings.TestSearchTestThreadTimeOut, this.Database);
			this.Error = Strings.TestSearchTestThreadTimeOut;
			this.DetailEvents.Add(item);
		}

		internal void SetTestResult(bool bResult, double SearchTimeInSeconds)
		{
			if (bResult)
			{
				MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring ExchangeSearch", 1000, EventTypeEnumeration.Success, Strings.TestSearchSucceeded(this.Database), this.Database);
				this.DetailEvents.Add(item);
			}
			else if (!this.resultTimeout)
			{
				MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring ExchangeSearch", 1001, EventTypeEnumeration.Error, Strings.TestSearchFailed(this.Mailbox, this.Database, this.Server), this.Database);
				this.Error = Strings.TestSearchFailed(this.Mailbox, this.Database, this.Server);
				this.DetailEvents.Add(item);
			}
			this.SetResult(bResult, SearchTimeInSeconds);
		}

		internal const string MOMEventSource = "MSExchange Monitoring ExchangeSearch";

		private static ObjectSchema schema = ObjectSchema.GetInstance<SearchTestResultSchema>();

		private bool resultTimeout;

		private uint documentId;

		private byte[] entryId;
	}
}
