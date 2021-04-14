using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMServerRpcComponent : UMRPCComponentBase
	{
		internal static UMServerRpcComponent Instance
		{
			get
			{
				return UMServerRpcComponent.instance;
			}
		}

		internal override void RegisterServer()
		{
			ActiveDirectorySecurity sd = null;
			Util.GetServerSecurityDescriptor(ref sd);
			RpcServerBase.RegisterServer(typeof(UMServerRpcComponent.UMServerRpc), sd, 32);
		}

		private static UMServerRpcComponent instance = new UMServerRpcComponent();

		internal sealed class UMServerRpc : UMRpcServer
		{
			public override byte[] GetUmActiveCalls(bool isDialPlan, string dialPlan, bool isIpGateway, string ipGateway)
			{
				if (!UMServerPingRpcServerComponent.Instance.GuardBeforeExecution())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "GetUmActiveCalls: WP is shutting down.", new object[0]);
					return null;
				}
				byte[] array = null;
				try
				{
					this.isDialPlan = isDialPlan;
					this.dialPlan = dialPlan;
					this.isIpGateway = isIpGateway;
					this.ipGateway = ipGateway;
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "GetUmActiveCalls: Executing RPC request", new object[0]);
					ActiveCalls[] activeCalls = this.GetActiveCalls();
					array = Serialization.ObjectToBytes(activeCalls);
				}
				finally
				{
					UMServerPingRpcServerComponent.Instance.GuardAfterExecution();
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "GetUmActiveCalls: RPC request succeeded. Response {0}", new object[]
				{
					(array != null) ? "not null" : "null"
				});
				return array;
			}

			private ActiveCalls[] GetActiveCalls()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this.GetHashCode(), "In GetActiveCalls", new object[0]);
				BaseUMCallSession[] array = null;
				lock (UmServiceGlobals.VoipPlatform.CallSessionHashTable.SyncRoot)
				{
					int count = UmServiceGlobals.VoipPlatform.CallSessionHashTable.Count;
					if (count == 0)
					{
						return null;
					}
					array = new BaseUMCallSession[count];
					UmServiceGlobals.VoipPlatform.CallSessionHashTable.Values.CopyTo(array, 0);
				}
				List<ActiveCalls> list = new List<ActiveCalls>();
				foreach (BaseUMCallSession cs in array)
				{
					ActiveCalls item = null;
					if (this.ValidCallDataToAdd(cs, out item))
					{
						list.Add(item);
					}
				}
				return list.ToArray();
			}

			private bool ValidCallDataToAdd(BaseUMCallSession cs, out ActiveCalls call)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this.GetHashCode(), "In ValidCallDataToAdd", new object[0]);
				bool result;
				try
				{
					call = null;
					if (this.isDialPlan)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this.GetHashCode(), "In ValidCallDataToAdd, dialPlan = {0}", new object[]
						{
							this.dialPlan
						});
						if (string.Compare(cs.CurrentCallContext.DialPlan.Name, this.dialPlan, StringComparison.InvariantCultureIgnoreCase) != 0)
						{
							return false;
						}
					}
					else if (this.isIpGateway)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this.GetHashCode(), "In ValidCallDataToAdd, IpGateway = {0}", new object[]
						{
							this.ipGateway
						});
						if (!cs.CurrentCallContext.GatewayDetails.Equals(new UMSmartHost(this.ipGateway)))
						{
							return false;
						}
					}
					call = new ActiveCalls();
					call.GatewayId = cs.CurrentCallContext.GatewayDetails.ToString();
					call.ServerId = Utils.GetLocalHostName();
					call.DialPlan = cs.CurrentCallContext.DialPlan.Name;
					call.DialedNumber = cs.CurrentCallContext.CalleeId.ToDial;
					call.CallType = cs.TaskCallType.ToString();
					call.CallingNumber = cs.CurrentCallContext.CallerId.ToDial;
					call.DiversionNumber = cs.CurrentCallContext.Extension;
					call.CallState = cs.State.ToString();
					call.AppState = cs.AppState.ToString();
					result = true;
				}
				catch (NullReferenceException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.DiagnosticTracer, this.GetHashCode(), "Exception in ValidCallDataToAdd , error ={0}", new object[]
					{
						ex.ToString()
					});
					call = null;
					result = false;
				}
				return result;
			}

			private bool isDialPlan;

			private string dialPlan;

			private bool isIpGateway;

			private string ipGateway;
		}
	}
}
