using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Mserve;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Common.Internal;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Mserve
{
	internal class MserveSynchronizationProvider : SynchronizationProvider
	{
		public static int PartnerId
		{
			get
			{
				return MserveSynchronizationProvider.partnerId;
			}
			set
			{
				MserveSynchronizationProvider.partnerId = value;
			}
		}

		public override string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public override int LeaseLockTryCount
		{
			get
			{
				return 1;
			}
		}

		public override List<TargetServerConfig> TargetServerConfigs
		{
			get
			{
				return this.targetServerConfigs;
			}
		}

		public override EnhancedTimeSpan RecipientSyncInterval
		{
			get
			{
				return EdgeSyncSvc.EdgeSync.Config.ServiceConfig.RecipientSyncInterval;
			}
		}

		public override EnhancedTimeSpan ConfigurationSyncInterval
		{
			get
			{
				return EdgeSyncSvc.EdgeSync.Config.ServiceConfig.ConfigurationSyncInterval;
			}
		}

		public override void Initialize(EdgeSyncConnector connector)
		{
			this.identity = ((EdgeSyncMservConnector)connector).Name;
			this.targetServerConfigs = new List<TargetServerConfig>();
			this.targetServerConfigs.Add(new MserveTargetServerConfig(((EdgeSyncMservConnector)connector).Name, ((EdgeSyncMservConnector)connector).ProvisionUrl.AbsoluteUri, ((EdgeSyncMservConnector)connector).SettingUrl.AbsoluteUri, ((EdgeSyncMservConnector)connector).RemoteCertificate, ((EdgeSyncMservConnector)connector).PrimaryLeaseLocation, ((EdgeSyncMservConnector)connector).BackupLeaseLocation));
			if (MserveSynchronizationProvider.partnerId == -1)
			{
				MserveSynchronizationProvider.LoadPartnerId();
			}
			if (string.IsNullOrEmpty(MserveSynchronizationProvider.rootDomainLostAndFoundContainerDN))
			{
				MserveSynchronizationProvider.LoadRootDomainLostAndFoundContainerDN();
			}
		}

		private static void LoadRootDomainLostAndFoundContainerDN()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				string rootDomainNamingContextFromCurrentReadConnection = MserveSynchronizationProvider.ConfigSession.GetRootDomainNamingContextFromCurrentReadConnection();
				LostAndFound lostAndFound = MserveSynchronizationProvider.ConfigSession.ResolveWellKnownGuid<LostAndFound>(WellKnownGuid.LostAndFoundContainerWkGuid, rootDomainNamingContextFromCurrentReadConnection);
				if (lostAndFound != null)
				{
					MserveSynchronizationProvider.rootDomainLostAndFoundContainerDN = lostAndFound.DistinguishedName;
				}
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				throw new ExDirectoryException("Failed to get rootDomainLostAndFoundContainerDN because of AD exception.", adoperationResult.Exception);
			}
			if (string.IsNullOrEmpty(MserveSynchronizationProvider.rootDomainLostAndFoundContainerDN))
			{
				throw new ExDirectoryException("Failed to get rootDomainLostAndFoundContainerDN because its value is null.", null);
			}
		}

		private static void LoadPartnerId()
		{
			ADSite localSite = null;
			ADOperationResult adoperationResult = ADOperationResult.Success;
			adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localSite = MserveSynchronizationProvider.ConfigSession.GetLocalSite();
				if (localSite == null)
				{
					throw new ExDirectoryException(Strings.CannotGetLocalSite, null);
				}
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				throw new ExDirectoryException(Strings.CannotGetLocalSite, adoperationResult.Exception);
			}
			MserveSynchronizationProvider.partnerId = localSite.PartnerId;
		}

		public override List<TypeSynchronizer> CreateTypeSynchronizer(SyncTreeType type)
		{
			List<TypeSynchronizer> list = new List<TypeSynchronizer>();
			if (type == SyncTreeType.Recipients)
			{
				list.Add(new TypeSynchronizer(new Filter(MserveSynchronizationProvider.LoadAndFilter), new PreDecorate(MserveSynchronizationProvider.PreDecorate), null, null, null, null, "Mserve hosted recipients", Schema.Query.QueryAllHostedSmtpRecipients, null, SearchScope.Subtree, MserveSynchronizationProvider.ReplicationAttributes, null, true));
				ExTraceGlobals.ProviderTracer.TraceDebug((long)this.GetHashCode(), "Mserve provider created typeSynchronizer");
			}
			return list;
		}

		public override TargetConnection CreateTargetConnection(TargetServerConfig targetServerConfig, SyncTreeType type, TestShutdownAndLeaseDelegate testShutdownAndLease, EdgeSyncLogSession logSession)
		{
			return new MserveTargetConnection(EdgeSyncSvc.EdgeSync.Topology.LocalServer.VersionNumber, targetServerConfig as MserveTargetServerConfig, this.RecipientSyncInterval, testShutdownAndLease, logSession);
		}

		private static bool PreDecorate(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection, object state)
		{
			if (!entry.IsDeleted)
			{
				foreach (string text in MserveSynchronizationProvider.AddressAttributeNames)
				{
					if (entry.Attributes.ContainsKey(text))
					{
						DirectoryAttribute directoryAttribute = entry.Attributes[text];
						if (text.Equals("msExchArchiveGUID", StringComparison.OrdinalIgnoreCase))
						{
							if (entry.Attributes.ContainsKey("msExchArchiveGUID"))
							{
								DirectoryAttribute value = null;
								if (MserveSynchronizationProvider.TryTransformAchiveGuidToSmtpAddress(directoryAttribute, out value))
								{
									entry.Attributes["ArchiveAddress"] = value;
								}
								else
								{
									entry.Attributes["ArchiveAddress"] = new DirectoryAttribute("ArchiveAddress", MserveSynchronizationProvider.EmptyList);
								}
							}
							entry.Attributes.Remove("msExchArchiveGUID");
						}
						else
						{
							List<string> list = new List<string>();
							for (int j = 0; j < directoryAttribute.Count; j++)
							{
								string text2 = directoryAttribute[j] as string;
								if (text2.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("meum:", StringComparison.OrdinalIgnoreCase))
								{
									list.Add(text2);
								}
							}
							entry.Attributes[text] = new DirectoryAttribute(directoryAttribute.Name, list.ToArray());
						}
					}
				}
			}
			else
			{
				Dictionary<string, DirectoryAttribute> dictionary = sourceConnection.ReadObjectAttribute(entry.DistinguishedName, true, new string[]
				{
					"msExchExternalSyncState"
				});
				foreach (string key in dictionary.Keys)
				{
					entry.Attributes.Add(key, dictionary[key]);
				}
			}
			return true;
		}

		private static bool TryTransformAchiveGuidToSmtpAddress(DirectoryAttribute archiveGuidAttribute, out DirectoryAttribute archiveAddressAttribute)
		{
			archiveAddressAttribute = null;
			if (archiveGuidAttribute == null || archiveGuidAttribute.Count < 1)
			{
				return false;
			}
			byte[] array = archiveGuidAttribute[0] as byte[];
			if (array != null)
			{
				Guid guid = new Guid(array);
				string text = guid.ToString() + "@archive.exchangelabs.com";
				archiveAddressAttribute = new DirectoryAttribute("ArchiveAddress", new string[]
				{
					text
				});
				return true;
			}
			return false;
		}

		private static FilterResult LoadAndFilter(ExSearchResultEntry entry, Connection sourceConnection, TargetConnection targetConnection)
		{
			if (entry.DistinguishedName.EndsWith(MserveSynchronizationProvider.rootDomainLostAndFoundContainerDN, StringComparison.OrdinalIgnoreCase))
			{
				return FilterResult.Skip;
			}
			ExSearchResultEntry exSearchResultEntry = sourceConnection.ReadObjectEntry(entry.DistinguishedName, MserveSynchronizationProvider.RequiredAttributes);
			if (exSearchResultEntry == null)
			{
				return FilterResult.Skip;
			}
			RecipientTypeDetails recipientTypeDetails = RecipientTypeDetails.None;
			DirectoryAttribute directoryAttribute = null;
			if (exSearchResultEntry.Attributes.TryGetValue("msExchRecipientTypeDetails", out directoryAttribute) && directoryAttribute != null && directoryAttribute.Count > 0)
			{
				try
				{
					recipientTypeDetails = (RecipientTypeDetails)Enum.Parse(typeof(RecipientTypeDetails), directoryAttribute[0] as string, true);
				}
				catch (ArgumentException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			if (recipientTypeDetails == RecipientTypeDetails.MailboxPlan || recipientTypeDetails == RecipientTypeDetails.RoleGroup)
			{
				return FilterResult.Skip;
			}
			if (!entry.Attributes.ContainsKey("msExchExternalSyncState") && exSearchResultEntry.Attributes.ContainsKey("msExchExternalSyncState"))
			{
				entry.Attributes.Add("msExchExternalSyncState", exSearchResultEntry.Attributes["msExchExternalSyncState"]);
			}
			if (entry.Attributes.ContainsKey("msExchTransportRecipientSettingsFlags"))
			{
				if (!entry.Attributes.ContainsKey("proxyAddresses") && exSearchResultEntry.Attributes.ContainsKey("proxyAddresses"))
				{
					entry.Attributes.Add("proxyAddresses", exSearchResultEntry.Attributes["proxyAddresses"]);
				}
				if (!entry.Attributes.ContainsKey("msExchSignupAddresses") && exSearchResultEntry.Attributes.ContainsKey("msExchSignupAddresses"))
				{
					entry.Attributes.Add("msExchSignupAddresses", exSearchResultEntry.Attributes["msExchSignupAddresses"]);
				}
				if (!entry.Attributes.ContainsKey("msExchUMAddresses") && exSearchResultEntry.Attributes.ContainsKey("msExchUMAddresses"))
				{
					entry.Attributes.Add("msExchUMAddresses", exSearchResultEntry.Attributes["msExchUMAddresses"]);
				}
				if (!entry.Attributes.ContainsKey("msExchArchiveGUID") && exSearchResultEntry.Attributes.ContainsKey("msExchArchiveGUID"))
				{
					entry.Attributes.Add("msExchArchiveGUID", exSearchResultEntry.Attributes["msExchArchiveGUID"]);
				}
			}
			if (exSearchResultEntry.Attributes.ContainsKey("msExchTransportRecipientSettingsFlags"))
			{
				entry.Attributes["msExchTransportRecipientSettingsFlags"] = exSearchResultEntry.Attributes["msExchTransportRecipientSettingsFlags"];
			}
			if (exSearchResultEntry.Attributes.ContainsKey("msExchCU"))
			{
				entry.Attributes["msExchCU"] = exSearchResultEntry.Attributes["msExchCU"];
			}
			if (exSearchResultEntry.Attributes.ContainsKey("mailNickname"))
			{
				entry.Attributes["mailNickname"] = exSearchResultEntry.Attributes["mailNickname"];
			}
			if (exSearchResultEntry.Attributes.ContainsKey("msExchHomeServerName"))
			{
				entry.Attributes["msExchHomeServerName"] = exSearchResultEntry.Attributes["msExchHomeServerName"];
			}
			bool flag = false;
			int num;
			if (entry.Attributes.ContainsKey("msExchTransportRecipientSettingsFlags") && entry.Attributes["msExchTransportRecipientSettingsFlags"].Count != 0 && int.TryParse((string)entry.Attributes["msExchTransportRecipientSettingsFlags"][0], NumberStyles.Number, CultureInfo.InvariantCulture, out num) && (num & 8) != 0)
			{
				bool flag2 = (num & 64) != 0;
				foreach (string text in MserveSynchronizationProvider.AddressAttributeNames)
				{
					if (text.Equals("msExchArchiveGUID"))
					{
						if (!flag2)
						{
							entry.Attributes["msExchArchiveGUID"] = new DirectoryAttribute("msExchArchiveGUID", MserveSynchronizationProvider.EmptyList);
						}
					}
					else if (entry.Attributes.ContainsKey(text))
					{
						DirectoryAttribute directoryAttribute2 = entry.Attributes[text];
						entry.Attributes[text] = new DirectoryAttribute(directoryAttribute2.Name, MserveSynchronizationProvider.EmptyList);
						flag = text.Equals("proxyAddresses", StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			if (!flag)
			{
				MserveTargetConnection mserveTargetConnection = targetConnection as MserveTargetConnection;
				if (targetConnection == null)
				{
					throw new InvalidOperationException("targetConnection is not the type of MserveTargetConnection");
				}
				mserveTargetConnection.FilterSmtpProxyAddressesBasedOnTenantSetting(entry, recipientTypeDetails);
			}
			return FilterResult.None;
		}

		private const int LeaseLockTryCountInternal = 1;

		internal static readonly string[] EmptyList = new string[0];

		private static readonly string[] ReplicationAttributes = new string[]
		{
			"proxyAddresses",
			"msExchSignupAddresses",
			"msExchTransportRecipientSettingsFlags",
			"msExchUMAddresses",
			"msExchArchiveGUID"
		};

		private static readonly string[] RequiredAttributes = new string[]
		{
			"proxyAddresses",
			"msExchSignupAddresses",
			"msExchExternalSyncState",
			"msExchTransportRecipientSettingsFlags",
			"targetAddress",
			"msExchArchiveGUID",
			"msExchRecipientTypeDetails",
			"msExchCU",
			"mailNickname",
			"msExchHomeServerName"
		};

		private static readonly string[] AddressAttributeNames = new string[]
		{
			"proxyAddresses",
			"msExchSignupAddresses",
			"msExchUMAddresses",
			"msExchArchiveGUID"
		};

		private static readonly ITopologyConfigurationSession ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 91, "ConfigSession", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Mserve\\MserveSynchronizationProvider.cs");

		private static int partnerId = -1;

		private static string rootDomainLostAndFoundContainerDN;

		private List<TargetServerConfig> targetServerConfigs;

		private string identity;
	}
}
