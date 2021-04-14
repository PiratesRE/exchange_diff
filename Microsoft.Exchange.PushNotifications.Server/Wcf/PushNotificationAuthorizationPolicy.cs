using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	public class PushNotificationAuthorizationPolicy : IAuthorizationPolicy, IAuthorizationComponent
	{
		public string Id
		{
			get
			{
				return PushNotificationAuthorizationPolicy.PolicyId;
			}
		}

		public ClaimSet Issuer
		{
			get
			{
				return ClaimSet.System;
			}
		}

		public bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			IIdentity clientIdentity = this.GetClientIdentity(evaluationContext);
			IPrincipal principal;
			if (clientIdentity != null)
			{
				principal = new ServicePrincipal(clientIdentity, ExTraceGlobals.PushNotificationServiceTracer);
			}
			else
			{
				principal = OperationContext.Current.GetPrincipal();
			}
			if (principal == null)
			{
				throw new SecurityAccessDeniedException();
			}
			evaluationContext.Properties["Principal"] = principal;
			return true;
		}

		private IIdentity GetClientIdentity(EvaluationContext evaluationContext)
		{
			object obj;
			if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
			{
				return null;
			}
			IList<IIdentity> list = obj as IList<IIdentity>;
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			if (list.Count > 1 && ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceWarning<string, string>((long)this.GetHashCode(), "Request has multiple identities. Identity {0} will be used. Other identities {1}", list[0].Name, string.Join<IIdentity>(",", list));
			}
			return list[0];
		}

		private static readonly string PolicyId = typeof(PushNotificationAuthorizationPolicy).FullName;
	}
}
