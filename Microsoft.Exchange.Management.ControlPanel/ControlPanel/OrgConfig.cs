using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class OrgConfig : BaseRow
	{
		public OrgConfig(OrganizationConfig organizationConfig) : base(organizationConfig)
		{
			if (organizationConfig == null)
			{
				throw new ArgumentNullException("organizationConfig");
			}
			this.OriginalOrganizationConfig = organizationConfig;
		}

		public OrganizationConfig OriginalOrganizationConfig { get; private set; }

		[DataMember]
		public string[] GroupNamingPolicyPrefixElements
		{
			get
			{
				return this.OriginalOrganizationConfig.DistributionGroupNamingPolicy.PrefixElements;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string[] GroupNamingPolicySuffixElements
		{
			get
			{
				return this.OriginalOrganizationConfig.DistributionGroupNamingPolicy.SuffixElements;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<string> DistributionGroupNameBlockedWordsList
		{
			get
			{
				return this.OriginalOrganizationConfig.DistributionGroupNameBlockedWordsList.ToStringArray<string>();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
