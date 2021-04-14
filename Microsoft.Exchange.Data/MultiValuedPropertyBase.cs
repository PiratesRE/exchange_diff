using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[TypeConverter(typeof(SimpleGenericsTypeConverter))]
	[Serializable]
	public abstract class MultiValuedPropertyBase : ICloneable, ICollection, IEnumerable
	{
		internal static string FormatMultiValuedProperty(IList mvp)
		{
			if (mvp == null)
			{
				throw new ArgumentNullException("mvp");
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < mvp.Count - 1; i++)
			{
				stringBuilder.Append("'");
				stringBuilder.Append(mvp[i].ToString());
				stringBuilder.Append("', ");
			}
			if (mvp.Count != 0)
			{
				stringBuilder.Append("'" + mvp[mvp.Count - 1].ToString() + "'");
			}
			return stringBuilder.ToString();
		}

		internal bool IsCompletelyRead
		{
			get
			{
				return this.isCompletelyRead;
			}
			set
			{
				this.isCompletelyRead = value;
			}
		}

		internal IntRange ValueRange { get; set; }

		internal abstract bool WasCleared { get; }

		internal abstract object[] Added { get; }

		internal abstract object[] Removed { get; }

		internal abstract void Add(object item);

		internal abstract bool Remove(object item);

		internal abstract void MarkAsChanged();

		internal abstract void ResetChangeTracking();

		internal abstract void UpdateValues(MultiValuedPropertyBase newMvp);

		internal abstract void UpdatePropertyDefinition(ProviderPropertyDefinition newPropertyDefinition);

		internal abstract void SetIsReadOnly(bool isReadOnly, LocalizedString? readOnlyErrorMessage);

		internal abstract ProviderPropertyDefinition PropertyDefinition { get; }

		internal virtual void FinalizeDeserialization()
		{
		}

		private void AddHandler(ref Delegate mainDelegate, Delegate value)
		{
			mainDelegate = Delegate.Combine(mainDelegate, value);
		}

		private void RemoveHandler(ref Delegate mainDelegate, Delegate value)
		{
			mainDelegate = Delegate.Remove(mainDelegate, value);
		}

		internal event EventHandler CollectionChanging
		{
			add
			{
				this.AddHandler(ref this.eventCollectionChangingDelegate, value);
			}
			remove
			{
				this.RemoveHandler(ref this.eventCollectionChangingDelegate, value);
			}
		}

		protected virtual void OnCollectionChanging(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.eventCollectionChangingDelegate;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		internal event EventHandler CollectionChanged
		{
			add
			{
				this.AddHandler(ref this.eventCollectionChangedDelegate, value);
			}
			remove
			{
				this.RemoveHandler(ref this.eventCollectionChangedDelegate, value);
			}
		}

		protected virtual void OnCollectionChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.eventCollectionChangedDelegate;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		protected void BeginUpdate()
		{
			if (this.updateCount == 0)
			{
				this.OnCollectionChanging(EventArgs.Empty);
			}
			this.updateCount++;
		}

		protected void EndUpdate()
		{
			this.updateCount--;
			if (this.updateCount == 0)
			{
				this.OnCollectionChanged(EventArgs.Empty);
			}
		}

		internal bool CopyChangesOnly
		{
			get
			{
				return this.copyChangesOnly;
			}
			set
			{
				this.copyChangesOnly = value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public static bool IsNullOrEmpty(MultiValuedPropertyBase property)
		{
			return property == null || property.Count == 0;
		}

		public abstract void Clear();

		public abstract bool Changed { get; }

		public abstract bool IsChangesOnlyCopy { get; }

		public abstract object Clone();

		public abstract void CopyChangesFrom(MultiValuedPropertyBase changedMvp);

		public abstract int Count { get; }

		public abstract bool IsReadOnly { get; }

		public abstract void CopyTo(Array array, int index);

		public abstract bool IsSynchronized { get; }

		public abstract object SyncRoot { get; }

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.copyChangesOnly = false;
			this.updateCount = 0;
		}

		[NonSerialized]
		private bool copyChangesOnly;

		[NonSerialized]
		private int updateCount;

		private bool isCompletelyRead = true;

		[NonSerialized]
		private Delegate eventCollectionChangingDelegate;

		[NonSerialized]
		private Delegate eventCollectionChangedDelegate;
	}
}
