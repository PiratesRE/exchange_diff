using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class DataTreeListViewColumnMappingCollection : CollectionBase
	{
		public void Add(string dataMember)
		{
			this.Add(dataMember, -1, null);
		}

		public void Add(string dataMember, int imageIndex, string[] propertyNames)
		{
			this.Add(dataMember, imageIndex, propertyNames, string.Empty, ListSortDirection.Ascending);
		}

		public void Add(string dataMember, int imageIndex, string[] propertyNames, string sortProperty, ListSortDirection sortDirection)
		{
			if (string.IsNullOrEmpty(dataMember))
			{
				throw new ArgumentNullException("dataMember");
			}
			if (this.Find(dataMember) != null)
			{
				throw new ArgumentException(Strings.DuplicateDataMember.ToString(), "dataMember");
			}
			DataTreeListViewColumnMapping value = new DataTreeListViewColumnMapping(dataMember, imageIndex, propertyNames, sortProperty, sortDirection);
			base.List.Add(value);
		}

		public DataTreeListViewColumnMapping Find(string dataMember)
		{
			DataTreeListViewColumnMapping result = null;
			foreach (object obj in this)
			{
				DataTreeListViewColumnMapping dataTreeListViewColumnMapping = (DataTreeListViewColumnMapping)obj;
				if (dataTreeListViewColumnMapping.DataMember == dataMember)
				{
					result = dataTreeListViewColumnMapping;
					break;
				}
			}
			return result;
		}

		protected override void OnInsert(int index, object value)
		{
			DataTreeListViewColumnMapping dataTreeListViewColumnMapping = value as DataTreeListViewColumnMapping;
			if (dataTreeListViewColumnMapping == null)
			{
				throw new InvalidOperationException();
			}
			if (string.IsNullOrEmpty(dataTreeListViewColumnMapping.DataMember))
			{
				throw new ArgumentNullException("DataMember");
			}
			for (int i = 0; i < base.Count; i++)
			{
				if ((base.List[i] as DataTreeListViewColumnMapping).DataMember == dataTreeListViewColumnMapping.DataMember && i != index)
				{
					throw new ArgumentException(Strings.DuplicateDataMember.ToString(), "DataMember");
				}
			}
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Remove, null));
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			}
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
			}
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, newValue));
			}
		}

		public event CollectionChangeEventHandler CollectionChanged;
	}
}
