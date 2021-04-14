using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Inference
{
	public class InferenceActivityLogSyntheticProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, int recurrenceIntervalSeconds, bool enabled)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(InferenceActivityLogSyntheticProbe).FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = recurrenceIntervalSeconds,
				MaxRetryAttempts = 3,
				ServiceName = ExchangeComponent.Inference.Name,
				Enabled = enabled
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			string targetResource = base.Definition.TargetResource;
			if (!SearchMonitoringHelper.IsDatabaseActive(targetResource))
			{
				base.Result.StateAttribute1 = "Skipped on passive copy.";
				return;
			}
			string @string = attributeHelper.GetString("MonitoringMailboxSmtpAddress", false, string.Empty);
			try
			{
				using (MailboxSession mailboxSession = SearchStoreHelper.GetMailboxSession(@string, false, "OWA"))
				{
					using (Folder inboxFolder = SearchStoreHelper.GetInboxFolder(mailboxSession))
					{
						string subject = "InferenceActivityLogProbe" + Guid.NewGuid().ToString();
						SearchStoreHelper.CreateMessage(mailboxSession, inboxFolder, subject);
						ExDateTime exDateTime;
						VersionedId messageBySubject = SearchStoreHelper.GetMessageBySubject(inboxFolder, subject, out exDateTime);
						using (MessageItem messageItem = MessageItem.Bind(mailboxSession, messageBySubject))
						{
							if (messageItem.IsRead)
							{
								inboxFolder.MarkAsUnread(false, new StoreId[]
								{
									messageBySubject
								});
								inboxFolder.MarkAsRead(false, new StoreId[]
								{
									messageBySubject
								});
							}
							else
							{
								inboxFolder.MarkAsRead(false, new StoreId[]
								{
									messageBySubject
								});
								inboxFolder.MarkAsUnread(false, new StoreId[]
								{
									messageBySubject
								});
							}
						}
						string stateAttribute = StoreId.GetStoreObjectId(messageBySubject).ToBase64ProviderLevelItemId();
						base.Result.StateAttribute2 = mailboxSession.MailboxGuid.ToString();
						base.Result.StateAttribute3 = stateAttribute;
						base.Result.StateAttribute4 = "MarkAsRead;MarkAsUnread";
					}
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute1 = ex.ToString();
			}
		}

		private const string MonitoringEmailSubject = "InferenceActivityLogProbe";

		internal static class AttributeNames
		{
			internal const string MonitoringMailboxSmtpAddress = "MonitoringMailboxSmtpAddress";
		}
	}
}
