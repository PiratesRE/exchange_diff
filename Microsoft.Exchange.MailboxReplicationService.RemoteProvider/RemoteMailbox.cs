using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class RemoteMailbox : RemoteObject, IMailbox, IDisposable
	{
		public RemoteMailbox(string serverName, string remoteOrgName, NetworkCredential remoteCred, ProxyControlFlags proxyControlFlags, IEnumerable<MRSProxyCapabilities> requiredCapabilities, bool useHttps, LocalMailboxFlags flags) : base(null, 0L)
		{
			this.serverName = serverName;
			this.remoteOrgName = remoteOrgName;
			this.remoteCred = remoteCred;
			this.proxyControlFlags = proxyControlFlags;
			this.requiredCapabilities = requiredCapabilities;
			this.useHttps = useHttps;
			this.flags = flags;
			TestIntegration.Instance.ForceRefresh();
			this.longOperationTimeout = ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("MRSProxyLongOperationTimeout");
			this.ExportBufferSizeKB = ConfigBase<MRSConfigSchema>.GetConfig<int>("ExportBufferSizeKB");
		}

		public int ExportBufferSizeKB { get; private set; }

		LatencyInfo IMailbox.GetLatencyInfo()
		{
			if (base.MrsProxyClient != null)
			{
				return base.MrsProxyClient.LatencyInfo;
			}
			return new LatencyInfo();
		}

		bool IMailbox.IsConnected()
		{
			return base.MrsProxy != null;
		}

		bool IMailbox.IsCapabilitySupported(MRSProxyCapabilities capability)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.IsCapabilitySupported({0})", new object[]
			{
				capability
			});
			this.VerifyMailboxConnection();
			return base.MrsProxyClient.ServerVersion[(int)capability];
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.IsMailboxCapabilitySupported({0})", new object[]
			{
				capability
			});
			bool result;
			if (this.mailboxCapabilities.TryGetValue(capability, out result))
			{
				return result;
			}
			this.VerifyMailboxConnection();
			if (capability == MailboxCapabilities.FolderRules && this is RemoteDestinationMailbox)
			{
				MailboxInformation mailboxInformation = ((IMailbox)this).GetMailboxInformation();
				if (mailboxInformation != null)
				{
					this.mailboxCapabilities[capability] = (mailboxInformation.ServerVersion >= Server.E14MinVersion);
					return this.mailboxCapabilities[capability];
				}
			}
			if (capability == MailboxCapabilities.FolderRules || capability == MailboxCapabilities.FolderAcls)
			{
				this.mailboxCapabilities[capability] = true;
				if (base.ServerVersion[60])
				{
					this.mailboxCapabilities[capability] = base.MrsProxy.IMailbox_IsMailboxCapabilitySupported2(base.Handle, (int)capability);
				}
				return this.mailboxCapabilities[capability];
			}
			if (base.ServerVersion[47])
			{
				this.mailboxCapabilities[capability] = base.MrsProxy.IMailbox_IsMailboxCapabilitySupported2(base.Handle, (int)capability);
				return this.mailboxCapabilities[capability];
			}
			if (base.ServerVersion[43] && (capability == MailboxCapabilities.PagedEnumerateChanges || capability == MailboxCapabilities.PagedGetActions || capability == MailboxCapabilities.ReplayActions))
			{
				this.mailboxCapabilities[capability] = base.MrsProxy.IMailbox_IsMailboxCapabilitySupported(base.Handle, capability);
				return this.mailboxCapabilities[capability];
			}
			this.mailboxCapabilities[capability] = false;
			return this.mailboxCapabilities[capability];
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			if (TestIntegration.Instance.RemoteExchangeGuidOverride != Guid.Empty)
			{
				bool flag = physicalMailboxGuid != primaryMailboxGuid;
				primaryMailboxGuid = TestIntegration.Instance.RemoteExchangeGuidOverride;
				physicalMailboxGuid = (flag ? TestIntegration.Instance.RemoteArchiveGuidOverride : TestIntegration.Instance.RemoteExchangeGuidOverride);
			}
			this.reservation = reservation;
			this.physicalMailboxGuid = physicalMailboxGuid;
			this.primaryMailboxGuid = primaryMailboxGuid;
			this.partitionHint = partitionHint;
			this.mdbGuid = mdbGuid;
			this.mbxType = mbxType;
			this.mailboxContainerGuid = mailboxContainerGuid;
		}

		void IMailbox.ConfigRestore(MailboxRestoreType restoreFlags)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.ConfigRestore(restoreFlags={0}", new object[]
			{
				restoreFlags
			});
			this.restoreType = restoreFlags;
		}

		void IMailbox.ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred)
		{
			if (!base.ServerVersion[24])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_ConfigADConnection");
			}
			base.MrsProxy.IMailbox_ConfigADConnection(base.Handle, domainControllerName, (cred != null) ? cred.UserName : null, (cred != null) ? cred.Domain : null, (cred != null) ? cred.Password : null);
		}

		void IMailbox.ConfigMDBByName(string mdbName)
		{
			this.mdbName = mdbName;
		}

		void IMailbox.ConfigMailboxOptions(MailboxOptions options)
		{
			if (base.ServerVersion[13])
			{
				base.MrsProxy.IMailbox_ConfigMailboxOptions(base.Handle, (int)options);
				return;
			}
			if (options != MailboxOptions.None)
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_ConfigMailboxOptions");
			}
		}

		void IMailbox.ConfigPreferredADConnection(string preferredDomainControllerName)
		{
			this.preferredDomainControllerName = preferredDomainControllerName;
		}

		void IMailbox.ConfigPst(string filePath, int? contentCodePage)
		{
			this.filePath = filePath;
			this.contentCodePage = contentCodePage;
		}

		void IMailbox.ConfigEas(NetworkCredential userCredential, SmtpAddress smtpAddress, Guid mailboxGuid, string remoteHostName)
		{
			this.primaryMailboxGuid = mailboxGuid;
			this.easConfiguration = new RemoteMailbox.EasConfiguration(userCredential, smtpAddress, remoteHostName);
		}

		void IMailbox.ConfigOlc(OlcMailboxConfiguration config)
		{
			this.olcConfig = config;
			this.serverName = this.olcConfig.RemoteHostName;
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetMailboxInformation", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_GetMailboxInformation(base.Handle);
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			if (base.MrsProxy == null)
			{
				MailboxReplicationProxyClient mailboxReplicationProxyClient = null;
				IMailboxReplicationProxyService iMRPS = null;
				long handle = 0L;
				string database = null;
				if (!this.flags.HasFlag(LocalMailboxFlags.PstExport) && (this.mbxType == MailboxType.DestMailboxCrossOrg || this.restoreType != MailboxRestoreType.None))
				{
					database = ((this.mdbGuid != Guid.Empty) ? this.mdbGuid.ToString() : this.mdbName);
				}
				try
				{
					if (this.proxyControlFlags.HasFlag(ProxyControlFlags.Olc))
					{
						mailboxReplicationProxyClient = MailboxReplicationProxyClient.CreateForOlcConnection(this.serverName, this.proxyControlFlags);
					}
					else
					{
						mailboxReplicationProxyClient = MailboxReplicationProxyClient.Create(this.serverName, this.remoteOrgName, this.remoteCred, this.physicalMailboxGuid, this.primaryMailboxGuid, this.filePath, database, this.partitionHint, this.useHttps, this.proxyControlFlags, this.longOperationTimeout);
					}
					if (this.requiredCapabilities != null)
					{
						foreach (MRSProxyCapabilities mrsproxyCapabilities in this.requiredCapabilities)
						{
							if (!mailboxReplicationProxyClient.ServerVersion[(int)mrsproxyCapabilities])
							{
								MrsTracer.ProxyClient.Error("Talking to downlevel server '{0}': no {1} support", new object[]
								{
									mailboxReplicationProxyClient.ServerVersion.ComputerName,
									mrsproxyCapabilities.ToString()
								});
								throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerVersion.ComputerName, mailboxReplicationProxyClient.ServerVersion.ToString(), mrsproxyCapabilities.ToString());
							}
						}
					}
					if (!mailboxReplicationProxyClient.ServerVersion[24])
					{
						this.flags &= RemoteMailbox.PreE15LocalMailboxFlags;
					}
					if ((this.flags & ~LocalMailboxFlags.StripLargeRulesForDownlevelTargets) != LocalMailboxFlags.None && !mailboxReplicationProxyClient.ServerVersion[24])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "TenantHint");
					}
					if (this.mailboxContainerGuid != null && !mailboxReplicationProxyClient.ServerVersion[46])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "ContainerOperations");
					}
					if ((connectFlags & MailboxConnectFlags.DoNotOpenMapiSession) != MailboxConnectFlags.None && !mailboxReplicationProxyClient.ServerVersion[11])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "PostMoveCleanup");
					}
					if (this.restoreType != MailboxRestoreType.None && !mailboxReplicationProxyClient.ServerVersion[34])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "IMailbox_ConfigRestore");
					}
					iMRPS = mailboxReplicationProxyClient;
					if (this.flags.HasFlag(LocalMailboxFlags.PstImport) && !mailboxReplicationProxyClient.ServerVersion[39])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "Pst");
					}
					if (this.flags.HasFlag(LocalMailboxFlags.PstExport) && !mailboxReplicationProxyClient.ServerVersion[57])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "RemotePstExport");
					}
					if (this.flags.HasFlag(LocalMailboxFlags.EasSync) && !mailboxReplicationProxyClient.ServerVersion[44])
					{
						throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "Eas");
					}
					if (mailboxReplicationProxyClient.ServerVersion[46])
					{
						handle = iMRPS.IMailbox_Config7((this.reservation != null) ? this.reservation.Id : Guid.Empty, this.primaryMailboxGuid, this.physicalMailboxGuid, (this.partitionHint != null) ? this.partitionHint.GetPersistablePartitionHint() : null, this.mdbGuid, this.mdbName, this.mbxType, (int)this.proxyControlFlags, (int)this.flags, this.mailboxContainerGuid);
					}
					else if (mailboxReplicationProxyClient.ServerVersion[41])
					{
						handle = iMRPS.IMailbox_Config5((this.reservation != null) ? this.reservation.Id : Guid.Empty, this.primaryMailboxGuid, this.physicalMailboxGuid, (this.partitionHint != null) ? this.partitionHint.GetPersistablePartitionHint() : null, this.mdbGuid, this.mdbName, this.mbxType, (int)this.proxyControlFlags, (int)this.flags);
					}
					else if (mailboxReplicationProxyClient.ServerVersion[39])
					{
						handle = iMRPS.IMailbox_Config6((this.reservation != null) ? this.reservation.Id : Guid.Empty, this.primaryMailboxGuid, this.physicalMailboxGuid, this.filePath, (this.partitionHint != null) ? this.partitionHint.GetPersistablePartitionHint() : null, this.mdbGuid, this.mdbName, this.mbxType, (int)this.proxyControlFlags, (int)this.flags);
					}
					else if (mailboxReplicationProxyClient.ServerVersion[37])
					{
						handle = iMRPS.IMailbox_Config5((this.reservation != null) ? this.reservation.Id : Guid.Empty, this.primaryMailboxGuid, this.physicalMailboxGuid, (this.partitionHint != null) ? this.partitionHint.GetPersistablePartitionHint() : null, this.mdbGuid, this.mdbName, this.mbxType, (int)this.proxyControlFlags, (int)this.flags);
					}
					else
					{
						RemoteReservation remoteReservation = this.reservation as RemoteReservation;
						if (remoteReservation != null)
						{
							remoteReservation.ConfirmLegacyReservation(mailboxReplicationProxyClient);
						}
						if (mailboxReplicationProxyClient.ServerVersion[24])
						{
							handle = iMRPS.IMailbox_Config4(this.primaryMailboxGuid, this.physicalMailboxGuid, (this.partitionHint != null) ? this.partitionHint.GetPersistablePartitionHint() : null, this.mdbGuid, this.mdbName, this.mbxType, (int)this.proxyControlFlags, (int)this.flags);
						}
						else
						{
							ProxyControlFlags proxyControlFlags = this.proxyControlFlags;
							if ((this.flags & LocalMailboxFlags.StripLargeRulesForDownlevelTargets) != LocalMailboxFlags.None)
							{
								proxyControlFlags |= ProxyControlFlags.StripLargeRulesForDownlevelTargets;
							}
							handle = iMRPS.IMailbox_Config3(this.primaryMailboxGuid, this.physicalMailboxGuid, this.mdbGuid, this.mdbName, this.mbxType, (int)proxyControlFlags);
						}
					}
					if (!string.IsNullOrEmpty(this.preferredDomainControllerName))
					{
						if (mailboxReplicationProxyClient.ServerVersion[48])
						{
							iMRPS.IMailbox_ConfigPreferredADConnection(handle, this.preferredDomainControllerName);
						}
						else
						{
							MrsTracer.ProxyClient.Warning("IMailbox_ConfigPreferredADConnection not expected to be called for server:{0} version:{1}", new object[]
							{
								mailboxReplicationProxyClient.ServerName,
								mailboxReplicationProxyClient.ServerVersion.ToString()
							});
						}
					}
					if ((this.flags.HasFlag(LocalMailboxFlags.PstImport) || this.flags.HasFlag(LocalMailboxFlags.PstExport)) && mailboxReplicationProxyClient.ServerVersion[41])
					{
						iMRPS.IMailbox_ConfigPst(handle, this.filePath, this.contentCodePage);
					}
					if (this.flags.HasFlag(LocalMailboxFlags.EasSync))
					{
						if (mailboxReplicationProxyClient.ServerVersion[53])
						{
							iMRPS.IMailbox_ConfigEas2(handle, this.easConfiguration.UserCred.Password, this.easConfiguration.SmtpAddress, this.primaryMailboxGuid, this.easConfiguration.RemoteHostName);
						}
						else
						{
							iMRPS.IMailbox_ConfigEas(handle, this.easConfiguration.UserCred.Password, this.easConfiguration.SmtpAddress);
						}
					}
					if (this.proxyControlFlags.HasFlag(ProxyControlFlags.Olc))
					{
						if (!mailboxReplicationProxyClient.ServerVersion[55])
						{
							throw new UnsupportedRemoteServerVersionWithOperationPermanentException(mailboxReplicationProxyClient.ServerName, mailboxReplicationProxyClient.ServerVersion.ToString(), "IMailbox_ConfigOlc");
						}
						iMRPS.IMailbox_ConfigOlc(handle, this.olcConfig);
					}
					if (mailboxReplicationProxyClient.ServerVersion[42])
					{
						iMRPS.IMailbox_ConfigureProxyService(new ProxyConfiguration());
					}
					if (this.restoreType != MailboxRestoreType.None)
					{
						iMRPS.IMailbox_ConfigRestore(handle, (int)this.restoreType);
					}
					if (mailboxReplicationProxyClient.ServerVersion[11])
					{
						iMRPS.IMailbox_Connect2(handle, (int)connectFlags);
					}
					else
					{
						iMRPS.IMailbox_Connect(handle);
					}
					base.MrsProxy = mailboxReplicationProxyClient;
					base.Handle = handle;
					mailboxReplicationProxyClient = null;
					handle = 0L;
				}
				finally
				{
					if (handle != 0L)
					{
						CommonUtils.CatchKnownExceptions(delegate
						{
							iMRPS.CloseHandle(handle);
						}, null);
					}
					if (mailboxReplicationProxyClient != null)
					{
						mailboxReplicationProxyClient.Dispose();
					}
				}
			}
		}

		void IMailbox.Disconnect()
		{
			if (!((IMailbox)this).IsConnected())
			{
				return;
			}
			CommonUtils.CatchKnownExceptions(delegate
			{
				base.MrsProxy.IMailbox_Disconnect(base.Handle);
			}, null);
			base.MrsProxyClient.Dispose();
			base.Handle = 0L;
			base.MrsProxy = null;
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetMailboxServerInformation", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_GetMailboxServerInformation(base.Handle);
		}

		VersionInformation IMailbox.GetVersion()
		{
			return base.ServerVersion;
		}

		void IMailbox.SetOtherSideVersion(VersionInformation otherSideVersion)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetMailboxServerInformation", new object[0]);
			this.VerifyMailboxConnection();
			if (!base.ServerVersion[56])
			{
				return;
			}
			base.MrsProxy.IMailbox_SetOtherSideVersion(base.Handle, otherSideVersion);
		}

		void IMailbox.DeleteMailbox(int flags)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.DeleteMailbox({0})", new object[]
			{
				flags
			});
			this.VerifyMailboxConnection();
			base.MrsProxy.IMailbox_DeleteMailbox(base.Handle, flags);
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			this.VerifyMailboxConnection();
			bool flag;
			List<FolderRec> list = base.MrsProxy.IMailbox_EnumerateFolderHierarchyPaged2(base.Handle, flags, DataConverter<PropTagConverter, PropTag, int>.GetData(additionalPtagsToLoad), out flag);
			while (flag)
			{
				List<FolderRec> collection = base.MrsProxy.IMailbox_EnumerateFolderHierarchyNextBatch(base.Handle, out flag);
				list.AddRange(collection);
			}
			return list;
		}

		List<WellKnownFolder> IMailbox.DiscoverWellKnownFolders(int flags)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.DiscoverWellKnownFolders", new object[0]);
			this.VerifyMailboxConnection();
			if (!base.ServerVersion[35])
			{
				return FolderHierarchyUtils.DiscoverWellKnownFolders(this, flags);
			}
			return base.MrsProxy.IMailbox_DiscoverWellKnownFolders(base.Handle, flags);
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npa)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetIDsFromNames", new object[0]);
			this.VerifyMailboxConnection();
			int[] array = base.MrsProxy.IMailbox_GetIDsFromNames(base.Handle, createIfNotExists, npa);
			PropTag[] array2 = new PropTag[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (PropTag)array[i];
			}
			return array2;
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetSessionSpecificEntryId", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_GetSessionSpecificEntryId(base.Handle, entryId);
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetNamesFromIDs", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_GetNamesFromIDs(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(pta));
		}

		MappedPrincipal[] IMailbox.ResolvePrincipals(MappedPrincipal[] principals)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.ResolvePrincipals", new object[0]);
			this.VerifyMailboxConnection();
			if (!base.ServerVersion[8])
			{
				Guid[] array = new Guid[principals.Length];
				int i = 0;
				foreach (MappedPrincipal mappedPrincipal in principals)
				{
					if (!mappedPrincipal.HasField(MappedPrincipalFields.MailboxGuid))
					{
						break;
					}
					array[i++] = mappedPrincipal.MailboxGuid;
				}
				if (i < principals.Length)
				{
					array = base.MrsProxy.IMailbox_GetMailboxGuidsFromPrincipals(base.Handle, principals);
				}
				List<Guid> list = new List<Guid>();
				foreach (Guid guid in array)
				{
					if (guid != Guid.Empty)
					{
						list.Add(guid);
					}
				}
				MappedPrincipal[] array3 = base.MrsProxy.IMailbox_GetPrincipalsFromMailboxGuids(base.Handle, list.ToArray());
				Dictionary<Guid, MappedPrincipal> dictionary = new Dictionary<Guid, MappedPrincipal>();
				for (i = 0; i < array3.Length; i++)
				{
					if (array3[i] != null)
					{
						dictionary[list[i]] = array3[i];
					}
				}
				MappedPrincipal[] array4 = new MappedPrincipal[array.Length];
				for (i = 0; i < array.Length; i++)
				{
					array4[i] = null;
					MappedPrincipal mappedPrincipal2;
					if (array[i] != Guid.Empty && dictionary.TryGetValue(array[i], out mappedPrincipal2))
					{
						array4[i] = mappedPrincipal2;
					}
				}
				return array4;
			}
			return base.MrsProxy.IMailbox_ResolvePrincipals(base.Handle, principals);
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.SetInTransitStatus({0})", new object[]
			{
				status
			});
			this.VerifyMailboxConnection();
			base.MrsProxy.IMailbox_SetInTransitStatus(base.Handle, (int)status, out onlineMoveSupported);
		}

		void IMailbox.SeedMBICache()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.SeedMBICache", new object[0]);
			this.VerifyMailboxConnection();
			if (!base.ServerVersion[11])
			{
				return;
			}
			base.MrsProxy.IMailbox_SeedMBICache(base.Handle);
		}

		bool IMailbox.UpdateRemoteHostName(string value)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.UpdateRemoteHostName", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_UpdateRemoteHostName(base.Handle, value);
		}

		ADUser IMailbox.GetADUser()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetADUser", new object[0]);
			this.VerifyMailboxConnection();
			string xml = base.MrsProxy.IMailbox_GetADUser(base.Handle);
			return ConfigurableObjectXML.Deserialize<ADUser>(xml);
		}

		void IMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId)
		{
			entries = null;
			MrsTracer.ProxyClient.Function("RemoteMailbox.UpdateMovedMailbox", new object[0]);
			this.VerifyMailboxConnection();
			string remoteRecipientData2 = ConfigurableObjectXML.Serialize<ADUser>(remoteRecipientData);
			string text = null;
			if (base.ServerVersion[46])
			{
				byte[] newUnifiedMailboxIdData = (newUnifiedMailboxId == null) ? null : newUnifiedMailboxId.GetBytes();
				base.MrsProxy.IMailbox_UpdateMovedMailbox4(base.Handle, op, remoteRecipientData2, domainController, out text, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, (int)archiveStatus, (int)updateMovedMailboxFlags, newMailboxContainerGuid, newUnifiedMailboxIdData);
			}
			else if (base.ServerVersion[36])
			{
				base.MrsProxy.IMailbox_UpdateMovedMailbox3(base.Handle, op, remoteRecipientData2, domainController, out text, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, (int)archiveStatus, (int)updateMovedMailboxFlags);
			}
			else if (base.ServerVersion[9])
			{
				base.MrsProxy.IMailbox_UpdateMovedMailbox2(base.Handle, op, remoteRecipientData2, domainController, out text, newDatabaseGuid, newArchiveDatabaseGuid, archiveDomain, (int)archiveStatus);
			}
			else
			{
				base.MrsProxy.IMailbox_UpdateMovedMailbox(base.Handle, op, remoteRecipientData2, domainController, out text);
			}
			if (text != null)
			{
				List<ReportEntry> list = XMLSerializableBase.Deserialize<List<ReportEntry>>(text, false);
				entries = ((list != null) ? list.ToArray() : null);
			}
		}

		RawSecurityDescriptor IMailbox.GetMailboxSecurityDescriptor()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetMailboxSecurityDescriptor", new object[0]);
			this.VerifyMailboxConnection();
			byte[] array = base.MrsProxy.IMailbox_GetMailboxSecurityDescriptor(base.Handle);
			if (array == null)
			{
				return null;
			}
			return new RawSecurityDescriptor(array, 0);
		}

		RawSecurityDescriptor IMailbox.GetUserSecurityDescriptor()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetUserSecurityDescriptor", new object[0]);
			this.VerifyMailboxConnection();
			byte[] array = base.MrsProxy.IMailbox_GetUserSecurityDescriptor(base.Handle);
			if (array == null)
			{
				return null;
			}
			return new RawSecurityDescriptor(array, 0);
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.AddMoveHistoryEntry", new object[0]);
			this.VerifyMailboxConnection();
			string mheData = mhei.Serialize(false);
			base.MrsProxy.IMailbox_AddMoveHistoryEntry(base.Handle, mheData, maxMoveHistoryLength);
		}

		ServerHealthStatus IMailbox.CheckServerHealth()
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.CheckServerHealth", new object[0]);
			this.VerifyMailboxConnection();
			if (base.ServerVersion[12])
			{
				return base.MrsProxy.IMailbox_CheckServerHealth2(base.Handle);
			}
			ServerHealthStatus serverHealthStatus = new ServerHealthStatus(ServerHealthState.Healthy);
			try
			{
				base.MrsProxy.IMailbox_CheckServerHealth(base.Handle);
			}
			catch (MailboxReplicationTransientException)
			{
				serverHealthStatus.HealthState = ServerHealthState.NotHealthy;
			}
			return serverHealthStatus;
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetProps", new object[0]);
			this.VerifyMailboxConnection();
			if (base.ServerVersion[8])
			{
				return base.MrsProxy.IMailbox_GetProps(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(ptags));
			}
			if (this is RemoteSourceMailbox)
			{
				return base.MrsProxy.ISourceMailbox_GetProps(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(ptags));
			}
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_GetProps");
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetReceiveFolderEntryId", new object[0]);
			this.VerifyMailboxConnection();
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_GetReceiveFolderEntryId");
			}
			return base.MrsProxy.IMailbox_GetReceiveFolderEntryId(base.Handle, msgClass);
		}

		Guid[] IMailbox.ResolvePolicyTag(string policyTagStr)
		{
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_ResolvePolicyTag");
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.LoadSyncState", new object[0]);
			if (!(this is RemoteDestinationMailbox))
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_LoadSyncState");
			}
			this.VerifyMailboxConnection();
			DataExportBatch dataExportBatch = base.MrsProxy.IDestinationMailbox_LoadSyncState2(base.Handle, key);
			string syncState = null;
			using (PagedReceiver pagedReceiver = new PagedReceiver(delegate(string data)
			{
				syncState = data;
			}, base.MrsProxyClient.UseCompression))
			{
				RemoteDataExport.ExportRoutine(base.MrsProxy, dataExportBatch.DataExportHandle, pagedReceiver, dataExportBatch, base.MrsProxyClient.UseCompression);
			}
			return syncState;
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncState)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.SaveSyncState", new object[0]);
			if (!(this is RemoteDestinationMailbox))
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IMailbox_SaveSyncState");
			}
			this.VerifyMailboxConnection();
			IDataExport dataExport = new PagedTransmitter(syncState, base.MrsProxyClient.UseCompression);
			DataExportBatch dataExportBatch = dataExport.ExportData();
			long handle = base.MrsProxy.IDestinationMailbox_SaveSyncState2(base.Handle, key, dataExportBatch);
			if (!dataExportBatch.IsLastBatch)
			{
				using (IDataImport dataImport = new RemoteDataImport(base.MrsProxy, handle, null))
				{
					do
					{
						dataExportBatch = dataExport.ExportData();
						IDataMessage message = DataMessageSerializer.Deserialize(dataExportBatch.Opcode, dataExportBatch.Data, base.MrsProxyClient.UseCompression);
						dataImport.SendMessage(message);
					}
					while (!dataExportBatch.IsLastBatch);
				}
			}
			return null;
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			MrsTracer.ProxyClient.Function("RemoteMailbox.GetSessionStatistics()", new object[0]);
			if (!base.ServerVersion[31])
			{
				return new SessionStatistics();
			}
			return base.MrsProxy.IMailbox_GetSessionStatistics(base.Handle, (int)statisticsTypes);
		}

		Guid IMailbox.StartIsInteg(List<uint> mailboxCorruptionTypes)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.StartIsInteg()", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_StartIsInteg(base.Handle, mailboxCorruptionTypes);
		}

		List<StoreIntegrityCheckJob> IMailbox.QueryIsInteg(Guid isIntegRequestGuid)
		{
			MrsTracer.ProxyClient.Function("RemoteSourceMailbox.QueryIsInteg()", new object[0]);
			this.VerifyMailboxConnection();
			return base.MrsProxy.IMailbox_QueryIsInteg(base.Handle, isIntegRequestGuid);
		}

		protected void VerifyMailboxConnection()
		{
			if (!((IMailbox)this).IsConnected())
			{
				throw new NotConnectedPermanentException();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				((IMailbox)this).Disconnect();
			}
		}

		private static readonly TimeSpan DelaySamplingIntervalForMRSProxy = TimeSpan.FromSeconds(60.0);

		private static readonly LocalMailboxFlags PreE15LocalMailboxFlags = LocalMailboxFlags.StripLargeRulesForDownlevelTargets | LocalMailboxFlags.UseHomeMDB | LocalMailboxFlags.PureMAPI | LocalMailboxFlags.CredentialIsNotAdmin | LocalMailboxFlags.UseNTLMAuth | LocalMailboxFlags.ConnectToMoMT | LocalMailboxFlags.LegacyPublicFolders | LocalMailboxFlags.Restore;

		private readonly NetworkCredential remoteCred;

		private readonly string remoteOrgName;

		private readonly ProxyControlFlags proxyControlFlags;

		private readonly bool useHttps;

		private readonly TimeSpan longOperationTimeout;

		private Dictionary<MailboxCapabilities, bool> mailboxCapabilities = new Dictionary<MailboxCapabilities, bool>();

		private string serverName;

		private string preferredDomainControllerName;

		private string filePath;

		private IReservation reservation;

		private Guid? mailboxContainerGuid;

		private Guid primaryMailboxGuid;

		private Guid physicalMailboxGuid;

		private TenantPartitionHint partitionHint;

		private Guid mdbGuid;

		private string mdbName;

		private MailboxType mbxType;

		private LocalMailboxFlags flags;

		private MailboxRestoreType restoreType;

		private IEnumerable<MRSProxyCapabilities> requiredCapabilities;

		private int? contentCodePage;

		private RemoteMailbox.EasConfiguration easConfiguration;

		private OlcMailboxConfiguration olcConfig;

		private class EasConfiguration
		{
			public EasConfiguration(NetworkCredential userCredential, SmtpAddress smtpAddress, string remoteHostName)
			{
				this.UserCred = userCredential;
				this.SmtpAddress = smtpAddress.ToString();
				this.RemoteHostName = remoteHostName;
			}

			public NetworkCredential UserCred { get; private set; }

			public string SmtpAddress { get; private set; }

			public string RemoteHostName { get; private set; }
		}
	}
}
