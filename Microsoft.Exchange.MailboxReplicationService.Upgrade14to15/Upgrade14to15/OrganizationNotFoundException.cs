using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OrganizationNotFoundException : MigrationPermanentException
	{
		public OrganizationNotFoundException(string org) : base(UpgradeHandlerStrings.OrganizationNotFound(org))
		{
			this.org = org;
		}

		public OrganizationNotFoundException(string org, Exception innerException) : base(UpgradeHandlerStrings.OrganizationNotFound(org), innerException)
		{
			this.org = org;
		}

		protected OrganizationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.org = (string)info.GetValue("org", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("org", this.org);
		}

		public string Org
		{
			get
			{
				return this.org;
			}
		}

		private readonly string org;
	}
}
