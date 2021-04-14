using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TaskRequest : MessageItem
	{
		internal TaskRequest(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public new static TaskRequest Bind(StoreSession session, StoreId storeId)
		{
			return TaskRequest.Bind(session, storeId, null);
		}

		public new static TaskRequest Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<TaskRequest>(session, storeId, TaskRequestSchema.Instance, propsToReturn);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return TaskRequestSchema.Instance;
			}
		}
	}
}
