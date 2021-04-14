using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IPseudoIndex : IIndex
	{
		bool ShouldBeCurrent { get; }

		object[] IndexTableFunctionParameters { get; }

		int RedundantKeyColumnsCount { get; }

		CategorizedTableParams GetCategorizedTableParams(Context context);
	}
}
