using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.PST.Common;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class PstMailbox : MailboxProviderBase, IMailbox, IDisposable
	{
		public PstMailbox() : base(LocalMailboxFlags.None)
		{
		}

		public IPST IPst
		{
			get
			{
				return this.iPst;
			}
		}

		public PSTPropertyBag MessageStorePropertyBag
		{
			get
			{
				if (this.messageStorePropertyBag == null)
				{
					this.messageStorePropertyBag = new PSTPropertyBag(this, this.iPst.MessageStore.PropertyBag);
				}
				return this.messageStorePropertyBag;
			}
		}

		public Encoding CachedEncoding
		{
			get
			{
				if (this.cachedEncoding == null)
				{
					this.cachedEncoding = (this.ContentEncoding ?? this.TryGetEncodingFromLanguage());
					this.encodingFound = new bool?(this.cachedEncoding != null);
				}
				return this.cachedEncoding;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				return this.contentEncoding;
			}
		}

		public override int ServerVersion
		{
			get
			{
				throw new NotImplementedException();
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		public static PropertyTag MoMTPtagFromPtag(PropTag ptag)
		{
			return new PropertyTag((uint)ptag);
		}

		public static PropertyTag[] MoMTPtaFromPta(PropTag[] pta)
		{
			if (pta == null)
			{
				return null;
			}
			PropertyTag[] array = new PropertyTag[pta.Length];
			for (int i = 0; i < pta.Length; i++)
			{
				array[i] = PstMailbox.MoMTPtagFromPtag(pta[i]);
			}
			return array;
		}

		public static PropValue[] PvaFromMoMTPva(PropertyValue[] momtPva)
		{
			PropValue[] array = new PropValue[momtPva.Length];
			for (int i = 0; i < momtPva.Length; i++)
			{
				array[i] = PstMailbox.PvFromMoMTPv(momtPva[i]);
			}
			return array;
		}

		public static PropValue PvFromMoMTPv(PropertyValue momtPv)
		{
			object value;
			if (momtPv.PropertyTag.PropertyType == PropertyType.SysTime)
			{
				value = (DateTime)((ExDateTime)momtPv.Value);
			}
			else
			{
				value = momtPv.Value;
			}
			return new PropValue((PropTag)momtPv.PropertyTag, value);
		}

		public static PropertyValue MoMTPvFromPv(PropValue pv)
		{
			PropertyValue result;
			if (pv.PropType == PropType.SysTime)
			{
				result = new PropertyValue(new PropertyTag((uint)pv.PropTag), (ExDateTime)pv.GetDateTime());
			}
			else
			{
				result = new PropertyValue(new PropertyTag((uint)pv.PropTag), pv.Value);
			}
			return result;
		}

		public static NamedPropData[] NpdaFromMoMTNpva(NamedProperty[] momtNpva)
		{
			NamedPropData[] array = new NamedPropData[momtNpva.Length];
			for (int i = 0; i < momtNpva.Length; i++)
			{
				array[i] = new NamedPropData();
				array[i].Guid = momtNpva[i].Guid;
				if (momtNpva[i].Kind == NamedPropertyKind.Id)
				{
					array[i].Id = (int)momtNpva[i].Id;
				}
				else
				{
					array[i].Name = momtNpva[i].Name;
				}
			}
			return array;
		}

		public static byte[] CreateEntryIdFromNodeId(Guid guid, uint nodeId)
		{
			byte[] array = new byte[24];
			Array.Copy(guid.ToByteArray(), 0, array, 4, 16);
			Array.Copy(BitConverter.GetBytes(nodeId), 0, array, 20, 4);
			return array;
		}

		public static uint GetNodeIdFromEntryId(Guid guid, byte[] entryId)
		{
			return PstMailbox.GetNodeIdFromEntryId(guid, entryId, false);
		}

		public override SyncProtocol GetSyncProtocol()
		{
			return SyncProtocol.None;
		}

		void IMailbox.ConfigPst(string filePath, int? contentCodePage)
		{
			this.filePath = filePath;
			this.contentCodePage = contentCodePage;
		}

		List<WellKnownFolder> IMailbox.DiscoverWellKnownFolders(int flags)
		{
			return FolderHierarchyUtils.DiscoverWellKnownFolders(this, flags);
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.Config", new object[0]);
		}

		void IMailbox.ConfigRestore(MailboxRestoreType restoreFlags)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigMDBByName(string mdbName)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigMailboxOptions(MailboxOptions options)
		{
		}

		bool IMailbox.IsConnected()
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.IsConnected returns '{0}'", new object[]
			{
				this.iPst != null
			});
			return this.iPst != null;
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			return false;
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.GetMailboxInformation", new object[0]);
			return PstMailbox.MailboxInformation;
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.Connect", new object[0]);
			this.ValidateCodePageAndSetEncoding();
			IPST ipst = null;
			try
			{
				ipst = new PSTSession(new Tracer.TraceMethod(MrsTracer.Provider.Debug), new Tracer.TraceMethod(MrsTracer.Provider.Warning), new Tracer.TraceMethod(MrsTracer.Provider.Error));
				ipst.Open(this.filePath, this is PstDestinationMailbox, this is PstDestinationMailbox, (connectFlags & MailboxConnectFlags.ValidateOnly) != MailboxConnectFlags.None);
			}
			catch (PSTExceptionBase pstexceptionBase)
			{
				throw new UnableToOpenPSTPermanentException(this.filePath, pstexceptionBase.Message, pstexceptionBase);
			}
			catch (UnauthorizedAccessException ex)
			{
				if (Directory.Exists(this.filePath))
				{
					throw new PSTPathMustBeAFilePermanentException(ex);
				}
				throw new UnableToOpenPSTPermanentException(this.filePath, ex.Message, ex);
			}
			catch (IOException ex2)
			{
				if (ex2 is PathTooLongException || ex2 is DirectoryNotFoundException || ex2 is FileNotFoundException)
				{
					throw new UnableToOpenPSTPermanentException(this.filePath, ex2.Message, ex2);
				}
				throw new PSTIOExceptionTransientException(ex2);
			}
			catch (SecurityException ex3)
			{
				throw new UnableToOpenPSTPermanentException(this.filePath, ex3.Message, ex3);
			}
			this.iPst = ipst;
			MrsTracer.Provider.Debug("Successfully opened PST file '{0}'", new object[]
			{
				this.filePath
			});
		}

		void IMailbox.Disconnect()
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.Disconnect", new object[0]);
			if (this.iPst == null)
			{
				return;
			}
			try
			{
				this.iPst.Close();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new UnableToClosePSTPermanentException(this.filePath, innerException);
			}
			finally
			{
				this.iPst = null;
				this.messageStorePropertyBag = null;
			}
			MrsTracer.Provider.Debug("Successfully closed PST file '{0}'", new object[]
			{
				this.filePath
			});
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.GetMailboxServerInformation", new object[0]);
			return null;
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			throw new NotImplementedException();
		}

		void IMailbox.SeedMBICache()
		{
			throw new NotImplementedException();
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.EnumerateFolderHierarchy({0})", new object[]
			{
				flags
			});
			List<FolderRec> list = new List<FolderRec>(50);
			List<PropTag> list2 = new List<PropTag>(FolderRec.PtagsToLoad);
			if (additionalPtagsToLoad != null)
			{
				list2.AddRange(additionalPtagsToLoad);
			}
			PropertyTag[] array = PstMailbox.MoMTPtaFromPta(list2.ToArray());
			try
			{
				IFolder folder = this.iPst.ReadFolder(290U);
				if (folder == null)
				{
					throw new UnableToGetPSTHierarchyPermanentException(this.filePath);
				}
				using (PstFxFolder pstFxFolder = new PstFxFolder(this, folder))
				{
					list.Add(FolderRec.Create(PstMailbox.PvaFromMoMTPva(pstFxFolder.GetProps(array))));
					PropertyValue property = this.MessageStorePropertyBag.GetProperty(PropertyTag.IPMSubtreeFolder);
					if (property.IsError || ((byte[])property.Value).Length != 24)
					{
						throw new UnableToGetPSTHierarchyPermanentException(this.filePath);
					}
					uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.iPst.MessageStore.Guid, (byte[])property.Value, true);
					IFolder folder2 = this.iPst.ReadFolder(nodeIdFromEntryId);
					if (folder2 == null)
					{
						throw new UnableToGetPSTHierarchyPermanentException(this.filePath);
					}
					this.GetHierarchy(folder2, list, array);
				}
			}
			catch (PSTIOException innerException)
			{
				throw new UnableToGetPSTHierarchyTransientException(this.filePath, innerException);
			}
			catch (PSTExceptionBase innerException2)
			{
				throw new UnableToGetPSTHierarchyPermanentException(this.filePath, innerException2);
			}
			MrsTracer.Provider.Debug("PST hierarchy contains {0} folders including root", new object[]
			{
				list.Count
			});
			return list;
		}

		void IMailbox.DeleteMailbox(int flags)
		{
			throw new NotImplementedException();
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			throw new NotImplementedException();
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npda)
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.GetIDsFromNames", new object[0]);
			PropTag[] array = new PropTag[npda.Length];
			for (int i = 0; i < npda.Length; i++)
			{
				ushort num;
				try
				{
					if (npda[i].Kind == 0)
					{
						num = this.iPst.ReadIdFromNamedProp(null, (uint)npda[i].Id, npda[i].Guid, createIfNotExists);
					}
					else
					{
						num = this.iPst.ReadIdFromNamedProp(npda[i].Name, 0U, npda[i].Guid, createIfNotExists);
					}
				}
				catch (PSTExceptionBase innerException)
				{
					throw new MailboxReplicationPermanentException(new LocalizedString(this.IPst.FileName), innerException);
				}
				array[i] = ((num != 0) ? PropTagHelper.PropTagFromIdAndType((int)num, PropType.Unspecified) : PropTag.Unresolved);
			}
			return array;
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			throw new NotImplementedException();
		}

		bool IMailbox.UpdateRemoteHostName(string value)
		{
			throw new NotImplementedException();
		}

		ADUser IMailbox.GetADUser()
		{
			throw new NotImplementedException();
		}

		void IMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid targetDatabaseGuid, Guid targetArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId)
		{
			throw new NotImplementedException();
		}

		MappedPrincipal[] IMailbox.ResolvePrincipals(MappedPrincipal[] principals)
		{
			throw new NotImplementedException();
		}

		Guid[] IMailbox.ResolvePolicyTag(string policyTagStr)
		{
			throw new NotImplementedException();
		}

		RawSecurityDescriptor IMailbox.GetMailboxSecurityDescriptor()
		{
			throw new NotImplementedException();
		}

		RawSecurityDescriptor IMailbox.GetUserSecurityDescriptor()
		{
			throw new NotImplementedException();
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			throw new NotImplementedException();
		}

		ServerHealthStatus IMailbox.CheckServerHealth()
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.CheckServerHealth", new object[0]);
			return new ServerHealthStatus(ServerHealthState.Healthy);
		}

		PropValueData[] IMailbox.GetProps(PropTag[] pta)
		{
			MrsTracer.Provider.Function("PstMailbox.IMailbox.GetProps", new object[0]);
			PropValue[] array = new PropValue[pta.Length];
			for (int i = 0; i < pta.Length; i++)
			{
				try
				{
					array[i] = PstMailbox.PvFromMoMTPv(this.MessageStorePropertyBag.GetProperty(PstMailbox.MoMTPtagFromPtag(pta[i])));
				}
				catch (PSTIOException innerException)
				{
					throw new UnableToGetPSTPropsTransientException(this.filePath, innerException);
				}
				catch (PSTExceptionBase innerException2)
				{
					throw new UnableToGetPSTPropsPermanentException(this.filePath, innerException2);
				}
			}
			return DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(array);
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			MrsTracer.Provider.Function("PstMailbox.GetReceiveFolderEntryId('{0}')", new object[]
			{
				msgClass
			});
			if (this.receiveFoldersTable == null)
			{
				try
				{
					this.receiveFoldersTable = this.iPst.ReadReceiveFoldersTable();
				}
				catch (PSTIOException innerException)
				{
					throw new UnableToGetPSTReceiveFolderTransientException(this.filePath, msgClass, innerException);
				}
				catch (PSTExceptionBase innerException2)
				{
					throw new UnableToGetPSTReceiveFolderPermanentException(this.filePath, msgClass, innerException2);
				}
			}
			uint num = 0U;
			this.receiveFoldersTable.TryGetValue(msgClass, out num);
			MrsTracer.Provider.Debug("receive folder for messageClass '{0}' {1}found", new object[]
			{
				msgClass,
				(num == 0U) ? "NOT " : string.Empty
			});
			if (num != 0U)
			{
				return PstMailbox.CreateEntryIdFromNodeId(this.iPst.MessageStore.Guid, num);
			}
			return null;
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			throw new NotImplementedException();
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncStateStr)
		{
			throw new NotImplementedException();
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			return new SessionStatistics();
		}

		Guid IMailbox.StartIsInteg(List<uint> mailboxCorruptionTypes)
		{
			throw new NotImplementedException();
		}

		List<StoreIntegrityCheckJob> IMailbox.QueryIsInteg(Guid isIntegRequestGuid)
		{
			throw new NotImplementedException();
		}

		public T GetFolder<T>(byte[] folderId) where T : PstFolder, new()
		{
			MrsTracer.Provider.Function("PstMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(folderId)
			});
			uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.iPst.MessageStore.Guid, folderId);
			IFolder folder;
			try
			{
				folder = this.iPst.ReadFolder(nodeIdFromEntryId);
			}
			catch (PSTIOException innerException)
			{
				throw new UnableToReadPSTFolderTransientException(nodeIdFromEntryId, innerException);
			}
			catch (PSTExceptionBase innerException2)
			{
				throw new UnableToReadPSTFolderPermanentException(nodeIdFromEntryId, innerException2);
			}
			if (folder == null)
			{
				MrsTracer.Provider.Debug("Folder does not exist", new object[0]);
				return default(T);
			}
			PstFxFolder pstFxFolder = new PstFxFolder(this, folder);
			if (MrsTracer.Provider.IsEnabled(TraceType.DebugTrace))
			{
				MrsTracer.Provider.Debug("Opened folder '{0}'", new object[]
				{
					(string)pstFxFolder.GetProp(PropertyTag.DisplayName).Value
				});
			}
			T result = Activator.CreateInstance<T>();
			result.Config(folderId, pstFxFolder);
			return result;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
			if (calledFromDispose)
			{
				((IMailbox)this).Disconnect();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PstMailbox>(this);
		}

		private static uint GetNodeIdFromEntryId(Guid guid, byte[] entryId, bool ignoreGuidMismatch)
		{
			if (entryId == null || entryId.Length != 24)
			{
				throw new ArgumentException("EntryId is null or wrong length", "entryId");
			}
			if (entryId[0] != 0 || entryId[1] != 0 || entryId[2] != 0 || entryId[3] != 0)
			{
				throw new ArgumentException("EntryId prefix is incorrect", "entryId");
			}
			if (!ignoreGuidMismatch)
			{
				byte[] array = new byte[16];
				Array.Copy(entryId, 4, array, 0, 16);
				if (!ArrayComparer<byte>.EqualityComparer.Equals(array, guid.ToByteArray()))
				{
					throw new ArgumentException("EntryId embedded guid is incorrect", "entryId");
				}
			}
			return BitConverter.ToUInt32(entryId, 20);
		}

		private void GetHierarchy(IFolder iFolder, List<FolderRec> hierarchy, PropertyTag[] momtPtagsToLoad)
		{
			using (PstFxFolder pstFxFolder = new PstFxFolder(this, iFolder))
			{
				hierarchy.Add(FolderRec.Create(PstMailbox.PvaFromMoMTPva(pstFxFolder.GetProps(momtPtagsToLoad))));
				foreach (uint num in iFolder.SubFolderIds)
				{
					IFolder folder;
					try
					{
						folder = this.iPst.ReadFolder(num);
					}
					catch (PSTIOException innerException)
					{
						throw new UnableToReadPSTFolderTransientException(num, innerException);
					}
					catch (PSTExceptionBase innerException2)
					{
						throw new UnableToReadPSTFolderPermanentException(num, innerException2);
					}
					if (folder == null)
					{
						MrsTracer.Provider.Error("Pst folder 0x{0:x} does not exist", new object[]
						{
							num
						});
					}
					else
					{
						this.GetHierarchy(folder, hierarchy, momtPtagsToLoad);
					}
				}
			}
		}

		private Encoding TryGetEncodingFromLanguage()
		{
			if (this.encodingFound != null)
			{
				return this.cachedEncoding;
			}
			byte[] array;
			bool flag;
			try
			{
				PSTPropertyStream pstpropertyStream = this.MessageStorePropertyBag.GetPropertyStream(PstMailbox.Language) as PSTPropertyStream;
				if (pstpropertyStream == null)
				{
					return null;
				}
				array = new byte[pstpropertyStream.Length];
				pstpropertyStream.Read(array, 0, array.Length);
				flag = (pstpropertyStream.PropTag.PropertyType == PropertyType.Unicode);
				pstpropertyStream.Close();
			}
			catch (PSTIOException innerException)
			{
				throw new UnableToGetPSTPropsTransientException(this.IPst.FileName, innerException);
			}
			catch (PSTExceptionBase innerException2)
			{
				throw new UnableToGetPSTPropsPermanentException(this.IPst.FileName, innerException2);
			}
			string @string;
			if (flag)
			{
				@string = Encoding.Unicode.GetString(array);
			}
			else
			{
				@string = Encoding.ASCII.GetString(array);
			}
			int culture;
			if (!int.TryParse(@string, out culture))
			{
				MrsTracer.Provider.Warning("PstMailbox.IMailbox: cannot resolve culture info from {0}", new object[]
				{
					@string
				});
				return null;
			}
			try
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);
				return Encoding.GetEncoding(cultureInfo.TextInfo.ANSICodePage);
			}
			catch (ArgumentException)
			{
				MrsTracer.Provider.Warning("PstMailbox.IMailbox: cannot resolve culture info from {0}", new object[]
				{
					@string
				});
			}
			catch (NotSupportedException)
			{
				MrsTracer.Provider.Warning("PstMailbox.IMailbox: cannot resolve culture info from {0}", new object[]
				{
					@string
				});
			}
			return null;
		}

		private void ValidateCodePageAndSetEncoding()
		{
			if (this.contentCodePage != null)
			{
				Exception ex = null;
				try
				{
					this.contentEncoding = Encoding.GetEncoding(this.contentCodePage.Value);
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (NotSupportedException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					throw new NotSupportedCodePagePermanentException(this.contentCodePage.Value, LocalServer.GetServer().Name);
				}
			}
		}

		private const uint RootFolderId = 290U;

		private static readonly MailboxInformation MailboxInformation = new MailboxInformation
		{
			ProviderName = "PstProvider"
		};

		public static readonly PropertyTag Language = new PropertyTag(973864991U);

		public static readonly PropertyTag MessageCodePage = new PropertyTag(1073545219U);

		public static readonly PropertyTag InternetCPID = new PropertyTag(1071513603U);

		public static readonly PropertyTag MessageSizeExtended = new PropertyTag(235405332U);

		private string filePath;

		private IPST iPst;

		private PSTPropertyBag messageStorePropertyBag;

		private Dictionary<string, uint> receiveFoldersTable;

		private Encoding cachedEncoding;

		private Encoding contentEncoding;

		private int? contentCodePage = null;

		private bool? encodingFound;
	}
}
