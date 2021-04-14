using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOrganizationVersionException : MigrationPermanentException
	{
		public InvalidOrganizationVersionException(string org, ExchangeObjectVersion version) : base(UpgradeHandlerStrings.InvalidOrganizationVersion(org, version))
		{
			this.org = org;
			this.version = version;
		}

		public InvalidOrganizationVersionException(string org, ExchangeObjectVersion version, Exception innerException) : base(UpgradeHandlerStrings.InvalidOrganizationVersion(org, version), innerException)
		{
			this.org = org;
			this.version = version;
		}

		protected InvalidOrganizationVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.org = (string)info.GetValue("org", typeof(string));
			this.version = (ExchangeObjectVersion)info.GetValue("version", typeof(ExchangeObjectVersion));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("org", this.org);
			info.AddValue("version", this.version);
		}

		public string Org
		{
			get
			{
				return this.org;
			}
		}

		public ExchangeObjectVersion Version
		{
			get
			{
				return this.version;
			}
		}

		private readonly string org;

		private readonly ExchangeObjectVersion version;
	}
}
