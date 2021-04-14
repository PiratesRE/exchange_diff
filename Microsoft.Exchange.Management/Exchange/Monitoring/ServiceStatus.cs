using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ServiceStatus : ConfigurableObject
	{
		public ServiceStatus() : base(new ServiceStatusPropertyBag())
		{
			this.statusList = new MultiValuedProperty<Status>();
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ServiceStatus.schema;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[ServiceStatusSchema.Identity];
			}
			internal set
			{
				this.propertyBag[ServiceStatusSchema.Identity] = value;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public MultiValuedProperty<Status> StatusList
		{
			get
			{
				return this.statusList;
			}
			internal set
			{
				this.statusList = value;
			}
		}

		public uint MaintenanceWindowDays
		{
			get
			{
				return (uint)this.propertyBag[ServiceStatusSchema.MaintenanceWindowDays];
			}
			internal set
			{
				this.propertyBag[ServiceStatusSchema.MaintenanceWindowDays] = value;
			}
		}

		private static ServiceStatusSchema schema = ObjectSchema.GetInstance<ServiceStatusSchema>();

		private MultiValuedProperty<Status> statusList;
	}
}
