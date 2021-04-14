using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class AdvancedBindingSource : BindingSource, IAdvancedBindingListView, IBindingListView, IBindingList, IList, ICollection, IEnumerable
	{
		public AdvancedBindingSource()
		{
			base.DataSourceChanged += this.AdvancedBindingSource_DataSourceChanged;
		}

		private void AdvancedBindingSource_DataSourceChanged(object sender, EventArgs e)
		{
			IAdvancedBindingListView advancedBindingListView = this.GetAdvancedBindingListView();
			if (advancedBindingListView != this.currentList)
			{
				if (this.currentList != null)
				{
					this.currentList.FilteringChanged -= this.CurrentList_FilteringChanged;
				}
				this.currentList = advancedBindingListView;
				if (this.currentList != null)
				{
					this.currentList.FilteringChanged += this.CurrentList_FilteringChanged;
				}
			}
		}

		private IAdvancedBindingListView GetAdvancedBindingListView()
		{
			if (base.List == null)
			{
				return null;
			}
			IAdvancedBindingListView advancedBindingListView = base.List as IAdvancedBindingListView;
			if (advancedBindingListView == null)
			{
				throw new NotSupportedException("IAdvancedBindingListView are required");
			}
			return advancedBindingListView;
		}

		private void CurrentList_FilteringChanged(object sender, EventArgs e)
		{
			this.OnFilteringChanged(EventArgs.Empty);
		}

		public virtual bool IsSortSupported(string propertyName)
		{
			return this.currentList != null && this.currentList.IsSortSupported(propertyName);
		}

		public virtual bool IsFilterSupported(string propertyName)
		{
			return this.currentList != null && this.currentList.IsFilterSupported(propertyName);
		}

		public virtual void ApplyFilter(QueryFilter filter)
		{
			if (this.currentList == null)
			{
				return;
			}
			this.currentList.ApplyFilter(filter);
		}

		public virtual QueryFilter QueryFilter
		{
			get
			{
				if (this.currentList != null)
				{
					return this.currentList.QueryFilter;
				}
				return null;
			}
		}

		public virtual bool Filtering
		{
			get
			{
				return this.currentList != null && this.currentList.Filtering;
			}
		}

		public event EventHandler FilteringChanged
		{
			add
			{
				base.Events.AddHandler(AdvancedBindingSource.EventFilteringChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(AdvancedBindingSource.EventFilteringChanged, value);
			}
		}

		protected virtual void OnFilteringChanged(EventArgs e)
		{
			if (base.Events[AdvancedBindingSource.EventFilteringChanged] != null)
			{
				((EventHandler)base.Events[AdvancedBindingSource.EventFilteringChanged])(this, e);
			}
		}

		public virtual bool SupportCancelFiltering
		{
			get
			{
				return this.currentList != null && this.currentList.SupportCancelFiltering;
			}
		}

		public virtual void CancelFiltering()
		{
			if (this.currentList == null)
			{
				return;
			}
			this.currentList.CancelFiltering();
		}

		private IAdvancedBindingListView currentList;

		private static object EventFilteringChanged = new object();
	}
}
