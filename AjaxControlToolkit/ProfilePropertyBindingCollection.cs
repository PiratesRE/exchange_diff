using System;
using System.Collections;

namespace AjaxControlToolkit
{
	public class ProfilePropertyBindingCollection : CollectionBase
	{
		internal event EventHandler CollectionChanged;

		internal ProfilePropertyBindingCollection()
		{
		}

		public ProfilePropertyBinding this[int index]
		{
			get
			{
				return (ProfilePropertyBinding)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public void Add(ProfilePropertyBinding binding)
		{
			base.InnerList.Add(binding);
		}

		public void Insert(int index, ProfilePropertyBinding binding)
		{
			base.InnerList.Insert(index, binding);
		}

		protected virtual void OnCollectionChanged(EventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		public void Remove(ProfilePropertyBinding binding)
		{
			base.InnerList.Remove(binding);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			this.OnCollectionChanged(EventArgs.Empty);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);
			this.OnCollectionChanged(EventArgs.Empty);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);
			this.OnCollectionChanged(EventArgs.Empty);
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			this.OnCollectionChanged(EventArgs.Empty);
		}
	}
}
