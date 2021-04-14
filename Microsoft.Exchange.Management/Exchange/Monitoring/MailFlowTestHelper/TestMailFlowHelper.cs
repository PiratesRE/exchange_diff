using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring.MailFlowTestHelper
{
	internal class TestMailFlowHelper
	{
		internal TestMailFlowHelper(TestMailFlow taskInstance) : this()
		{
			this.taskInstance = taskInstance;
		}

		internal TestMailFlowHelper()
		{
			this.monitoringData = new MonitoringData();
			this.monitoringDataSource = "MSExchange Monitoring Mailflow ";
		}

		protected ADSystemMailbox SourceSystemMailbox
		{
			get
			{
				return this.sourceSystemMailbox;
			}
			set
			{
				this.sourceSystemMailbox = value;
			}
		}

		protected Server SourceMailboxServer
		{
			get
			{
				return this.sourceMailboxServer;
			}
			set
			{
				this.sourceMailboxServer = value;
			}
		}

		protected MailboxDatabase SourceSystemMailboxMdb
		{
			get
			{
				return this.sourceSystemMailboxMdb;
			}
			set
			{
				this.sourceSystemMailboxMdb = value;
			}
		}

		protected TestMailFlow Task
		{
			get
			{
				return this.task;
			}
			set
			{
				this.task = value;
			}
		}

		protected bool IsRemoteTest
		{
			get
			{
				return this.isRemoteTest;
			}
			set
			{
				this.isRemoteTest = value;
			}
		}

		protected string MonitoringDataSource
		{
			get
			{
				return this.monitoringDataSource;
			}
			set
			{
				this.monitoringDataSource = value;
			}
		}

		protected MonitoringData MonitoringData
		{
			get
			{
				return this.monitoringData;
			}
			set
			{
				this.monitoringData = value;
			}
		}

		internal virtual void InternalValidate()
		{
			TimeSpan value = TimeSpan.FromSeconds((double)this.Task.ActiveDirectoryTimeout);
			this.Task.TenantGlobalCatalogSession.ServerTimeout = new TimeSpan?(value);
			this.Task.ConfigurationSession.ServerTimeout = new TimeSpan?(value);
		}

		internal virtual void InternalProcessRecord()
		{
		}

		internal virtual void InternalStateReset()
		{
			this.monitoringData = new MonitoringData();
		}

		internal void SetTask(TestMailFlow task)
		{
			this.Task = task;
		}

		internal void OutputMonitoringData()
		{
			this.Task.WriteObject(this.monitoringData);
		}

		internal void DiagnoseAndReportMapiException(LocalizedException ex)
		{
			MapiTransaction mapiTransaction = new MapiTransaction(this.SourceMailboxServer, this.SourceSystemMailboxMdb, this.SourceSystemMailbox, false, true);
			bool flag;
			bool flag2;
			string value = mapiTransaction.DiagnoseMapiOperationException(ex, out flag, out flag2);
			RecipientTaskException ex2 = new RecipientTaskException(new LocalizedString(value));
			if (flag)
			{
				this.AddErrorMonitoringEvent(1009, ex2.Message);
			}
			else
			{
				this.AddErrorMonitoringEvent(1007, ex2.Message);
			}
			this.Task.WriteError(ex2, ErrorCategory.InvalidData, null);
		}

		protected static MapiMessage GetDeliveryReceipt(MapiFolder folder, string subject, bool dsnOnly)
		{
			string value = DsnHumanReadableWriter.GetLocalizedSubjectPrefix(DsnFlags.Relay).ToString();
			MapiTable contentsTable;
			MapiTable mapiTable = contentsTable = folder.GetContentsTable();
			try
			{
				mapiTable.SetColumns(new PropTag[]
				{
					PropTag.EntryId,
					PropTag.Subject,
					PropTag.MessageClass
				});
				PropValue[][] array = mapiTable.QueryRows(1000, QueryRowsFlags.None);
				for (int i = 0; i <= array.GetUpperBound(0); i++)
				{
					if (!dsnOnly || ObjectClass.IsDsnPositive(array[i][2].Value.ToString()))
					{
						string text = array[i][1].Value.ToString();
						if ((!text.StartsWith(value, StringComparison.OrdinalIgnoreCase) || subject.StartsWith(value, StringComparison.OrdinalIgnoreCase)) && text.EndsWith(subject, StringComparison.OrdinalIgnoreCase))
						{
							byte[] bytes = array[i][0].GetBytes();
							return (MapiMessage)folder.OpenEntry(bytes);
						}
					}
				}
			}
			finally
			{
				if (contentsTable != null)
				{
					((IDisposable)contentsTable).Dispose();
				}
			}
			return null;
		}

		protected static void CreateAndSubmitMessage(MapiFolder folder, string sourceMailboxName, string targetMailAddress, string subject, bool deleteAfterSubmit)
		{
			using (MapiMessage mapiMessage = folder.CreateMessage())
			{
				PropValue[] props = new PropValue[]
				{
					new PropValue(PropTag.DeleteAfterSubmit, deleteAfterSubmit),
					new PropValue(PropTag.Subject, subject),
					new PropValue(PropTag.ReceivedByEmailAddress, targetMailAddress),
					new PropValue(PropTag.Body, "This is a Test-Mailflow probe message.")
				};
				mapiMessage.SetProps(props);
				mapiMessage.ModifyRecipients(ModifyRecipientsFlags.AddRecipients, new AdrEntry[]
				{
					new AdrEntry(new PropValue[]
					{
						new PropValue(PropTag.EmailAddress, targetMailAddress),
						new PropValue(PropTag.OriginatorDeliveryReportRequested, deleteAfterSubmit),
						new PropValue(PropTag.AddrType, "SMTP"),
						new PropValue(PropTag.RecipientType, RecipientType.To),
						new PropValue(PropTag.DisplayName, sourceMailboxName)
					})
				});
				mapiMessage.SaveChanges();
				mapiMessage.SubmitMessage(SubmitMessageFlags.ForceSubmit);
			}
		}

		protected static bool IsValidPropData(PropValue[][] array, int dimOneIndex, int dimTwoSize)
		{
			for (int i = 0; i < dimTwoSize; i++)
			{
				if (array[dimOneIndex][i].IsError() || array[dimOneIndex][i].Value == null)
				{
					return false;
				}
			}
			return true;
		}

		protected void WriteVerbose(LocalizedString text)
		{
			if (this.taskInstance != null)
			{
				this.taskInstance.WriteVerbose(text);
			}
		}

		protected void SetMonitoringDataSourceType(string source)
		{
			if (!string.IsNullOrEmpty(source))
			{
				this.monitoringDataSource = "MSExchange Monitoring Mailflow " + source;
			}
		}

		protected void AddMonitoringEvent(int id, EventTypeEnumeration type, string message)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(this.monitoringDataSource, id, type, message));
		}

		protected void AddSuccessMonitoringEvent(int id, string message)
		{
			this.AddMonitoringEvent(id, EventTypeEnumeration.Success, message);
		}

		protected void AddErrorMonitoringEvent(int id, string message)
		{
			this.AddMonitoringEvent(id, EventTypeEnumeration.Error, message);
		}

		protected void AddWarningMonitoringEvent(int id, string message)
		{
			this.AddMonitoringEvent(id, EventTypeEnumeration.Warning, message);
		}

		protected void AddInformationMonitoringEvent(int id, string message)
		{
			this.AddMonitoringEvent(id, EventTypeEnumeration.Information, message);
		}

		protected void WriteErrorAndMonitoringEvent(Exception ex, ErrorCategory category, int eventId)
		{
			this.AddErrorMonitoringEvent(eventId, ex.Message);
			this.Task.WriteError(ex, category, null);
		}

		protected void AddPerfCounter(string counter, string instance, double value)
		{
			this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.monitoringDataSource, counter, instance, value));
		}

		protected ADSystemMailbox GetServerSystemMailbox(Server server)
		{
			ADSystemMailbox result = null;
			try
			{
				this.SourceSystemMailboxMdb = this.GetServerMdb(this.SourceMailboxServer);
				result = this.GetSystemMailboxFromMdb(this.SourceSystemMailboxMdb);
			}
			catch (MailboxServerNotHostingMdbException)
			{
				this.AddErrorMonitoringEvent(1005, Strings.TestMailflowServerWithoutMdbs(server.Name));
			}
			this.SourceSystemMailbox = result;
			return result;
		}

		protected ADSystemMailbox GetSystemMailboxFromMdb(MailboxDatabase mdb)
		{
			ADSystemMailbox adsystemMailbox = null;
			GeneralMailboxIdParameter id = GeneralMailboxIdParameter.Parse(string.Format(CultureInfo.InvariantCulture, "SystemMailbox{{{0}}}", new object[]
			{
				mdb.Guid.ToString()
			}));
			IEnumerable<ADSystemMailbox> adDataObjects = this.Task.GetAdDataObjects<ADSystemMailbox>(id, this.Task.TenantGlobalCatalogSession);
			using (IEnumerator<ADSystemMailbox> enumerator = adDataObjects.GetEnumerator())
			{
				adsystemMailbox = (enumerator.MoveNext() ? enumerator.Current : null);
			}
			if (adsystemMailbox == null)
			{
				this.WriteErrorAndMonitoringEvent(new RecipientTaskException(Strings.TestMailflowNoSystemMailbox), ErrorCategory.InvalidOperation, 1005);
			}
			return adsystemMailbox;
		}

		protected MailboxDatabase GetServerMdb(Server server)
		{
			MailboxDatabase[] mailboxDatabases = server.GetMailboxDatabases();
			if (mailboxDatabases.Length < 1)
			{
				this.Task.WriteError(new NoMdbForOperationException(server.Name), ErrorCategory.PermissionDenied, null);
			}
			for (int i = 0; i < mailboxDatabases.Length; i++)
			{
				if (mailboxDatabases[i].Server.ObjectGuid == server.Guid && (this.IsMdbMounted(mailboxDatabases[i], null) || i >= mailboxDatabases.Length - 1))
				{
					return mailboxDatabases[i];
				}
			}
			throw new MailboxServerNotHostingMdbException(server.Name);
		}

		protected bool IsMdbMounted(Database mdb, string fqdn)
		{
			string server = string.IsNullOrEmpty(fqdn) ? mdb.Server.Name : fqdn;
			this.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(server));
			bool result;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", server, null, null, null))
			{
				MdbStatus[] array = exRpcAdmin.ListMdbStatus(new Guid[]
				{
					mdb.Guid
				});
				result = ((array[0].Status & MdbStatusFlags.Online) != MdbStatusFlags.Offline);
			}
			return result;
		}

		protected void OutputResult(string result, EnhancedTimeSpan latency, bool remoteTest)
		{
			this.Task.WriteObject(new TestMailflowOutput(result, latency, remoteTest));
		}

		protected const string MonitoringLatencyPerfCounter = "Mailflow Latency";

		private const string MonitoringSourceBase = "MSExchange Monitoring Mailflow ";

		private const string MonitoringProbeNumberPerfCounter = "Probe Number";

		private MonitoringData monitoringData;

		private string monitoringDataSource;

		private ADSystemMailbox sourceSystemMailbox;

		private Server sourceMailboxServer;

		private MailboxDatabase sourceSystemMailboxMdb;

		private TestMailFlow task;

		private bool isRemoteTest;

		private TestMailFlow taskInstance;

		public static class EventId
		{
			public const int TestMailflowSuccess = 1000;

			public const int TestMailflowFailure = 1001;

			public const int TestMailflowError = 1002;

			public const int TestMailflowWarning = 1003;

			public const int NodeDoesNotOwnExchangeVirtualServer = 1004;

			public const int NoSystemMailboxAvailable = 1005;

			public const int OperationOnInvalidServer = 1006;

			public const int MapiError = 1007;

			public const int InvalidEmailAddressFormat = 1008;

			public const int MdbMovedWhilePerformingTest = 1009;

			public const int TargetMdbInRecovery = 1010;

			public const int MdbServerNotFound = 1011;

			public const int CrossPremiseProbeResponseMatch = 2000;

			public const int CrossPremiseProbesPending = 2001;

			public const int CrossPremiseProbeNdred = 2002;

			public const int CrossPremiseNoEgressTargets = 2003;

			public const int CrossPremiseServerNotSelected = 2004;
		}
	}
}
