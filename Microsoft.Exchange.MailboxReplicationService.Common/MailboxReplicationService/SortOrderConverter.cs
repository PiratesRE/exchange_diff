using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SortOrderConverter : IDataConverter<SortOrder, SortOrderData>
	{
		SortOrder IDataConverter<SortOrder, SortOrderData>.GetNativeRepresentation(SortOrderData sod)
		{
			SortOrder sortOrder = new SortOrder();
			foreach (SortOrderMember sortOrderMember in sod.Members)
			{
				if (sortOrderMember.IsCategory)
				{
					sortOrder.AddCategory((PropTag)sortOrderMember.PropTag, (SortFlags)sortOrderMember.Flags);
				}
				else
				{
					sortOrder.Add((PropTag)sortOrderMember.PropTag, (SortFlags)sortOrderMember.Flags);
				}
			}
			return sortOrder;
		}

		SortOrderData IDataConverter<SortOrder, SortOrderData>.GetDataRepresentation(SortOrder so)
		{
			SortOrderData sortOrderData = new SortOrderData();
			List<SortOrderMember> soList = new List<SortOrderMember>();
			so.EnumerateSortOrder(delegate(PropTag ptag, SortFlags flags, bool isCategory, object ctx)
			{
				SortOrderMember sortOrderMember = new SortOrderMember();
				sortOrderMember.PropTag = (int)ptag;
				sortOrderMember.Flags = (int)flags;
				sortOrderMember.IsCategory = isCategory;
				soList.Add(sortOrderMember);
			}, null);
			sortOrderData.Members = soList.ToArray();
			return sortOrderData;
		}
	}
}
