using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FilterNotSupportedException : StoragePermanentException
	{
		public FilterNotSupportedException(LocalizedString message, QueryFilter filter, params PropertyDefinition[] properties) : base(message)
		{
			this.filter = filter;
			this.properties = properties;
		}

		public PropertyDefinition[] Properties
		{
			get
			{
				return this.properties;
			}
		}

		public QueryFilter Filter
		{
			get
			{
				return this.filter;
			}
		}

		private readonly QueryFilter filter;

		private readonly PropertyDefinition[] properties;
	}
}
