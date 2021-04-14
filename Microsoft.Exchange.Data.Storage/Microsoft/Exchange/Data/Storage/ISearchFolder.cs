using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISearchFolder : IFolder, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		SearchFolderCriteria GetSearchCriteria();

		void ApplyContinuousSearch(SearchFolderCriteria searchFolderCriteria);

		void ApplyOneTimeSearch(SearchFolderCriteria searchFolderCriteria);

		IAsyncResult BeginApplyContinuousSearch(SearchFolderCriteria searchFolderCriteria, AsyncCallback asyncCallback, object state);

		void EndApplyContinuousSearch(IAsyncResult asyncResult);

		IAsyncResult BeginApplyOneTimeSearch(SearchFolderCriteria searchFolderCriteria, AsyncCallback asyncCallback, object state);

		void EndApplyOneTimeSearch(IAsyncResult asyncResult);
	}
}
