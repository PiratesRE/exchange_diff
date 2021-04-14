using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ADQueryPolicy : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADQueryPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADQueryPolicy.mostDerivedClass;
			}
		}

		[ValidateRange(5, 2147483647)]
		[Parameter]
		public int? MaxNotificationPerConnection
		{
			get
			{
				return (int?)this.propertyBag[ADQueryPolicySchema.MaxNotificationPerConn];
			}
			set
			{
				this.propertyBag[ADQueryPolicySchema.MaxNotificationPerConn] = value;
			}
		}

		private static ADQueryPolicySchema schema = ObjectSchema.GetInstance<ADQueryPolicySchema>();

		private static string mostDerivedClass = "queryPolicy";

		public static readonly string ADDefaultQueryPolicyName = "Default Query Policy";
	}
}
