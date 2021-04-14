using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class MailboxReplicationServiceClient : WcfClientWithFaultHandling<IMailboxReplicationService, FaultException<MailboxReplicationServiceFault>>
	{
		private MailboxReplicationServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public static MailboxReplicationServiceClient Create(string serverName)
		{
			return MailboxReplicationServiceClient.Create(serverName, MRSJobType.Unknown);
		}

		public static MailboxReplicationServiceClient Create(string serverName, MRSJobType jobType)
		{
			return MailboxReplicationServiceClient.Create(serverName, RequestJobXML.MapJobTypeToCapability(jobType));
		}

		public static MailboxReplicationServiceClient Create(string serverName, MRSCapabilities requiredCapability)
		{
			MrsTracer.Common.Debug("MRSClient: attempting to connect to '{0}'", new object[]
			{
				serverName
			});
			string text = string.Format("net.tcp://{0}/Microsoft.Exchange.MailboxReplicationService", serverName);
			NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.Transport);
			netTcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
			netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			int config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MrsBindingMaxMessageSize");
			netTcpBinding.MaxReceivedMessageSize = (long)config;
			netTcpBinding.ReaderQuotas.MaxStringContentLength = config;
			netTcpBinding.ReaderQuotas.MaxArrayLength = config;
			EndpointAddress remoteAddress;
			try
			{
				remoteAddress = new EndpointAddress(text);
			}
			catch (UriFormatException innerException)
			{
				throw new InvalidEndpointAddressPermanentException(text, innerException);
			}
			MailboxReplicationServiceClient mailboxReplicationServiceClient = null;
			bool flag = false;
			try
			{
				mailboxReplicationServiceClient = new MailboxReplicationServiceClient(netTcpBinding, remoteAddress);
				mailboxReplicationServiceClient.ExchangeVersionInformation(requiredCapability);
				flag = true;
			}
			finally
			{
				if (!flag && mailboxReplicationServiceClient != null)
				{
					mailboxReplicationServiceClient.Dispose();
				}
			}
			MrsTracer.Common.Debug("MRSClient: connected to '{0}', version {1}", new object[]
			{
				mailboxReplicationServiceClient.ServerVersion.ComputerName,
				mailboxReplicationServiceClient.ServerVersion
			});
			return mailboxReplicationServiceClient;
		}

		public MoveRequestInfo GetMoveRequestInfo(Guid requestGuid)
		{
			MoveRequestInfo result = null;
			this.CallService(delegate()
			{
				result = this.Channel.GetMoveRequestInfo(requestGuid);
			});
			return result;
		}

		public void SyncNow(List<SyncNowNotification> notifications)
		{
			this.CallService(delegate()
			{
				this.Channel.SyncNow(notifications);
			});
		}

		public void RefreshMoveRequest2(Guid requestGuid, Guid mdbGuid, int requestFlags, MoveRequestNotification op)
		{
			this.CallService(delegate()
			{
				this.Channel.RefreshMoveRequest2(requestGuid, mdbGuid, requestFlags, op);
			});
		}

		public void RefreshMoveRequest(Guid requestGuid, Guid mdbGuid, MoveRequestNotification op)
		{
			this.CallService(delegate()
			{
				this.Channel.RefreshMoveRequest(requestGuid, mdbGuid, op);
			});
		}

		public MailboxInformation GetMailboxInformation(TransactionalRequestJob requestJob, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, NetworkCredential cred)
		{
			MailboxInformation result = null;
			this.CallService(delegate()
			{
				string username = (cred != null) ? cred.UserName : null;
				string password = (cred != null) ? cred.Password : null;
				string domain = (cred != null) ? cred.Domain : null;
				if (this.ServerVersion[11])
				{
					string requestJobXml = XMLSerializableBase.Serialize(new RequestJobXML(requestJob), false);
					result = this.Channel.GetMailboxInformation4(requestJobXml, primaryMailboxGuid, physicalMailboxGuid, (partitionHint != null) ? partitionHint.GetPersistablePartitionHint() : null, targetMdbGuid, targetMdbName, remoteHostName, remoteOrgName, remoteDCName, username, password, domain);
					return;
				}
				if (this.ServerVersion[4])
				{
					result = this.Channel.GetMailboxInformation3(primaryMailboxGuid, physicalMailboxGuid, (partitionHint != null) ? partitionHint.GetPersistablePartitionHint() : null, targetMdbGuid, targetMdbName, remoteHostName, remoteOrgName, remoteDCName, username, password, domain);
					return;
				}
				result = this.Channel.GetMailboxInformation2(primaryMailboxGuid, physicalMailboxGuid, targetMdbGuid, targetMdbName, remoteHostName, remoteOrgName, remoteDCName, username, password, domain);
			});
			return result;
		}

		public TransactionalRequestJob ValidateAndPopulateRequestJob(TransactionalRequestJob requestJob, out List<ReportEntry> entries)
		{
			List<ReportEntry> entriesInternal = null;
			string resultString = null;
			string requestJobXML = XMLSerializableBase.Serialize(new RequestJobXML(requestJob), false);
			try
			{
				this.CallService(delegate()
				{
					string text = null;
					try
					{
						resultString = this.Channel.ValidateAndPopulateRequestJob(requestJobXML, out text);
					}
					finally
					{
						if (text != null)
						{
							entriesInternal = XMLSerializableBase.Deserialize<List<ReportEntry>>(text, false);
						}
					}
				});
			}
			finally
			{
				entries = entriesInternal;
			}
			return new TransactionalRequestJob(XMLSerializableBase.Deserialize<RequestJobXML>(resultString, false));
		}

		public void AttemptConnectToMRSProxy(string remoteHostName, Guid mbxGuid, string username, string password, string domain)
		{
			this.CallService(delegate()
			{
				this.Channel.AttemptConnectToMRSProxy(remoteHostName, mbxGuid, username, password, domain);
			});
		}

		public void PingMRSProxy(string serverFqdn, string username, string password, string domain, bool useHttps)
		{
			this.CallService(delegate()
			{
				this.Channel.PingMRSProxy(serverFqdn, username, password, domain, useHttps);
			});
		}

		internal static MailboxReplicationServiceClient Create(IConfigurationSession session, MRSJobType jobType, Guid mdbGuid, List<string> unreachableMrsServers)
		{
			MRSCapabilities requiredCapability = RequestJobXML.MapJobTypeToCapability(jobType);
			string text = null;
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.None);
			if (!string.IsNullOrEmpty(databaseInformation.ServerFqdn))
			{
				text = databaseInformation.ServerFqdn.ToLower(CultureInfo.InvariantCulture);
			}
			List<string> mrsServers = MailboxReplicationServiceClient.GetMrsServers(session, mdbGuid);
			List<string> list = new List<string>(mrsServers.Count);
			foreach (string text2 in mrsServers)
			{
				string text3 = text2.ToLower(CultureInfo.InvariantCulture);
				if (string.Compare(text3, text, CultureInfo.InvariantCulture, CompareOptions.Ordinal) != 0 && !unreachableMrsServers.Contains(text3))
				{
					list.Add(text2);
				}
			}
			List<string> list2 = CommonUtils.RandomizeSequence<string>(list);
			if (text != null)
			{
				list2.Insert(0, text);
			}
			foreach (string text4 in list2)
			{
				try
				{
					return MailboxReplicationServiceClient.Create(text4, requiredCapability);
				}
				catch (MailboxReplicationPermanentException ex)
				{
					MrsTracer.Common.Warning("Attempt to connect to CAS Server {0} failed with error: {1}", new object[]
					{
						text4,
						CommonUtils.FullExceptionMessage(ex)
					});
				}
				catch (MailboxReplicationTransientException ex2)
				{
					MrsTracer.Common.Warning("Attempt to connect to CAS Server {0} failed with error: {1}", new object[]
					{
						text4,
						CommonUtils.FullExceptionMessage(ex2)
					});
				}
				unreachableMrsServers.Add(text4.ToLower(CultureInfo.InvariantCulture));
			}
			throw new NoMRSAvailableTransientException();
		}

		internal static List<string> GetMrsServers(IConfigurationSession session, Guid mdbGuid)
		{
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.None);
			if (databaseInformation.ServerSite == null)
			{
				throw new UnableToDetermineMDBSitePermanentException(mdbGuid);
			}
			MrsTracer.Common.Debug("MDB '{0}' ({1}) found to belong to Site: '{2}'", new object[]
			{
				databaseInformation.DatabaseName,
				mdbGuid,
				databaseInformation.ServerSite
			});
			ServerVersion serverVersion = MailboxReplicationServiceClient.minRequiredVersion;
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 4UL),
				new ComparisonFilter(ComparisonOperator.NotEqual, ActiveDirectoryServerSchema.IsOutOfService, true),
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, serverVersion.ToInt()),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, databaseInformation.ServerSite.DistinguishedName)
			});
			Server[] array = session.Find<Server>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				MrsTracer.Common.Error("No Client Access servers running an appropriate version of Exchange 2010 (or later) were found in site '{0}'.", new object[]
				{
					databaseInformation.ServerSite.ToString()
				});
				throw new ErrorNoCASServersInSitePermanentException(databaseInformation.ServerSite.ToString(), serverVersion.ToString());
			}
			List<string> list = new List<string>(array.Length);
			foreach (Server server in array)
			{
				string fqdn = server.Fqdn;
				if (!string.IsNullOrEmpty(fqdn))
				{
					list.Add(fqdn);
				}
			}
			return list;
		}

		internal static string GetMrsServer(Guid mdbGuid)
		{
			return MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.None).ServerFqdn;
		}

		protected override void HandleFaultException(FaultException<MailboxReplicationServiceFault> fault, string context)
		{
			fault.Detail.ReconstructAndThrow(context, base.ServerVersion);
		}

		private void ExchangeVersionInformation(MRSCapabilities requiredCapability)
		{
			this.CallService(delegate()
			{
				VersionInformation serverVersion = null;
				base.Channel.ExchangeVersionInformation(VersionInformation.MRS, out serverVersion);
				base.ServerVersion = serverVersion;
			});
			if (!base.ServerVersion[0])
			{
				MrsTracer.Common.Error("Talking to downlevel server: no RTM support", new object[0]);
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.Endpoint.Address.ToString(), base.ServerVersion.ToString(), "E14_RTM_Support");
			}
			if (!base.ServerVersion[(int)requiredCapability])
			{
				MrsTracer.Common.Error("Talking to downlevel server: no {0} support", new object[]
				{
					requiredCapability.ToString()
				});
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.Endpoint.Address.ToString(), base.ServerVersion.ToString(), requiredCapability.ToString());
			}
		}

		private void CallService(Action serviceCall)
		{
			this.CallService(serviceCall, base.Endpoint.Address.ToString());
		}

		private static ServerVersion minRequiredVersion = new ServerVersion(15, 0, 0, 0);
	}
}
