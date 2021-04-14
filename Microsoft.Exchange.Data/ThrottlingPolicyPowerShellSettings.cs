using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyPowerShellSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyPowerShellSettings()
		{
		}

		private ThrottlingPolicyPowerShellSettings(string value) : base(value)
		{
			Unlimited<uint>? maxTenantConcurrency = this.MaxTenantConcurrency;
			Unlimited<uint>? maxOperations = this.MaxOperations;
			Unlimited<uint>? maxCmdletsTimePeriod = this.MaxCmdletsTimePeriod;
			Unlimited<uint>? maxCmdletQueueDepth = this.MaxCmdletQueueDepth;
			Unlimited<uint>? exchangeMaxCmdlets = this.ExchangeMaxCmdlets;
			Unlimited<uint>? maxDestructiveCmdlets = this.MaxDestructiveCmdlets;
			Unlimited<uint>? maxDestructiveCmdletsTimePeriod = this.MaxDestructiveCmdletsTimePeriod;
			Unlimited<uint>? maxCmdlets = this.MaxCmdlets;
			Unlimited<uint>? maxRunspaces = this.MaxRunspaces;
			Unlimited<uint>? maxTenantRunspaces = this.MaxTenantRunspaces;
			Unlimited<uint>? maxRunspacesTimePeriod = this.MaxRunspacesTimePeriod;
			Unlimited<uint>? pswsMaxConcurrency = this.PswsMaxConcurrency;
			Unlimited<uint>? pswsMaxRequest = this.PswsMaxRequest;
			Unlimited<uint>? pswsMaxRequestTimePeriod = this.PswsMaxRequestTimePeriod;
		}

		public static ThrottlingPolicyPowerShellSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyPowerShellSettings(stateToParse);
		}

		internal Unlimited<uint>? MaxTenantConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("TenantConcur");
			}
			set
			{
				base.SetValueInPropertyBag("TenantConcur", value);
			}
		}

		internal Unlimited<uint>? MaxOperations
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxOps");
			}
			set
			{
				base.SetValueInPropertyBag("MaxOps", value);
			}
		}

		internal Unlimited<uint>? MaxCmdletsTimePeriod
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxCmdPeriod");
			}
			set
			{
				base.SetValueInPropertyBag("MaxCmdPeriod", value);
			}
		}

		internal Unlimited<uint>? MaxCmdletQueueDepth
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxCmdQueue");
			}
			set
			{
				base.SetValueInPropertyBag("MaxCmdQueue", value);
			}
		}

		internal Unlimited<uint>? ExchangeMaxCmdlets
		{
			get
			{
				return base.GetValueFromPropertyBag("ExMaxCmd");
			}
			set
			{
				base.SetValueInPropertyBag("ExMaxCmd", value);
			}
		}

		internal Unlimited<uint>? MaxDestructiveCmdlets
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxDestruct");
			}
			set
			{
				base.SetValueInPropertyBag("MaxDestruct", value);
			}
		}

		internal Unlimited<uint>? MaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxDestructPeriod");
			}
			set
			{
				base.SetValueInPropertyBag("MaxDestructPeriod", value);
			}
		}

		internal Unlimited<uint>? MaxCmdlets
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxCmdlet");
			}
			set
			{
				base.SetValueInPropertyBag("MaxCmdlet", value);
			}
		}

		internal Unlimited<uint>? MaxRunspaces
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxRun");
			}
			set
			{
				base.SetValueInPropertyBag("MaxRun", value);
			}
		}

		internal Unlimited<uint>? MaxTenantRunspaces
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxTenantRun");
			}
			set
			{
				base.SetValueInPropertyBag("MaxTenantRun", value);
			}
		}

		internal Unlimited<uint>? MaxRunspacesTimePeriod
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxRunPeriod");
			}
			set
			{
				base.SetValueInPropertyBag("MaxRunPeriod", value);
			}
		}

		internal Unlimited<uint>? PswsMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("PswsMaxConn");
			}
			set
			{
				base.SetValueInPropertyBag("PswsMaxConn", value);
			}
		}

		internal Unlimited<uint>? PswsMaxRequest
		{
			get
			{
				return base.GetValueFromPropertyBag("PswsMaxRequest");
			}
			set
			{
				base.SetValueInPropertyBag("PswsMaxRequest", value);
			}
		}

		internal Unlimited<uint>? PswsMaxRequestTimePeriod
		{
			get
			{
				return base.GetValueFromPropertyBag("PswsMaxRequestPeriod");
			}
			set
			{
				base.SetValueInPropertyBag("PswsMaxRequestPeriod", value);
			}
		}

		private const string TenantConcurrencyPrefix = "TenantConcur";

		private const string MaxOperationsPrefix = "MaxOps";

		private const string MaxCmdletsTimePeriodPrefix = "MaxCmdPeriod";

		private const string ExchangeMaxCmdletsPrefix = "ExMaxCmd";

		private const string MaxCmdletQueueDepthPrefix = "MaxCmdQueue";

		private const string MaxDestructiveCmdletsPrefix = "MaxDestruct";

		private const string MaxDestructiveCmdletsTimePeriodPrefix = "MaxDestructPeriod";

		private const string MaxCmdletsPrefix = "MaxCmdlet";

		private const string MaxRunspacesPrefix = "MaxRun";

		private const string MaxTenantRunspacesPrefix = "MaxTenantRun";

		private const string MaxRunspacesTimePeriodPrefix = "MaxRunPeriod";

		private const string PswsMaxConcurrencyPrefix = "PswsMaxConn";

		private const string PswsMaxRequestPrefix = "PswsMaxRequest";

		private const string PswsMaxRequestTimePeriodPrefix = "PswsMaxRequestPeriod";
	}
}
