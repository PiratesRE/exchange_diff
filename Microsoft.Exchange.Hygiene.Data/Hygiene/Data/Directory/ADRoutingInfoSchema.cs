using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADRoutingInfoSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition IdProp = new HygienePropertyDefinition("ADFfoRoutingInfo.Id", typeof(ADObject));
	}
}
