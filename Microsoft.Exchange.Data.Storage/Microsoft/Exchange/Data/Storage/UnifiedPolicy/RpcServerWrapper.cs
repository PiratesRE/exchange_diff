using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UnifiedPolicyNotification;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcServerWrapper : UnifiedPolicyNotificationRpcServer
	{
		public static bool Start(TimeSpan notifyRequestTimeout, out Exception e)
		{
			RpcServerWrapper.notifyRequestTimeout = notifyRequestTimeout;
			e = null;
			if (RpcServerWrapper.Registered == 1)
			{
				return true;
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.Read, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			bool result;
			try
			{
				RpcServerBase.RegisterServer(typeof(RpcServerWrapper), fileSecurity, 131209);
				Interlocked.CompareExchange(ref RpcServerWrapper.Registered, 1, 0);
				result = true;
			}
			catch (RpcException ex)
			{
				e = ex;
				Interlocked.CompareExchange(ref RpcServerWrapper.Registered, 0, 1);
				result = false;
			}
			return result;
		}

		public static void Stop()
		{
			int num = Interlocked.CompareExchange(ref RpcServerWrapper.Registered, 0, 1);
			if (num == 1)
			{
				RpcServerBase.StopServer(UnifiedPolicyNotificationRpcServer.RpcIntfHandle);
			}
		}

		private static void CrashOnCallTimeout(object state)
		{
			Thread thread = (Thread)state;
			Exception exception = new TimeoutException(string.Format("RpcServerWrapper.NotifyRequest call timed out after {0} on thread {1}", RpcServerWrapper.notifyRequestTimeout, (thread != null) ? thread.ManagedThreadId : -1));
			ExWatson.SendReportAndCrashOnAnotherThread(exception);
		}

		public override byte[] Notify(int version, int type, byte[] inputParameterBytes)
		{
			byte[] result2;
			try
			{
				using (new Timer(new TimerCallback(RpcServerWrapper.CrashOnCallTimeout), Thread.CurrentThread, RpcServerWrapper.notifyRequestTimeout, TimeSpan.FromMilliseconds(-1.0)))
				{
					WorkItemBase workItem = WorkItemBase.Deserialize(inputParameterBytes);
					SyncNotificationResult result = null;
					try
					{
						WorkItemBase workItem2 = SyncManager.EnqueueWorkItem(workItem);
						result = new SyncNotificationResult(UnifiedPolicyNotificationFactory.Create(workItem2, new ADObjectId()));
					}
					catch (SyncAgentExceptionBase error)
					{
						result = new SyncNotificationResult(error);
					}
					NotificationRpcOutParameters notificationRpcOutParameters = new NotificationRpcOutParameters(result);
					result2 = notificationRpcOutParameters.Serialize();
				}
			}
			catch (Exception ex)
			{
				ExWatson.SendReport(ex, ReportOptions.None, null);
				NotificationRpcOutParameters notificationRpcOutParameters = new NotificationRpcOutParameters(new SyncNotificationResult(ex));
				result2 = notificationRpcOutParameters.Serialize();
			}
			return result2;
		}

		private static TimeSpan notifyRequestTimeout = TimeSpan.MaxValue;

		public static int Registered;
	}
}
