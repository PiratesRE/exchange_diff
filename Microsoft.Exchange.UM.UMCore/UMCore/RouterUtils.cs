using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RouterUtils
	{
		public static PlatformSipUri GetRedirectContactUri(PlatformSipUri requestUriOfCall, SipRoutingHelper routingHelper, string host, int port, TransportParameter transportParam, string orgGuidParam)
		{
			ValidateArgument.NotNull(requestUriOfCall, "requestUriOfCall");
			ValidateArgument.NotNull(routingHelper, "routingHelper");
			ValidateArgument.NotNullOrEmpty(host, "host");
			PlatformSipUri platformSipUri = Platform.Builder.CreateSipUri(requestUriOfCall.ToString());
			platformSipUri.Host = host;
			platformSipUri.Port = port;
			if (!string.IsNullOrEmpty(platformSipUri.FindParameter("maddr")))
			{
				platformSipUri.AddParameter("maddr", host);
			}
			platformSipUri.TransportParameter = transportParam;
			string value;
			platformSipUri.Host = routingHelper.GetContactHeaderHost(platformSipUri.Host, out value);
			if (!string.IsNullOrEmpty(value))
			{
				platformSipUri.AddParameter("ms-fe", value);
			}
			if (CommonConstants.UseDataCenterCallRouting && !string.IsNullOrEmpty(orgGuidParam))
			{
				if (platformSipUri.FindParameter("ms-organization-guid") != null)
				{
					platformSipUri.RemoveParameter("ms-organization-guid");
				}
				platformSipUri.AddParameter("ms-organization-guid", orgGuidParam);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "RouterUtils.GetRedirectContactUri - Added orgGuidParam '{0}' as SIP URI parameter", new object[]
				{
					orgGuidParam
				});
			}
			return platformSipUri;
		}

		public static bool TryGetHeader(IList<PlatformSignalingHeader> headerList, string headerName, out PlatformSignalingHeader header)
		{
			ValidateArgument.NotNullOrEmpty(headerName, "HeaderName");
			header = null;
			if (headerList == null)
			{
				return false;
			}
			for (int i = 0; i < headerList.Count; i++)
			{
				PlatformSignalingHeader platformSignalingHeader = headerList[i];
				if (platformSignalingHeader != null && string.Equals(platformSignalingHeader.Name, headerName, StringComparison.OrdinalIgnoreCase))
				{
					header = platformSignalingHeader;
					break;
				}
			}
			return header != null;
		}

		public static bool TryGetHeaderValue(IList<PlatformSignalingHeader> headers, string headerName, out string headerValue)
		{
			headerValue = string.Empty;
			PlatformSignalingHeader platformSignalingHeader = null;
			if (RouterUtils.TryGetHeader(headers, headerName, out platformSignalingHeader))
			{
				headerValue = platformSignalingHeader.Value;
				return true;
			}
			return false;
		}

		public static bool ShouldAccept(PlatformCallInfo callInfo, Guid tenantGuid, out string fqdn, out UMIPGateway gateway)
		{
			ValidateArgument.NotNull(callInfo, "CallInfo");
			gateway = null;
			fqdn = callInfo.RemoteMatchedFQDN;
			bool result;
			if (SipPeerManager.Instance.IsAuthorized(callInfo.RemoteCertificate, callInfo.RemotePeer, callInfo.RemoteHeaders, callInfo.RequestUri, tenantGuid, ref fqdn, out gateway))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMCallRouterTracer, null, "Accepting call authorized by diagnostics.", new object[0]);
				result = true;
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMCallRouterTracer, null, "Denying unauthorized call.", new object[0]);
				result = false;
			}
			return result;
		}

		public static StringBuilder GetDiversionLogString(ReadOnlyCollection<PlatformDiversionInfo> diversionInfo)
		{
			ValidateArgument.NotNull(diversionInfo, "diversionInfo");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PlatformDiversionInfo platformDiversionInfo in diversionInfo)
			{
				stringBuilder.Append(platformDiversionInfo.DiversionHeader).Append(" ");
			}
			return stringBuilder;
		}

		public static string GetReferredByHeader(IList<PlatformSignalingHeader> headers)
		{
			string result = null;
			RouterUtils.TryGetHeaderValue(headers, "Referred-By", out result);
			return result;
		}

		public static void ParseTelephonyAddress(PlatformTelephonyAddress address, UMDialPlan dialPlan, bool throwIfInValid, out PhoneNumber phoneNumber)
		{
			phoneNumber = null;
			try
			{
				if (UtilityMethods.IsAnonymousAddress(address))
				{
					phoneNumber = PhoneNumber.Empty;
				}
				else
				{
					RouterUtils.ParseSipUri(address.Uri, dialPlan, out phoneNumber);
				}
			}
			catch (CallRejectedException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "RouterUtils : ParseTelephonyAddress threw exception {0}.", new object[]
				{
					ex
				});
				if (throwIfInValid)
				{
					throw ex;
				}
			}
		}

		public static void ParseSipUri(PlatformSipUri sipUri, UMDialPlan dialPlan, out PhoneNumber phoneNumber)
		{
			if (UtilityMethods.IsAnonymousNumber(sipUri.User))
			{
				phoneNumber = PhoneNumber.Empty;
				return;
			}
			string text;
			if (sipUri.UserParameter == UserParameter.Phone || dialPlan.URIType != UMUriType.SipName)
			{
				text = sipUri.User.Split(new char[]
				{
					';'
				})[0];
			}
			else
			{
				text = sipUri.SimplifiedUri;
			}
			if (!PhoneNumber.TryParse(text, out phoneNumber))
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, 0, "CallContext::ParseSipUri: phoneOrSipUri:{0} is invalid, assuming Anonymous(Empty)", new object[]
				{
					text
				});
				phoneNumber = PhoneNumber.Empty;
			}
		}

		public static bool TryGetReferDataForSourceSourcePartyInfoDiversion(IRoutingContext context, UMRecipient recipient, string extension, out PlatformSipUri referredByWithContextUri, out IRedirectTargetChooser targetChooser)
		{
			targetChooser = null;
			referredByWithContextUri = null;
			BricksRoutingBasedServerChooser bricksRoutingBasedServerChooser = new BricksRoutingBasedServerChooser(context, recipient, 4);
			if (bricksRoutingBasedServerChooser.IsRedirectionNeeded)
			{
				targetChooser = bricksRoutingBasedServerChooser;
				UserTransferWithContext userTransferWithContext = new UserTransferWithContext(null);
				referredByWithContextUri = userTransferWithContext.SerializeCACallTransferWithContextUri(extension, context.DialPlan.PhoneContext);
				return true;
			}
			return false;
		}

		public static bool TryGetServerToServerReferTargetUri(UMDialPlan dialPlan, IRedirectTargetChooser targetChooser, PlatformSipUri requestUri, PlatformSipUri toUri, bool isTLSCall, out PlatformSipUri referTargetUri)
		{
			referTargetUri = null;
			if (dialPlan.URIType == UMUriType.SipName || CommonConstants.UseDataCenterCallRouting)
			{
				referTargetUri = Platform.Builder.CreateSipUri(toUri.ToString());
			}
			else
			{
				string host = null;
				int port;
				if (!targetChooser.GetTargetServer(out host, out port))
				{
					CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, 0, "Did not find a target server", new object[0]);
					targetChooser.HandleServerNotFound();
				}
				else
				{
					referTargetUri = Platform.Builder.CreateSipUri(SipUriScheme.Sip, requestUri.User, host);
					referTargetUri.Port = port;
					if (referTargetUri.TransportParameter == TransportParameter.None)
					{
						referTargetUri.TransportParameter = (isTLSCall ? TransportParameter.Tls : TransportParameter.Tcp);
					}
				}
			}
			return referTargetUri != null;
		}
	}
}
