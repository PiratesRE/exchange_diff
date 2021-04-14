using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IModifyTable : IDisposable
	{
		void Clear();

		void AddRow(params PropValue[] propValues);

		void ModifyRow(params PropValue[] propValues);

		void RemoveRow(params PropValue[] propValues);

		IQueryResult GetQueryResult(QueryFilter queryFilter, ICollection<PropertyDefinition> columns);

		void ApplyPendingChanges();

		void SuppressRestriction();

		StoreSession Session { get; }
	}
}
