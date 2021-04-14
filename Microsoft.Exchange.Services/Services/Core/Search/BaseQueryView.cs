using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal abstract class BaseQueryView
	{
		public bool RetrievedLastItem
		{
			get
			{
				return this.retrievedLastItem;
			}
			set
			{
				this.retrievedLastItem = value;
			}
		}

		public virtual int TotalItems
		{
			get
			{
				return this.totalItems;
			}
		}

		public abstract ItemType[] ConvertToItems(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, IdAndSession idAndSession);

		public abstract FindItemParentWrapper ConvertToFindItemParentWrapper(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, IdAndSession idAndSession, BasePageResult pageResult, QueryType queryType);

		public abstract BaseFolderType[] ConvertToFolderObjects(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, IdAndSession idAndSession);

		public abstract ConversationType[] ConvertToConversationObjects(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, IdAndSession idAndSession, RequestDetailsLogger logger);

		public Persona[] ConvertPersonViewToPersonaObjects(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, IdAndSession idAndSession)
		{
			return this.ConvertPersonViewToPersonaObjects(propsToFetch, classDeterminer, idAndSession.Session as MailboxSession);
		}

		public abstract Persona[] ConvertPersonViewToPersonaObjects(PropertyDefinition[] propsToFetch, PropertyListForViewRowDeterminer classDeterminer, MailboxSession storeSession);

		internal static IDictionary<PropertyDefinition, object> GetRowData(PropertyDefinition[] keys, object[] viewRow)
		{
			IDictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>();
			for (int i = 0; i < keys.Length; i++)
			{
				if (!(viewRow[i] is PropertyError))
				{
					dictionary[keys[i]] = viewRow[i];
				}
			}
			return dictionary;
		}

		protected bool CanAllocateFoundObjects(BasePagingType paging, uint foundCount, out int maxPossible)
		{
			if (!CallContext.Current.Budget.CanAllocateFoundObjects(foundCount, out maxPossible))
			{
				if (paging == null || !paging.BudgetInducedTruncationAllowed)
				{
					ExceededFindCountLimitException.Throw();
				}
				return false;
			}
			return true;
		}

		protected int AllocateBudgetFoundObjects(int rowsToFetch, BasePagingType paging)
		{
			int num = rowsToFetch;
			if (rowsToFetch > 0)
			{
				while (!this.IncrementBudgetFoundObjects(rowsToFetch, paging, out num))
				{
					rowsToFetch = num;
					if (rowsToFetch <= 0)
					{
						break;
					}
				}
			}
			return num;
		}

		private bool IncrementBudgetFoundObjects(int rowsObtained, BasePagingType paging, out int maxAllowed)
		{
			maxAllowed = rowsObtained;
			if (rowsObtained > 0 && CallContext.Current != null && !CallContext.Current.Budget.TryIncrementFoundObjectCount((uint)rowsObtained, out maxAllowed))
			{
				if (paging == null || !paging.BudgetInducedTruncationAllowed)
				{
					ExceededFindCountLimitException.Throw();
				}
				return false;
			}
			return true;
		}

		protected virtual void CheckClientConnection()
		{
			if (CallContext.Current != null && !CallContext.Current.IsClientConnected)
			{
				BailOut.SetHTTPStatusAndClose(HttpStatusCode.NoContent);
			}
		}

		protected bool retrievedLastItem;

		protected int totalItems;
	}
}
