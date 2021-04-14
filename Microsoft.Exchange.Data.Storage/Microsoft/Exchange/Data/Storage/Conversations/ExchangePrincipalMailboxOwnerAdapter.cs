using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangePrincipalMailboxOwnerAdapter : MailboxOwnerAdapter
	{
		public ExchangePrincipalMailboxOwnerAdapter(IExchangePrincipal principal, IConstraintProvider constraintProvider, RecipientTypeDetails recipientTypeDetails, LogonType logonType) : base(constraintProvider, recipientTypeDetails, logonType)
		{
			this.principal = principal;
		}

		protected override IGenericADUser CalculateGenericADUser()
		{
			return new GenericADUser(this.principal);
		}

		private readonly IExchangePrincipal principal;
	}
}
