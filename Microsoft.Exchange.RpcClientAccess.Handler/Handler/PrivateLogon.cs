using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PrivateLogon : Logon
	{
		internal PrivateLogon(MailboxSession mailboxSession, ClientSecurityContext delegatedClientSecurityContext, ConnectionHandler connectionHandler, NotificationHandler notificationHandler, OpenFlags openFlags, byte logonId, ProtocolLogLogonType protocolLogLogonType) : base(mailboxSession, delegatedClientSecurityContext, connectionHandler, notificationHandler, openFlags, logonId, protocolLogLogonType)
		{
			this.sessionReference = new ReferenceCount<MailboxSession>(mailboxSession);
		}

		public override StoreSession Session
		{
			get
			{
				return this.MailboxSession;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.sessionReference.ReferencedObject;
			}
		}

		public void TransportNewMail(StoreId folderId, StoreId messageId, string messageClass, MessageFlags messageFlags)
		{
			Util.ThrowOnNullArgument(messageClass, "messageClass");
			RopHandler.CheckEnum<MessageFlags>(messageFlags);
			StoreObjectId messageId2 = this.Session.IdConverter.CreateMessageId(folderId, messageId);
			this.Session.SpoolerManager.TransportNewMail(messageId2, messageClass, (int)messageFlags);
		}

		public void SetSpooler()
		{
			this.Session.SpoolerManager.SetSpooler();
		}

		public void SpoolerLockMessage(StoreId messageId, LockState lockState)
		{
			RopHandler.CheckEnum<LockState>(lockState);
			StoreObjectId defaultFolderId = this.MailboxSession.GetDefaultFolderId(DefaultFolderType.LegacySpoolerQueue);
			StoreObjectId messageId2 = this.Session.IdConverter.CreateMessageId(this.Session.IdConverter.GetFidFromId(defaultFolderId), messageId);
			SpoolerMessageLockState lockState2 = SpoolerMessageLockState.Lock;
			switch (lockState)
			{
			case LockState.Locked:
				lockState2 = SpoolerMessageLockState.Lock;
				break;
			case LockState.Unlocked:
				lockState2 = SpoolerMessageLockState.Unlock;
				break;
			case LockState.Finished:
				lockState2 = SpoolerMessageLockState.Finished;
				break;
			}
			this.Session.SpoolerManager.SpoolerLockMessage(messageId2, lockState2);
		}

		internal static MailboxSession CreateMailboxSession(PrivateLogon createAlternateFor, IConnection connection, OpenFlags openFlags, LogonExtendedRequestFlags extendedFlags, ExchangePrincipal exchangeMailboxPrincipal, string applicationId, ClientSecurityContext delegatedClientSecurityContext)
		{
			return Logon.CreateStoreSession<MailboxSession>(connection, openFlags, extendedFlags, exchangeMailboxPrincipal, delegatedClientSecurityContext, applicationId, (createAlternateFor != null) ? new Func<ExchangePrincipal, ClientSecurityContext, OpenFlags, IConnection, string, MailboxSession>(createAlternateFor.OpenAlternateMailboxSession) : new Func<ExchangePrincipal, ClientSecurityContext, OpenFlags, IConnection, string, MailboxSession>(PrivateLogon.OpenMailboxSession));
		}

		internal static void ValidatePrivateLogonSettings(LogonFlags logonFlags, OpenFlags openFlags, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId)
		{
			if (mailboxId == null)
			{
				throw new RopExecutionException("Private logon requires a mailboxId.", (ErrorCode)2147942487U);
			}
			if ((byte)(logonFlags & LogonFlags.Ghosted) != 0)
			{
				throw new RopExecutionException(string.Format("Unsupported private logon flags [{0}]", logonFlags & LogonFlags.Ghosted), (ErrorCode)2147942487U);
			}
			if ((openFlags & (OpenFlags.Public | OpenFlags.AlternateServer | OpenFlags.IgnoreHomeMdb)) != OpenFlags.None)
			{
				throw new RopExecutionException(string.Format("Unsupported private open flags [{0}]", openFlags & (OpenFlags.Public | OpenFlags.AlternateServer | OpenFlags.IgnoreHomeMdb)), (ErrorCode)2147942487U);
			}
			if ((openFlags & OpenFlags.CliWithPerMdbFix) != OpenFlags.CliWithPerMdbFix)
			{
				throw new RopExecutionException(string.Format("Required private open flags not passed [{0}]", ~openFlags & OpenFlags.CliWithPerMdbFix), (ErrorCode)2147942487U);
			}
			if ((openFlags & (OpenFlags.NoLocalization | OpenFlags.XForestMove)) != OpenFlags.None)
			{
				throw new RopExecutionException(string.Format("Not implemented private open flags [{0}]", openFlags & (OpenFlags.NoLocalization | OpenFlags.XForestMove)), (ErrorCode)2147749887U);
			}
			if ((extendedFlags & LogonExtendedRequestFlags.AuthContextCompressed) != LogonExtendedRequestFlags.None)
			{
				throw new RopExecutionException(string.Format("Not implemented private logon extended request flags [{0}]", extendedFlags & LogonExtendedRequestFlags.AuthContextCompressed), (ErrorCode)2147749887U);
			}
			if ((openFlags & OpenFlags.OverrideHomeMdb) == OpenFlags.OverrideHomeMdb && (openFlags & OpenFlags.RestoreDatabase) != OpenFlags.RestoreDatabase)
			{
				throw new RopExecutionException("Flags not implemented.", (ErrorCode)2147749887U);
			}
			if ((openFlags & OpenFlags.CliWithPerMdbFix) == OpenFlags.CliWithPerMdbFix)
			{
				openFlags |= (OpenFlags.CliWithNamedPropFix | OpenFlags.CliWithReplidGuidMappingFix);
			}
			bool flag = (openFlags & OpenFlags.RestoreDatabase) == OpenFlags.RestoreDatabase;
			if ((!mailboxId.Value.IsLegacyDn && !flag) || (byte)(logonFlags & LogonFlags.MbxGuids) == 32 || (openFlags & OpenFlags.MailboxGuid) == OpenFlags.MailboxGuid)
			{
				throw new RopExecutionException("Lookups based on guid are not implemented.", (ErrorCode)2147749887U);
			}
			if (mailboxId.Value.IsLegacyDn && !LegacyDN.IsValidLegacyDN(mailboxId.Value.LegacyDn))
			{
				throw new RopExecutionException("Invalid LegacyDN syntax.", (ErrorCode)2147942487U);
			}
		}

		internal void SetReceiveFolder(string messageClass, StoreId folderId)
		{
			this.MailboxSession.SetReceiveFolder(messageClass, this.MailboxSession.IdConverter.CreateFolderId(folderId));
		}

		internal override StoreId[] GetDefaultFolderIds()
		{
			StoreId[] array = new StoreId[PrivateLogon.PrivateDefaultFolders.Length];
			for (int i = 0; i < array.Length; i++)
			{
				DefaultFolderType defaultFolderType = PrivateLogon.PrivateDefaultFolders[i];
				StoreObjectId defaultFolderId = this.MailboxSession.GetDefaultFolderId(defaultFolderType);
				if (defaultFolderId != null)
				{
					array[i] = new StoreId(this.Session.IdConverter.GetFidFromId(defaultFolderId));
				}
			}
			return array;
		}

		internal string[] GetSupportedAddressTypes()
		{
			return this.Session.SupportedRoutingTypes.Where(new Func<string, bool>(this.IsAddressTypeSupported)).ToArray<string>();
		}

		internal Guid GetPerUserGuid(StoreLongTermId folderLongTermId)
		{
			Guid perUserGuid = this.MailboxSession.GetPerUserGuid(folderLongTermId.Guid, folderLongTermId.GlobCount);
			if (perUserGuid == Guid.Empty)
			{
				throw new RopExecutionException("No per-user guid found for the specified folder.", (ErrorCode)2147746063U);
			}
			return perUserGuid;
		}

		internal StoreLongTermId[] GetPerUserLongTermIds(Guid replicaGuid)
		{
			byte[][] perUserLongTermIds = this.MailboxSession.GetPerUserLongTermIds(replicaGuid);
			StoreLongTermId[] array = new StoreLongTermId[perUserLongTermIds.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StoreLongTermId.Parse(perUserLongTermIds[i], true);
			}
			return array;
		}

		internal bool GetAllPerUserLongTermIds(StoreLongTermId lastId, PerUserDataCollector collector)
		{
			byte[] lastLtid = lastId.ToBytes();
			PerUserData[] array;
			bool result = this.MailboxSession.GetAllPerUserLongTermIds(lastLtid, out array);
			foreach (PerUserData perUserData in array)
			{
				if (!collector.TryAddPerUserData(perUserData))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		internal PropertyValue[][] GetReceiveFolderInfo()
		{
			ReceiveFolderInfo[] receiveFolderInfo = this.MailboxSession.GetReceiveFolderInfo();
			PropertyValue[][] array = new PropertyValue[receiveFolderInfo.Length][];
			for (long num = 0L; num < (long)receiveFolderInfo.Length; num += 1L)
			{
				checked
				{
					array[(int)((IntPtr)num)] = new PropertyValue[]
					{
						new PropertyValue(PropertyTag.Fid, this.MailboxSession.IdConverter.GetFidFromId(receiveFolderInfo[(int)((IntPtr)num)].FolderId)),
						new PropertyValue(PropertyTag.MessageClass, receiveFolderInfo[(int)((IntPtr)num)].MessageClass),
						new PropertyValue(PropertyTag.LastModificationTime, receiveFolderInfo[(int)((IntPtr)num)].LastModification)
					};
				}
			}
			return array;
		}

		internal void UpdateDeferredActionMessages(byte[] serverEntryId, byte[] clientEntryId)
		{
			this.MailboxSession.UpdateDeferredActionMessages(serverEntryId, clientEntryId);
		}

		internal StoreId GetReceiveFolderId(string messageClass, out string explicitMessageClass)
		{
			if (messageClass.Length > 255)
			{
				throw new RopExecutionException("Message class length exceeded.", (ErrorCode)2147942487U);
			}
			StoreObjectId receiveFolderId = this.MailboxSession.GetReceiveFolderId(messageClass, out explicitMessageClass);
			return new StoreId(this.Session.IdConverter.GetFidFromId(receiveFolderId));
		}

		internal StoreId GetTransportQueueId()
		{
			if (this.transportQueueId == null)
			{
				StoreObjectId transportQueueFolderId = this.MailboxSession.GetTransportQueueFolderId();
				if (transportQueueFolderId == null)
				{
					throw new RopExecutionException("Transport queue folder was not found.", (ErrorCode)2147746063U);
				}
				this.transportQueueId = new StoreId?(new StoreId(this.Session.IdConverter.GetFidFromId(transportQueueFolderId)));
			}
			return this.transportQueueId.Value;
		}

		protected override IResourceTracker CreateResourceTracker()
		{
			int valueOrDefault = base.PropertyBag.GetValueOrDefault<int>(MailboxSchema.MaxUserMessageSize, 10240);
			return new ResourceTracker(valueOrDefault * 1024);
		}

		protected override bool TryGetOneOffPropertyStream(PropertyTag propertyTag, OpenMode openMode, bool isAppend, out Stream momtStream, out uint length)
		{
			if (propertyTag == PropertyTag.UnsearchableItems)
			{
				if (openMode != OpenMode.ReadOnly || isAppend)
				{
					throw new RopExecutionException(string.Format("UnsearchableItems one-off computed property must be ReadOnly and no Append. PropertyTag: {0}, OpenMode: {1}, IsAppend: {2}", propertyTag, openMode, isAppend), (ErrorCode)2147746050U);
				}
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Stream unsearchableItems = ((MailboxSession)this.Session).GetUnsearchableItems();
					disposeGuard.Add<Stream>(unsearchableItems);
					StreamSource streamSource = this.GetStreamSource();
					disposeGuard.Add<StreamSource>(streamSource);
					momtStream = Stream.Create(unsearchableItems, propertyTag.PropertyType, base.LogonObject, streamSource);
					length = uint.MaxValue;
					disposeGuard.Success();
					return true;
				}
			}
			return base.TryGetOneOffPropertyStream(propertyTag, openMode, isAppend, out momtStream, out length);
		}

		protected override StreamSource GetStreamSource()
		{
			return new StreamSource<MailboxSession>(this.sessionReference, (MailboxSession mailboxSession) => mailboxSession.Mailbox.CoreObject.PropertyBag);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PrivateLogon>(this);
		}

		protected override void InternalDispose()
		{
			this.sessionReference.Release();
			base.InternalDispose();
		}

		private static MailboxSession OpenMailboxSession(ExchangePrincipal exchangeMailboxPrincipal, ClientSecurityContext delegatedClientSecurityContext, OpenFlags openFlags, IConnection connection, string applicationId)
		{
			if ((openFlags & OpenFlags.UseAdminPrivilege) != OpenFlags.UseAdminPrivilege)
			{
				return MailboxSession.OpenWithBestAccess(exchangeMailboxPrincipal, connection.ActAsLegacyDN, delegatedClientSecurityContext ?? connection.AccessingClientSecurityContext, connection.CultureInfo, applicationId);
			}
			if (!connection.IsFederatedSystemAttendant)
			{
				return MailboxSession.OpenAsAdmin(exchangeMailboxPrincipal, connection.ActAsLegacyDN, delegatedClientSecurityContext ?? connection.AccessingClientSecurityContext, connection.CultureInfo, applicationId, false, false, (openFlags & OpenFlags.RestoreDatabase) == OpenFlags.RestoreDatabase);
			}
			if (!exchangeMailboxPrincipal.MailboxInfo.OrganizationId.Equals(connection.OrganizationId))
			{
				throw new RopExecutionException(string.Format("OrganizationId on user '{0}' doesn't match that on connection '{1}' for mailbox '{2}'", exchangeMailboxPrincipal.MailboxInfo.OrganizationId, connection.OrganizationId, exchangeMailboxPrincipal.LegacyDn), ErrorCode.UnknownUser);
			}
			if ((connection.ConnectionFlags & ConnectionFlags.RemoteSystemService) == ConnectionFlags.RemoteSystemService)
			{
				return MailboxSession.OpenAsSystemService(exchangeMailboxPrincipal, connection.CultureInfo, applicationId);
			}
			return MailboxSession.OpenAsAdmin(exchangeMailboxPrincipal, connection.CultureInfo, applicationId);
		}

		private bool IsAddressTypeSupported(string addressType)
		{
			if (addressType == "MOBILE")
			{
				return base.Connection.ClientInformation.Version >= MapiVersion.Outlook14 && TextMessagingHelper.ShouldReturnMobile(this.MailboxSession);
			}
			return !(Participant.IsRoutable(addressType, this.Session) == false);
		}

		private MailboxSession OpenAlternateMailboxSession(ExchangePrincipal exchangeMailboxPrincipal, ClientSecurityContext delegatedClientSecurityContext, OpenFlags openFlags, IConnection connection, string applicationId)
		{
			if (delegatedClientSecurityContext != null)
			{
				throw new InvalidOperationException("delegatedClientSecurityContext should never be specified for alternate private logons");
			}
			if ((openFlags & OpenFlags.UseAdminPrivilege) == OpenFlags.UseAdminPrivilege)
			{
				return MailboxSession.OpenAsAdmin(exchangeMailboxPrincipal, connection.ActAsLegacyDN, connection.AccessingClientSecurityContext, connection.CultureInfo, applicationId, false, false);
			}
			return this.MailboxSession.OpenAlternate(exchangeMailboxPrincipal);
		}

		private const LogonFlags InvalidLogonFlags = LogonFlags.Ghosted;

		private const OpenFlags InvalidOpenFlags = OpenFlags.Public | OpenFlags.AlternateServer | OpenFlags.IgnoreHomeMdb;

		private const OpenFlags RequiredOpenFlags = OpenFlags.CliWithPerMdbFix;

		private const OpenFlags NotImplementedOpenFlags = OpenFlags.NoLocalization | OpenFlags.XForestMove;

		private const LogonExtendedRequestFlags NotImplementedExtendedFlags = LogonExtendedRequestFlags.AuthContextCompressed;

		private static readonly DefaultFolderType[] PrivateDefaultFolders = new DefaultFolderType[]
		{
			DefaultFolderType.Configuration,
			DefaultFolderType.DeferredActionFolder,
			DefaultFolderType.LegacySpoolerQueue,
			DefaultFolderType.Root,
			DefaultFolderType.Inbox,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.CommonViews,
			DefaultFolderType.LegacySchedule,
			DefaultFolderType.SearchFolders,
			DefaultFolderType.LegacyViews,
			DefaultFolderType.LegacyShortcuts
		};

		private readonly ReferenceCount<MailboxSession> sessionReference;

		private StoreId? transportQueueId = null;
	}
}
