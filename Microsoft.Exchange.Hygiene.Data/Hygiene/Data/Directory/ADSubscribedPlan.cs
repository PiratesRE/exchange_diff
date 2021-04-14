using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class ADSubscribedPlan : ADObject
	{
		internal ADObjectId AccountId
		{
			get
			{
				return this[ADSubscribedPlanSchema.AccountIdProperty] as ADObjectId;
			}
			set
			{
				this[ADSubscribedPlanSchema.AccountIdProperty] = value;
			}
		}

		internal string ServiceType
		{
			get
			{
				return this[ADSubscribedPlanSchema.ServiceTypeProperty] as string;
			}
			set
			{
				this[ADSubscribedPlanSchema.ServiceTypeProperty] = value;
			}
		}

		internal string Capability
		{
			get
			{
				return this[ADSubscribedPlanSchema.CapabilityProperty] as string;
			}
			set
			{
				this[ADSubscribedPlanSchema.CapabilityProperty] = value;
			}
		}

		internal string MaximumOverageUnitsDetail
		{
			get
			{
				return this[ADSubscribedPlanSchema.MaximumOverageUnitsDetailProperty] as string;
			}
			set
			{
				this[ADSubscribedPlanSchema.MaximumOverageUnitsDetailProperty] = value;
			}
		}

		internal string PrepaidUnitsDetail
		{
			get
			{
				return this[ADSubscribedPlanSchema.PrepaidUnitsDetailProperty] as string;
			}
			set
			{
				this[ADSubscribedPlanSchema.PrepaidUnitsDetailProperty] = value;
			}
		}

		internal string TotalTrialUnitsDetail
		{
			get
			{
				return this[ADSubscribedPlanSchema.TotalTrialUnitsDetailProperty] as string;
			}
			set
			{
				this[ADSubscribedPlanSchema.TotalTrialUnitsDetailProperty] = value;
			}
		}

		internal int EffectiveSeatCount
		{
			get
			{
				return this.PrepaidUnitsEnabled + this.PrepaidUnitsWarning + this.MaximumOverageUnitsEnabled + this.MaximumOverageUnitsWarning;
			}
		}

		internal bool IsTrialOnly
		{
			get
			{
				return this.TotalTrialUnitsDetail != null && (this.TotalTrialUnitsEnabled == this.MaximumOverageUnitsEnabled + this.PrepaidUnitsEnabled && this.TotalTrialUnitsWarning == this.MaximumOverageUnitsWarning + this.PrepaidUnitsWarning) && this.TotalTrialUnitsSuspended == this.MaximumOverageUnitsSuspended + this.PrepaidUnitsSuspended;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSubscribedPlan.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSubscribedPlan.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal int PrepaidUnitsEnabled
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.PrepaidUnitsDetail, "Enabled");
			}
		}

		internal int PrepaidUnitsWarning
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.PrepaidUnitsDetail, "Warning");
			}
		}

		internal int PrepaidUnitsSuspended
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.PrepaidUnitsDetail, "Suspended");
			}
		}

		internal int MaximumOverageUnitsEnabled
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.MaximumOverageUnitsDetail, "Enabled");
			}
		}

		internal int MaximumOverageUnitsWarning
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.MaximumOverageUnitsDetail, "Warning");
			}
		}

		internal int MaximumOverageUnitsSuspended
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.MaximumOverageUnitsDetail, "Suspended");
			}
		}

		internal int TotalTrialUnitsEnabled
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.TotalTrialUnitsDetail, "Enabled");
			}
		}

		internal int TotalTrialUnitsWarning
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.TotalTrialUnitsDetail, "Warning");
			}
		}

		internal int TotalTrialUnitsSuspended
		{
			get
			{
				return ADSubscribedPlan.GetPlanAttributeValue(this.TotalTrialUnitsDetail, "Suspended");
			}
		}

		private static int GetPlanAttributeValue(string plan, string attributeName)
		{
			if (!string.IsNullOrWhiteSpace(plan))
			{
				XElement xelement = XElement.Parse(plan);
				return Convert.ToInt32(xelement.Attribute(attributeName).Value);
			}
			return 0;
		}

		private const string EnabledAttribueName = "Enabled";

		private const string WarningAttribueName = "Warning";

		private const string SuspendedAttribueName = "Suspended";

		private static readonly ADSubscribedPlanSchema schema = ObjectSchema.GetInstance<ADSubscribedPlanSchema>();

		private static string mostDerivedClass = "ADSubscribedPlan";
	}
}
