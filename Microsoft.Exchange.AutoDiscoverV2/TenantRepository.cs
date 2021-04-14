using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Exchange.Autodiscover;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	[ExcludeFromCodeCoverage]
	internal class TenantRepository : ITenantRepository
	{
		public TenantRepository(RequestDetailsLogger logger)
		{
			this.logger = logger;
		}

		public ADRecipient GetOnPremUser(SmtpAddress emailAddress)
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 49, "GetOnPremUser", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\AutoDiscoverV2\\TenantRepository.cs");
			this.logger.AppendGenericInfo("GetOnPremUser", "Start Ad lookup");
			return recipientSession.FindByProxyAddress(new SmtpProxyAddress(emailAddress.Address, false));
		}

		public IAutodMiniRecipient GetNextUserFromSortedList(SmtpAddress emailAddress)
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 66, "GetNextUserFromSortedList", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\AutoDiscoverV2\\TenantRepository.cs");
			this.logger.AppendGenericInfo("GetOnPremUser", "Start Ad lookup");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.UserMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.MailUser)
				}),
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADUserSchema.UserPrincipalName, emailAddress.Address)
			});
			ADRawEntry[] array = recipientSession.Find(null, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName,
				ADRecipientSchema.ExternalEmailAddress
			});
			if (array != null)
			{
				ADRawEntry adrawEntry = array.FirstOrDefault<ADRawEntry>();
				if (adrawEntry != null)
				{
					return new AutodMiniRecipient(adrawEntry);
				}
			}
			return null;
		}

		private readonly RequestDetailsLogger logger;
	}
}
