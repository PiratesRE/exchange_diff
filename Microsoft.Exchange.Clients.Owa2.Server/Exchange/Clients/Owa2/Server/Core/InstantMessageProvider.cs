using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class InstantMessageProvider : DisposeTrackableBase
	{
		protected InstantMessageProvider(IUserContext userContext, InstantMessageNotifier notifier)
		{
			if (notifier == null)
			{
				throw new ArgumentNullException("payload");
			}
			this.Notifier = notifier;
			this.UserContext = userContext;
			this.IsEarlierSignInSuccessful = true;
			this.ExpandedGroupIds = new HashSet<string>();
		}

		internal static bool IsInitialized
		{
			get
			{
				return InstantMessageProvider.isInitialized;
			}
		}

		internal static object InitializationLock
		{
			get
			{
				return InstantMessageProvider.initializationLock;
			}
		}

		internal static string OcsServerName { get; private set; }

		internal static int IMPortNumber { get; private set; }

		internal static Func<long> GetElapsedMilliseconds { get; private set; }

		internal static int ActivityBasedPresenceDuration { get; private set; }

		internal static bool ArePerfCountersEnabled { get; private set; }

		internal static IXSOFactory XsoFactory { get; private set; }

		internal HashSet<string> ExpandedGroupIds { get; set; }

		internal virtual bool IsActivityBasedPresenceSet { get; set; }

		internal IUserContext UserContext { get; private set; }

		internal InstantMessageNotifier Notifier { get; private set; }

		internal bool IsEarlierSignInSuccessful { get; set; }

		internal abstract bool IsSessionStarted { get; }

		internal abstract bool IsUserUcsMode { get; }

		public static InstantMessageOperationError Initialize()
		{
			if (InstantMessageProvider.isInitialized)
			{
				return InstantMessageOperationError.Success;
			}
			InstantMessageOperationError result;
			try
			{
				if (!Monitor.TryEnter(InstantMessageProvider.initializationLock))
				{
					result = InstantMessageOperationError.InitializationInProgress;
				}
				else if (InstantMessageProvider.IsInitialized)
				{
					result = InstantMessageOperationError.Success;
				}
				else
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					InstantMessagingConfiguration instantMessagingConfiguration = null;
					try
					{
						bool appSetting = BaseApplication.GetAppSetting<bool>("EnableIMForOwaPremium", false);
						if (appSetting)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Globals.Initialize: OWA2 Instant Messaging integration is disabled by web.config.");
							return InstantMessageOperationError.NotEnabled;
						}
						if (!VdirConfiguration.Instance.InstantMessagingEnabled)
						{
							return InstantMessageOperationError.NotEnabled;
						}
						instantMessagingConfiguration = InstantMessagingConfiguration.GetInstance(VdirConfiguration.Instance);
						if (!instantMessagingConfiguration.CheckConfiguration())
						{
							return InstantMessageOperationError.NotConfigured;
						}
					}
					finally
					{
						stopwatch.Stop();
						OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.CheckConfiguration, stopwatch.ElapsedMilliseconds);
					}
					if (InstantMessageProvider.Initialize(instantMessagingConfiguration.ServerName, instantMessagingConfiguration.PortNumber, () => Globals.ApplicationTime, Globals.ActivityBasedPresenceDuration, Globals.ArePerfCountersEnabled))
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Globals.Initialize: Success!");
						result = InstantMessageOperationError.Success;
					}
					else
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "Globals.Initialize: Initialization failed.");
						result = InstantMessageOperationError.NotConfigured;
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(InstantMessageProvider.initializationLock))
				{
					Monitor.Exit(InstantMessageProvider.initializationLock);
				}
			}
			return result;
		}

		public static void DisposeProvider()
		{
			InstantMessageOCSProvider.DisposeEndpointManager();
		}

		internal abstract void EstablishSession();

		internal abstract void GetExpandedGroups(MailboxSession session);

		internal abstract int ResetPresence();

		internal abstract int SendChatMessage(ChatMessage message);

		internal abstract int SendNewChatMessage(ChatMessage message);

		internal abstract void AddBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, InstantMessageGroup group);

		internal abstract void RemoveBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, StoreId contactId);

		internal abstract void EndChatSession(int chatSessionId, bool disconnectSession);

		internal abstract void NotifyTyping(int chatSessionId, bool typingCanceled);

		internal abstract int PublishSelfPresence(InstantMessagePresenceType presence);

		protected abstract void CreateGroup(string groupName);

		internal abstract void AcceptBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, InstantMessageGroup group);

		internal abstract void DeclineBuddy(InstantMessageBuddy buddy);

		protected abstract void GetBuddyList();

		internal abstract void AddSubscription(string[] sipUris);

		internal abstract void RemoveSubscription(string sipUri);

		internal abstract void QueryPresence(string[] sipUris);

		internal abstract void PublishResetStatus();

		internal abstract int ParticipateInConversation(int conversationId);

		protected static void Log(Enum key, object value)
		{
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null)
			{
				OwaApplication.GetRequestDetailsLogger.Set(key, value);
			}
		}

		private static bool Initialize(string ocsServerName, int imPortNumber, Func<long> getElapsedMilliseconds, int activityBasedPresenceDuration, bool arePerfCountersEnabled)
		{
			InstantMessageProvider.OcsServerName = ocsServerName;
			InstantMessageProvider.IMPortNumber = imPortNumber;
			InstantMessageProvider.GetElapsedMilliseconds = getElapsedMilliseconds;
			InstantMessageProvider.ActivityBasedPresenceDuration = activityBasedPresenceDuration;
			InstantMessageProvider.ArePerfCountersEnabled = arePerfCountersEnabled;
			InstantMessageProvider.XsoFactory = new XSOFactory();
			InstantMessageProvider.isInitialized = InstantMessageOCSProvider.InitializeProvider();
			return InstantMessageProvider.isInitialized;
		}

		private static bool isInitialized = false;

		private static object initializationLock = new object();
	}
}
