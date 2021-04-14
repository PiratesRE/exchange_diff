using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SidImpersonationProcessor : ImpersonationProcessorBase
	{
		internal SidImpersonationProcessor(string impersonatedSidString, AuthZClientInfo impersonatingClientInfo, IIdentity impersonatingIdentity) : base(impersonatingClientInfo, impersonatingIdentity)
		{
			this.impersonatedSidString = impersonatedSidString;
			try
			{
				this.impersonatedSid = ADIdentityInformationCache.CreateSid(this.impersonatedSidString);
			}
			catch (InvalidSidException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, InvalidSidException>(0L, "SidImpersonationProcessor.Ctor. InvalidSidException encountered.  Sid: {0}, Exception {1}", this.impersonatedSidString, ex);
				throw this.CreateUserIdentitySearchFailedException(ex);
			}
		}

		protected override ADRecipientSessionContext CreateADRecipientSessionContext()
		{
			ADRecipientSessionContext result;
			try
			{
				result = ADRecipientSessionContext.CreateFromSidInRootOrg(this.impersonatedSid);
			}
			catch (ADConfigurationException innerException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "SidImpersonationProcessor.CreateADRecipientSessionContext. Failed to look up user by SID {0}", this.impersonatedSidString);
				throw this.CreateUserIdentitySearchFailedException(innerException);
			}
			return result;
		}

		protected override UserIdentity GetImpersonatedUserIdentity(ADRecipientSessionContext adRecipientSessionContext)
		{
			return ADIdentityInformationCache.Singleton.GetUserIdentity(this.impersonatedSid, adRecipientSessionContext);
		}

		protected override ServicePermanentException CreateUserIdentitySearchFailedException(Exception innerException)
		{
			return new InvalidUserSidException(innerException);
		}

		private readonly string impersonatedSidString;

		private SecurityIdentifier impersonatedSid;
	}
}
