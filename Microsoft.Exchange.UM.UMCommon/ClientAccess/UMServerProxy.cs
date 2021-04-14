using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal class UMServerProxy : UMVersionedRpcTargetBase
	{
		internal UMServerProxy(Server server) : base(server)
		{
			this.Fqdn = server.Fqdn;
			CallIdTracer.TraceDebug(ExTraceGlobals.ClientAccessTracer, this.GetHashCode(), "UMServerProxy(fqdn={0})", new object[]
			{
				this.Fqdn
			});
		}

		internal UMServerProxy(string serverFqdn) : base(null)
		{
			this.Fqdn = serverFqdn;
		}

		internal string Fqdn { get; private set; }

		internal UMCallInfoEx GetCallInfo(string callId)
		{
			GetCallInfoResponse getCallInfoResponse = (GetCallInfoResponse)this.SendReceive(new GetCallInfoRequest
			{
				CallId = callId
			});
			return getCallInfoResponse.CallInfo;
		}

		internal void Disconnect(string callId)
		{
			this.SendReceive(new DisconnectRequest
			{
				CallId = callId
			});
		}

		internal string PlayOnPhoneMessage(string proxyAddress, Guid userObjectGuid, Guid tenantGuid, string objectId, string dialString)
		{
			PlayOnPhoneResponse playOnPhoneResponse = (PlayOnPhoneResponse)this.SendReceive(new PlayOnPhoneMessageRequest
			{
				ProxyAddress = proxyAddress,
				UserObjectGuid = userObjectGuid,
				TenantGuid = tenantGuid,
				ObjectId = objectId,
				DialString = dialString
			});
			return playOnPhoneResponse.CallId;
		}

		internal string PlayOnPhoneGreeting(string proxyAddress, Guid userObjectGuid, Guid tenantGuid, UMGreetingType greetingType, string dialString)
		{
			PlayOnPhoneResponse playOnPhoneResponse = (PlayOnPhoneResponse)this.SendReceive(new PlayOnPhoneGreetingRequest
			{
				ProxyAddress = proxyAddress,
				UserObjectGuid = userObjectGuid,
				TenantGuid = tenantGuid,
				GreetingType = greetingType,
				DialString = dialString
			});
			return playOnPhoneResponse.CallId;
		}

		internal string PlayOnPhoneAAGreeting(UMAutoAttendant aa, Guid tenantGuid, string dialString, string userName, string fileName)
		{
			PlayOnPhoneResponse playOnPhoneResponse = (PlayOnPhoneResponse)this.SendReceive(new PlayOnPhoneAAGreetingRequest
			{
				AAIdentity = aa.Id.ObjectGuid,
				TenantGuid = tenantGuid,
				FileName = fileName,
				DialString = dialString,
				UserRecordingTheGreeting = userName
			});
			return playOnPhoneResponse.CallId;
		}

		internal string PlayOnPhonePAAGreeting(string proxyAddress, Guid userObjectGuid, Guid tenantGuid, Guid paaIdentity, string dialString)
		{
			PlayOnPhoneResponse playOnPhoneResponse = (PlayOnPhoneResponse)this.SendReceive(new PlayOnPhonePAAGreetingRequest
			{
				ProxyAddress = proxyAddress,
				UserObjectGuid = userObjectGuid,
				TenantGuid = tenantGuid,
				Identity = paaIdentity,
				DialString = dialString
			});
			return playOnPhoneResponse.CallId;
		}

		protected override UMVersionedRpcClientBase CreateRpcClient()
		{
			return new UMPlayOnPhoneRpcClient(this.Fqdn);
		}

		private ResponseBase SendReceive(RequestBase request)
		{
			ResponseBase responseBase = null;
			try
			{
				responseBase = (base.ExecuteRequest(request) as ResponseBase);
				if (responseBase == null)
				{
					throw new InvalidResponseException(this.Fqdn, string.Empty);
				}
				ErrorResponse errorResponse = responseBase as ErrorResponse;
				if (errorResponse != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.ClientAccessTracer, this.GetHashCode(), "UMServerProxy(fqdn={0}): ErrorResponse={1}", new object[]
					{
						this.Fqdn,
						errorResponse.ErrorType
					});
					throw errorResponse.GetException();
				}
			}
			catch (RpcException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.ClientAccessTracer, this.GetHashCode(), "UMServerProxy.SendReceive(fqdn={0}): Exception={1}", new object[]
				{
					this.Fqdn,
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CasToUmRpcFailure, this.Fqdn, new object[]
				{
					request.ProxyAddress,
					CommonUtil.ToEventLogString(ex)
				});
				throw new InvalidResponseException(this.Fqdn, ex.Message, ex);
			}
			return responseBase;
		}
	}
}
