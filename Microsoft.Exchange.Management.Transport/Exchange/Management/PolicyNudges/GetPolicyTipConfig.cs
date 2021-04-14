using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[Cmdlet("Get", "PolicyTipConfig", DefaultParameterSetName = "Identity")]
	public sealed class GetPolicyTipConfig : GetMultitenancySystemConfigurationObjectTask<PolicyTipConfigIdParameter, PolicyTipMessageConfig>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Paramters")]
		public SwitchParameter Original
		{
			get
			{
				return (SwitchParameter)(base.Fields["Original"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Original"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Paramters")]
		public CultureInfo Locale
		{
			get
			{
				return (CultureInfo)base.Fields["Locale"];
			}
			set
			{
				base.Fields["Locale"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Paramters")]
		public PolicyTipMessageConfigAction Action
		{
			get
			{
				return (PolicyTipMessageConfigAction)base.Fields["Action"];
			}
			set
			{
				base.Fields["Action"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return null;
				}
				ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
				return orgContainerId.GetDescendantId(PolicyTipMessageConfig.PolicyTipMessageConfigContainer);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					base.Fields.IsModified("Action") ? new ComparisonFilter(ComparisonOperator.Equal, PolicyTipMessageConfigSchema.Action, this.Action) : null,
					(this.Locale != null) ? new ComparisonFilter(ComparisonOperator.Equal, PolicyTipMessageConfigSchema.Locale, this.Locale.Name) : null
				});
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.Fields.IsModified("Action") && this.Action == PolicyTipMessageConfigAction.Url)
			{
				base.WriteError(new GetPolicyTipConfigUrlUsedAsActionFilterException(), ErrorCategory.InvalidArgument, null);
			}
			HashSet<int> expectedCultureLcids = LanguagePackInfo.expectedCultureLcids;
			if (this.Locale != null && !expectedCultureLcids.Contains(this.Locale.LCID))
			{
				string locales = string.Join(", ", from lcid in expectedCultureLcids
				select new CultureInfo(lcid).Name);
				base.WriteError(new NewPolicyTipConfigInvalidLocaleException(locales), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.Original)
			{
				IEnumerable<PolicyTipMessageConfig> enumerable = PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.builtInConfigs.Value;
				if (this.Locale != null)
				{
					enumerable = from c in enumerable
					where c.Locale == this.Locale.Name
					select c;
				}
				if (base.Fields.IsModified("Action"))
				{
					enumerable = from c in enumerable
					where c.Action == this.Action
					select c;
				}
				this.WriteResult<PolicyTipMessageConfig>(enumerable);
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private const string ParametersSetName = "Paramters";
	}
}
