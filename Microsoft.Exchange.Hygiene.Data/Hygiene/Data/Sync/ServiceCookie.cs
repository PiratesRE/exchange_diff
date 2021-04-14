using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	public class ServiceCookie : BaseCookie
	{
		public ServiceCookie()
		{
		}

		public ServiceCookie(string serviceInstance, byte[] data) : base(serviceInstance, data)
		{
		}

		public DateTime LastCompletedTime
		{
			get
			{
				return (DateTime)this[ServiceCookieSchema.LastCompletedTimeProperty];
			}
			set
			{
				this[ServiceCookieSchema.LastCompletedTimeProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ServiceCookie.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ServiceCookie.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ServiceCookieSchema schema = ObjectSchema.GetInstance<ServiceCookieSchema>();

		private static string mostDerivedClass = "ServiceCookie";
	}
}
