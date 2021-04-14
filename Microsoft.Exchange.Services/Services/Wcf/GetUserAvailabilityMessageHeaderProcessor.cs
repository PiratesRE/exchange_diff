using System;
using System.Security.Principal;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetUserAvailabilityMessageHeaderProcessor : MessageHeaderProcessor
	{
		protected GetUserAvailabilityMessageHeaderProcessor()
		{
		}

		public new static GetUserAvailabilityMessageHeaderProcessor GetInstance()
		{
			if (GetUserAvailabilityMessageHeaderProcessor.singletonInstance == null)
			{
				GetUserAvailabilityMessageHeaderProcessor.singletonInstance = new GetUserAvailabilityMessageHeaderProcessor();
			}
			return GetUserAvailabilityMessageHeaderProcessor.singletonInstance;
		}

		internal override void ValidateRights(CallContext callContext, AuthZClientInfo callerClientInfo, Message request)
		{
			try
			{
				if (callContext.MailboxAccessType == MailboxAccessType.ServerToServer)
				{
					if (S2SRightsWrapper.AllowsTokenSerializationBy(callerClientInfo.ClientSecurityContext))
					{
						MessageHeaderProcessor.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "This request has original requester's context in the SOAP header and the caller has privileges to serialize token.");
					}
					else
					{
						this.ValidateServerToServerRequest(callContext, callerClientInfo);
					}
				}
				else
				{
					this.ValidateNormalOrImpersonationRequest(callContext, callerClientInfo, request);
				}
			}
			catch (LocalizedException exception)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(exception, FaultParty.Sender);
			}
		}

		private void ValidateServerToServerRequest(CallContext callContext, AuthZClientInfo callerClientInfo)
		{
			if (callContext.AvailabilityProxyRequestType == null)
			{
				MessageHeaderProcessor.SecurityTracer.TraceError((long)this.GetHashCode(), "Client context header found but no request type found in Soap header.");
				throw new ProxyRequestNotAllowedException(CoreResources.descNoRequestType);
			}
			SecurityIdentifier userSid = callerClientInfo.ClientSecurityContext.UserSid;
			if (callContext.AvailabilityProxyRequestType.Value == ProxyRequestType.CrossForest && LocalForestConfiguration.HasPerUserAccess(OrganizationId.ForestWideOrgId, callerClientInfo.ClientSecurityContext))
			{
				MessageHeaderProcessor.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "The request has original requester's context in the SOAP header and the caller was found to have per-user access. Accepting this request.");
				return;
			}
			MessageHeaderProcessor.CalendarViewTracer.TraceError((long)this.GetHashCode(), "Caller doesn't have enough privileges to issue this request.");
			AvailabilityGlobals.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_WebRequestFailedSecurityChecks, userSid.ToString(), new object[]
			{
				Globals.ProcessId,
				userSid
			});
			throw new ProxyRequestNotAllowedException(CoreResources.descNotEnoughPrivileges);
		}

		private void ValidateNormalOrImpersonationRequest(CallContext callContext, AuthZClientInfo callerClientInfo, Message request)
		{
			if (callContext.AvailabilityProxyRequestType == null)
			{
				MessageHeaderProcessor.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "No headers found in the request. This request is being processed as a direct client request.");
				return;
			}
			OrganizationId organizationId = null;
			if (callContext.AccessingADUser != null)
			{
				organizationId = callContext.AccessingADUser.OrganizationId;
			}
			if (organizationId == null)
			{
				MessageHeaderProcessor.ConfigurationTracer.TraceError<CallContext, SecurityIdentifier>((long)this.GetHashCode(), "Unable to retrieve the OrganizationId for the caller - {0}, SID - {1}.", callContext, callerClientInfo.ClientSecurityContext.UserSid);
				AvailabilityGlobals.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_WebRequestFailedSecurityChecks, callContext.ToString(), new object[]
				{
					Globals.ProcessId,
					callerClientInfo.ClientSecurityContext.UserSid
				});
				throw new ProxyRequestNotAllowedException(CoreResources.descNotEnoughPrivileges);
			}
			if (callContext.AvailabilityProxyRequestType.Value != ProxyRequestType.CrossForest || callContext.MailboxAccessType != MailboxAccessType.Normal)
			{
				MessageHeaderProcessor.CalendarViewTracer.TraceError<ProxyRequestType, MailboxAccessType>((long)this.GetHashCode(), "Invalid request type: {0}, mailbox access type: {1}", callContext.AvailabilityProxyRequestType.Value, callContext.MailboxAccessType);
				throw new ProxyRequestNotAllowedException(CoreResources.descInvalidRequestType);
			}
			SecurityIdentifier userSid = callerClientInfo.ClientSecurityContext.UserSid;
			if (LocalForestConfiguration.HasOrgWideAccess(organizationId, callerClientInfo.ClientSecurityContext) || LocalForestConfiguration.HasPerUserAccess(organizationId, callerClientInfo.ClientSecurityContext))
			{
				request.Properties["DefaultFreeBusyAccessOnly"] = true;
				return;
			}
			MessageHeaderProcessor.ConfigurationTracer.TraceError((long)this.GetHashCode(), "Caller doesn't have enough privileges to issue this request.");
			AvailabilityGlobals.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_WebRequestFailedSecurityChecks, userSid.ToString(), new object[]
			{
				Globals.ProcessId,
				userSid
			});
			throw new ProxyRequestNotAllowedException(CoreResources.descNotEnoughPrivileges);
		}

		public const string DefaultFreeBusyAccessOnly = "DefaultFreeBusyAccessOnly";

		private static GetUserAvailabilityMessageHeaderProcessor singletonInstance;
	}
}
