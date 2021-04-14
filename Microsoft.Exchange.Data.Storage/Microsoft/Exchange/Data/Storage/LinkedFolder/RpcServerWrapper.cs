using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.JobQueue;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcServerWrapper : JobQueueRpcServer
	{
		public static bool Start(TimeSpan enqueueRequestTimeout, out Exception e)
		{
			RpcServerWrapper.enqueueRequestTimeout = enqueueRequestTimeout;
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
				RpcServerBase.StopServer(JobQueueRpcServer.RpcIntfHandle);
			}
		}

		private static void CrashOnCallTimeout(object state)
		{
			Thread thread = (Thread)state;
			Exception exception = new TimeoutException(string.Format("RpcServerWrapper.EnqueueRequest call timed out after {0} on thread {1}", RpcServerWrapper.enqueueRequestTimeout, (thread != null) ? thread.ManagedThreadId : -1));
			ExWatson.SendReportAndCrashOnAnotherThread(exception);
		}

		public override byte[] EnqueueRequest(int version, int type, byte[] inputParameterBytes)
		{
			byte[] result;
			try
			{
				using (new Timer(new TimerCallback(RpcServerWrapper.CrashOnCallTimeout), Thread.CurrentThread, RpcServerWrapper.enqueueRequestTimeout, TimeSpan.FromMilliseconds(-1.0)))
				{
					EnqueueResult enqueueResult = JobQueueManager.Enqueue((QueueType)type, inputParameterBytes);
					EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(enqueueResult);
					result = enqueueRequestRpcOutParameters.Serialize();
				}
			}
			catch (Exception ex)
			{
				ExWatson.SendReport(ex, ReportOptions.None, null);
				EnqueueRequestRpcOutParameters enqueueRequestRpcOutParameters = new EnqueueRequestRpcOutParameters(new EnqueueResult(EnqueueResultType.UnexpectedServerError, ServerStrings.RpcServerUnhandledException(ex.Message)));
				result = enqueueRequestRpcOutParameters.Serialize();
			}
			return result;
		}

		private static TimeSpan enqueueRequestTimeout = TimeSpan.MaxValue;

		public static int Registered;
	}
}
