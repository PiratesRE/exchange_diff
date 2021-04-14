using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiContainer : IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		int GetContentsTable(int ulFlags, out IExMapiTable iMAPITable);

		int GetHierarchyTable(int ulFlags, out IExMapiTable iMAPITable);

		int OpenEntry(byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out IExInterface iObj);

		int SetSearchCriteria(Restriction lpRestriction, byte[][] lpContainerList, int ulSearchFlags);

		int GetSearchCriteria(int ulFlags, out Restriction lpRestriction, out byte[][] lpContainerList, out int ulSearchState);
	}
}
