using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SmtpAddressImpersonationProcessor : ImpersonationProcessorBase
	{
		internal SmtpAddressImpersonationProcessor(string impersonatedSmtpAddress, bool requirePrimarySmtpAddress, AuthZClientInfo impersonatingClientInfo, IIdentity impersonatingIdentity) : base(impersonatingClientInfo, impersonatingIdentity)
		{
			this.impersonatedSmtpAddress = impersonatedSmtpAddress;
			this.requirePrimarySmtpAddress = requirePrimarySmtpAddress;
			if (!SmtpAddress.IsValidSmtpAddress(this.impersonatedSmtpAddress))
			{
				throw this.CreateUserIdentitySearchFailedException(new InvalidSmtpAddressException());
			}
		}

		protected override ADRecipientSessionContext CreateADRecipientSessionContext()
		{
			ADRecipientSessionContext result;
			try
			{
				result = ADRecipientSessionContext.CreateFromSmtpAddress(this.impersonatedSmtpAddress);
			}
			catch (ADConfigurationException innerException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "SmtpAddressImpersonationProcessor.CreateADRecipientSessionContext. Failed to look up user by smtp address {0}", this.impersonatedSmtpAddress);
				throw this.CreateUserIdentitySearchFailedException(innerException);
			}
			return result;
		}

		protected override UserIdentity GetImpersonatedUserIdentity(ADRecipientSessionContext adRecipientSessionContext)
		{
			UserIdentity userIdentity = ADIdentityInformationCache.Singleton.GetUserIdentity(this.impersonatedSmtpAddress, adRecipientSessionContext);
			if (this.requirePrimarySmtpAddress)
			{
				SmtpAddress primarySmtpAddress = userIdentity.Recipient.PrimarySmtpAddress;
				if (!string.Equals(this.impersonatedSmtpAddress, userIdentity.Recipient.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					throw new NonPrimarySmtpAddressException(userIdentity.Recipient.PrimarySmtpAddress.ToString());
				}
			}
			return userIdentity;
		}

		protected override ServicePermanentException CreateUserIdentitySearchFailedException(Exception innerException)
		{
			return new NonExistentMailboxException((CoreResources.IDs)4088802584U, this.impersonatedSmtpAddress);
		}

		private readonly string impersonatedSmtpAddress;

		private readonly bool requirePrimarySmtpAddress;
	}
}
