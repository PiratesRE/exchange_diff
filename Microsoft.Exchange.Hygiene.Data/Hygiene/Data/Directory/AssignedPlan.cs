using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class AssignedPlan : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return new ADObjectId(this.SubscribedPlanId);
			}
		}

		internal ADObjectId TenantId
		{
			get
			{
				return this[ADObjectSchema.OrganizationalUnitRoot] as ADObjectId;
			}
			set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
			}
		}

		internal ADObjectId ObjectId
		{
			get
			{
				return this[CommonSyncProperties.ObjectIdProp] as ADObjectId;
			}
			set
			{
				this[CommonSyncProperties.ObjectIdProp] = value;
			}
		}

		internal Guid SubscribedPlanId
		{
			get
			{
				return (Guid)this[AssignedPlanSchema.SubscribedPlanIdProp];
			}
			set
			{
				this[AssignedPlanSchema.SubscribedPlanIdProp] = value;
			}
		}

		internal string Capability
		{
			get
			{
				return this[AssignedPlanSchema.CapabilityProp] as string;
			}
			set
			{
				this[AssignedPlanSchema.CapabilityProp] = value;
			}
		}

		internal AssignedCapabilityStatus CapabilityStatus
		{
			get
			{
				return (AssignedCapabilityStatus)this[AssignedPlanSchema.CapabilityStatusProp];
			}
			set
			{
				this[AssignedPlanSchema.CapabilityStatusProp] = value;
			}
		}

		internal DateTime AssignedTimeStamp
		{
			get
			{
				return (DateTime)this[AssignedPlanSchema.AssignedTimeStampProp];
			}
			set
			{
				this[AssignedPlanSchema.AssignedTimeStampProp] = value;
			}
		}

		internal string InitialState
		{
			get
			{
				return this[AssignedPlanSchema.InitialStateProp] as string;
			}
			set
			{
				this[AssignedPlanSchema.InitialStateProp] = value;
			}
		}

		internal DirectoryObjectClass ObjectType
		{
			get
			{
				return (DirectoryObjectClass)this[CommonSyncProperties.ObjectTypeProp];
			}
			set
			{
				this[CommonSyncProperties.ObjectTypeProp] = value;
			}
		}

		internal string ServiceInstance
		{
			get
			{
				return this[CommonSyncProperties.ServiceInstanceProp] as string;
			}
			set
			{
				this[CommonSyncProperties.ServiceInstanceProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AssignedPlan.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AssignedPlan.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly AssignedPlanSchema schema = ObjectSchema.GetInstance<AssignedPlanSchema>();

		private static string mostDerivedClass = "AssignedPlan";
	}
}
