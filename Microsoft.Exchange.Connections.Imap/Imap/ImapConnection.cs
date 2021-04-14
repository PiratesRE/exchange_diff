using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapConnection : DisposeTrackableBase, IConnection<ImapConnection>
	{
		private ImapConnection(ConnectionParameters connectionParameters)
		{
			this.connectionParameters = connectionParameters;
		}

		internal static IList<string> MessageInfoDataItemsForChangesOnly
		{
			get
			{
				return ImapConnection.messageInfoDataItemsForChangesOnly;
			}
		}

		internal static IList<string> MessageInfoDataItemsForNewMessages
		{
			get
			{
				return ImapConnection.messageInfoDataItemsForNewMessages;
			}
		}

		internal static IList<string> MessageBodyDataItems
		{
			get
			{
				return ImapConnection.messageBodyDataItems;
			}
		}

		internal ImapConnectionContext ConnectionContext
		{
			get
			{
				base.CheckDisposed();
				return this.connectionContext;
			}
		}

		private ConnectionParameters ConnectionParameters
		{
			get
			{
				base.CheckDisposed();
				return this.connectionParameters;
			}
		}

		public static ImapConnection CreateInstance(ConnectionParameters connectionParameters)
		{
			return new ImapConnection(connectionParameters).Initialize();
		}

		public ImapConnection Initialize()
		{
			this.connectionContext = new ImapConnectionContext(this.ConnectionParameters, null);
			return this;
		}

		public void ConnectAndAuthenticate(ImapServerParameters serverParameters, ImapAuthenticationParameters authenticationParameters, IServerCapabilities capabilities = null)
		{
			base.CheckDisposed();
			this.ThrowIfConnected();
			ImapConnectionContext imapConnectionContext = this.ConnectionContext;
			imapConnectionContext.AuthenticationParameters = authenticationParameters;
			imapConnectionContext.ServerParameters = serverParameters;
			imapConnectionContext.NetworkFacade = this.CreateNetworkFacade.Value(this.ConnectionContext, serverParameters);
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.ConnectAndAuthenticate(this.ConnectionContext, capabilities, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public OperationStatusCode TestLogon(ImapServerParameters serverParameters, ImapAuthenticationParameters authenticationParameters, IServerCapabilities capabilities)
		{
			base.CheckDisposed();
			this.ThrowIfConnected();
			OperationStatusCode result;
			try
			{
				this.ConnectAndAuthenticate(serverParameters, authenticationParameters, capabilities);
				result = OperationStatusCode.Success;
			}
			catch (ImapConnectionException)
			{
				result = OperationStatusCode.ErrorCannotCommunicateWithRemoteServer;
			}
			catch (ImapAuthenticationException ex)
			{
				if (ex.InnerException == null)
				{
					result = OperationStatusCode.ErrorInvalidCredentials;
				}
				else
				{
					result = OperationStatusCode.ErrorInvalidRemoteServer;
				}
			}
			catch (ImapCommunicationException)
			{
				result = OperationStatusCode.ErrorInvalidRemoteServer;
			}
			catch (MissingCapabilitiesException)
			{
				result = OperationStatusCode.ErrorUnsupportedProtocolVersion;
			}
			finally
			{
				if (this.IsConnected())
				{
					ImapConnectionCore.LogOff(this.ConnectionContext, null, null);
				}
			}
			return result;
		}

		public ImapServerCapabilities GetServerCapabilities()
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<ImapServerCapabilities> asyncOperationResult = ImapConnectionCore.Capabilities(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public void Expunge()
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.Expunge(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public ImapMailbox SelectImapMailbox(ImapMailbox imapMailbox)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<ImapMailbox> asyncOperationResult = ImapConnectionCore.SelectImapMailbox(this.ConnectionContext, imapMailbox, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public ImapResultData GetMessageInfoByRange(string start, string end, bool uidFetch, IList<string> messageDataItems)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<ImapResultData> messageInfoByRange = ImapConnectionCore.GetMessageInfoByRange(this.ConnectionContext, start, end, uidFetch, messageDataItems, null, null);
			this.ThrowIfExceptionNotNull(messageInfoByRange.Exception);
			return messageInfoByRange.Data;
		}

		public ImapResultData GetMessageItemByUid(string uid, IList<string> messageBodyDataItems)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<ImapResultData> messageItemByUid = ImapConnectionCore.GetMessageItemByUid(this.ConnectionContext, uid, messageBodyDataItems, null, null);
			this.ThrowIfExceptionNotNull(messageItemByUid.Exception);
			return messageItemByUid.Data;
		}

		public string AppendMessageToImapMailbox(string mailboxName, ImapMailFlags messageFlags, Stream messageMimeStream)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<string> asyncOperationResult = ImapConnectionCore.AppendMessageToImapMailbox(this.ConnectionContext, mailboxName, messageFlags, messageMimeStream, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public IList<string> SearchForMessageByMessageId(string messageId)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<IList<string>> asyncOperationResult = ImapConnectionCore.SearchForMessageByMessageId(this.ConnectionContext, messageId, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public void StoreMessageFlags(string uid, ImapMailFlags flagsToStore, ImapMailFlags previousFlags)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.StoreMessageFlags(this.ConnectionContext, uid, flagsToStore, previousFlags, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public void CreateImapMailbox(string mailboxName)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.CreateImapMailbox(this.ConnectionContext, mailboxName, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public void DeleteImapMailbox(string mailboxName)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.DeleteImapMailbox(this.ConnectionContext, mailboxName, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public void RenameImapMailbox(string oldMailboxName, string newMailboxName)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.RenameImapMailbox(this.ConnectionContext, oldMailboxName, newMailboxName, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public IList<ImapMailbox> ListImapMailboxesByLevel(int level, char separator)
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<IList<ImapMailbox>> asyncOperationResult = ImapConnectionCore.ListImapMailboxesByLevel(this.ConnectionContext, level, separator, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public void LogOff()
		{
			base.CheckDisposed();
			this.ThrowIfNotConnected();
			AsyncOperationResult<DBNull> asyncOperationResult = ImapConnectionCore.LogOff(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
		}

		public bool IsConnected()
		{
			base.CheckDisposed();
			ImapConnectionContext imapConnectionContext = this.ConnectionContext;
			return imapConnectionContext != null && imapConnectionContext.NetworkFacade != null && imapConnectionContext.NetworkFacade.IsConnected;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.ConnectionContext != null)
			{
				this.ConnectionContext.Dispose();
				this.connectionContext = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ImapConnection>(this);
		}

		private void ThrowIfConnected()
		{
			if (this.IsConnected())
			{
				throw new ConnectionAlreadyOpenException();
			}
		}

		private void ThrowIfNotConnected()
		{
			if (!this.IsConnected())
			{
				throw new ConnectionClosedException();
			}
		}

		private void ThrowIfExceptionNotNull(Exception exceptionOrNull)
		{
			if (exceptionOrNull == null)
			{
				return;
			}
			if (exceptionOrNull is LocalizedException)
			{
				throw exceptionOrNull;
			}
			string fullName = exceptionOrNull.GetType().FullName;
			throw new UnhandledException(fullName, exceptionOrNull);
		}

		internal readonly Hookable<Func<ImapConnectionContext, ImapServerParameters, INetworkFacade>> CreateNetworkFacade = Hookable<Func<ImapConnectionContext, ImapServerParameters, INetworkFacade>>.Create(true, (ImapConnectionContext connectionContext, ImapServerParameters serverParams) => new ImapNetworkFacade(connectionContext.ConnectionParameters, serverParams));

		private static readonly IList<string> messageInfoDataItemsForChangesOnly = new List<string>(new string[]
		{
			"UID",
			"FLAGS"
		}).AsReadOnly();

		private static readonly IList<string> messageInfoDataItemsForUidValidityRecovery = new List<string>(new string[]
		{
			"UID",
			"BODY.PEEK[HEADER.FIELDS (Message-ID)]"
		}).AsReadOnly();

		private static readonly IList<string> messageInfoDataItemsForNewMessages = new List<string>(new string[]
		{
			"UID",
			"FLAGS",
			"BODY.PEEK[HEADER.FIELDS (Message-ID)]",
			"RFC822.SIZE"
		}).AsReadOnly();

		private static readonly IList<string> messageBodyDataItems = new List<string>(new string[]
		{
			"UID",
			"INTERNALDATE",
			"BODY.PEEK[]"
		}).AsReadOnly();

		private ImapConnectionContext connectionContext;

		private ConnectionParameters connectionParameters;
	}
}
