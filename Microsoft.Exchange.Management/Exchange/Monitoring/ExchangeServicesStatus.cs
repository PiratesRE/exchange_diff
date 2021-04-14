using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ExchangeServicesStatus : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ExchangeServicesStatus.schema;
			}
		}

		public string Role
		{
			get
			{
				return (string)this[ExchangeServicesStatusSchema.Role];
			}
			internal set
			{
				this[ExchangeServicesStatusSchema.Role] = value;
			}
		}

		public bool RequiredServicesRunning
		{
			get
			{
				return (bool)this[ExchangeServicesStatusSchema.RequiredServicesRunning];
			}
			internal set
			{
				this[ExchangeServicesStatusSchema.RequiredServicesRunning] = value;
			}
		}

		public MultiValuedProperty<string> ServicesNotRunning
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExchangeServicesStatusSchema.ServicesNotRunning];
			}
			internal set
			{
				this[ExchangeServicesStatusSchema.ServicesNotRunning] = value;
			}
		}

		public MultiValuedProperty<string> ServicesRunning
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExchangeServicesStatusSchema.ServicesRunning];
			}
			internal set
			{
				this[ExchangeServicesStatusSchema.ServicesRunning] = value;
			}
		}

		internal ExchangeServicesStatus(ServerRole roleBitfiedFlag, bool requiredServicesRunning, string[] servicesNotRunning, string[] servicesRunning) : base(new SimpleProviderPropertyBag())
		{
			if (servicesNotRunning == null)
			{
				throw new ArgumentNullException("servicesNotRunning");
			}
			if (servicesRunning == null)
			{
				throw new ArgumentNullException("servicesRunning");
			}
			this.Role = MpServerRoles.DisplayRoleName(roleBitfiedFlag);
			this.RequiredServicesRunning = requiredServicesRunning;
			this.ServicesNotRunning = new MultiValuedProperty<string>(servicesNotRunning);
			this.ServicesRunning = new MultiValuedProperty<string>(servicesRunning);
		}

		private static ExchangeServicesStatusSchema schema = ObjectSchema.GetInstance<ExchangeServicesStatusSchema>();
	}
}
