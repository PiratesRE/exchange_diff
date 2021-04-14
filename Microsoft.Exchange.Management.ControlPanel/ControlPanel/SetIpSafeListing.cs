using System;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetIpSafeListing : SetObjectProperties
	{
		[DataMember]
		public string[] InternalServerIPAddresses
		{
			get
			{
				return ((IPAddress[])base["InternalServerIPAddresses"]).ToStringArray();
			}
			set
			{
				base["InternalServerIPAddresses"] = SetIpSafeListing.GetIPAddressArrayFromStringArray(value);
			}
		}

		[DataMember]
		public string[] GatewayIPAddresses
		{
			get
			{
				return ((IPAddress[])base["GatewayIPAddresses"]).ToStringArray();
			}
			set
			{
				base["GatewayIPAddresses"] = SetIpSafeListing.GetIPAddressArrayFromStringArray(value);
			}
		}

		[DataMember]
		public bool IPSkiplistingEnabled
		{
			get
			{
				return (bool)(base["IPSkiplistingEnabled"] ?? false);
			}
			set
			{
				base["IPSkiplistingEnabled"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-PerimeterConfig";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		private static IPAddress[] GetIPAddressArrayFromStringArray(string[] value)
		{
			if (value == null)
			{
				return new IPAddress[0];
			}
			IPAddress[] array = new IPAddress[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				try
				{
					array[i] = IPAddress.Parse(value[i]);
				}
				catch (FormatException)
				{
					throw new FaultException(Strings.InvalidIPAddressFormat(value[i].ToStringWithNull()));
				}
			}
			return array;
		}
	}
}
