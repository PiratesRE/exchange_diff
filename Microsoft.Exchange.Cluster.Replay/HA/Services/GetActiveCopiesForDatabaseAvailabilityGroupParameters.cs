using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HA.Services
{
	[DataContract(Name = "GetActiveCopiesForDatabaseAvailabilityGroupParameters", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public class GetActiveCopiesForDatabaseAvailabilityGroupParameters
	{
		[DataMember(Name = "CachedData", IsRequired = false, Order = 0)]
		public bool CachedData
		{
			get
			{
				return this.m_cachedData;
			}
			set
			{
				this.m_cachedData = value;
			}
		}

		private bool m_cachedData;
	}
}
