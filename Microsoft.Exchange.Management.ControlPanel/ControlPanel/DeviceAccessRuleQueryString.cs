using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DeviceAccessRuleQueryString : IComparable
	{
		[DataMember]
		public bool IsWildcard { get; set; }

		[DataMember]
		public string QueryString { get; set; }

		public int CompareTo(object obj)
		{
			if (obj is DeviceAccessRuleQueryString)
			{
				DeviceAccessRuleQueryString deviceAccessRuleQueryString = (DeviceAccessRuleQueryString)obj;
				return this.QueryString.CompareTo(deviceAccessRuleQueryString.QueryString);
			}
			throw new ArgumentException("object is not a DeviceAccessRuleQueryString");
		}
	}
}
