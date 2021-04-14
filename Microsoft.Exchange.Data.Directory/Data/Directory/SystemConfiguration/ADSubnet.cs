using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public sealed class ADSubnet : ADNonExchangeObject, IComparable<ADSubnet>
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSubnet.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSubnet.mostDerivedClass;
			}
		}

		public ADObjectId Site
		{
			get
			{
				return (ADObjectId)this[ADSubnetSchema.Site];
			}
			set
			{
				this[ADSubnetSchema.Site] = value;
			}
		}

		[Parameter]
		public IPAddress IPAddress
		{
			get
			{
				return (IPAddress)this[ADSubnetSchema.IPAddress];
			}
			set
			{
				this[ADSubnetSchema.IPAddress] = value;
			}
		}

		[Parameter]
		public int MaskBits
		{
			get
			{
				return (int)this[ADSubnetSchema.MaskBits];
			}
			set
			{
				this[ADSubnetSchema.MaskBits] = value;
			}
		}

		internal static object IPAddressGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				string[] nameParts = ADSubnet.GetNameParts(propertyBag);
				IPAddress ipaddress = IPAddress.Parse(nameParts[0]);
				result = ipaddress;
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("IPAddress", ex.Message), ADSubnetSchema.IPAddress, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static void IPAddressSetter(object value, IPropertyBag propertyBag)
		{
			string[] nameParts = ADSubnet.GetNameParts(propertyBag);
			string value2 = value.ToString() + "/" + nameParts[1];
			propertyBag[ADObjectSchema.RawName] = value2;
		}

		internal static object MaskBitsGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				string[] nameParts = ADSubnet.GetNameParts(propertyBag);
				result = int.Parse(nameParts[1]);
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("MaskBits", ex.Message), ADSubnetSchema.MaskBits, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static void MaskBitsSetter(object value, IPropertyBag propertyBag)
		{
			try
			{
				string[] nameParts = ADSubnet.GetNameParts(propertyBag);
				string value2 = nameParts[0] + "/" + value.ToString();
				propertyBag[ADObjectSchema.RawName] = value2;
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("MaskBits", ex.Message), ADObjectSchema.Id, propertyBag[ADObjectSchema.Id]), ex);
			}
		}

		private static string[] GetNameParts(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.RawName];
			string[] array = text.Split(new char[]
			{
				'/'
			});
			if (array.Length != 2 || string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]))
			{
				throw new FormatException(DirectoryStrings.InvalidSubnetNameFormat(text));
			}
			return array;
		}

		public int CompareTo(ADSubnet rhs)
		{
			return this.MaskBits.CompareTo(rhs.MaskBits);
		}

		internal bool Match(IPAddress iPAddress)
		{
			if (this.subnetIPRange == null || !this.subnetIPRange.LowerBound.Equals(this.IPAddress) || (this.subnetIPRange.RangeFormat == IPRange.Format.CIDR && (int)this.subnetIPRange.CIDRLength != this.MaskBits))
			{
				this.subnetIPRange = IPRange.CreateIPAndCIDR(this.IPAddress, (short)this.MaskBits);
			}
			return this.subnetIPRange.Contains(iPAddress);
		}

		private static IPAddress Mask(IPAddress iPAddress, int maskBits)
		{
			IPvxAddress v = new IPvxAddress(iPAddress);
			IPvxAddress mask = ~(~IPvxAddress.Zero >> maskBits);
			return v & mask;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			int num = 32;
			if (this.IPAddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				num = 128;
			}
			if (this.MaskBits < 2 || this.MaskBits > num)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSubnetMaskOutOfRange(this.MaskBits, this.IPAddress.ToString(), 2, num), base.Id, string.Empty));
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.MaskBits < 2)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSubnetMaskLessThanMinRange(this.MaskBits, 2), base.Id, string.Empty));
				return;
			}
			if (this.MaskBits > this.IPAddress.GetAddressBytes().Length * 8)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSubnetMaskGreaterThanAddress(this.MaskBits, this.IPAddress.ToString()), base.Id, string.Empty));
				return;
			}
			if (!this.Match(this.IPAddress))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSubnetAddressDoesNotMatchMask(this.IPAddress.ToString(), this.MaskBits, ADSubnet.Mask(this.IPAddress, this.MaskBits).ToString()), base.Id, string.Empty));
				return;
			}
			if (new IPvxAddress(this.IPAddress) != this.subnetIPRange.LowerBound)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSubnetAddressDoesNotMatchMask(this.IPAddress.ToString(), this.MaskBits, this.subnetIPRange.LowerBound.ToString()), base.Id, string.Empty));
			}
		}

		private const int IpMaskMinRange = 2;

		private const int Ipv4MaskMaxRange = 32;

		private const int Ipv6MaskMaxRange = 128;

		private static ADSubnetSchema schema = ObjectSchema.GetInstance<ADSubnetSchema>();

		private static string mostDerivedClass = "subnet";

		private IPRange subnetIPRange;
	}
}
