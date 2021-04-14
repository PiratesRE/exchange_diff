using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class QueryableObjectImplBase<T> : QueryableObject where T : SimpleProviderObjectSchema
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return QueryableObjectImplBase<T>.schema;
			}
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance(typeof(T));
	}
}
