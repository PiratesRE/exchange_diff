using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.EventLog;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverQueryItem
	{
		public AutoDiscoverQueryItem(RecipientData recipientData, LocalizedString applicationName, BaseQuery sourceQuery)
		{
			this.EmailAddress = recipientData.EmailAddress;
			this.initialEmailAddress = recipientData.EmailAddress;
			this.recipientData = recipientData;
			this.applicationName = applicationName;
			this.sourceQuery = sourceQuery;
		}

		public EmailAddress EmailAddress { get; set; }

		public EmailAddress InitialEmailAddress
		{
			get
			{
				return this.initialEmailAddress;
			}
		}

		public AutoDiscoverResult Result
		{
			get
			{
				return this.result;
			}
		}

		public RecipientData RecipientData
		{
			get
			{
				return this.recipientData;
			}
		}

		public BaseQuery SourceQuery
		{
			get
			{
				return this.sourceQuery;
			}
		}

		public static AutoDiscoverQueryItem[] CreateAutoDiscoverQueryItems(Application application, QueryList queryList, Uri autoDiscoverUrl)
		{
			AutoDiscoverQueryItem[] array = new AutoDiscoverQueryItem[queryList.Count];
			string target = autoDiscoverUrl.ToString();
			for (int i = 0; i < queryList.Count; i++)
			{
				queryList[i].Target = target;
				array[i] = new AutoDiscoverQueryItem(queryList[i].RecipientData, application.Name, queryList[i]);
			}
			return array;
		}

		public void SetResult(AutoDiscoverResult result)
		{
			if (Interlocked.CompareExchange<AutoDiscoverResult>(ref this.result, result, null) == null && result.Exception != null)
			{
				Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_AutoDiscoverFailed, this.EmailAddress.Domain, new object[]
				{
					Globals.ProcessId,
					this.EmailAddress,
					this.applicationName,
					result.Exception.ToString()
				});
			}
		}

		private AutoDiscoverResult result;

		private EmailAddress initialEmailAddress;

		private RecipientData recipientData;

		private LocalizedString applicationName;

		private BaseQuery sourceQuery;
	}
}
