using System;
using System.Globalization;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapConnectionContext : DisposeTrackableBase
	{
		internal ImapConnectionContext(ConnectionParameters connectionParameters, IMonitorEvents eventsMonitor = null)
		{
			this.connectionParameters = connectionParameters;
			this.cachedCommand = new ImapCommand();
			this.WireUpOptionalEventHandlers(eventsMonitor);
		}

		private event EventHandler<RoundtripCompleteEventArgs> RoundtripComplete;

		private event EventHandler<DownloadCompleteEventArgs> DownloadsCompleted;

		private event EventHandler<EventArgs> MessagesDownloaded;

		private event EventHandler<EventArgs> MessagesUploaded;

		internal ConnectionParameters ConnectionParameters
		{
			get
			{
				base.CheckDisposed();
				return this.connectionParameters;
			}
		}

		internal INetworkFacade NetworkFacade
		{
			get
			{
				base.CheckDisposed();
				return this.networkFacade;
			}
			set
			{
				base.CheckDisposed();
				this.networkFacade = value;
			}
		}

		internal ImapAuthenticationParameters AuthenticationParameters
		{
			get
			{
				base.CheckDisposed();
				return this.authenticationParameters;
			}
			set
			{
				base.CheckDisposed();
				this.authenticationParameters = value;
			}
		}

		internal ServerParameters ServerParameters
		{
			get
			{
				base.CheckDisposed();
				return this.serverParameters;
			}
			set
			{
				base.CheckDisposed();
				this.serverParameters = value;
			}
		}

		internal string UserName
		{
			get
			{
				base.CheckDisposed();
				if (this.AuthenticationParameters == null || this.AuthenticationParameters.NetworkCredential == null)
				{
					return string.Empty;
				}
				return this.AuthenticationParameters.NetworkCredential.UserName;
			}
		}

		internal string Server
		{
			get
			{
				base.CheckDisposed();
				if (this.ServerParameters == null)
				{
					return string.Empty;
				}
				return this.ServerParameters.Server;
			}
		}

		internal ImapCommand CachedCommand
		{
			get
			{
				base.CheckDisposed();
				return this.cachedCommand;
			}
		}

		internal int CurrentCommandIndex
		{
			get
			{
				base.CheckDisposed();
				return this.currentCommandIndex;
			}
		}

		internal ImapMailFlags FlagsToRemove
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

		internal ILog Log
		{
			get
			{
				return this.ConnectionParameters.Log;
			}
		}

		internal ImapRootPathProcessingFlags ImapRootPathProcessingFlags
		{
			get
			{
				base.CheckDisposed();
				return this.imapRootPathProcessingFlags;
			}
		}

		internal ImapAuthenticationMechanism ImapAuthenticationMechanism
		{
			get
			{
				return this.AuthenticationParameters.ImapAuthenticationMechanism;
			}
		}

		internal ImapSecurityMechanism ImapSecurityMechanism
		{
			get
			{
				return this.AuthenticationParameters.ImapSecurityMechanism;
			}
		}

		internal bool IsConnected
		{
			get
			{
				return this.NetworkFacade.IsConnected;
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

		internal string UniqueCommandId()
		{
			base.CheckDisposed();
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
			{
				"E",
				this.currentCommandIndex++
			});
		}

		internal void InitializeRootPathProcessingFlags(int level, char separator)
		{
			base.CheckDisposed();
			if (level < 1 || level > 20)
			{
				throw new ArgumentException("level");
			}
			if (string.IsNullOrEmpty(this.RootFolderPath) || this.ImapRootPathProcessingFlags != ImapRootPathProcessingFlags.None)
			{
				return;
			}
			this.imapRootPathProcessingFlags = ImapRootPathProcessingFlags.FlagsInitialized;
			if (level != 1)
			{
				this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.UnableToProcess;
			}
		}

		internal void UpdateRootPathProcessingFlags(ImapRootPathProcessingFlags flagToAdd)
		{
			base.CheckDisposed();
			if (string.IsNullOrEmpty(this.RootFolderPath))
			{
				return;
			}
			if ((this.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.UnableToProcess) == ImapRootPathProcessingFlags.UnableToProcess)
			{
				this.Log.Debug("Can't add the processing flag. We are in unable to process state.", new object[0]);
				return;
			}
			this.Log.Assert((this.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.FlagsInitialized) == ImapRootPathProcessingFlags.FlagsInitialized, "We should not have uninitialized root path processing flags at this point", new object[0]);
			this.imapRootPathProcessingFlags |= flagToAdd;
		}

		internal void UpdateRootPathProcessingFlags(string mailboxName, char separator, int? level, int currentLevelMailboxCount)
		{
			base.CheckDisposed();
			if (string.IsNullOrEmpty(this.RootFolderPath))
			{
				return;
			}
			if (level == null || level.Value <= 0)
			{
				throw new ArgumentException("level");
			}
			if ((this.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.FlagsDetermined) == ImapRootPathProcessingFlags.FlagsDetermined || (this.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.UnableToProcess) == ImapRootPathProcessingFlags.UnableToProcess)
			{
				return;
			}
			this.Log.Assert((this.ImapRootPathProcessingFlags & ImapRootPathProcessingFlags.FlagsInitialized) == ImapRootPathProcessingFlags.FlagsInitialized, "We should never have uninitialized root path processing flags at this point", new object[0]);
			this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.FlagsDetermined;
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
					this.Log.Debug("The server returns prefixed mailbox names", new object[0]);
					this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.ResponseIncludesRootPathPrefix;
					if (text.Equals(text2) && level == 1)
					{
						if (currentLevelMailboxCount == 1)
						{
							this.Log.Debug("Single folder equal to the path prefix at level 1. Will be treated as INBOX.", new object[0]);
							this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.FolderPathPrefixIsInbox;
							return;
						}
						this.Log.Error("Invalid server response. mailbox name at level 1 equals the path prefix but multiple folders exist", new object[0]);
						this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.UnableToProcess;
						return;
					}
				}
				else
				{
					this.Log.Error("Cannot parse prefixed mailbox name {0}. It does not start with expected prefix {1}", new object[]
					{
						mailboxName,
						this.RootFolderPath
					});
					this.imapRootPathProcessingFlags |= ImapRootPathProcessingFlags.UnableToProcess;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ImapConnectionContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.NetworkFacade != null)
			{
				this.NetworkFacade.Dispose();
				this.NetworkFacade = null;
			}
		}

		private void WireUpOptionalEventHandlers(IMonitorEvents eventsMonitor)
		{
			if (eventsMonitor == null)
			{
				return;
			}
			this.DownloadsCompleted += eventsMonitor.DownloadsCompletedEventHandler;
			this.MessagesDownloaded += eventsMonitor.MessagesDownloadedEventHandler;
			this.MessagesUploaded += eventsMonitor.MessagesUploadedEventHandler;
			if (eventsMonitor.RoundtripCompleteEventHandler != null)
			{
				this.RoundtripComplete += eventsMonitor.RoundtripCompleteEventHandler;
			}
		}

		private readonly ConnectionParameters connectionParameters;

		private readonly ImapCommand cachedCommand;

		private ServerParameters serverParameters;

		private ImapAuthenticationParameters authenticationParameters;

		private INetworkFacade networkFacade;

		private int currentCommandIndex;

		private ImapMailFlags flagsToRemove;

		private ImapRootPathProcessingFlags imapRootPathProcessingFlags;

		private string rootFolderPath;

		private ExDateTime timeSent;
	}
}
