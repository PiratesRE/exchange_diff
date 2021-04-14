using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface IAdvancedBindingListView : IBindingListView, IBindingList, IList, ICollection, IEnumerable
	{
		bool IsSortSupported(string propertyName);

		bool IsFilterSupported(string propertyName);

		void ApplyFilter(QueryFilter filter);

		QueryFilter QueryFilter { get; }

		bool Filtering { get; }

		event EventHandler FilteringChanged;

		bool SupportCancelFiltering { get; }

		void CancelFiltering();
	}
}
