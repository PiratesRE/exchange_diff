using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Supervision
{
	[Serializable]
	public class SupervisionTransportRule : TransportRule
	{
		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
			set
			{
				this[ADObjectSchema.Name] = value;
			}
		}
	}
}
