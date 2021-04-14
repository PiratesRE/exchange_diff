using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ComputerIdentity : ADIdentityInformation
	{
		public ComputerIdentity(ADComputer computer)
		{
			this.computer = computer;
		}

		public ADComputer Computer
		{
			get
			{
				return this.computer;
			}
		}

		public override string SmtpAddress
		{
			get
			{
				return null;
			}
		}

		public override OrganizationId OrganizationId
		{
			get
			{
				return OrganizationId.ForestWideOrgId;
			}
		}

		public override SecurityIdentifier Sid
		{
			get
			{
				return this.computer.Sid;
			}
		}

		public override Guid ObjectGuid
		{
			get
			{
				return this.computer.Id.ObjectGuid;
			}
		}

		public override IRecipientSession GetADRecipientSession()
		{
			return Directory.CreateRootADRecipientSession();
		}

		public override IRecipientSession GetGALScopedADRecipientSession(ClientSecurityContext clientSecurityContext)
		{
			return Directory.CreateRootADRecipientSession();
		}

		private ADComputer computer;
	}
}
