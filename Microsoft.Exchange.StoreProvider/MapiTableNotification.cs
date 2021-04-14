using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiTableNotification : MapiNotification
	{
		public TableEvent TableEvent
		{
			get
			{
				return this.tableEvent;
			}
		}

		public int HResult
		{
			get
			{
				return this.hResult;
			}
		}

		public PropValue Index
		{
			get
			{
				return this.index;
			}
		}

		public PropValue Prior
		{
			get
			{
				return this.prior;
			}
		}

		public PropValue[] Row
		{
			get
			{
				return this.row;
			}
		}

		internal unsafe MapiTableNotification(NOTIFICATION* notification) : base(notification)
		{
			this.tableEvent = (TableEvent)notification->info.tab.ulTableEvent;
			this.hResult = notification->info.tab.hResult;
			if (this.tableEvent == TableEvent.TableRowAdded || this.tableEvent == TableEvent.TableRowDeleted || this.tableEvent == TableEvent.TableRowDeletedExtended || this.tableEvent == TableEvent.TableRowModified)
			{
				this.index = new PropValue(&notification->info.tab.propIndex);
			}
			else
			{
				this.index = new PropValue(PropTag.Null, null);
			}
			if (this.tableEvent == TableEvent.TableRowAdded || this.tableEvent == TableEvent.TableRowModified)
			{
				this.prior = new PropValue(&notification->info.tab.propPrior);
				this.row = Array<PropValue>.New(notification->info.tab.row.cValues);
				SPropValue* ptr = (SPropValue*)notification->info.tab.row.lpProps.ToPointer();
				for (int i = 0; i < this.row.Length; i++)
				{
					this.row[i] = new PropValue(ptr + i);
				}
				return;
			}
			if (this.tableEvent == TableEvent.TableRowDeletedExtended)
			{
				this.row = Array<PropValue>.New(notification->info.tab.row.cValues);
				SPropValue* ptr2 = (SPropValue*)notification->info.tab.row.lpProps.ToPointer();
				for (int j = 0; j < this.row.Length; j++)
				{
					this.row[j] = new PropValue(ptr2 + j);
				}
				return;
			}
			this.prior = new PropValue(PropTag.Null, null);
			this.row = Array<PropValue>.Empty;
		}

		private readonly TableEvent tableEvent;

		private readonly int hResult;

		private readonly PropValue index;

		private readonly PropValue prior;

		private readonly PropValue[] row;
	}
}
