using System;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal interface ISearchViewState
	{
		string ClearSearchQueryString();

		ClientViewState ClientViewStateBeforeSearch();
	}
}
