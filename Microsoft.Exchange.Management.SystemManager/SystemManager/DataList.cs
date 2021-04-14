using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataList<T> : BindingList<T>
	{
		public bool AllowSorting
		{
			get
			{
				return this.allowSorting;
			}
			set
			{
				if (this.AllowSorting != value)
				{
					this.allowSorting = value;
					base.ResetBindings();
				}
			}
		}

		protected override bool SupportsSortingCore
		{
			get
			{
				return this.AllowSorting;
			}
		}

		protected override ListSortDirection SortDirectionCore
		{
			get
			{
				return this.sortDirection;
			}
		}

		protected override PropertyDescriptor SortPropertyCore
		{
			get
			{
				return this.sortProperty;
			}
		}

		protected override bool IsSortedCore
		{
			get
			{
				return null != this.sortProperty;
			}
		}

		protected override void InsertItem(int index, T item)
		{
			if (this.IsSortedCore)
			{
				List<T> list = (List<T>)base.Items;
				index = list.BinarySearch(item, new PropertyComparer<T>(this.SortPropertyCore, this.SortDirectionCore));
				if (index < 0)
				{
					index = ~index;
				}
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);
			if (this.IsSortedCore)
			{
				this.ApplySortCore(this.sortProperty, this.sortDirection);
			}
		}

		public void ApplySort(string propertyName, ListSortDirection direction)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
			PropertyDescriptor propertyDescriptor = properties[propertyName];
			if (propertyDescriptor == null)
			{
				throw new ArgumentOutOfRangeException("propertyName");
			}
			this.ApplySortCore(propertyDescriptor, direction);
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			if (!this.SupportsSortingCore)
			{
				throw new NotSupportedException();
			}
			List<T> list = (List<T>)base.Items;
			list.Sort(new PropertyComparer<T>(property, direction));
			this.sortProperty = property;
			this.sortDirection = direction;
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override void RemoveSortCore()
		{
			this.sortProperty = null;
			this.sortDirection = ListSortDirection.Ascending;
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override bool SupportsSearchingCore
		{
			get
			{
				return true;
			}
		}

		public int Find(string propertyName, object key)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
			PropertyDescriptor prop = properties[propertyName];
			return this.FindCore(prop, key);
		}

		protected override int FindCore(PropertyDescriptor property, object key)
		{
			if (property != null)
			{
				for (int i = 0; i < base.Items.Count; i++)
				{
					object value = property.GetValue(base.Items[i]);
					if (StringComparer.OrdinalIgnoreCase.Compare(key, value) == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public T[] ToArray()
		{
			T[] array = new T[base.Count];
			base.CopyTo(array, 0);
			return array;
		}

		public void CopyFrom(IEnumerable enumerable)
		{
			bool raiseListChangedEvents = base.RaiseListChangedEvents;
			base.RaiseListChangedEvents = false;
			PropertyDescriptor propertyDescriptor = this.sortProperty;
			this.sortProperty = null;
			try
			{
				base.Clear();
				if (enumerable != null)
				{
					foreach (object obj in enumerable)
					{
						T item = (T)((object)obj);
						base.Add(item);
					}
				}
			}
			finally
			{
				base.RaiseListChangedEvents = raiseListChangedEvents;
				if (propertyDescriptor != null && this.SupportsSortingCore)
				{
					this.ApplySortCore(propertyDescriptor, this.SortDirectionCore);
				}
				else
				{
					base.ResetBindings();
				}
			}
		}

		internal void FillWith(string commandText, MonadConnection connection)
		{
			using (MonadCommand monadCommand = new LoggableMonadCommand(commandText, connection))
			{
				this.FillWith(monadCommand);
			}
		}

		internal void FillWith(MonadCommand command)
		{
			using (new OpenConnection(command.Connection))
			{
				this.CopyFrom(command.Execute());
			}
		}

		private bool allowSorting = true;

		private ListSortDirection sortDirection;

		private PropertyDescriptor sortProperty;
	}
}
