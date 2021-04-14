using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HA.Services
{
	[DataContract(Name = "ServiceVersion", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public class ServiceVersion
	{
		[DataMember(Name = "Version", IsRequired = true, Order = 0)]
		public long Version
		{
			get
			{
				return this.m_version;
			}
			set
			{
				this.m_version = value;
			}
		}

		public const int VERSION_V1 = 1;

		public const int VERSION_V2 = 2;

		private long m_version;
	}
}
