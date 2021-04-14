using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableOnlineDatabaseObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition DatabaseName = QueryableObjectSchema.DatabaseName;

		public static readonly SimpleProviderPropertyDefinition DatabaseGuid = QueryableObjectSchema.DatabaseGuid;

		public static readonly SimpleProviderPropertyDefinition RestartRequired = QueryableObjectSchema.RestartRequired;

		public static readonly SimpleProviderPropertyDefinition EventController = QueryableObjectSchema.EventController;
	}
}
