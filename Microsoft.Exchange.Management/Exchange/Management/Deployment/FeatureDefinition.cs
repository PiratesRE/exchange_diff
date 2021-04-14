using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class FeatureDefinition : PropertyDefinition
	{
		public FeatureDefinition(string name, Type type, ServicePlanSkus servicePlanSkus, params FeatureCategory[] categories) : base(name, type)
		{
			this.categories = categories.ToList<FeatureCategory>();
			this.servicePlanSkus = servicePlanSkus;
		}

		public FeatureDefinition(string name, FeatureCategory category, Type type, ServicePlanSkus servicePlanSkus) : this(name, type, servicePlanSkus, new FeatureCategory[]
		{
			category
		})
		{
		}

		public List<FeatureCategory> Categories
		{
			get
			{
				return this.categories;
			}
			set
			{
				this.categories = value;
			}
		}

		public ServicePlanSkus ServicePlanSkus
		{
			get
			{
				return this.servicePlanSkus;
			}
			set
			{
				this.servicePlanSkus = value;
			}
		}

		public object DefaultValue
		{
			get
			{
				if (base.Type == typeof(bool))
				{
					return false;
				}
				if (base.Type == typeof(string))
				{
					return null;
				}
				if (base.Type.BaseType == typeof(Enum))
				{
					return Enum.GetValues(base.Type).GetValue(0);
				}
				if (base.Type == typeof(Unlimited<int>))
				{
					return new Unlimited<int>(0);
				}
				throw new NotImplementedException();
			}
		}

		public bool IsValueEqual(object a, object b)
		{
			return object.Equals(a, b);
		}

		private List<FeatureCategory> categories;

		private ServicePlanSkus servicePlanSkus;
	}
}
