using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Deployment
{
	public abstract class BooleanFeatureBag
	{
		protected virtual void InitializeDependencies()
		{
		}

		internal abstract ServicePlanElementSchema Schema { get; }

		protected BooleanFeatureBag()
		{
			this.propertyBag = new PropertyBag();
			this.dependencies = new List<DependencyEntry>();
			this.InitializeDependencies();
		}

		internal List<DependencyEntry> Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		internal PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal object this[FeatureDefinition featureDefinition]
		{
			get
			{
				return this.propertyBag[featureDefinition] ?? featureDefinition.DefaultValue;
			}
			set
			{
				this.propertyBag[featureDefinition] = value;
			}
		}

		public List<string> GetEnabledPermissionFeatures()
		{
			return this.GetEnabledFeatures(new FeatureCategory[]
			{
				FeatureCategory.MailboxPlanPermissions,
				FeatureCategory.AdminPermissions
			});
		}

		public List<string> GetEnabledRoleGroupRoleAssignmentFeatures()
		{
			return this.GetEnabledFeatures(new FeatureCategory[]
			{
				FeatureCategory.RoleGroupRoleAssignment
			});
		}

		public List<string> GetEnabledMailboxPlanPermissionsFeatures()
		{
			return this.GetEnabledFeatures(new FeatureCategory[]
			{
				FeatureCategory.MailboxPlanPermissions
			});
		}

		public List<string> GetEnabledMailboxPlanRoleAssignmentFeatures()
		{
			return this.GetEnabledFeatures(new FeatureCategory[]
			{
				FeatureCategory.MailboxPlanRoleAssignment
			});
		}

		private List<string> GetEnabledFeatures(params FeatureCategory[] features)
		{
			if (features == null)
			{
				throw new ArgumentNullException("Parameter features should not be null.");
			}
			List<string> list = new List<string>();
			list.Add("*");
			foreach (object obj in ((IEnumerable)this.Schema))
			{
				FeatureDefinition featureDefinition = (FeatureDefinition)obj;
				if (featureDefinition.Type.Equals(typeof(bool)) && (bool)this[featureDefinition])
				{
					if (featureDefinition.Categories.Any((FeatureCategory x) => features.Contains(x)))
					{
						list.Add(featureDefinition.Name);
					}
				}
			}
			return list;
		}

		internal List<ValidationError> ValidateDependencies(ServicePlan sp)
		{
			List<ValidationError> list = new List<ValidationError>();
			foreach (DependencyEntry dependencyEntry in this.dependencies)
			{
				bool flag = dependencyEntry.GetDependencyValue(sp);
				bool flag2 = dependencyEntry.GetFeatureValue();
				if (flag2 && !flag)
				{
					list.Add(new DependencyValidationError(dependencyEntry.FeatureName, flag2, dependencyEntry.DependencyFeatureName, flag));
				}
			}
			return list;
		}

		internal List<ValidationError> ValidateFeaturesAllowedForSKU()
		{
			List<ValidationError> list = new List<ValidationError>();
			ServicePlanSkus servicePlanSkus;
			if (Datacenter.IsPartnerHostedOnly(false))
			{
				servicePlanSkus = ServicePlanSkus.Hosted;
			}
			else
			{
				servicePlanSkus = ServicePlanSkus.Datacenter;
			}
			foreach (object obj in this.propertyBag.Keys)
			{
				FeatureDefinition featureDefinition = (FeatureDefinition)obj;
				if ((byte)(featureDefinition.ServicePlanSkus & servicePlanSkus) == 0)
				{
					list.Add(new ServicePlanFeaturesValidationError(featureDefinition.Name, servicePlanSkus.ToString()));
				}
			}
			return list;
		}

		internal void FixDependencies(ServicePlan sp)
		{
			foreach (DependencyEntry dependencyEntry in this.dependencies)
			{
				if (dependencyEntry.GetFeatureValue() && !dependencyEntry.GetDependencyValue(sp))
				{
					dependencyEntry.SetDependencyValue(sp, true);
				}
			}
		}

		private PropertyBag propertyBag;

		private List<DependencyEntry> dependencies;
	}
}
