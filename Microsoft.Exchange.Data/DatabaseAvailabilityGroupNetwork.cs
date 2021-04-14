using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DatabaseAvailabilityGroupNetwork : ConfigurableObject
	{
		[Parameter]
		public string Name
		{
			get
			{
				return (string)this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.Name];
			}
			set
			{
				this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.Name] = value;
			}
		}

		internal static StringComparer NameComparer
		{
			get
			{
				return StringComparer.CurrentCultureIgnoreCase;
			}
		}

		internal static StringComparison NameComparison
		{
			get
			{
				return StringComparison.CurrentCultureIgnoreCase;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return (string)this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.Description];
			}
			set
			{
				this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.Description] = value;
			}
		}

		public DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkSubnet> Subnets
		{
			get
			{
				return (DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkSubnet>)this[DatabaseAvailabilityGroupNetworkSchema.Subnets];
			}
			set
			{
				this[DatabaseAvailabilityGroupNetworkSchema.Subnets] = value;
			}
		}

		public DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkInterface> Interfaces
		{
			get
			{
				return (DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkInterface>)this[DatabaseAvailabilityGroupNetworkSchema.Interfaces];
			}
			set
			{
				this[DatabaseAvailabilityGroupNetworkSchema.Interfaces] = value;
			}
		}

		public bool MapiAccessEnabled
		{
			get
			{
				return (bool)this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.MapiAccessEnabled];
			}
			internal set
			{
				this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.MapiAccessEnabled] = value;
			}
		}

		[Parameter]
		public bool ReplicationEnabled
		{
			get
			{
				return (bool)this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.ReplicationEnabled];
			}
			set
			{
				this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.ReplicationEnabled] = value;
			}
		}

		[Parameter]
		public bool IgnoreNetwork
		{
			get
			{
				return (bool)this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.IgnoreNetwork];
			}
			set
			{
				this.propertyBag[DatabaseAvailabilityGroupNetworkSchema.IgnoreNetwork] = value;
			}
		}

		internal void SetIdentity(DagNetworkObjectId id)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = id;
		}

		public DatabaseAvailabilityGroupNetwork() : base(new DagNetPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return DatabaseAvailabilityGroupNetwork.s_schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal void ReplaceSubnets(IEnumerable<DatabaseAvailabilityGroupSubnetId> newSubnetIds)
		{
			this.Subnets.Clear();
			foreach (DatabaseAvailabilityGroupSubnetId netId in newSubnetIds)
			{
				DatabaseAvailabilityGroupNetworkSubnet item = new DatabaseAvailabilityGroupNetworkSubnet(netId);
				this.Subnets.Add(item);
			}
		}

		private static ObjectSchema s_schema = ObjectSchema.GetInstance<DatabaseAvailabilityGroupNetworkSchema>();
	}
}
