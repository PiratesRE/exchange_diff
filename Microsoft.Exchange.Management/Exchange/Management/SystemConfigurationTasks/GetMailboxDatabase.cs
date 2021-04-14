using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailSubmission;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxDatabase", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxDatabase : GetDatabaseTask<MailboxDatabase>
	{
		[Parameter]
		public SwitchParameter DumpsterStatistics
		{
			get
			{
				return (SwitchParameter)(base.Fields["DumpsterStatistics"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DumpsterStatistics"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MailboxDatabase mailboxDatabase = (MailboxDatabase)dataObject;
			if (this.DumpsterStatistics)
			{
				Server server = this.m_ServerObject;
				if (server == null)
				{
					MailboxServerIdParameter id = MailboxServerIdParameter.Parse(mailboxDatabase.ServerName);
					server = (Server)base.GetDataObject<Server>(id, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(mailboxDatabase.ServerName)), new LocalizedString?(Strings.ErrorServerNotUnique(mailboxDatabase.ServerName)));
				}
				this.QueryDumpsterStats(server, mailboxDatabase.DistinguishedName, ref mailboxDatabase);
			}
			base.WriteResult(dataObject);
		}

		private void QueryDumpsterStats(Server mailboxServer, string databaseDn, ref MailboxDatabase entry)
		{
			List<string> serversWithServerRoleInSiteByServer = ReplayConfiguration.GetServersWithServerRoleInSiteByServer(ADObjectWrapperFactory.CreateWrapper(mailboxServer), ServerRole.HubTransport);
			List<string> list = new List<string>();
			List<DumpsterStatisticsEntry> list2 = new List<DumpsterStatisticsEntry>();
			foreach (string text in serversWithServerRoleInSiteByServer)
			{
				MailSubmissionServiceRpcClient mailSubmissionServiceRpcClient2;
				MailSubmissionServiceRpcClient mailSubmissionServiceRpcClient = mailSubmissionServiceRpcClient2 = new MailSubmissionServiceRpcClient(text);
				try
				{
					long ticksOldestItem = 0L;
					long queueSize = 0L;
					int numberOfItems = 0;
					long num = mailSubmissionServiceRpcClient.QueryDumpsterStats(databaseDn, ref ticksOldestItem, ref queueSize, ref numberOfItems);
					if (num != 0L)
					{
						ExTraceGlobals.CmdletsTracer.TraceError<string, long>((long)this.GetHashCode(), "GetDatabase.QueryDumpsterStats: RPC to server '{0}' returned {1}", text, num);
						list.Add(text);
					}
					else
					{
						list2.Add(new DumpsterStatisticsEntry(text, ticksOldestItem, queueSize, numberOfItems));
					}
				}
				catch (RpcException)
				{
					list.Add(text);
				}
				finally
				{
					if (mailSubmissionServiceRpcClient2 != null)
					{
						((IDisposable)mailSubmissionServiceRpcClient2).Dispose();
					}
				}
			}
			entry.m_DumpsterStatistics = new DumpsterStatisticsEntry[list2.Count];
			list2.CopyTo(entry.m_DumpsterStatistics);
			entry.m_DumpsterServersNotAvailable = new string[list.Count];
			list.CopyTo(entry.m_DumpsterServersNotAvailable);
		}
	}
}
