using System;
using System.Globalization;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPClientState : DisposeTrackableBase
	{
		internal IMAPClientState(Fqdn imapServer, int imapPort, string logonName, SecureString logonPassword, string rootFolderPath, SyncLogSession syncLogSession, string sessionId, Guid subscriptionGuid, IMAPAuthenticationMechanism authMechanism, IMAPSecurityMechanism secMechanism, AggregationType aggregationType, long maxDownloadBytesAllowed, int connectionTimeout, EventHandler<DownloadCompleteEventArgs> downloadsCompletedEventHandler, EventHandler<EventArgs> messagesDownloadedEventHandler, EventHandler<EventArgs> messagesUploadedEventHandler, EventHandler<RoundtripCompleteEventArgs> roundtripCompleteEventHandler) : this(logonName, logonPassword, rootFolderPath, syncLogSession, authMechanism, secMechanism, aggregationType)
		{
			this.commClient = new IMAPCommClient(imapServer, imapPort, syncLogSession, sessionId, subscriptionGuid, maxDownloadBytesAllowed, connectionTimeout);
			this.DownloadsCompleted += downloadsCompletedEventHandler;
			this.MessagesDownloaded += messagesDownloadedEventHandler;
			this.MessagesUploaded += messagesUploadedEventHandler;
			if (roundtripCompleteEventHandler != null)
			{
				this.RoundtripComplete += roundtripCompleteEventHandler;
			}
		}

		internal IMAPClientState(ICommClient commClient, string logonName, SecureString logonPassword, string rootFolderPath, SyncLogSession syncLogSession, IMAPAuthenticationMechanism authMechanism, IMAPSecurityMechanism secMechanism) : this(logonName, logonPassword, rootFolderPath, syncLogSession, authMechanism, secMechanism, AggregationType.Aggregation)
		{
			this.commClient = commClient;
		}

		private IMAPClientState(string logonName, SecureString logonPassword, string rootFolderPath, SyncLogSession syncLogSession, IMAPAuthenticationMechanism authMechanism, IMAPSecurityMechanism secMechanism, AggregationType aggregationType)
		{
			this.logonName = logonName;
			this.logonPassword = logonPassword;
			this.rootFolderPath = rootFolderPath;
			this.rootPathProcessingFlags = RootPathProcessingFlags.None;
			this.logSession = syncLogSession;
			this.imapAuthenticationMechanism = authMechanism;
			this.imapSecurityMechanism = secMechanism;
			this.aggregationType = aggregationType;
		}

		private event EventHandler<DownloadCompleteEventArgs> DownloadsCompleted;

		private event EventHandler<EventArgs> MessagesDownloaded;

		private event EventHandler<EventArgs> MessagesUploaded;

		private event EventHandler<RoundtripCompleteEventArgs> RoundtripComplete;

		internal SyncLogSession Log
		{
			get
			{
				base.CheckDisposed();
				return this.logSession;
			}
		}

		internal ICommClient CommClient
		{
			get
			{
				base.CheckDisposed();
				return this.commClient;
			}
		}

		internal string LogonName
		{
			get
			{
				base.CheckDisposed();
				return this.logonName;
			}
		}

		internal SecureString LogonPassword
		{
			get
			{
				base.CheckDisposed();
				return this.logonPassword;
			}
		}

		internal IMAPCommand CachedCommand
		{
			get
			{
				base.CheckDisposed();
				if (this.cachedCommand == null)
				{
					this.cachedCommand = new IMAPCommand();
				}
				return this.cachedCommand;
			}
		}

		internal IMAPMailFlags FlagsToRemove
		{
			get
			{
				base.CheckDisposed();
				return this.flagsToRemove;
			}
			set
			{
				base.CheckDisposed();
				this.flagsToRemove = value;
			}
		}

		internal string UniqueCommandId
		{
			get
			{
				base.CheckDisposed();
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
				{
					"E",
					this.currentCommandIndex++
				});
			}
		}

		internal string RootFolderPath
		{
			get
			{
				base.CheckDisposed();
				return this.rootFolderPath;
			}
			set
			{
				base.CheckDisposed();
				this.rootFolderPath = value;
			}
		}

		internal ExDateTime TimeSent
		{
			get
			{
				base.CheckDisposed();
				return this.timeSent;
			}
			set
			{
				base.CheckDisposed();
				this.timeSent = value;
			}
		}

		internal RootPathProcessingFlags RootPathProcessingFlags
		{
			get
			{
				base.CheckDisposed();
				return this.rootPathProcessingFlags;
			}
		}

		internal IMAPAuthenticationMechanism IMAPAuthenticationMechanism
		{
			get
			{
				base.CheckDisposed();
				return this.imapAuthenticationMechanism;
			}
		}

		internal IMAPSecurityMechanism IMAPSecurityMechanism
		{
			get
			{
				base.CheckDisposed();
				return this.imapSecurityMechanism;
			}
		}

		internal AggregationType AggregationType
		{
			get
			{
				base.CheckDisposed();
				return this.aggregationType;
			}
		}

		internal void NotifyRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.CheckDisposed();
			if (this.RoundtripComplete != null)
			{
				this.RoundtripComplete(sender, roundtripCompleteEventArgs);
			}
		}

		internal void ActivatePerfDownloadEvent(object sender, DownloadCompleteEventArgs eventArgs)
		{
			base.CheckDisposed();
			if (this.DownloadsCompleted != null)
			{
				this.DownloadsCompleted(sender, eventArgs);
			}
		}

		internal void ActivatePerfMsgDownloadEvent(object sender, EventArgs eventArgs)
		{
			base.CheckDisposed();
			if (this.MessagesDownloaded != null)
			{
				this.MessagesDownloaded(sender, eventArgs);
			}
		}

		internal void ActivatePerfMsgUploadEvent(object sender, EventArgs eventArgs)
		{
			base.CheckDisposed();
			if (this.MessagesUploaded != null)
			{
				this.MessagesUploaded(sender, eventArgs);
			}
		}

		internal void InitializeRootPathProcessingFlags(int level, char separator)
		{
			if (level < 1 || level > IMAPSyncStorageProvider.MaxFolderLevelDepth)
			{
				throw new ArgumentException("level");
			}
			if (string.IsNullOrEmpty(this.rootFolderPath) || this.rootPathProcessingFlags != RootPathProcessingFlags.None)
			{
				return;
			}
			this.rootPathProcessingFlags = RootPathProcessingFlags.FlagsInitialized;
			if (level != 1)
			{
				this.rootPathProcessingFlags |= RootPathProcessingFlags.UnableToProcess;
			}
		}

		internal void UpdateRootPathProcessingFlags(Trace tracer, RootPathProcessingFlags flagToAdd)
		{
			if (string.IsNullOrEmpty(this.rootFolderPath))
			{
				return;
			}
			if ((this.rootPathProcessingFlags & RootPathProcessingFlags.UnableToProcess) == RootPathProcessingFlags.UnableToProcess)
			{
				this.Log.LogDebugging((TSLID)842UL, tracer, "Can't add the processing flag. We are in unable to process state.", new object[0]);
				return;
			}
			this.rootPathProcessingFlags |= flagToAdd;
		}

		internal void UpdateRootPathProcessingFlags(Trace tracer, string mailboxName, char separator, int? level, int currentLevelMailboxCount)
		{
			if (string.IsNullOrEmpty(this.rootFolderPath))
			{
				return;
			}
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxName", mailboxName);
			if (level == null || level.Value <= 0)
			{
				throw new ArgumentException("level");
			}
			if ((this.rootPathProcessingFlags & RootPathProcessingFlags.FlagsDetermined) == RootPathProcessingFlags.FlagsDetermined || (this.rootPathProcessingFlags & RootPathProcessingFlags.UnableToProcess) == RootPathProcessingFlags.UnableToProcess)
			{
				return;
			}
			this.rootPathProcessingFlags |= RootPathProcessingFlags.FlagsDetermined;
			string text = this.RootFolderPath.TrimEnd(new char[]
			{
				separator
			});
			string text2 = mailboxName.TrimEnd(new char[]
			{
				separator
			});
			if (text2.Split(new char[]
			{
				separator
			}).Length > level || (level == 1 && text.Equals(text)))
			{
				if (mailboxName.StartsWith(text))
				{
					this.Log.LogDebugging((TSLID)843UL, tracer, "The server returns prefixed mailbox names", new object[0]);
					this.rootPathProcessingFlags |= RootPathProcessingFlags.ResponseIncludesRootPathPrefix;
					if (text.Equals(text2) && level == 1)
					{
						if (currentLevelMailboxCount == 1)
						{
							this.Log.LogVerbose((TSLID)1409UL, tracer, "Single folder equal to the path prefix at level 1. Will be treated as INBOX.", new object[0]);
							this.rootPathProcessingFlags |= RootPathProcessingFlags.FolderPathPrefixIsInbox;
							return;
						}
						this.Log.LogError((TSLID)1410UL, tracer, "Invalid server response. Mailbox name at level 1 equals the path prefix but multiple folders exist", new object[0]);
						this.rootPathProcessingFlags |= RootPathProcessingFlags.UnableToProcess;
						return;
					}
				}
				else
				{
					this.Log.LogError((TSLID)844UL, tracer, "Cannot parse prefixed mailbox name {0}. It does not start with expected prefix {1}", new object[]
					{
						mailboxName,
						this.rootFolderPath
					});
					this.rootPathProcessingFlags |= RootPathProcessingFlags.UnableToProcess;
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.commClient != null)
			{
				this.commClient.Dispose();
				this.commClient = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPClientState>(this);
		}

		private ICommClient commClient;

		private string logonName;

		private SecureString logonPassword;

		private SyncLogSession logSession;

		private IMAPAuthenticationMechanism imapAuthenticationMechanism;

		private IMAPSecurityMechanism imapSecurityMechanism;

		private AggregationType aggregationType;

		private string rootFolderPath;

		private RootPathProcessingFlags rootPathProcessingFlags;

		private int currentCommandIndex;

		private IMAPCommand cachedCommand;

		private IMAPMailFlags flagsToRemove;

		private ExDateTime timeSent;
	}
}
