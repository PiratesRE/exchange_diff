using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
	internal class MailboxReplicationService : IMailboxReplicationService, IMailboxReplicationServiceSlim
	{
		public MailboxReplicationService()
		{
			this.clientVersion = null;
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, params object[] args)
		{
			MRSService.LogEvent(tuple, args);
		}

		void IMailboxReplicationService.ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			serverVersion = VersionInformation.MRS;
			this.clientVersion = clientVersion;
			this.ForwardKnownExceptions(delegate
			{
				if (clientVersion.ProductMajor < VersionInformation.MRS.ProductMajor)
				{
					throw new DownlevelClientsNotSupportedPermanentException();
				}
				if (!clientVersion[0])
				{
					MrsTracer.Service.Error("Talking to downlevel client: no Archive support", new object[0]);
					throw new UnsupportedClientVersionPermanentException(clientVersion.ComputerName, clientVersion.ToString(), "Archives");
				}
			}, null);
		}

		MoveRequestInfo IMailboxReplicationService.GetMoveRequestInfo(Guid requestGuid)
		{
			MoveRequestInfo result = new MoveRequestInfo();
			this.ForwardKnownExceptions(delegate
			{
				if (!MailboxSyncerJobs.ProcessJob(requestGuid, false, delegate(BaseJob job)
				{
					result = job.GetMoveRequestInfo();
				}))
				{
					MrsTracer.Service.Debug("Request {0} is not active.", new object[]
					{
						requestGuid
					});
					result.Message = MRSQueue.GetJobPickupFailureMessageForRequest(requestGuid);
				}
			}, null);
			return result;
		}

		void IMailboxReplicationService.SyncNow(List<SyncNowNotification> notifications)
		{
			this.SyncNow(notifications);
		}

		void IMailboxReplicationService.RefreshMoveRequest(Guid requestGuid, Guid mdbGuid, MoveRequestNotification op)
		{
			this.RefreshMoveRequest(requestGuid, mdbGuid, 0, op);
		}

		void IMailboxReplicationService.RefreshMoveRequest2(Guid requestGuid, Guid mdbGuid, int requestFlags, MoveRequestNotification op)
		{
			this.RefreshMoveRequest(requestGuid, mdbGuid, requestFlags, op);
		}

		MailboxInformation IMailboxReplicationService.GetMailboxInformation2(Guid primaryMailboxGuid, Guid physicalMailboxGuid, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain)
		{
			return ((IMailboxReplicationService)this).GetMailboxInformation3(primaryMailboxGuid, physicalMailboxGuid, null, targetMdbGuid, targetMdbName, remoteHostName, remoteOrgName, remoteDCName, username, password, domain);
		}

		MailboxInformation IMailboxReplicationService.GetMailboxInformation3(Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain)
		{
			return ((IMailboxReplicationService)this).GetMailboxInformation4(null, primaryMailboxGuid, physicalMailboxGuid, partitionHint, targetMdbGuid, targetMdbName, remoteHostName, remoteOrgName, remoteDCName, username, password, domain);
		}

		MailboxInformation IMailboxReplicationService.GetMailboxInformation4(string requestJobXml, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain)
		{
			MailboxInformation info = null;
			this.ForwardKnownExceptions(delegate
			{
				TenantPartitionHint partitionHint2 = (partitionHint != null) ? TenantPartitionHint.FromPersistablePartitionHint(partitionHint) : null;
				bool flag = string.IsNullOrEmpty(targetMdbName) && targetMdbGuid == Guid.Empty;
				NetworkCredential networkCredential = (!string.IsNullOrEmpty(username)) ? new NetworkCredential(username, password, domain) : null;
				MailboxType mbxType;
				IMailbox mailbox;
				if (string.IsNullOrEmpty(remoteHostName))
				{
					if (!string.IsNullOrEmpty(remoteDCName))
					{
						if (flag)
						{
							mbxType = MailboxType.SourceMailbox;
							mailbox = new MapiSourceMailbox(LocalMailboxFlags.Move);
						}
						else
						{
							mbxType = MailboxType.DestMailboxIntraOrg;
							mailbox = new MapiDestinationMailbox(LocalMailboxFlags.None);
						}
						mailbox.ConfigADConnection(remoteDCName, remoteDCName, networkCredential);
					}
					else
					{
						ProxyServerSettings proxyServerSettings;
						if (targetMdbGuid != Guid.Empty)
						{
							proxyServerSettings = CommonUtils.MapDatabaseToProxyServer(targetMdbGuid);
						}
						else
						{
							proxyServerSettings = CommonUtils.MapMailboxToProxyServer(new Guid?(physicalMailboxGuid), new Guid?(primaryMailboxGuid), partitionHint);
						}
						if (flag)
						{
							if (proxyServerSettings.Scenario == ProxyScenarios.LocalMdbAndProxy)
							{
								mailbox = new StorageSourceMailbox(LocalMailboxFlags.Move);
							}
							else
							{
								mailbox = new RemoteSourceMailbox(proxyServerSettings.Fqdn, null, null, ProxyControlFlags.DoNotApplyProxyThrottling, null, false, LocalMailboxFlags.Move);
							}
							mbxType = MailboxType.SourceMailbox;
						}
						else
						{
							if (proxyServerSettings.Scenario == ProxyScenarios.LocalMdbAndProxy)
							{
								mailbox = new StorageDestinationMailbox(LocalMailboxFlags.Move);
							}
							else
							{
								mailbox = new RemoteDestinationMailbox(proxyServerSettings.Fqdn, null, null, ProxyControlFlags.DoNotApplyProxyThrottling, null, false, LocalMailboxFlags.Move);
							}
							mbxType = MailboxType.DestMailboxIntraOrg;
						}
					}
				}
				else
				{
					ProxyControlFlags proxyControlFlags = ProxyControlFlags.DoNotApplyProxyThrottling;
					RequestJobXML requestJobXML = XMLSerializableBase.Deserialize<RequestJobXML>(requestJobXml, true);
					if (requestJobXML != null)
					{
						using (TransactionalRequestJob transactionalRequestJob = new TransactionalRequestJob(requestJobXML))
						{
							transactionalRequestJob.IsFake = true;
							proxyControlFlags |= transactionalRequestJob.GetProxyControlFlags();
						}
					}
					if (flag)
					{
						mailbox = new RemoteSourceMailbox(remoteHostName, remoteOrgName, networkCredential, proxyControlFlags, null, true, LocalMailboxFlags.Move);
						mbxType = MailboxType.SourceMailbox;
					}
					else
					{
						mailbox = new RemoteDestinationMailbox(remoteHostName, remoteOrgName, networkCredential, proxyControlFlags, null, true, LocalMailboxFlags.Move);
						mbxType = MailboxType.DestMailboxCrossOrg;
					}
				}
				using (mailbox)
				{
					mailbox.Config(null, primaryMailboxGuid, physicalMailboxGuid, partitionHint2, targetMdbGuid, mbxType, null);
					if (!string.IsNullOrEmpty(targetMdbName))
					{
						mailbox.ConfigMDBByName(targetMdbName);
					}
					mailbox.Connect(MailboxConnectFlags.None);
					using (SettingsContextBase.ActivateContext(mailbox as ISettingsContextProvider))
					{
						info = mailbox.GetMailboxInformation();
						ADUser aduser = mailbox.GetADUser();
						if (!this.clientVersion[2] && aduser.HasSeparatedArchive)
						{
							throw new UnsupportedClientVersionPermanentException(this.clientVersion.ComputerName, this.clientVersion.ToString(), "ArchiveSeparation");
						}
						info.UserDataXML = ConfigurableObjectXML.Serialize<ADUser>(aduser);
						info.ServerInformation = mailbox.GetMailboxServerInformation();
						mailbox.Disconnect();
					}
				}
			}, null);
			return info;
		}

		string IMailboxReplicationService.ValidateAndPopulateRequestJob(string requestJobXML, out string reportEntryXMLs)
		{
			string reportString = null;
			string resultString = null;
			try
			{
				this.ForwardKnownExceptions(delegate
				{
					List<ReportEntry> list = new List<ReportEntry>();
					try
					{
						RequestJobXML requestJob = XMLSerializableBase.Deserialize<RequestJobXML>(requestJobXML, true);
						using (TransactionalRequestJob transactionalRequestJob = new TransactionalRequestJob(requestJob))
						{
							transactionalRequestJob.IsFake = true;
							transactionalRequestJob.Identity = new RequestJobObjectId((transactionalRequestJob.RequestType == MRSRequestType.Move) ? transactionalRequestJob.ExchangeGuid : transactionalRequestJob.RequestGuid, (transactionalRequestJob.WorkItemQueueMdb == null) ? Guid.Empty : transactionalRequestJob.WorkItemQueueMdb.ObjectGuid, null);
							RequestIndexEntryProvider requestIndexEntryProvider = new RequestIndexEntryProvider();
							using (requestIndexEntryProvider.RescopeTo(transactionalRequestJob.DomainControllerToUpdate, transactionalRequestJob.OrganizationId))
							{
								if (transactionalRequestJob.SourceUserId != null)
								{
									transactionalRequestJob.SourceUser = requestIndexEntryProvider.ReadADUser(transactionalRequestJob.SourceUserId, transactionalRequestJob.SourceExchangeGuid);
								}
								if (transactionalRequestJob.TargetUserId != null)
								{
									transactionalRequestJob.TargetUser = requestIndexEntryProvider.ReadADUser(transactionalRequestJob.TargetUserId, transactionalRequestJob.TargetExchangeGuid);
								}
							}
							if (MailboxSyncerJobs.ContainsJob(transactionalRequestJob.IdentifyingGuid))
							{
								resultString = requestJobXML;
							}
							else
							{
								BaseJob baseJob = MailboxSyncerJobs.ConstructJob(transactionalRequestJob);
								if (baseJob == null)
								{
									MrsTracer.Service.Error("Don't know how to process '{0}' request", new object[]
									{
										transactionalRequestJob.RequestType
									});
									throw new RequestTypeNotUnderstoodPermanentException(CommonUtils.LocalComputerName, VersionInformation.MRS.ToString(), (int)transactionalRequestJob.RequestType);
								}
								using (baseJob)
								{
									baseJob.Initialize(transactionalRequestJob);
									baseJob.ValidateAndPopulateRequestJob(list);
									transactionalRequestJob.Message = baseJob.CachedRequestJob.Message;
									transactionalRequestJob.SourceVersion = baseJob.CachedRequestJob.SourceVersion;
									transactionalRequestJob.SourceArchiveVersion = baseJob.CachedRequestJob.SourceArchiveVersion;
									transactionalRequestJob.SourceServer = baseJob.CachedRequestJob.SourceServer;
									transactionalRequestJob.SourceArchiveServer = baseJob.CachedRequestJob.SourceArchiveServer;
									transactionalRequestJob.TargetVersion = baseJob.CachedRequestJob.TargetVersion;
									transactionalRequestJob.TargetArchiveVersion = baseJob.CachedRequestJob.TargetArchiveVersion;
									transactionalRequestJob.TargetServer = baseJob.CachedRequestJob.TargetServer;
									transactionalRequestJob.TargetArchiveServer = baseJob.CachedRequestJob.TargetArchiveServer;
									transactionalRequestJob.RemoteDatabaseGuid = baseJob.CachedRequestJob.RemoteDatabaseGuid;
									resultString = XMLSerializableBase.Serialize(new RequestJobXML(transactionalRequestJob), false);
								}
							}
						}
					}
					finally
					{
						reportString = XMLSerializableBase.Serialize(list, false);
					}
				}, null);
			}
			finally
			{
				reportEntryXMLs = reportString;
			}
			return resultString;
		}

		void IMailboxReplicationService.AttemptConnectToMRSProxy(string remoteHostName, Guid mbxGuid, string username, string password, string domain)
		{
			if (TestIntegration.Instance.SkipMrsProxyValidation)
			{
				return;
			}
			this.ForwardKnownExceptions(delegate
			{
				using (MailboxReplicationProxyClient.CreateConnectivityTestClient(remoteHostName, new NetworkCredential(username, password, domain), true))
				{
				}
			}, (Exception exception) => exception == null || exception.Message.IndexOf("The value of wsrm:Identifier is not a known Sequence identifier.", StringComparison.OrdinalIgnoreCase) >= 0);
		}

		void IMailboxReplicationService.PingMRSProxy(string serverFqdn, string username, string password, string domain, bool useHttps)
		{
			NetworkCredential credentials = null;
			if (!string.IsNullOrEmpty(username))
			{
				credentials = new NetworkCredential(username, password, domain);
			}
			this.ForwardKnownExceptions(delegate
			{
				using (MailboxReplicationProxyClient.CreateConnectivityTestClient(serverFqdn, credentials, useHttps))
				{
				}
			}, null);
		}

		void IMailboxReplicationServiceSlim.SyncNow(List<SyncNowNotification> notifications)
		{
			this.SyncNow(notifications);
		}

		private void SyncNow(List<SyncNowNotification> notifications)
		{
			this.ForwardKnownExceptions(delegate
			{
				foreach (SyncNowNotification notification in notifications)
				{
					MRSService.SyncNow(notification);
				}
			}, null);
		}

		private void RefreshMoveRequest(Guid mailboxGuid, Guid mdbGuid, int requestFlags, MoveRequestNotification op)
		{
			this.ForwardKnownExceptions(delegate
			{
				Guid empty = Guid.Empty;
				switch (op)
				{
				case MoveRequestNotification.Created:
					MRSService.ProcessMoveRequestCreatedNotification(mailboxGuid, mdbGuid);
					MRSService.Tickle(mailboxGuid, mdbGuid, op);
					return;
				case MoveRequestNotification.Updated:
					MRSService.Tickle(mailboxGuid, mdbGuid, op);
					return;
				case MoveRequestNotification.Canceled:
					MRSService.Tickle(mailboxGuid, mdbGuid, op);
					return;
				case MoveRequestNotification.SuspendResume:
					MRSService.Tickle(mailboxGuid, mdbGuid, op);
					return;
				default:
					return;
				}
			}, null);
		}

		private void ForwardKnownExceptions(Action operation, Func<Exception, bool> ignoreExceptionOperation = null)
		{
			Exception failure = null;
			try
			{
				CommonUtils.CatchKnownExceptions(operation, delegate(Exception f)
				{
					failure = f;
				});
			}
			catch (Exception ex)
			{
				MrsTracer.Service.Error("Unhandled exception in MRS:\n{0}\n{1}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex),
					ex.StackTrace
				});
				ExWatson.SendReport(ex);
				throw;
			}
			if (failure != null && (ignoreExceptionOperation == null || !ignoreExceptionOperation(failure)))
			{
				MailboxReplicationServiceFault.Throw(failure);
			}
		}

		private const string SequenceError = "The value of wsrm:Identifier is not a known Sequence identifier.";

		private VersionInformation clientVersion;
	}
}
