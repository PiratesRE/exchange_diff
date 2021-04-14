using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class IPListProvider : ADConfigurationObject
	{
		public IPListProvider()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return IPListProvider.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain LookupDomain
		{
			get
			{
				return (SmtpDomain)this[IPListProviderSchema.LookupDomain];
			}
			set
			{
				this[IPListProviderSchema.LookupDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[IPListProviderSchema.Enabled];
			}
			set
			{
				this[IPListProviderSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AnyMatch
		{
			get
			{
				return (bool)this[IPListProviderSchema.AnyMatch];
			}
			set
			{
				this[IPListProviderSchema.AnyMatch] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress BitmaskMatch
		{
			get
			{
				return (IPAddress)this[IPListProviderSchema.Bitmask];
			}
			set
			{
				this[IPListProviderSchema.Bitmask] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> IPAddressesMatch
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[IPListProviderSchema.IPAddress];
			}
			set
			{
				this[IPListProviderSchema.IPAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)this[IPListProviderSchema.Priority];
			}
			set
			{
				this[IPListProviderSchema.Priority] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!this.AnyMatch && this.BitmaskMatch == null && MultiValuedPropertyBase.IsNullOrEmpty(this.IPAddressesMatch))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.BitMaskOrIpAddressMatchMustBeSet, IPListProviderSchema.AnyMatch, this));
			}
		}

		private static IPListProviderSchema schema = ObjectSchema.GetInstance<IPListProviderSchema>();
	}
}
