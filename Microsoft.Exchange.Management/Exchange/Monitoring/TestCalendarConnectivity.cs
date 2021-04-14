using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "CalendarConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestCalendarConnectivity : TestVirtualDirectoryConnectivity
	{
		public TestCalendarConnectivity() : base(Strings.CasHealthCalendarLongName, Strings.CasHealthCalendarShortName, TransientErrorCache.CalendarTransientErrorCache, "MSExchange Monitoring CalendarConnectivity Internal", "MSExchange Monitoring CalendarConnectivity External")
		{
		}

		protected override IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter)
		{
			return base.GetVirtualDirectories<ADOwaVirtualDirectory>(serverId, new AndFilter(new QueryFilter[]
			{
				filter,
				TestCalendarConnectivity.VersionFilter
			}));
		}

		protected override CasTransactionOutcome BuildOutcome(string scenarioName, string scenarioDescription, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return new CasTransactionOutcome(instance.CasFqdn, scenarioName, scenarioDescription, "Calendar Latency", base.LocalSiteName, false, instance.credentials.UserName, instance.VirtualDirectoryName, instance.baseUri, instance.UrlType);
		}

		protected override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			TaskLogger.LogEnter();
			try
			{
				instance.baseUri = new UriBuilder(new Uri(instance.baseUri, "calendar/"))
				{
					Scheme = Uri.UriSchemeHttp,
					Port = 80
				}.Uri;
				CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(base.ApplicationShortName, base.ApplicationName, instance);
				casTransactionOutcome.Update(CasTransactionResultEnum.Success);
				this.ExecuteCalendarVDirTests(instance, casTransactionOutcome);
				instance.Result.Outcomes.Add(casTransactionOutcome);
			}
			finally
			{
				instance.Result.Complete();
				TaskLogger.LogExit();
			}
			return null;
		}

		private void ExecuteCalendarVDirTests(TestCasConnectivity.TestCasConnectivityRunInstance instance, CasTransactionOutcome outcome)
		{
			string text = string.Format("{0}/Calendar/calendar.html", instance.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			string text2 = string.Format("{0}/Calendar/calendar.ics", instance.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			ADSessionSettings adsessionSettings = instance.exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 293, "ExecuteCalendarVDirTests", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestCalendarConnectivity.cs");
			using (MailboxCalendarFolderDataProvider mailboxCalendarFolderDataProvider = new MailboxCalendarFolderDataProvider(adsessionSettings, DirectoryHelper.ReadADRecipient(instance.exchangePrincipal.MailboxInfo.MailboxGuid, instance.exchangePrincipal.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession) as ADUser, "Test-CalendarConnectivity"))
			{
				StoreObjectId defaultFolderId = mailboxCalendarFolderDataProvider.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
				Microsoft.Exchange.Data.Storage.Management.MailboxFolderId identity = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(instance.exchangePrincipal.ObjectId, defaultFolderId, null);
				MailboxCalendarFolder mailboxCalendarFolder = (MailboxCalendarFolder)mailboxCalendarFolderDataProvider.Read<MailboxCalendarFolder>(identity);
				if (!mailboxCalendarFolder.PublishEnabled)
				{
					mailboxCalendarFolder.SearchableUrlEnabled = true;
					mailboxCalendarFolder.PublishEnabled = true;
					mailboxCalendarFolder.PublishedCalendarUrl = new Uri(instance.baseUri, text).ToString();
					mailboxCalendarFolder.PublishedICalUrl = new Uri(instance.baseUri, text2).ToString();
					try
					{
						mailboxCalendarFolderDataProvider.Save(mailboxCalendarFolder);
					}
					catch (NotAllowedPublishingByPolicyException ex)
					{
						instance.Outcomes.Enqueue(new Warning(ex.LocalizedString));
						return;
					}
				}
			}
			ADOwaVirtualDirectory adowaVirtualDirectory = instance.VirtualDirectory as ADOwaVirtualDirectory;
			if (adowaVirtualDirectory != null && !(adowaVirtualDirectory.AnonymousFeaturesEnabled != true))
			{
				base.WriteMonitoringEvent(1104, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.CasHealthCalendarVDirSuccess);
				TimeSpan latency;
				if (!this.TestCalendarUrlResponse(text2, instance, TestCalendarConnectivity.CalendarContext.ICalContext, out latency))
				{
					outcome.Update(CasTransactionResultEnum.Failure);
				}
				else
				{
					outcome.UpdateLatency(latency);
				}
				if (!this.TestCalendarUrlResponse(text, instance, TestCalendarConnectivity.CalendarContext.ViewCalendarContext, out latency))
				{
					outcome.Update(CasTransactionResultEnum.Failure);
				}
				return;
			}
			instance.Outcomes.Enqueue(new Warning(Strings.CasHealthCalendarVDirWarning(instance.VirtualDirectoryName, instance.CasFqdn)));
			outcome.Update(CasTransactionResultEnum.Skipped);
			base.WriteMonitoringEvent(1105, this.MonitoringEventSource, EventTypeEnumeration.Warning, Strings.CasHealthCalendarVDirWarning(instance.VirtualDirectoryName, instance.CasFqdn));
		}

		private bool TestCalendarUrlResponse(string relativePath, TestCasConnectivity.TestCasConnectivityRunInstance instance, TestCalendarConnectivity.CalendarContext context, out TimeSpan latency)
		{
			bool flag = false;
			string text = string.Empty;
			Uri uri = new Uri(instance.baseUri, relativePath);
			latency = TimeSpan.Zero;
			for (int i = 0; i < 3; i++)
			{
				if (i > 0)
				{
					Thread.Sleep(20000);
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
				httpWebRequest.Method = "GET";
				httpWebRequest.Accept = "*/*";
				httpWebRequest.Credentials = null;
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.UserAgent = context.UserAgent;
				httpWebRequest.Timeout = 30000;
				base.WriteVerbose(Strings.CasHealthWebAppSendingRequest(uri));
				ExDateTime now = ExDateTime.Now;
				try
				{
					string text2 = string.Empty;
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
					{
						text2 = streamReader.ReadToEnd();
					}
					base.WriteVerbose(Strings.CasHealthWebAppResponseReceived(uri, httpWebResponse.StatusCode, string.Empty, text2));
					if (httpWebResponse.StatusCode != HttpStatusCode.OK)
					{
						text = Strings.CasHealthCalendarResponseError(httpWebResponse.StatusCode.ToString());
					}
					else if (string.IsNullOrEmpty(text2) || !text2.Contains(context.CheckText))
					{
						text = Strings.CasHealthCalendarResponseError(text2);
					}
					else
					{
						text = string.Empty;
						flag = true;
					}
				}
				catch (WebException ex)
				{
					base.WriteVerbose(Strings.CasHealthWebAppRequestException(uri, ex.Status, string.Empty, ex.Message));
					text = Strings.CasHealthCalendarWebRequestException(ex.Message);
				}
				latency = base.ComputeLatency(now);
				if (flag)
				{
					break;
				}
			}
			CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(context.ScenarioName, context.ScenarioDescription, instance);
			casTransactionOutcome.Update(flag ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure, latency, text);
			instance.Outcomes.Enqueue(casTransactionOutcome);
			if (!flag)
			{
				base.WriteMonitoringEvent(context.EventIdError, this.MonitoringEventSource, EventTypeEnumeration.Error, Strings.CasHealthCalendarCheckError(context.ScenarioDescription, text));
			}
			else
			{
				base.WriteMonitoringEvent(context.EventIdSuccess, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.CasHealthCalendarCheckSuccess(context.ScenarioDescription));
			}
			return flag;
		}

		private const string MonitoringEventSourceInternal = "MSExchange Monitoring CalendarConnectivity Internal";

		private const string MonitoringEventSourceExternal = "MSExchange Monitoring CalendarConnectivity External";

		public const string PublishedStartPageString = "AnonymousCalendar";

		private const string VCalendarString = "BEGIN:VCALENDAR";

		private const int RequestTimeOut = 30000;

		private const int TimeToWaitBeforRetry = 20000;

		private const int MaxReTryTimes = 3;

		private const string MonitoringLatencyPerfCounter = "Calendar Latency";

		private static readonly QueryFilter VersionFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADOwaVirtualDirectorySchema.OwaVersion, OwaVersions.Exchange2010);

		private static class CalendarEventId
		{
			internal const int ICalUrlSuccess = 1100;

			internal const int ICalUrlError = 1101;

			internal const int ViewUrlSuccess = 1102;

			internal const int ViewUrlError = 1103;

			internal const int PolicyVDirSuccess = 1104;

			internal const int PolicyVDirWarning = 1105;
		}

		private struct CalendarContext
		{
			internal string ScenarioName { get; private set; }

			internal string ScenarioDescription { get; private set; }

			internal string CheckText { get; private set; }

			internal int EventIdSuccess { get; private set; }

			internal int EventIdError { get; private set; }

			internal string UserAgent { get; private set; }

			internal static readonly TestCalendarConnectivity.CalendarContext ViewCalendarContext = new TestCalendarConnectivity.CalendarContext
			{
				ScenarioName = Strings.CasHealthCalendarScenarioTestView,
				ScenarioDescription = Strings.CasHealthCalendarScenarioTestViewDesc,
				CheckText = "AnonymousCalendar",
				EventIdSuccess = 1102,
				EventIdError = 1103,
				UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)"
			};

			internal static readonly TestCalendarConnectivity.CalendarContext ICalContext = new TestCalendarConnectivity.CalendarContext
			{
				ScenarioName = Strings.CasHealthCalendarScenarioTestICal,
				ScenarioDescription = Strings.CasHealthCalendarScenarioTestICalDesc,
				CheckText = "BEGIN:VCALENDAR",
				EventIdSuccess = 1100,
				EventIdError = 1101,
				UserAgent = null
			};
		}
	}
}
