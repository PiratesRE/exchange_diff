using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class AddressRewriteEntry : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AddressRewriteEntry.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AddressRewriteEntry.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public string InternalAddress
		{
			get
			{
				return (string)this[AddressRewriteEntrySchema.InternalAddress];
			}
			set
			{
				this[AddressRewriteEntrySchema.InternalAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExternalAddress
		{
			get
			{
				return (string)this[AddressRewriteEntrySchema.ExternalAddress];
			}
			set
			{
				this[AddressRewriteEntrySchema.ExternalAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptionList
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressRewriteEntrySchema.ExceptionList];
			}
			set
			{
				this[AddressRewriteEntrySchema.ExceptionList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutboundOnly
		{
			get
			{
				return (EntryDirection)this[AddressRewriteEntrySchema.Direction] == EntryDirection.OutboundOnly;
			}
			set
			{
				this[AddressRewriteEntrySchema.Direction] = (value ? EntryDirection.OutboundOnly : EntryDirection.Bidirectional);
			}
		}

		private static AddressRewriteEntrySchema schema = ObjectSchema.GetInstance<AddressRewriteEntrySchema>();

		private static string mostDerivedClass = "msExchAddressRewriteEntry";
	}
}
