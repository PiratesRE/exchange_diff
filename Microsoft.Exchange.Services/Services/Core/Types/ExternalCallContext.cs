using System;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalCallContext : CallContext
	{
		internal Offer Offer
		{
			get
			{
				return this.offer;
			}
		}

		internal SmtpAddress ExternalId
		{
			get
			{
				return this.externalId;
			}
		}

		internal SmtpAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public override string ToString()
		{
			return "External call context for " + this.emailAddress;
		}

		internal ExternalCallContext(MessageHeaderProcessor headerProcessor, Message request, ExternalIdentity externalIdentity, UserWorkloadManager workloadManager)
		{
			this.emailAddress = externalIdentity.EmailAddress;
			this.offer = externalIdentity.Offer;
			this.externalId = externalIdentity.ExternalId;
			this.wsSecurityHeader = externalIdentity.WSSecurityHeader;
			this.sharingSecurityHeader = externalIdentity.SharingSecurityHeader;
			if (headerProcessor.SeeksProxyingOrS2S(request))
			{
				throw FaultExceptionUtilities.CreateFault(new ImpersonationFailedException(null), FaultParty.Sender);
			}
			this.availabilityProxyRequestType = headerProcessor.ProcessRequestTypeHeader(request);
			this.userKind = CallContext.UserKind.External;
			headerProcessor.ProcessMailboxCultureHeader(request);
			this.clientCulture = EWSSettings.ClientCulture;
			this.serverCulture = EWSSettings.ServerCulture;
			try
			{
				headerProcessor.ProcessTimeZoneContextHeader(request);
			}
			catch (LocalizedException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			headerProcessor.ProcessDateTimePrecisionHeader(request);
			this.sessionCache = new SessionCache(null, this);
			this.workloadManager = workloadManager;
			this.authZBehavior = AuthZBehavior.DefaultBehavior;
			this.requestedLogonType = RequestedLogonType.Default;
			this.adRecipientSessionContext = this.CreateADRecipientSessionContext();
			base.MethodName = CallContext.GetMethodName(request);
			HttpContext.Current.Items["CallContext"] = this;
		}

		internal WSSecurityHeader WSSecurityHeader
		{
			get
			{
				return this.wsSecurityHeader;
			}
		}

		internal SharingSecurityHeader SharingSecurityHeader
		{
			get
			{
				return this.sharingSecurityHeader;
			}
		}

		private ADRecipientSessionContext CreateADRecipientSessionContext()
		{
			string text = base.HttpContext.Request.Headers[WellKnownHeader.AnchorMailbox];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<SmtpAddress>(0L, "AnchorMailbox header not passed by {0} client", this.emailAddress);
				return ADRecipientSessionContext.CreateForRootOrganization();
			}
			return ADRecipientSessionContext.CreateFromSmtpAddress(text);
		}

		private Offer offer;

		private SmtpAddress emailAddress;

		private SmtpAddress externalId;

		private WSSecurityHeader wsSecurityHeader;

		private SharingSecurityHeader sharingSecurityHeader;
	}
}
