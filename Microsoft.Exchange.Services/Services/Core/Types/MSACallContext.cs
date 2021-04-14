using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class MSACallContext : CallContext
	{
		internal string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		internal override string EffectiveCallerNetId
		{
			get
			{
				return this.effectiveCallerNetId;
			}
		}

		internal MSACallContext(HttpContext httpContext, AppWideStoreSessionCache mailboxSessionCache, AcceptedDomainCache acceptedDomainCache, MSAIdentity msaIdentity, UserWorkloadManager workloadManager, CultureInfo clientCulture) : this(httpContext, mailboxSessionCache, acceptedDomainCache, msaIdentity, workloadManager, clientCulture, false)
		{
		}

		internal MSACallContext(HttpContext httpContext, AppWideStoreSessionCache mailboxSessionCache, AcceptedDomainCache acceptedDomainCache, MSAIdentity msaIdentity, UserWorkloadManager workloadManager, CultureInfo clientCulture, bool isMock) : base(httpContext, EwsOperationContextBase.Current, RequestDetailsLogger.Current, isMock)
		{
			this.memberName = msaIdentity.MemberName;
			this.userKind = CallContext.UserKind.MSA;
			this.effectiveCallerNetId = msaIdentity.NetId;
			this.clientCulture = clientCulture;
			this.sessionCache = new SessionCache(mailboxSessionCache, this);
			this.workloadManager = workloadManager;
			this.acceptedDomainCache = acceptedDomainCache;
			this.serverCulture = EWSSettings.ServerCulture;
			this.userAgent = httpContext.Request.UserAgent;
			this.requestedLogonType = RequestedLogonType.Default;
		}

		public override string ToString()
		{
			return "MSA call context for " + this.memberName;
		}

		private readonly string memberName;

		private readonly string effectiveCallerNetId;
	}
}
