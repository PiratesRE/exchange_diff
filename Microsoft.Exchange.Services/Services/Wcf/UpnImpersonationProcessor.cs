using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class UpnImpersonationProcessor : ImpersonationProcessorBase
	{
		internal UpnImpersonationProcessor(string impersonatedUPN, AuthZClientInfo impersonatingClientInfo, IIdentity impersonatingIdentity) : base(impersonatingClientInfo, impersonatingIdentity)
		{
			this.impersonatedUPN = impersonatedUPN;
			if (string.IsNullOrEmpty(this.impersonatedUPN) || !SmtpAddress.IsValidSmtpAddress(this.impersonatedUPN))
			{
				throw this.CreateUserIdentitySearchFailedException(null);
			}
			ADSessionSettings sessionSettings = Directory.SessionSettingsFromAddress(this.impersonatedUPN);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 54, ".ctor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\wcf\\HeaderProcessors\\UpnImpersonationProcessor.cs");
			ADRawEntry[] array = tenantOrRootOrgRecipientSession.FindRecipient(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, this.impersonatedUPN), null, 1, UpnImpersonationProcessor.PropertySet);
			if (array.Length == 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "UpnImpersonationProcessor.CreateADRecipientSessionContext. Failed to look up user by UPN {0}.", this.impersonatedUPN);
				throw this.CreateUserIdentitySearchFailedException(null);
			}
			this.impersonatedSmtpAddress = ((SmtpAddress)array[0][ADRecipientSchema.PrimarySmtpAddress]).ToString();
			this.impersonatedSid = (SecurityIdentifier)array[0][ADMailboxRecipientSchema.Sid];
			if (string.IsNullOrEmpty(this.impersonatedSmtpAddress) && this.impersonatedSid == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "UpnImpersonationProcessor.CreateADRecipientSessionContext. Failed to look up user by UPN {0}.", this.impersonatedUPN);
				throw this.CreateUserIdentitySearchFailedException(null);
			}
		}

		protected override ADRecipientSessionContext CreateADRecipientSessionContext()
		{
			ADRecipientSessionContext result;
			try
			{
				if (!string.IsNullOrEmpty(this.impersonatedSmtpAddress))
				{
					result = ADRecipientSessionContext.CreateFromSmtpAddress(this.impersonatedSmtpAddress);
				}
				else
				{
					result = ADRecipientSessionContext.CreateFromSidInRootOrg(this.impersonatedSid);
				}
			}
			catch (ADConfigurationException innerException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "UpnImpersonationProcessor.Ctor. Failed to look up user by UPN {0}.", this.impersonatedUPN);
				throw this.CreateUserIdentitySearchFailedException(innerException);
			}
			return result;
		}

		protected override UserIdentity GetImpersonatedUserIdentity(ADRecipientSessionContext adRecipientSessionContext)
		{
			if (!string.IsNullOrEmpty(this.impersonatedSmtpAddress))
			{
				return ADIdentityInformationCache.Singleton.GetUserIdentity(this.impersonatedSmtpAddress, adRecipientSessionContext);
			}
			return ADIdentityInformationCache.Singleton.GetUserIdentity(this.impersonatedSid, adRecipientSessionContext);
		}

		protected override ServicePermanentException CreateUserIdentitySearchFailedException(Exception innerException)
		{
			if (innerException != null)
			{
				return new InvalidUserPrincipalNameException(innerException);
			}
			return new InvalidUserPrincipalNameException();
		}

		private readonly string impersonatedUPN;

		private readonly string impersonatedSmtpAddress;

		private readonly SecurityIdentifier impersonatedSid;

		private static readonly PropertyDefinition[] PropertySet = new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress,
			ADMailboxRecipientSchema.Sid
		};
	}
}
