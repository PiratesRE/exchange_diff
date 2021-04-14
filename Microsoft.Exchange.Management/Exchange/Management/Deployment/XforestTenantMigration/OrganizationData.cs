using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Serializable]
	public sealed class OrganizationData : IConfigurable
	{
		public string Name { get; set; }

		public DirectoryObjectCollection OrganizationalUnit { get; set; }

		public DirectoryObjectCollection ConfigurationUnit { get; set; }

		internal string RootOrgName { get; set; }

		internal string SourceFqdn { get; set; }

		internal string SourceADSite { get; set; }

		public OrganizationData()
		{
			this.Name = null;
			this.OrganizationalUnit = new DirectoryObjectCollection();
			this.ConfigurationUnit = new DirectoryObjectCollection();
		}

		public OrganizationData(string name, DirectoryObjectCollection orgUnit, DirectoryObjectCollection configUnit, string rootOrgName, string sourceFqdn, string sourceADSite)
		{
			this.Name = name;
			this.OrganizationalUnit = orgUnit;
			this.ConfigurationUnit = configUnit;
			this.RootOrgName = rootOrgName;
			this.SourceFqdn = sourceFqdn;
			this.SourceADSite = sourceADSite;
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return null;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
		}
	}
}
