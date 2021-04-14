using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class SipRoutingHelper
	{
		public static SipRoutingHelper Create(PlatformCallInfo callInfo)
		{
			string text = callInfo.RequestUri.FindParameter("ms-organization");
			string remoteMatchedFQDN = callInfo.RemoteMatchedFQDN;
			bool flag = SipPeerManager.Instance.IsLocalDiagnosticCall(callInfo.RemotePeer, callInfo.RemoteHeaders);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "Create: orgParameter:{0} remoteMatchedFqdn:{1} isLocalDiagnosticCall:{2}", new object[]
			{
				text,
				remoteMatchedFQDN,
				flag
			});
			SipRoutingHelper result;
			if (callInfo.IsInbound)
			{
				result = SipRoutingHelper.CreateForInbound(flag, remoteMatchedFQDN, text);
			}
			else
			{
				if (!callInfo.IsServiceRequest)
				{
					throw new InvalidOperationException();
				}
				result = SipRoutingHelper.CreateForServiceRequest(remoteMatchedFQDN, text);
			}
			return result;
		}

		public static SipRoutingHelper CreateForInbound(bool isLocalDiagnosticCall, string remotedMatchedFqdn, string orgParameter)
		{
			SipRoutingHelper sipRoutingHelper = (!isLocalDiagnosticCall && SipPeerManager.Instance.IsAccessProxyWithOrgTestHook(remotedMatchedFqdn, orgParameter)) ? SipRoutingHelper.MsOrganizationRoutingHelper.Create() : SipRoutingHelper.DefaultRoutingHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "CreateForInbound {0}", new object[]
			{
				sipRoutingHelper.GetType().Name
			});
			return sipRoutingHelper;
		}

		public static SipRoutingHelper CreateForServiceRequest(string remoteMatchedFqdn, string orgParameter)
		{
			if (string.IsNullOrEmpty(remoteMatchedFqdn))
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.UnsupportedRequest, "TLS connection required.", new object[0]);
			}
			if (CommonConstants.UseDataCenterCallRouting && !SipPeerManager.Instance.IsAccessProxy(remoteMatchedFqdn))
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.UnsupportedRequest, null, new object[0]);
			}
			SipRoutingHelper sipRoutingHelper = SipRoutingHelper.MsOrganizationRoutingHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "CreateForServiceRequest {0}", new object[]
			{
				sipRoutingHelper.GetType().Name
			});
			return sipRoutingHelper;
		}

		public static SipRoutingHelper CreateForOutbound(UMDialPlan dialPlan)
		{
			SipRoutingHelper sipRoutingHelper = (CommonConstants.UseDataCenterCallRouting && dialPlan.URIType == UMUriType.SipName) ? SipRoutingHelper.MsOrganizationRoutingHelper.Create() : SipRoutingHelper.DefaultRoutingHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "CreateForOutbound {0}", new object[]
			{
				sipRoutingHelper.GetType().Name
			});
			return sipRoutingHelper;
		}

		public static bool UseGlobalAccessProxyForOutbound(UMDialPlan dialPlan)
		{
			return CommonConstants.UseDataCenterCallRouting && dialPlan.URIType == UMUriType.SipName;
		}

		public static bool UseGlobalSBCSettingsForOutbound(UMIPGateway gw)
		{
			return CommonConstants.UseDataCenterCallRouting && gw.GlobalCallRoutingScheme != UMGlobalCallRoutingScheme.E164;
		}

		public static string GetCrossSiteRedirectTargetFqdnAndPort(Server exchangeServer, bool useTls, out int port)
		{
			UMServer umserver = new UMServer(exchangeServer);
			string text;
			if (umserver.ExternalServiceFqdn != null)
			{
				text = umserver.ExternalServiceFqdn.ToString();
				port = Utils.GetRedirectPort(useTls);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "GetCrossSiteRedirectTargetFqdnAndPort -> load balancer {0}:{1}", new object[]
				{
					text,
					port
				});
			}
			else
			{
				text = Utils.TryGetRedirectTargetFqdnForServer(exchangeServer);
				port = Utils.GetRedirectPort(umserver.SipTcpListeningPort, umserver.SipTlsListeningPort, useTls);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "GetCrossSiteRedirectTargetFqdnAndPort -> direct server {0}:{1}", new object[]
				{
					text,
					port
				});
			}
			return text;
		}

		public static string[] ParseMsOrganizationParameter(string parameterValue, string requestUriUser, bool allowMultipleEntries)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "ParseMsOrganizationParameter: parameterValue:{0} requestUriUser:{1} allowMultipleEntries:{2}", new object[]
			{
				parameterValue,
				requestUriUser,
				allowMultipleEntries
			});
			string[] array = (parameterValue != null) ? parameterValue.Split(CommonConstants.MsOrganizationDomainSeparators, StringSplitOptions.RemoveEmptyEntries) : null;
			if (array == null || array.Length == 0)
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationRequired, null, new object[0]);
			}
			if (array.Length > 1 && !allowMultipleEntries)
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationCannotHaveMultipleEntries, "Value: {0}.", new object[]
				{
					parameterValue
				});
			}
			if (array.Length > 3)
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationHasTooManyEntries, "Value: {0}.", new object[]
				{
					parameterValue
				});
			}
			return array;
		}

		public static OrganizationId GetOrganizationIdFromTenantDomainList(string[] domainList)
		{
			string text;
			return SipRoutingHelper.GetOrganizationIdFromTenantDomainList(domainList, out text);
		}

		public static OrganizationId GetOrganizationIdFromTenantDomainList(string[] domainList, out string acceptedDomain)
		{
			OrganizationId organizationId = null;
			bool flag = false;
			acceptedDomain = null;
			foreach (string text in domainList)
			{
				if (!SmtpAddress.IsValidDomain(text))
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationHasInvalidDomain, "Value: {0}.", new object[]
					{
						text
					});
				}
				try
				{
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromAcceptedDomain(text);
					organizationId = iadsystemConfigurationLookup.GetOrganizationIdFromDomainName(text, out flag);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "DomainName:{0} -> OrganizationId:{1}", new object[]
					{
						text,
						organizationId
					});
					if (null != organizationId)
					{
						if (!flag)
						{
							UmGlobals.ExEvent.LogEvent(organizationId, UMEventLogConstants.Tuple_MsOrganizationNotAuthoritativeDomain, text, text);
							throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationDoesNotMatchAuthoritativeDomain, "Value: {0}.", new object[]
							{
								text
							});
						}
						acceptedDomain = text;
						break;
					}
				}
				catch (CannotResolveTenantNameException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "DomainName='{0}' could not be found: {1}", new object[]
					{
						text,
						ex
					});
				}
			}
			return organizationId;
		}

		public abstract int RedirectResponseCode { get; }

		public abstract bool SupportsMsOrganizationRouting { get; }

		public abstract string GetContactHeaderHost(string targetHost, out string msfeParameter);

		public abstract SipRoutingHelper.Context GetRoutingContext(string toUri, string fromUri, string diversionUri, PlatformSipUri requestUri);

		public SipRoutingHelper.Context GetRoutingContext(string subscriberUri, PlatformSipUri requestUri)
		{
			return this.GetRoutingContext(null, null, subscriberUri, requestUri);
		}

		public sealed class Context
		{
			public ADObjectId DialPlanId { get; set; }

			public UMAutoAttendant AutoAttendant { get; set; }

			public string SubscriberUri { get; set; }

			public ADRecipient Recipient { get; set; }

			public OrganizationId OrgId { get; set; }
		}

		internal class MsOrganizationRoutingHelper : SipRoutingHelper
		{
			public override int RedirectResponseCode
			{
				get
				{
					return 303;
				}
			}

			public override bool SupportsMsOrganizationRouting
			{
				get
				{
					return true;
				}
			}

			public static SipRoutingHelper Create()
			{
				return new SipRoutingHelper.MsOrganizationRoutingHelper();
			}

			public override string GetContactHeaderHost(string targetHost, out string msfeParameter)
			{
				if (!CommonConstants.UseDataCenterCallRouting)
				{
					msfeParameter = null;
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "In enterprise scenario, returning the targethost {0}", new object[]
					{
						targetHost
					});
					return targetHost;
				}
				X509Certificate2 umcertificate = CertificateUtils.UMCertificate;
				ExAssert.RetailAssert(null != umcertificate, "CertificateUtils.UMCertificate not set");
				msfeParameter = targetHost;
				string subjectFqdn = CertificateUtils.GetSubjectFqdn(umcertificate);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetContactHeaderHost(targetHost:{0}) ->  HostFqdn:{1} MsFe:{2}", new object[]
				{
					targetHost,
					subjectFqdn,
					msfeParameter
				});
				return subjectFqdn;
			}

			public override SipRoutingHelper.Context GetRoutingContext(string toUri, string fromUri, string diversionUri, PlatformSipUri requestUri)
			{
				SipRoutingHelper.Context result = null;
				bool flag = !string.IsNullOrEmpty(diversionUri);
				string text = requestUri.FindParameter("ms-organization");
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "MsOrganizationRoutingHelper.GetRoutingContext. Ms-organization:{0} IsDivertedCall:{1} UserParameter:{2}", new object[]
				{
					text,
					flag,
					requestUri.UserParameter
				});
				bool flag2 = flag || requestUri.UserParameter != UserParameter.Phone;
				OrganizationId organizationId;
				if (!CommonConstants.UseDataCenterCallRouting)
				{
					organizationId = OrganizationId.ForestWideOrgId;
				}
				else
				{
					string[] domainList = SipRoutingHelper.ParseMsOrganizationParameter(text, requestUri.User, flag2);
					organizationId = SipRoutingHelper.GetOrganizationIdFromTenantDomainList(domainList);
					if (null == organizationId)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationCannotFindTenant, "Value: {0}.", new object[]
						{
							text
						});
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "RequestUri:{0}", new object[]
				{
					requestUri
				});
				string uri = requestUri.ToString();
				try
				{
					if (flag2)
					{
						uri = (flag ? diversionUri : toUri);
						result = this.ResolveContextFromSubscriberCall(toUri, fromUri, diversionUri, organizationId);
					}
					else
					{
						if (requestUri.UserParameter != UserParameter.Phone || !Utils.IsUriValid(requestUri.User, UMUriType.E164))
						{
							throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.InvalidAccessProxyRequestUri, "Value: {0}.", new object[]
							{
								requestUri
							});
						}
						result = this.ResolveContextFromE164Call(requestUri, organizationId);
					}
				}
				catch (CallRejectedException error)
				{
					SipRoutingHelper.MsOrganizationRoutingHelper.LogUnableToResolveContextTenantEvent(organizationId, uri, flag2, error);
					throw;
				}
				return result;
			}

			private static void LogUnableToResolveContextTenantEvent(OrganizationId organizationId, string uri, bool isSubscriberOrVoiceMailCall, CallRejectedException error)
			{
				if (isSubscriberOrVoiceMailCall)
				{
					UmGlobals.ExEvent.LogEvent(organizationId, UMEventLogConstants.Tuple_OCSUserNotProvisioned, uri, CommonUtil.ToEventLogString(CallId.Id), CommonUtil.ToEventLogString(uri));
					return;
				}
				UmGlobals.ExEvent.LogEvent(organizationId, UMEventLogConstants.Tuple_DialPlanOrAutoAttendantNotProvisioned, uri, CommonUtil.ToEventLogString(CallId.Id), CommonUtil.ToEventLogString(uri));
			}

			private static ADRecipient GetRecipientByEumOrSipProxy(OrganizationId organizationId, string lookupUri)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "Looking up recipient by EUM prefix: {0}", new object[]
				{
					lookupUri
				});
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(organizationId, null);
				ADRecipient adrecipient = iadrecipientLookup.LookupByEumSipResourceIdentifierPrefix(lookupUri);
				if (adrecipient == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "Looking up recipient by SIP proxy: {0}", new object[]
					{
						lookupUri
					});
					adrecipient = iadrecipientLookup.LookupBySipExtension(lookupUri);
					if (adrecipient == null)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.CouldNotFindUserBySipUri, "User: {0}.", new object[]
						{
							lookupUri
						});
					}
				}
				PIIMessage data = PIIMessage.Create(PIIType._User, adrecipient.Id);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, data, "Found Recipient:_User Type:{0}", new object[]
				{
					adrecipient.RecipientType
				});
				return adrecipient;
			}

			private SipRoutingHelper.Context ResolveContextFromSubscriberCall(string toUri, string fromUri, string diversionUri, OrganizationId organizationId)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ResolveContextFromSubscriberCall({0})", new object[]
				{
					organizationId
				});
				string text;
				if (!string.IsNullOrEmpty(diversionUri))
				{
					text = diversionUri;
				}
				else
				{
					text = toUri;
					if (!string.Equals(text, fromUri, StringComparison.OrdinalIgnoreCase))
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.ToAndFromHeadersMustMatch, "To: {0}. From:{1}.", new object[]
						{
							toUri,
							fromUri
						});
					}
				}
				ADRecipient recipientByEumOrSipProxy = SipRoutingHelper.MsOrganizationRoutingHelper.GetRecipientByEumOrSipProxy(organizationId, text);
				if (recipientByEumOrSipProxy.RecipientType != RecipientType.UserMailbox && recipientByEumOrSipProxy.RecipientType != RecipientType.MailUser)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.CouldNotFindUserBySipUri, "User: {0}.", new object[]
					{
						text
					});
				}
				ADUser aduser = recipientByEumOrSipProxy as ADUser;
				if (recipientByEumOrSipProxy.RecipientType == RecipientType.MailUser || aduser == null || aduser.UMRecipientDialPlanId == null)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MailboxIsNotUMEnabled, "User: {0}. PrimarySmtpAddress: {1}.", new object[]
					{
						text,
						recipientByEumOrSipProxy.PrimarySmtpAddress
					});
				}
				return new SipRoutingHelper.Context
				{
					Recipient = recipientByEumOrSipProxy,
					SubscriberUri = text,
					DialPlanId = aduser.UMRecipientDialPlanId,
					OrgId = aduser.OrganizationId
				};
			}

			private SipRoutingHelper.Context ResolveContextFromE164Call(PlatformSipUri requestUri, OrganizationId organizationId)
			{
				SipRoutingHelper.Context context = null;
				string user = requestUri.User;
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ResolveContextFromE164Call. Org:{0} E164Number:{1}", new object[]
				{
					organizationId,
					user
				});
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(organizationId);
				UMDialPlan dialPlanFromPilotIdentifier = iadsystemConfigurationLookup.GetDialPlanFromPilotIdentifier(user);
				if (dialPlanFromPilotIdentifier != null)
				{
					context = new SipRoutingHelper.Context();
					context.DialPlanId = dialPlanFromPilotIdentifier.Id;
					context.OrgId = dialPlanFromPilotIdentifier.OrganizationId;
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found DP:{0}", new object[]
					{
						dialPlanFromPilotIdentifier.Id
					});
				}
				else
				{
					UMAutoAttendant autoAttendantWithNoDialplanInformation = iadsystemConfigurationLookup.GetAutoAttendantWithNoDialplanInformation(user);
					if (autoAttendantWithNoDialplanInformation != null)
					{
						context = new SipRoutingHelper.Context();
						context.DialPlanId = autoAttendantWithNoDialplanInformation.UMDialPlan;
						context.AutoAttendant = autoAttendantWithNoDialplanInformation;
						context.OrgId = autoAttendantWithNoDialplanInformation.OrganizationId;
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found AA:{0}, DP:{1}", new object[]
						{
							autoAttendantWithNoDialplanInformation.Id,
							autoAttendantWithNoDialplanInformation.UMDialPlan
						});
					}
				}
				if (context == null)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.NoObjectFoundForE164Number, "Value: {0}.", new object[]
					{
						user
					});
				}
				return context;
			}
		}

		internal class DefaultRoutingHelper : SipRoutingHelper
		{
			public override int RedirectResponseCode
			{
				get
				{
					return 302;
				}
			}

			public override bool SupportsMsOrganizationRouting
			{
				get
				{
					return false;
				}
			}

			public static SipRoutingHelper Create()
			{
				return new SipRoutingHelper.DefaultRoutingHelper();
			}

			public override string GetContactHeaderHost(string targetHost, out string msfeParameter)
			{
				msfeParameter = null;
				return targetHost;
			}

			public override SipRoutingHelper.Context GetRoutingContext(string toUri, string fromUri, string diversionUri, PlatformSipUri requestUri)
			{
				return null;
			}
		}
	}
}
