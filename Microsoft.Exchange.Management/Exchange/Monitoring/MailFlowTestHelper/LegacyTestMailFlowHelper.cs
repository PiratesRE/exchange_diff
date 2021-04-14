using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring.MailFlowTestHelper
{
	internal class LegacyTestMailFlowHelper : TestMailFlowHelper
	{
		internal LegacyTestMailFlowHelper()
		{
			this.sourceServerHasMdb = true;
			this.targetServerHasMdb = true;
		}

		internal override void InternalValidate()
		{
			base.InternalValidate();
			base.IsRemoteTest = (!string.IsNullOrEmpty(base.Task.TargetEmailAddress) || base.Task.TargetMailboxServer != null || base.Task.TargetDatabase != null || base.Task.AutoDiscoverTargetMailboxServer.IsPresent);
			if (base.IsRemoteTest)
			{
				base.SetMonitoringDataSourceType("Remote");
			}
			else
			{
				base.SetMonitoringDataSourceType("Local");
			}
			if (base.Task.Identity == null)
			{
				string machineName = Environment.MachineName;
				base.Task.Identity = ServerIdParameter.Parse(machineName);
			}
			base.SourceMailboxServer = this.GetServerFromId(base.Task.Identity);
			try
			{
				base.SourceSystemMailboxMdb = base.GetServerMdb(base.SourceMailboxServer);
				base.SourceSystemMailbox = base.GetSystemMailboxFromMdb(base.SourceSystemMailboxMdb);
				this.sourceServerHasMdb = true;
			}
			catch (MailboxServerNotHostingMdbException)
			{
				this.sourceServerHasMdb = false;
			}
			if (this.sourceServerHasMdb)
			{
				this.friendlySourceAddress = base.Task.Identity.ToString() + "\\" + base.SourceSystemMailbox.PrimarySmtpAddress.ToString();
				if (!base.IsRemoteTest)
				{
					this.targetMailboxAddress = base.SourceSystemMailbox.PrimarySmtpAddress.ToString();
					this.friendlyTargetAddress = this.friendlySourceAddress;
					return;
				}
				this.SetTargetParameters();
			}
		}

		internal override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!this.sourceServerHasMdb)
			{
				base.AddSuccessMonitoringEvent(1000, Strings.TestMailflowSucceededNoDatabaseOnSourceServer(base.SourceMailboxServer.Name));
				base.Task.WriteWarning(Strings.TestMailflowServerWithoutMdbs(base.SourceMailboxServer.Name));
				return;
			}
			if (!this.targetServerHasMdb)
			{
				base.AddSuccessMonitoringEvent(1000, Strings.TestMailflowSucceededNoDatabaseOnTargetServer(base.Task.TargetMailboxServer.ToString()));
				base.Task.WriteWarning(Strings.TestMailflowServerWithoutMdbs(base.Task.TargetMailboxServer.ToString()));
				return;
			}
			using (MapiStore mapiStore = MapiStore.OpenMailbox(base.SourceMailboxServer.Fqdn, base.SourceSystemMailbox.LegacyExchangeDN, base.SourceSystemMailbox.ExchangeGuid, base.SourceSystemMailbox.Database.ObjectGuid, base.SourceSystemMailbox.Name, null, null, ConnectFlag.UseAdminPrivilege | ConnectFlag.UseSeparateConnection, OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.MailboxGuid, CultureInfo.InvariantCulture, null, "Client=Management;Action=LegacyTestMailFlow", null))
			{
				using (MapiFolder outboxFolder = mapiStore.GetOutboxFolder())
				{
					using (MapiFolder inboxFolder = mapiStore.GetInboxFolder())
					{
						LegacyTestMailFlowHelper.CleanUpInbox(inboxFolder);
						string subject = string.Format("Test-Mailflow {0} {1}", Guid.NewGuid(), "66c7004a-6860-44b2-983a-327aa3c9cfec");
						TestMailFlowHelper.CreateAndSubmitMessage(outboxFolder, base.SourceSystemMailbox.Name, this.targetMailboxAddress, subject, true);
						this.WaitAndProcessDeliveryReceipt(inboxFolder, subject, this.friendlySourceAddress, this.friendlyTargetAddress, this.GetPerfInstanceName());
					}
				}
			}
		}

		internal override void InternalStateReset()
		{
			base.InternalStateReset();
			this.sourceServerHasMdb = true;
			this.targetServerHasMdb = true;
		}

		private static void CleanUpInbox(MapiFolder folder)
		{
			DateTime t = DateTime.UtcNow.AddMinutes(-20.0);
			MapiTable contentsTable;
			MapiTable mapiTable = contentsTable = folder.GetContentsTable();
			try
			{
				mapiTable.SetColumns(new PropTag[]
				{
					PropTag.EntryId,
					PropTag.MessageDeliveryTime,
					PropTag.NormalizedSubject,
					PropTag.OriginalSubject
				});
				Restriction restriction = Restriction.Or(new Restriction[]
				{
					Restriction.Content(PropTag.NormalizedSubject, "66c7004a-6860-44b2-983a-327aa3c9cfec", ContentFlags.SubString | ContentFlags.IgnoreCase),
					Restriction.Content(PropTag.OriginalSubject, "66c7004a-6860-44b2-983a-327aa3c9cfec", ContentFlags.SubString | ContentFlags.IgnoreCase)
				});
				mapiTable.Restrict(restriction);
				mapiTable.SortTable(new SortOrder(PropTag.MessageDeliveryTime, SortFlags.Ascend), SortTableFlags.None);
				PropValue[][] array = mapiTable.QueryRows(100, QueryRowsFlags.None);
				List<byte[]> list = new List<byte[]>(100);
				for (int i = 0; i <= array.GetUpperBound(0); i++)
				{
					if (TestMailFlowHelper.IsValidPropData(array, i, 2))
					{
						if (array[i][1].GetDateTime() > t)
						{
							break;
						}
						list.Add(array[i][0].GetBytes());
					}
				}
				if (list.Count > 0)
				{
					folder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, list.ToArray());
				}
			}
			finally
			{
				if (contentsTable != null)
				{
					((IDisposable)contentsTable).Dispose();
				}
			}
		}

		private void SetTargetParameters()
		{
			if (base.Task.AutoDiscoverTargetMailboxServer.IsPresent)
			{
				base.Task.TargetMailboxServer = this.AutoDiscoverTargetServer();
			}
			if (base.Task.TargetMailboxServer != null)
			{
				this.targetMailboxServer = this.GetServerFromId(base.Task.TargetMailboxServer);
				ADSystemMailbox adsystemMailbox = null;
				try
				{
					this.targetSystemMailboxMdb = base.GetServerMdb(this.targetMailboxServer);
					adsystemMailbox = base.GetSystemMailboxFromMdb(this.targetSystemMailboxMdb);
					this.targetServerHasMdb = true;
				}
				catch (MailboxServerNotHostingMdbException)
				{
					this.targetServerHasMdb = false;
				}
				if (this.targetServerHasMdb)
				{
					this.targetMailboxAddress = adsystemMailbox.PrimarySmtpAddress.ToString();
					this.friendlyTargetAddress = base.Task.TargetMailboxServer.ToString() + "\\" + this.targetMailboxAddress;
					return;
				}
			}
			else if (base.Task.TargetDatabase != null)
			{
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.Task.GetAdDataObject<MailboxDatabase>(base.Task.TargetDatabase, base.Task.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(base.Task.TargetDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(base.Task.TargetDatabase.ToString())));
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(mailboxDatabase.Guid);
				if (serverForDatabase != null && !string.IsNullOrEmpty(serverForDatabase.ServerFqdn))
				{
					base.Task.TargetMailboxServer = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
					this.targetMailboxServer = this.GetServerFromId(base.Task.TargetMailboxServer);
					ADSystemMailbox systemMailboxFromMdb = base.GetSystemMailboxFromMdb(mailboxDatabase);
					this.targetSystemMailboxMdb = mailboxDatabase;
					this.targetMailboxAddress = systemMailboxFromMdb.PrimarySmtpAddress.ToString();
					this.friendlyTargetAddress = base.Task.TargetMailboxServer.ToString() + "\\" + this.targetMailboxAddress;
					return;
				}
				base.WriteErrorAndMonitoringEvent(new MdbServerNotFoundException(mailboxDatabase.ToString()), ErrorCategory.InvalidData, 1011);
				return;
			}
			else if (base.Task.TargetEmailAddress != null)
			{
				this.targetMailboxAddress = base.Task.TargetEmailAddress;
				if (string.IsNullOrEmpty(base.Task.TargetEmailAddressDisplayName))
				{
					this.friendlyTargetAddress = base.Task.TargetEmailAddress;
				}
				else
				{
					this.friendlyTargetAddress = base.Task.TargetEmailAddressDisplayName + "(" + base.Task.TargetEmailAddress + ")";
				}
				RoutingAddress routingAddress = new RoutingAddress(base.Task.TargetEmailAddress);
				if (!routingAddress.IsValid)
				{
					base.WriteErrorAndMonitoringEvent(new RecipientTaskException(Strings.TestMailflowInvalidTargetEmailAddress(base.Task.TargetEmailAddress)), ErrorCategory.InvalidData, 1008);
				}
			}
		}

		private MapiMessage WaitForDeliveryReceipt(MapiFolder folder, string subject, int errorLatency, int timeoutSeconds, string sourceAddress, string targetAddress)
		{
			bool flag = false;
			MapiMessage mapiMessage = null;
			for (int i = 0; i < timeoutSeconds; i++)
			{
				mapiMessage = TestMailFlowHelper.GetDeliveryReceipt(folder, subject, true);
				if (mapiMessage != null)
				{
					break;
				}
				Thread.Sleep(1000);
				if (!flag && i >= errorLatency)
				{
					flag = true;
					base.AddErrorMonitoringEvent(1002, Strings.TestMailflowError(sourceAddress, targetAddress, errorLatency));
				}
			}
			return mapiMessage;
		}

		private void WaitAndProcessDeliveryReceipt(MapiFolder folder, string subject, string fromAddress, string toAddress, string perfInstance)
		{
			using (MapiMessage mapiMessage = this.WaitForDeliveryReceipt(folder, subject, base.Task.ErrorLatency, base.Task.ExecutionTimeout, fromAddress, toAddress))
			{
				if (mapiMessage != null)
				{
					PropValue prop = mapiMessage.GetProp(PropTag.MessageDeliveryTime);
					PropValue prop2 = mapiMessage.GetProp(PropTag.OriginalSubmitTime);
					EnhancedTimeSpan enhancedTimeSpan = prop.GetDateTime() - prop2.GetDateTime();
					if (enhancedTimeSpan < EnhancedTimeSpan.Zero)
					{
						enhancedTimeSpan = EnhancedTimeSpan.Zero;
					}
					base.OutputResult(Strings.MapiTransactionResultSuccess, enhancedTimeSpan, base.IsRemoteTest);
					base.AddSuccessMonitoringEvent(1000, Strings.TestMailflowSucceeded(fromAddress, toAddress));
					base.AddPerfCounter("Mailflow Latency", perfInstance, enhancedTimeSpan.TotalSeconds);
				}
				else
				{
					EnhancedTimeSpan latency = EnhancedTimeSpan.FromSeconds(0.0);
					base.OutputResult(Strings.MapiTransactionResultFailure, latency, base.IsRemoteTest);
					base.AddErrorMonitoringEvent(1001, Strings.TestMailflowFailed(fromAddress, toAddress, base.Task.ExecutionTimeout));
					base.AddPerfCounter("Mailflow Latency", perfInstance, -1.0);
				}
			}
		}

		private Server GetServerFromId(ServerIdParameter serverId)
		{
			Server server = (Server)base.Task.GetAdDataObject<Server>(serverId, base.Task.ConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverId.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverId.ToString())));
			LocalizedException ex = null;
			if (!server.IsExchange2007OrLater)
			{
				ex = new OperationOnOldServerException(server.Name);
			}
			else if (!server.IsMailboxServer)
			{
				ex = new OperationOnlyOnMailboxServerException(server.Name);
			}
			if (ex != null)
			{
				base.WriteErrorAndMonitoringEvent(ex, ErrorCategory.InvalidArgument, 1006);
			}
			return server;
		}

		private string GetPerfInstanceName()
		{
			string result;
			if (!base.IsRemoteTest)
			{
				result = base.Task.Identity.ToString() + "\\" + base.SourceSystemMailboxMdb.Name;
			}
			else if (base.Task.TargetMailboxServer != null)
			{
				result = base.Task.TargetMailboxServer.ToString() + "\\" + this.targetSystemMailboxMdb.Name;
			}
			else if (string.IsNullOrEmpty(base.Task.TargetEmailAddressDisplayName))
			{
				result = base.Task.TargetEmailAddress;
			}
			else
			{
				result = base.Task.TargetEmailAddressDisplayName;
			}
			return result;
		}

		private ServerIdParameter AutoDiscoverTargetServer()
		{
			ServerIdParameter result = base.Task.Identity;
			ADPagedReader<Server> adpagedReader = base.Task.ConfigurationSession.FindAllPaged<Server>();
			foreach (Server server in adpagedReader)
			{
				if (server.IsExchange2007OrLater && server.IsMailboxServer && (!string.Equals(server.Name, base.SourceMailboxServer.Name) || (server.ServerSite != null && base.SourceMailboxServer.ServerSite != null && !string.Equals(server.ServerSite.Name, base.SourceMailboxServer.ServerSite.Name, StringComparison.OrdinalIgnoreCase))))
				{
					result = ServerIdParameter.Parse(server.Identity.ToString());
					break;
				}
			}
			return result;
		}

		private const string MonitoringDataSourceTypeLocal = "Local";

		private const string MonitoringDataSourceTypeRemote = "Remote";

		private const double LatencyPerformanceInCaseOfError = -1.0;

		private const string MailFlowGuidStr = "66c7004a-6860-44b2-983a-327aa3c9cfec";

		private MailboxDatabase targetSystemMailboxMdb;

		private Server targetMailboxServer;

		private string targetMailboxAddress;

		private string friendlySourceAddress;

		private string friendlyTargetAddress;

		private bool sourceServerHasMdb;

		private bool targetServerHasMdb;
	}
}
