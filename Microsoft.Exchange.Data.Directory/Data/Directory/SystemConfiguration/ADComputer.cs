using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADComputer : ADNonExchangeObject
	{
		internal void DisableComputerAccount()
		{
			this.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADComputer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADComputer.mostDerivedClass;
			}
		}

		public MultiValuedProperty<string> ServicePrincipalName
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADComputerSchema.ServicePrincipalName];
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this.propertyBag[ADComputerSchema.Sid];
			}
		}

		public string OperatingSystemVersion
		{
			get
			{
				return (string)this.propertyBag[ADComputerSchema.OperatingSystemVersion];
			}
		}

		public string OperatingSystemServicePack
		{
			get
			{
				return (string)this.propertyBag[ADComputerSchema.OperatingSystemServicePack];
			}
		}

		public string DnsHostName
		{
			get
			{
				return (string)this[ADComputerSchema.DnsHostName];
			}
			internal set
			{
				this[ADComputerSchema.DnsHostName] = value;
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return (ADObjectId)this[ADComputerSchema.ThrottlingPolicy];
			}
			set
			{
				this[ADComputerSchema.ThrottlingPolicy] = value;
			}
		}

		public MultiValuedProperty<string> ComponentStates
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADComputerSchema.ComponentStates];
			}
			internal set
			{
				this[ADComputerSchema.ComponentStates] = value;
			}
		}

		public string MonitoringGroup
		{
			get
			{
				return (string)this[ADComputerSchema.MonitoringGroup];
			}
			set
			{
				this[ADComputerSchema.MonitoringGroup] = value;
			}
		}

		public int MonitoringInstalled
		{
			get
			{
				return (int)this[ADComputerSchema.MonitoringInstalled];
			}
			set
			{
				this[ADComputerSchema.MonitoringInstalled] = value;
			}
		}

		internal bool IsOutOfService
		{
			get
			{
				return (bool)this[ADComputerSchema.IsOutOfService];
			}
			set
			{
				this[ADComputerSchema.IsOutOfService] = value;
			}
		}

		private UserAccountControlFlags UserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[ADComputerSchema.UserAccountControl];
			}
			set
			{
				this[ADComputerSchema.UserAccountControl] = value;
			}
		}

		private static ADComputerSchema schema = ObjectSchema.GetInstance<ADComputerSchema>();

		private static string mostDerivedClass = "computer";
	}
}
