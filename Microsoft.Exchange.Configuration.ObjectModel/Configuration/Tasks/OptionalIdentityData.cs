using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class OptionalIdentityData : ICloneable
	{
		public OptionalIdentityData Clone()
		{
			return (OptionalIdentityData)base.MemberwiseClone();
		}

		public QueryFilter AdditionalFilter { get; set; }

		public ADObjectId ConfigurationContainerRdn
		{
			get
			{
				return this.configurationContainerRdn;
			}
			set
			{
				this.configurationContainerRdn = value;
			}
		}

		public ADObjectId RootOrgDomainContainerId
		{
			get
			{
				return this.rootOrgDomainContainerId;
			}
			set
			{
				this.rootOrgDomainContainerId = value;
			}
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		private ADObjectId configurationContainerRdn;

		private ADObjectId rootOrgDomainContainerId;
	}
}
