using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Common;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class RpcKillServiceImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGenericReplyInfo tmpReplyInfo = null;
			RpcKillServiceImpl.Request req = null;
			RpcKillServiceImpl.Reply rep = new RpcKillServiceImpl.Reply();
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				req = ActiveManagerGenericRpcHelper.ValidateAndGetAttachedRequest<RpcKillServiceImpl.Request>(requestInfo, 1, 0);
				ReplayCrimsonEvents.ReceivedRequestToKillService.Log<string, string, RpcKillServiceImpl.Request>(req.ServiceName, req.OriginatingServerName, req);
				lock (RpcKillServiceImpl.locker)
				{
					RpcKillServiceImpl.HandleRequestInternal(req, rep);
				}
				tmpReplyInfo = ActiveManagerGenericRpcHelper.PrepareServerReply(requestInfo, rep, 1, 0);
			});
			if (tmpReplyInfo != null)
			{
				replyInfo = tmpReplyInfo;
			}
			if (ex != null)
			{
				throw new AmServerException(ex.Message, ex);
			}
		}

		private static void HandleRequestInternal(RpcKillServiceImpl.Request req, RpcKillServiceImpl.Reply rep)
		{
			Exception exception = null;
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				ServiceKillConfig serviceKillConfig = ServiceKillConfig.Read(req.ServiceName);
				rep.LastKillTime = serviceKillConfig.LastKillTime;
				if (!serviceKillConfig.IsDisabled)
				{
					using (Process serviceProcess = ServiceOperations.GetServiceProcess(req.ServiceName, out exception))
					{
						if (serviceProcess != null)
						{
							DateTime startTime = serviceProcess.StartTime;
							rep.ProcessStartTime = startTime;
							bool flag = false;
							if (req.IsForce)
							{
								flag = true;
							}
							else if (ExDateTime.Now.LocalTime - serviceKillConfig.LastKillTime > serviceKillConfig.DurationBetweenKill)
							{
								if (req.CheckTime > startTime)
								{
									flag = true;
								}
								else
								{
									rep.IsSkippedDueToTimeCheck = true;
								}
							}
							else
							{
								rep.IsThrottled = true;
							}
							if (flag)
							{
								bool flag2 = false;
								DateTime lastKillTime = serviceKillConfig.LastKillTime;
								try
								{
									ReplayCrimsonEvents.KillingServiceProcess.Log<string, string, int, RpcKillServiceImpl.Request>(req.ServiceName, serviceProcess.ProcessName, serviceProcess.Id, req);
									serviceKillConfig.UpdateKillTime(DateTime.UtcNow.ToLocalTime());
									Exception ex = null;
									try
									{
										serviceProcess.Kill();
									}
									catch (Win32Exception ex2)
									{
										ex = ex2;
									}
									catch (InvalidOperationException ex3)
									{
										ex = ex3;
									}
									if (ex != null)
									{
										throw new FailedToKillProcessForServiceException(req.ServiceName, ex.Message);
									}
									flag2 = true;
									goto IL_194;
								}
								finally
								{
									if (!flag2)
									{
										serviceKillConfig.UpdateKillTime(lastKillTime);
									}
									rep.IsSucceeded = flag2;
								}
								goto IL_178;
							}
							IL_194:
							return;
						}
						IL_178:
						throw new FailedToGetProcessForServiceException(req.ServiceName, exception.Message);
					}
				}
				rep.IsSkippedDueToRegistryOverride = true;
			});
		}

		internal static RpcKillServiceImpl.Reply SendKillRequest(string serverName, string serviceName, DateTime minTimeCheck, bool isForce, int timeoutInMSec)
		{
			Exception ex = null;
			RpcKillServiceImpl.Reply reply = null;
			bool isSucceded = false;
			bool flag = true;
			try
			{
				ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					ReplayCrimsonEvents.SendingServiceKillRequest.Log<string, string>(serviceName, serverName);
					RpcKillServiceImpl.Request attachedRequest = new RpcKillServiceImpl.Request(serviceName, minTimeCheck, false);
					RpcGenericRequestInfo requestInfo = ActiveManagerGenericRpcHelper.PrepareClientRequest(attachedRequest, 1, 1, 0);
					reply = ActiveManagerGenericRpcHelper.RunRpcAndGetReply<RpcKillServiceImpl.Reply>(requestInfo, serverName, timeoutInMSec);
					isSucceded = reply.IsSucceeded;
				});
				flag = false;
			}
			finally
			{
				if (isSucceded)
				{
					ReplayCrimsonEvents.ServiceKillRequestSucceeded.Log<string, string, string>(serviceName, serverName, reply.ToString());
				}
				else
				{
					string text = "Unknown";
					if (ex != null)
					{
						text = string.Format("Error: {0}", ex.Message);
					}
					else if (reply != null)
					{
						if (reply.IsThrottled)
						{
							text = "Throttled";
						}
						else if (reply.IsSkippedDueToTimeCheck)
						{
							text = "SkippedDueToTimeCheck";
						}
						else if (reply.IsSkippedDueToRegistryOverride)
						{
							text = "SkippedDueToRegistryOverride";
						}
					}
					ReplayCrimsonEvents.ServiceKillRequestFailed.Log<string, string, bool, string, bool, string>(serviceName, serverName, reply != null && reply.IsThrottled, text, flag, (reply != null) ? reply.ToString() : "<null>");
				}
			}
			return reply;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const int CommandCode = 1;

		private static object locker = new object();

		[Serializable]
		internal class Request
		{
			public Request(string serviceName, DateTime checkTime, bool isForce)
			{
				this.ServiceName = serviceName;
				this.CheckTime = checkTime;
				this.IsForce = isForce;
				this.OriginatingServerName = AmServerName.LocalComputerName.Fqdn;
			}

			public string OriginatingServerName { get; set; }

			public string ServiceName { get; private set; }

			public DateTimeOffset CheckTime { get; private set; }

			public bool IsForce { get; private set; }

			public override string ToString()
			{
				return string.Format("ServiceName: '{0}' CheckTime: '{1}' IsForce: '{2}'", this.ServiceName, this.CheckTime, this.IsForce);
			}
		}

		[Serializable]
		internal class Reply
		{
			public bool IsSucceeded { get; set; }

			public bool IsSkippedDueToTimeCheck { get; set; }

			public bool IsSkippedDueToRegistryOverride { get; set; }

			public bool IsThrottled { get; set; }

			public DateTimeOffset ProcessStartTime { get; set; }

			public DateTimeOffset LastKillTime { get; set; }

			public override string ToString()
			{
				return string.Format("IsSucceeded: '{0}' ProcessStartTime: '{1}' LastKillTime: '{2}' IsThrottled: '{3}' IsSkippedDueToTimeCheck: '{4}' IsSkippedDueToRegistryOverride: '{5}'", new object[]
				{
					this.IsSucceeded,
					this.ProcessStartTime,
					this.LastKillTime,
					this.IsThrottled,
					this.IsSkippedDueToTimeCheck,
					this.IsSkippedDueToRegistryOverride
				});
			}
		}
	}
}
