using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring.MailFlowTestHelper
{
	internal class CrossPremiseTestMailFlowHelper : TestMailFlowHelper
	{
		internal CrossPremiseTestMailFlowHelper(TestMailFlow taskInstance) : base(taskInstance)
		{
		}

		internal static bool IsCrossPremise(IConfigurationSession session)
		{
			CrossPremiseTestMailFlowHelper crossPremiseTestMailFlowHelper = new CrossPremiseTestMailFlowHelper(null);
			ADObjectId adFirstOrgRoot = CrossPremiseTestMailFlowHelper.GetAdFirstOrgRoot(session, "Administrative Groups");
			List<SmtpAddress> siteEgressTargets = crossPremiseTestMailFlowHelper.GetSiteEgressTargets(session, adFirstOrgRoot);
			return siteEgressTargets.Count<SmtpAddress>() != 0;
		}

		internal override void InternalValidate()
		{
			base.InternalValidate();
			base.SetMonitoringDataSourceType("Cross Premise");
			base.SourceMailboxServer = ((ITopologyConfigurationSession)base.Task.ConfigurationSession).FindLocalServer();
			this.localSite = base.SourceMailboxServer.ServerSite;
			this.adAdminGroups = CrossPremiseTestMailFlowHelper.GetAdFirstOrgRoot(base.Task.ConfigurationSession, "Administrative Groups");
			base.IsRemoteTest = true;
			this.localSystemMailbox = base.GetServerSystemMailbox(base.SourceMailboxServer);
		}

		internal override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.Task.MonitoringContext && !this.IsFirstActiveMdbLocallyMounted())
			{
				base.AddInformationMonitoringEvent(2004, Strings.CrossPremiseServerNotSelected(base.SourceMailboxServer.Name));
				return;
			}
			List<SmtpAddress> siteEgressTargets = this.GetSiteEgressTargets();
			if (siteEgressTargets.Count<SmtpAddress>() == 0)
			{
				base.AddWarningMonitoringEvent(2003, Strings.CrossPremiseNoEgressTargets(base.SourceMailboxServer.Name));
				base.Task.WriteWarning(Strings.CrossPremiseNoEgressTargets(base.SourceMailboxServer.Name));
				return;
			}
			ADSystemMailbox adsystemMailbox = this.localSystemMailbox as ADSystemMailbox;
			using (MapiStore mapiStore = MapiStore.OpenMailbox(base.SourceMailboxServer.Fqdn, this.localSystemMailbox.LegacyExchangeDN, adsystemMailbox.ExchangeGuid, adsystemMailbox.Database.ObjectGuid, this.localSystemMailbox.Name, null, null, ConnectFlag.UseAdminPrivilege | ConnectFlag.UseSeparateConnection, OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.MailboxGuid, CultureInfo.InvariantCulture, null, "Client=Management;Action=CrossPremiseTestMailFlow", null))
			{
				using (MapiFolder rootFolder = mapiStore.GetRootFolder())
				{
					using (MapiFolder inboxFolder = mapiStore.GetInboxFolder())
					{
						using (MapiFolder mapiFolder = rootFolder.CreateFolder("Cross Premise", "The folder for cross premise mailflow monitoring.", true))
						{
							if (base.Task.MonitoringContext)
							{
								this.MatchCrossPremiseMessages(inboxFolder, mapiFolder, this.localSystemMailbox.WindowsEmailAddress, siteEgressTargets);
								using (List<SmtpAddress>.Enumerator enumerator = siteEgressTargets.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										SmtpAddress smtpAddress = enumerator.Current;
										string subject = string.Format("{0}{{{1}}}", "CrossPremiseMailFlowMonitoring-", Guid.NewGuid());
										TestMailFlowHelper.CreateAndSubmitMessage(mapiFolder, this.localSystemMailbox.Name, smtpAddress.ToString(), subject, false);
									}
									goto IL_238;
								}
							}
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							ExDateTime utcNow = ExDateTime.UtcNow;
							foreach (SmtpAddress smtpAddress2 in siteEgressTargets)
							{
								string text = string.Format("{0}{{{1}}}", "CrossPremiseMailFlowMonitoring-", Guid.NewGuid());
								TestMailFlowHelper.CreateAndSubmitMessage(mapiFolder, this.localSystemMailbox.Name, smtpAddress2.ToString(), text, true);
								dictionary[text] = smtpAddress2.ToString();
							}
							this.WaitAndProcessProbeResponses(inboxFolder, this.localSystemMailbox.Name, dictionary, utcNow);
							IL_238:;
						}
					}
				}
			}
		}

		internal override void InternalStateReset()
		{
			base.InternalStateReset();
		}

		protected List<SmtpAddress> GetSiteEgressCrossPremiseTargets(IList<SmtpAddress> crossPremiseTargets, IList<SmtpSendConnectorConfig> sendConnectors, IList<SmtpSendConnectorConfig> siteSendConnectors)
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			foreach (SmtpAddress item in crossPremiseTargets)
			{
				string text = null;
				int num = int.MaxValue;
				SmtpSendConnectorConfig smtpSendConnectorConfig = null;
				foreach (SmtpSendConnectorConfig smtpSendConnectorConfig2 in sendConnectors)
				{
					if (smtpSendConnectorConfig2.Enabled)
					{
						foreach (AddressSpace addressSpace in smtpSendConnectorConfig2.AddressSpaces)
						{
							if (addressSpace.IsSmtpType && (addressSpace.DomainWithSubdomains.Match(item.Domain) > 0 || addressSpace.DomainWithSubdomains.IsStar))
							{
								if (text == null || text.Length < addressSpace.Domain.Length)
								{
									text = addressSpace.Domain;
									num = addressSpace.Cost;
									smtpSendConnectorConfig = smtpSendConnectorConfig2;
								}
								else if (text.Length == addressSpace.Domain.Length && (siteSendConnectors.Contains(smtpSendConnectorConfig2) || (!siteSendConnectors.Contains(smtpSendConnectorConfig) && num > addressSpace.Cost)))
								{
									text = addressSpace.Domain;
									num = addressSpace.Cost;
									smtpSendConnectorConfig = smtpSendConnectorConfig2;
								}
							}
						}
					}
				}
				if (smtpSendConnectorConfig != null && siteSendConnectors.Contains(smtpSendConnectorConfig))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static bool TryLoadAdObjects<T>(IConfigurationSession session, ADObjectId rootId, out IList<T> objects) where T : ADConfigurationObject, new()
		{
			List<T> results = new List<T>();
			bool result = ADNotificationAdapter.TryReadConfigurationPaged<T>(() => session.FindPaged<T>(rootId, QueryScope.SubTree, null, null, ADGenericPagedReader<T>.DefaultPageSize), delegate(T configObject)
			{
				results.Add(configObject);
			});
			objects = results;
			return result;
		}

		private static ADObjectId GetAdFirstOrgRoot(IConfigurationSession session, string groupName)
		{
			return CrossPremiseTestMailFlowHelper.GetAdChildRoot(session.GetOrgContainerId(), groupName);
		}

		private static ADObjectId GetAdChildRoot(ADObjectId currentRoot, string groupName)
		{
			if (currentRoot != null)
			{
				ADObjectId rootId = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					rootId = currentRoot.GetChildId(groupName);
				});
				if (adoperationResult.Succeeded)
				{
					return rootId;
				}
			}
			return null;
		}

		private static IList<SmtpSendConnectorConfig> GetEnabledSendConnectorsInSite(IConfigurationSession session, ADObjectId site, IList<SmtpSendConnectorConfig> sendConnectors)
		{
			IList<SmtpSendConnectorConfig> list = new List<SmtpSendConnectorConfig>();
			foreach (SmtpSendConnectorConfig smtpSendConnectorConfig in sendConnectors)
			{
				if (smtpSendConnectorConfig.Enabled)
				{
					ADObjectId adobjectId = smtpSendConnectorConfig.HomeMtaServerId;
					if (adobjectId == null && ADGlobalConfigSettings.SoftLinkEnabled)
					{
						adobjectId = ADGroup.GetServerIdFromHomeMta(ADObjectIdResolutionHelper.ResolveDN(smtpSendConnectorConfig.HomeMTA));
					}
					Server server = session.Read<Server>(adobjectId);
					if (server != null && string.Equals(server.ServerSite.ToString(), site.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						list.Add(smtpSendConnectorConfig);
					}
				}
			}
			return list;
		}

		private List<SmtpAddress> GetEnabledCrossPremiseRemoteDomainTargets(IConfigurationSession session)
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			ADObjectId adChildRoot = CrossPremiseTestMailFlowHelper.GetAdChildRoot(CrossPremiseTestMailFlowHelper.GetAdFirstOrgRoot(session, "Global Settings"), "Internet Message Formats");
			if (adChildRoot == null)
			{
				return list;
			}
			IList<DomainContentConfig> list2;
			if (!CrossPremiseTestMailFlowHelper.TryLoadAdObjects<DomainContentConfig>(session, adChildRoot, out list2))
			{
				return list;
			}
			foreach (DomainContentConfig domainContentConfig in list2)
			{
				if (!domainContentConfig.DomainName.IncludeSubDomains && domainContentConfig.TrustedMailOutboundEnabled)
				{
					SmtpAddress item = new SmtpAddress("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042", domainContentConfig.DomainName.Domain);
					if (item.IsValidAddress)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		private List<SmtpAddress> GetEnabledCrossPremiseAcceptedDomainTargets(IConfigurationSession session)
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			QueryFilter filter = new NotFilter(new BitMaskOrFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
			OrganizationId currentOrganizationId = session.SessionSettings.CurrentOrganizationId;
			ADObjectId rootId = (OrganizationId.ForestWideOrgId.Equals(currentOrganizationId) || currentOrganizationId == null) ? session.GetOrgContainerId() : currentOrganizationId.ConfigurationUnit;
			ADPagedReader<AcceptedDomain> adpagedReader = session.FindPaged<AcceptedDomain>(rootId, QueryScope.SubTree, filter, null, 0);
			using (IEnumerator<AcceptedDomain> enumerator = adpagedReader.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.DomainName.IncludeSubDomains && enumerator.Current.IsCoexistenceDomain)
					{
						SmtpAddress item = new SmtpAddress("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042", enumerator.Current.DomainName.Domain);
						if (item.IsValidAddress)
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		private List<SmtpAddress> GetEnabledCrossPremiseTargets(IConfigurationSession session)
		{
			HashSet<SmtpAddress> hashSet = new HashSet<SmtpAddress>();
			foreach (SmtpAddress item in this.GetEnabledCrossPremiseRemoteDomainTargets(session))
			{
				hashSet.Add(item);
			}
			foreach (SmtpAddress item2 in this.GetEnabledCrossPremiseAcceptedDomainTargets(session))
			{
				hashSet.Add(item2);
			}
			return hashSet.ToList<SmtpAddress>();
		}

		private List<SmtpAddress> GetSiteEgressTargets(IConfigurationSession session, ADObjectId adAdminGroups)
		{
			List<SmtpAddress> enabledCrossPremiseTargets = this.GetEnabledCrossPremiseTargets(session);
			IList<SmtpSendConnectorConfig> sendConnectors;
			if (enabledCrossPremiseTargets.Count == 0 || !CrossPremiseTestMailFlowHelper.TryLoadAdObjects<SmtpSendConnectorConfig>(session, adAdminGroups, out sendConnectors))
			{
				return new List<SmtpAddress>();
			}
			IList<SmtpSendConnectorConfig> enabledSendConnectorsInSite = CrossPremiseTestMailFlowHelper.GetEnabledSendConnectorsInSite(base.Task.ConfigurationSession, this.localSite, sendConnectors);
			return this.GetSiteEgressCrossPremiseTargets(enabledCrossPremiseTargets, sendConnectors, enabledSendConnectorsInSite);
		}

		private List<SmtpAddress> GetSiteEgressTargets()
		{
			return this.GetSiteEgressTargets(base.Task.ConfigurationSession, this.adAdminGroups);
		}

		private bool IsFirstActiveMdbLocallyMounted()
		{
			IList<Database> list;
			if (!base.SourceMailboxServer.IsMailboxServer || !CrossPremiseTestMailFlowHelper.TryLoadAdObjects<Database>(base.Task.ConfigurationSession, this.adAdminGroups, out list))
			{
				return false;
			}
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			SortedList<Guid, string> sortedList = new SortedList<Guid, string>();
			try
			{
				foreach (Database database in list)
				{
					DatabaseLocationInfo serverForDatabase = cachingActiveManagerInstance.GetServerForDatabase(database.Guid, GetServerForDatabaseFlags.None);
					if (serverForDatabase != null && serverForDatabase.ServerSite != null && serverForDatabase.ServerSite.Equals(this.localSite) && !string.IsNullOrEmpty(serverForDatabase.ServerFqdn))
					{
						sortedList.Add(database.Guid, serverForDatabase.ServerFqdn);
					}
				}
				using (IEnumerator<KeyValuePair<Guid, string>> enumerator2 = sortedList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<Guid, string> kvp = enumerator2.Current;
						Database database2 = list.ToList<Database>().Find(delegate(Database db)
						{
							Guid guid = db.Guid;
							KeyValuePair<Guid, string> kvp3 = kvp;
							return guid == kvp3.Key;
						});
						Database mdb = database2;
						KeyValuePair<Guid, string> kvp4 = kvp;
						if (base.IsMdbMounted(mdb, kvp4.Value))
						{
							KeyValuePair<Guid, string> kvp2 = kvp;
							return kvp2.Value.Equals(base.SourceMailboxServer.Fqdn, StringComparison.OrdinalIgnoreCase);
						}
					}
				}
				return false;
			}
			catch (ObjectNotFoundException ex)
			{
				base.Task.WriteWarning(Strings.CrossPremiseMapMdbToServerFailure(ex.ToString()));
			}
			catch (StoragePermanentException ex2)
			{
				base.Task.WriteWarning(Strings.CrossPremiseMapMdbToServerFailure(ex2.ToString()));
			}
			catch (StorageTransientException ex3)
			{
				base.Task.WriteWarning(Strings.CrossPremiseMapMdbToServerFailure(ex3.ToString()));
			}
			return false;
		}

		private void WaitAndProcessProbeResponses(MapiFolder folder, string fromAddress, Dictionary<string, string> subjectTargets, ExDateTime probeSentTime)
		{
			List<byte[]> list = new List<byte[]>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < base.Task.ExecutionTimeout; i++)
			{
				foreach (KeyValuePair<string, string> keyValuePair in subjectTargets)
				{
					if (!string.IsNullOrEmpty(keyValuePair.Value))
					{
						using (MapiMessage deliveryReceipt = TestMailFlowHelper.GetDeliveryReceipt(folder, keyValuePair.Key, false))
						{
							if (deliveryReceipt != null)
							{
								PropValue prop = deliveryReceipt.GetProp(PropTag.MessageDeliveryTime);
								PropValue prop2 = deliveryReceipt.GetProp(PropTag.EntryId);
								PropValue prop3 = deliveryReceipt.GetProp(PropTag.Subject);
								if (!prop.IsError() && prop.Value != null && prop3.Value != null)
								{
									string text = prop3.Value.ToString();
									if (text.StartsWith("RSP: CrossPremiseMailFlowMonitoring-", StringComparison.OrdinalIgnoreCase))
									{
										EnhancedTimeSpan latency = ((ExDateTime)prop.GetDateTime() > probeSentTime) ? ((ExDateTime)prop.GetDateTime() - probeSentTime) : EnhancedTimeSpan.Zero;
										base.OutputResult(Strings.TestMailflowSucceeded(fromAddress, keyValuePair.Value), latency, base.IsRemoteTest);
									}
									else if (text.StartsWith("Undeliverable: CrossPremiseMailFlowMonitoring-", StringComparison.OrdinalIgnoreCase))
									{
										EnhancedTimeSpan latency2 = EnhancedTimeSpan.FromSeconds(0.0);
										string info = (string)deliveryReceipt.GetProp(PropTag.Body).Value;
										base.OutputResult(Strings.CrossPremiseProbeNdred(fromAddress, keyValuePair.Value, info), latency2, base.IsRemoteTest);
									}
									list2.Add(keyValuePair.Key);
									list.Add(prop2.GetBytes());
								}
							}
						}
					}
				}
				foreach (string key in list2)
				{
					subjectTargets.Remove(key);
				}
				list2.Clear();
				if (subjectTargets.Count == 0)
				{
					break;
				}
				Thread.Sleep(1000);
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in subjectTargets)
			{
				if (!string.IsNullOrEmpty(keyValuePair2.Value))
				{
					EnhancedTimeSpan latency3 = EnhancedTimeSpan.FromSeconds(0.0);
					base.OutputResult(Strings.MapiTransactionResultFailure, latency3, base.IsRemoteTest);
				}
			}
			folder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, list.ToArray());
		}

		private bool ProcessCrossPremiseMessagePair(MapiMessage probe, MapiMessage response, string source, string target, out EnhancedTimeSpan probeLatency, out EnhancedTimeSpan responseLatency)
		{
			string text = response.GetProp(PropTag.Subject).Value.ToString();
			string info = (string)response.GetProp(PropTag.Body).Value;
			ExDateTime exDateTime = (ExDateTime)response.GetProp(PropTag.ClientSubmitTime).GetDateTime();
			ExDateTime exDateTime2 = (ExDateTime)response.GetProp(PropTag.MessageDeliveryTime).GetDateTime();
			ExDateTime exDateTime3 = (ExDateTime)probe.GetProp(PropTag.ClientSubmitTime).GetDateTime();
			ExDateTime exDateTime4 = exDateTime;
			probeLatency = ((exDateTime4 > exDateTime3) ? (exDateTime4 - exDateTime3) : EnhancedTimeSpan.Zero);
			responseLatency = ((exDateTime2 > exDateTime4) ? (exDateTime2 - exDateTime4) : EnhancedTimeSpan.Zero);
			if (text.StartsWith("RSP: CrossPremiseMailFlowMonitoring-", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (text.StartsWith("Undeliverable: CrossPremiseMailFlowMonitoring-", StringComparison.OrdinalIgnoreCase))
			{
				base.AddErrorMonitoringEvent(2002, Strings.CrossPremiseProbeNdred(source, target, info));
			}
			return false;
		}

		private void MatchCrossPremiseMessages(MapiFolder responseFolder, MapiFolder probeFolder, SmtpAddress source, List<SmtpAddress> targets)
		{
			using (MapiTable contentsTable = responseFolder.GetContentsTable())
			{
				using (MapiTable contentsTable2 = probeFolder.GetContentsTable())
				{
					contentsTable.SetColumns(new PropTag[]
					{
						PropTag.EntryId,
						PropTag.Subject,
						PropTag.ClientSubmitTime
					});
					contentsTable2.SetColumns(new PropTag[]
					{
						PropTag.EntryId,
						PropTag.Subject,
						PropTag.ClientSubmitTime
					});
					PropValue[][] array = contentsTable.QueryRows(1000, QueryRowsFlags.None);
					PropValue[][] array2 = contentsTable2.QueryRows(1000, QueryRowsFlags.None);
					List<byte[]> list = new List<byte[]>();
					List<byte[]> list2 = new List<byte[]>();
					Dictionary<SmtpAddress, CrossPremiseTestMailFlowHelper.HealthData> dictionary = new Dictionary<SmtpAddress, CrossPremiseTestMailFlowHelper.HealthData>(targets.Count);
					for (int i = 0; i <= array2.GetUpperBound(0); i++)
					{
						if (TestMailFlowHelper.IsValidPropData(array2, i, 3))
						{
							string text = (string)array2[i][1].Value;
							if (text.StartsWith("CrossPremiseMailFlowMonitoring-", StringComparison.OrdinalIgnoreCase))
							{
								byte[] bytes = array2[i][0].GetBytes();
								using (MapiMessage mapiMessage = (MapiMessage)probeFolder.OpenEntry(bytes))
								{
									SmtpAddress key = SmtpAddress.Parse((string)mapiMessage.GetProp(PropTag.ReceivedByEmailAddress).Value);
									if (!dictionary.ContainsKey(key))
									{
										dictionary.Add(key, new CrossPremiseTestMailFlowHelper.HealthData(EnhancedTimeSpan.Zero, EnhancedTimeSpan.Zero, 0, 0, 0, 0));
									}
									ExDateTime exDateTime = (ExDateTime)array2[i][2].GetDateTime();
									if (exDateTime.Add(base.Task.CrossPremisesExpirationTimeout) < ExDateTime.UtcNow)
									{
										dictionary[key].ExpiredNumber++;
										list.Add(bytes);
									}
									else
									{
										for (int j = 0; j <= array.GetUpperBound(0); j++)
										{
											if (TestMailFlowHelper.IsValidPropData(array, j, 3))
											{
												string text2 = (string)array[j][1].Value;
												if (text2.EndsWith(text, StringComparison.OrdinalIgnoreCase))
												{
													byte[] bytes2 = array[j][0].GetBytes();
													if (((ExDateTime)array[j][2].GetDateTime()).Add(base.Task.CrossPremisesExpirationTimeout) < ExDateTime.UtcNow)
													{
														list2.Add(bytes2);
													}
													else
													{
														using (MapiMessage mapiMessage2 = (MapiMessage)responseFolder.OpenEntry(bytes2))
														{
															EnhancedTimeSpan t;
															EnhancedTimeSpan t2;
															if (this.ProcessCrossPremiseMessagePair(mapiMessage, mapiMessage2, source.ToString(), key.ToString(), out t, out t2))
															{
																dictionary[key].ProbeLatency += t;
																dictionary[key].ResponseLatency += t2;
																dictionary[key].SuccessNumber++;
															}
															else
															{
																dictionary[key].FailedNumber++;
															}
														}
														list2.Add(bytes2);
														list.Add(bytes);
													}
												}
											}
										}
										if (!list.Contains(bytes) && exDateTime.AddMinutes(10.0) < ExDateTime.UtcNow)
										{
											dictionary[key].PendingNumber++;
										}
									}
								}
							}
						}
					}
					this.SaveHealthData(source, dictionary);
					if (list2.Count > 0)
					{
						responseFolder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, list2.ToArray());
					}
					if (list.Count > 0)
					{
						probeFolder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, list.ToArray());
					}
				}
			}
		}

		private void SaveHealthData(SmtpAddress source, Dictionary<SmtpAddress, CrossPremiseTestMailFlowHelper.HealthData> targetHealth)
		{
			foreach (KeyValuePair<SmtpAddress, CrossPremiseTestMailFlowHelper.HealthData> keyValuePair in targetHealth)
			{
				string str = string.Format("{0} -> {1} : ", this.localSite.Name, keyValuePair.Key.Domain);
				CrossPremiseTestMailFlowHelper.HealthData value = keyValuePair.Value;
				if (value.SuccessNumber > 0)
				{
					base.AddPerfCounter("Probe Latency", str + "Probe Latency", value.ProbeLatency.TotalSeconds / (double)value.SuccessNumber);
					base.AddPerfCounter("Response Latency", str + "Response Latency", value.ResponseLatency.TotalSeconds / (double)value.SuccessNumber);
					base.AddPerfCounter("Loop Latency", str + "Loop Latency", (value.ProbeLatency + value.ResponseLatency).TotalSeconds / (double)value.SuccessNumber);
				}
				base.AddPerfCounter("Success Probe Number", str + "Success Probe Number", (double)value.SuccessNumber);
				base.AddPerfCounter("Failed Probe Number", str + "Failed Probe Number", (double)value.FailedNumber);
				base.AddPerfCounter("Pending Probe Number", str + "Pending Probe Number", (double)value.PendingNumber);
				base.AddPerfCounter("Expired Probe Number", str + "Expired Probe Number", (double)value.ExpiredNumber);
				if (value.PendingNumber >= base.Task.CrossPremisesPendingErrorCount)
				{
					base.AddErrorMonitoringEvent(2001, Strings.CrossPremiseProbesPending(source.ToString(), keyValuePair.Key.ToString()));
				}
				else if (value.SuccessNumber > 0)
				{
					base.AddSuccessMonitoringEvent(2000, Strings.CrossPremiseProbeResponseMatch(source.ToString(), keyValuePair.Key.ToString()));
				}
			}
		}

		private const string MonitoringDataSourceType = "Cross Premise";

		private const string ProbeSubjectPrefix = "CrossPremiseMailFlowMonitoring-";

		private const string ResponseSubjectPrefix = "RSP: CrossPremiseMailFlowMonitoring-";

		private const string NdrSubjectPrefix = "Undeliverable: CrossPremiseMailFlowMonitoring-";

		private const string AdminGroupsName = "Administrative Groups";

		private const string GlobalSettingsGroupName = "Global Settings";

		private const string InternetMessageFormatsGroupName = "Internet Message Formats";

		private const string TargetLocalPart = "FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042";

		private ADObjectId localSite;

		private ADRecipient localSystemMailbox;

		private ADObjectId adAdminGroups;

		private class HealthData
		{
			public HealthData(EnhancedTimeSpan probeLatency, EnhancedTimeSpan responseLatency, int successNumber, int failedNumber, int pendingNumber, int expiredNumber)
			{
				this.probeLatency = probeLatency;
				this.responseLatency = responseLatency;
				this.successNumber = successNumber;
				this.failedNumber = failedNumber;
				this.pendingNumber = pendingNumber;
				this.expiredNumber = expiredNumber;
			}

			public EnhancedTimeSpan ProbeLatency
			{
				get
				{
					return this.probeLatency;
				}
				set
				{
					this.probeLatency = value;
				}
			}

			public EnhancedTimeSpan ResponseLatency
			{
				get
				{
					return this.responseLatency;
				}
				set
				{
					this.responseLatency = value;
				}
			}

			public int SuccessNumber
			{
				get
				{
					return this.successNumber;
				}
				set
				{
					this.successNumber = value;
				}
			}

			public int FailedNumber
			{
				get
				{
					return this.failedNumber;
				}
				set
				{
					this.failedNumber = value;
				}
			}

			public int PendingNumber
			{
				get
				{
					return this.pendingNumber;
				}
				set
				{
					this.pendingNumber = value;
				}
			}

			public int ExpiredNumber
			{
				get
				{
					return this.expiredNumber;
				}
				set
				{
					this.expiredNumber = value;
				}
			}

			private EnhancedTimeSpan probeLatency;

			private EnhancedTimeSpan responseLatency;

			private int successNumber;

			private int failedNumber;

			private int pendingNumber;

			private int expiredNumber;
		}
	}
}
