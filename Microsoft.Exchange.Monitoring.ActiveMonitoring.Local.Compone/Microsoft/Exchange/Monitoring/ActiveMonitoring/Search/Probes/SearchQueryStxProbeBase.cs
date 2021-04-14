using System;
using Microsoft.Ceres.SearchCore.Admin.Config;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public abstract class SearchQueryStxProbeBase : SearchProbeBase
	{
		internal string MonitoringMailboxSmtpAddress { get; set; }

		internal int MessageAgeMinutes { get; set; }

		protected override bool SkipOnNonHealthyCatalog
		{
			get
			{
				return true;
			}
		}

		protected override bool SkipOnNonActiveDatabase
		{
			get
			{
				return true;
			}
		}

		internal bool IsSimpleQueryMode(string databaseName)
		{
			string indexSystemName = FastIndexVersion.GetIndexSystemName(SearchMonitoringHelper.GetDatabaseInfo(databaseName).MailboxDatabaseGuid);
			IndexSystemModel indexSystemModel = IndexManager.Instance.GetIndexSystemModel(indexSystemName);
			IIndexSystemIndex indexSystemIndex = indexSystemModel.Indexes[0];
			string[] options = indexSystemIndex.Options;
			int i = 0;
			while (i < options.Length)
			{
				string text = options[i];
				bool result;
				if (text.Equals("query_mode=full", StringComparison.OrdinalIgnoreCase))
				{
					result = false;
				}
				else
				{
					if (!text.Equals("query_mode=simple", StringComparison.OrdinalIgnoreCase))
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			return false;
		}

		internal void CheckSimpleQueryMode(string databaseName)
		{
			try
			{
				if (this.IsSimpleQueryMode(databaseName))
				{
					base.Result.StateAttribute4 = "FailedSimpleMode";
					throw new SearchProbeFailureException(Strings.SearchQueryStxSimpleQueryMode);
				}
			}
			catch (PerformingFastOperationException)
			{
				SearchMonitoringHelper.LogInfo(this, "Failed to get query mode.", new object[0]);
			}
		}

		internal bool CheckExistenceAndCreateMessage(MailboxSession session, out ExDateTime creationTime)
		{
			bool result;
			using (Folder inboxFolder = SearchStoreHelper.GetInboxFolder(session))
			{
				if (SearchStoreHelper.GetMessageBySubject(inboxFolder, "SearchQueryStxProbe", out creationTime) == null)
				{
					SearchStoreHelper.CreateMessage(session, inboxFolder, "SearchQueryStxProbe");
					base.Result.StateAttribute1 = "Message is created in inbox.";
					result = false;
				}
				else
				{
					if (ExDateTime.UtcNow - creationTime > TimeSpan.FromMinutes((double)this.MessageAgeMinutes))
					{
						SearchStoreHelper.CreateMessage(session, inboxFolder, "SearchQueryStxProbe");
						base.Result.StateAttribute1 = string.Format("Message exists in inbox with timestamp {0}. A new message is created.", creationTime);
					}
					result = true;
				}
			}
			return result;
		}

		protected virtual void InitializeAttributes()
		{
			this.MonitoringMailboxSmtpAddress = base.AttributeHelper.GetString("MonitoringMailboxSmtpAddress", false, string.Empty);
			this.MessageAgeMinutes = base.AttributeHelper.GetInt("MessageAgeMinutes", false, 10, null, null);
			base.Result.StateAttribute2 = this.MonitoringMailboxSmtpAddress;
			base.Result.StateAttribute3 = "subject:SearchQueryStxProbe";
		}

		internal const string MonitoringEmailSubject = "SearchQueryStxProbe";

		internal const string QueryString = "subject:SearchQueryStxProbe";

		internal const int MaxResultsCount = 3;

		internal static class AttributeNames
		{
			internal const string MonitoringMailboxSmtpAddress = "MonitoringMailboxSmtpAddress";

			internal const string MessageAgeMinutes = "MessageAgeMinutes";
		}
	}
}
