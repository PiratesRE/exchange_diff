using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class ADIdentityInformation
	{
		public abstract string SmtpAddress { get; }

		public abstract SecurityIdentifier Sid { get; }

		public abstract Guid ObjectGuid { get; }

		public abstract OrganizationId OrganizationId { get; }

		public string OrganizationPrefix
		{
			get
			{
				if (this.OrganizationId.OrganizationalUnit != null)
				{
					return this.OrganizationId.OrganizationalUnit.Name;
				}
				return string.Empty;
			}
		}

		public abstract IRecipientSession GetADRecipientSession();

		public abstract IRecipientSession GetGALScopedADRecipientSession(ClientSecurityContext clientSecurityContext);
	}
}
