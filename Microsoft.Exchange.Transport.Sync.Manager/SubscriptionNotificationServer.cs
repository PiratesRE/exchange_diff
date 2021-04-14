using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.SubscriptionNotification;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionNotificationServer : SubscriptionNotificationRpcServer
	{
		internal static bool IgnoreNotifications
		{
			set
			{
				SubscriptionNotificationServer.ignoreNotifications = value;
			}
		}

		public override byte[] InvokeSubscriptionNotificationEndPoint(int version, byte[] inputBlob)
		{
			if (this.stopped)
			{
				return RpcHelper.CreateResponsePropertyCollection(2835349507U, 2);
			}
			if (version > 1)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)395UL, SubscriptionNotificationServer.tracer, (long)this.GetHashCode(), "Subscription Notification RPC server will return {0} since the current version is {1} and the requested version was {2}.", new object[]
				{
					SubscriptionNotificationResult.ServerVersionMismatch,
					1,
					version
				});
				return RpcHelper.CreateResponsePropertyCollection(2835349507U, 1);
			}
			if (SubscriptionNotificationServer.ignoreNotifications)
			{
				return SubscriptionNotificationServer.SuccessOutput;
			}
			byte[] result;
			try
			{
				result = this.notificationProcessor.InvokeSubscriptionNotificationEndPoint(inputBlob);
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = null;
			}
			return result;
		}

		internal static bool TryStartServer()
		{
			if (SubscriptionNotificationServer.ignoreNotifications)
			{
				return true;
			}
			if (SubscriptionNotificationServer.notificationServerInstance != null)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)1357UL, SubscriptionNotificationServer.tracer, 0L, "Subscription Notification RPC Server already registered and running.", new object[0]);
				return true;
			}
			FileSecurity fileSecurity = new FileSecurity();
			SecurityIdentifier exchangeServersUsgSid;
			try
			{
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 180, "TryStartServer", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\RPC\\SubscriptionNotificationServer.cs");
				exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
			}
			catch (LocalizedException ex)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)1366UL, SubscriptionNotificationServer.tracer, 0L, "Failed to extract SID for Exchange Servers Usage role due to exception {0}.", new object[]
				{
					ex
				});
				return false;
			}
			FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.SetAccessRule(fileSystemAccessRule);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			fileSecurity.SetOwner(securityIdentifier);
			string securityDescriptorSddlForm = fileSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			SubscriptionNotificationServer.notificationServerInstance = (SubscriptionNotificationServer)RpcServerBase.RegisterServer(typeof(SubscriptionNotificationServer), fileSecurity, 1, false, (uint)ContentAggregationConfig.MaxNotificationThreads);
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)1367UL, SubscriptionNotificationServer.tracer, 0L, "Subscription Notification RPC Server loaded with rpcSDDL {0}.", new object[]
			{
				securityDescriptorSddlForm
			});
			SubscriptionNotificationServer.notificationServerInstance.Start();
			return true;
		}

		internal static void StopServer()
		{
			if (SubscriptionNotificationServer.ignoreNotifications)
			{
				return;
			}
			if (SubscriptionNotificationServer.notificationServerInstance != null)
			{
				SubscriptionNotificationServer.notificationServerInstance.Stop();
				SubscriptionNotificationServer.notificationServerInstance = null;
			}
		}

		private void Start()
		{
			lock (this.syncObject)
			{
				this.stopped = false;
			}
			ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)1375UL, SubscriptionNotificationServer.tracer, (long)this.GetHashCode(), "Started the SubscriptionNotificationServer.", new object[0]);
		}

		private void Stop()
		{
			lock (this.syncObject)
			{
				if (this.stopped)
				{
					return;
				}
				this.stopped = true;
			}
			RpcServerBase.StopServer(SubscriptionNotificationRpcServer.RpcIntfHandle);
			ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)1376UL, SubscriptionNotificationServer.tracer, (long)this.GetHashCode(), "Waiting for existing completion server threads to finish.", new object[0]);
			this.workThreadsEvent.WaitOne();
			ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)1377UL, SubscriptionNotificationServer.tracer, (long)this.GetHashCode(), "Stopped the SubscriptionNotificationServer.", new object[0]);
		}

		private const int CurrentVersion = 1;

		private static readonly byte[] SuccessOutput = SubscriptionNotificationProcessor.SuccessOutput;

		private static readonly Trace tracer = ExTraceGlobals.SubscriptionNotificationServerTracer;

		private readonly ManualResetEvent workThreadsEvent = new ManualResetEvent(true);

		private readonly object syncObject = new object();

		private readonly SubscriptionNotificationProcessor notificationProcessor = new SubscriptionNotificationProcessor();

		private static SubscriptionNotificationServer notificationServerInstance;

		private static bool ignoreNotifications = false;

		private bool stopped;
	}
}
