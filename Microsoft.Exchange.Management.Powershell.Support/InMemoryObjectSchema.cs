using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal class InMemoryObjectSchema : ObjectSchema
	{
		public static readonly SimplePropertyDefinition Identity = new SimplePropertyDefinition("Identity", typeof(ObjectId), null);

		public static readonly SimplePropertyDefinition ObjectState = new SimplePropertyDefinition("ObjectState", typeof(ObjectState), Microsoft.Exchange.Data.ObjectState.New);

		public static readonly SimplePropertyDefinition ExchangeVersion = new SimplePropertyDefinition("ExchangeVersion", typeof(ExchangeObjectVersion), ExchangeObjectVersion.Exchange2010);
	}
}
