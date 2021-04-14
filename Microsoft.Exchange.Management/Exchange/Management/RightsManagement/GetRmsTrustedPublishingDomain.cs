using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Get", "RMSTrustedPublishingDomain", DefaultParameterSetName = "OrganizationSet")]
	public sealed class GetRmsTrustedPublishingDomain : GetMultitenancySystemConfigurationObjectTask<RmsTrustedPublishingDomainIdParameter, RMSTrustedPublishingDomain>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "OrganizationSet")]
		public override OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RmsTrustedPublishingDomainIdParameter Identity
		{
			get
			{
				return (RmsTrustedPublishingDomainIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Default
		{
			get
			{
				return (SwitchParameter)(base.Fields["Default"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Default"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.Fields.IsModified("Default"))
				{
					return ADObject.BoolFilterBuilder(new ComparisonFilter(ComparisonOperator.Equal, RMSTrustedPublishingDomainSchema.Default, true), new BitMaskAndFilter(RMSTrustedPublishingDomainSchema.Flags, 1UL));
				}
				return base.InternalFilter;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
