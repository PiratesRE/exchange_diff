using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentCollection : IAttachmentCollection, IEnumerable<AttachmentHandle>, IEnumerable
	{
		public AttachmentCollection(Item parent) : this(parent, false)
		{
		}

		public AttachmentCollection(Item parent, bool hideCalendarExceptions)
		{
			this.parent = parent;
			this.hideCalendarExceptions = hideCalendarExceptions;
		}

		public void Load(params PropertyDefinition[] propertyList)
		{
			this.CoreAttachmentCollection.Load(propertyList);
		}

		public ItemAttachment AddExistingItem(IItem item)
		{
			Util.ThrowOnNullArgument(item, "item");
			ItemAttachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.CoreAttachmentCollection.CreateFromExistingItem(item);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				result = (ItemAttachment)AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(AttachmentType.EmbeddedMessage));
				disposeGuard.Success();
			}
			return result;
		}

		public IAttachment CreateIAttachment(AttachmentType type)
		{
			return this.Create(type);
		}

		public IAttachment CreateIAttachment(AttachmentType type, IAttachment attachment)
		{
			return this.Create(new AttachmentType?(type), (Attachment)attachment);
		}

		public Attachment Create(AttachmentType type)
		{
			EnumValidator.ThrowIfInvalid<AttachmentType>(type, new AttachmentType[]
			{
				AttachmentType.Stream,
				AttachmentType.EmbeddedMessage,
				AttachmentType.Ole,
				AttachmentType.Reference
			});
			Attachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.CoreAttachmentCollection.Create(type);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				result = AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(type));
				disposeGuard.Success();
			}
			return result;
		}

		public Attachment Create(AttachmentType? type, Attachment clone)
		{
			if (type != null)
			{
				EnumValidator.ThrowIfInvalid<AttachmentType>(type.Value, "type");
			}
			Util.ThrowOnNullArgument(clone, "clone");
			Attachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.CoreAttachmentCollection.InternalCreateCopy(type, clone.CoreAttachment);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				result = AttachmentCollection.CreateTypedAttachment(coreAttachment, type);
				disposeGuard.Success();
			}
			return result;
		}

		public ItemAttachment Create(StoreObjectType type)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(type, "type");
			ItemAttachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.CoreAttachmentCollection.CreateItemAttachment(type);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				result = (ItemAttachment)AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(AttachmentType.EmbeddedMessage));
				disposeGuard.Success();
			}
			return result;
		}

		public IAttachment OpenIAttachment(AttachmentHandle handle)
		{
			return this.Open(handle);
		}

		public Attachment Open(AttachmentId id)
		{
			return this.Open(id, null);
		}

		public Attachment Open(AttachmentId id, ICollection<PropertyDefinition> propertyDefinitions)
		{
			Util.ThrowOnNullArgument(id, "id");
			return AttachmentCollection.CreateTypedAttachment(this.CoreAttachmentCollection.Open(id, propertyDefinitions), null);
		}

		public Attachment Open(AttachmentHandle handle)
		{
			return this.Open(handle, null);
		}

		public Attachment Open(AttachmentHandle handle, ICollection<PropertyDefinition> propertyDefinitions)
		{
			Util.ThrowOnNullArgument(handle, "handle");
			Attachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.CoreAttachmentCollection.Open(handle, propertyDefinitions);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				result = AttachmentCollection.CreateTypedAttachment(coreAttachment, null);
				disposeGuard.Success();
			}
			return result;
		}

		public Attachment Open(AttachmentHandle handle, AttachmentType type)
		{
			Util.ThrowOnNullArgument(handle, "handle");
			EnumValidator.ThrowIfInvalid<AttachmentType>(type, "type");
			return this.Open(handle, new AttachmentType?(type), null);
		}

		internal static Attachment CreateTypedAttachment(CoreAttachment attachment, AttachmentType? type)
		{
			int? num = ((IDirectPropertyBag)attachment.PropertyBag).GetValue(InternalSchema.AttachMethod) as int?;
			if (num != null)
			{
				switch (num.Value)
				{
				case 2:
				case 3:
				case 4:
				case 7:
					if (type != null && !(type == AttachmentType.Reference))
					{
						return null;
					}
					return new ReferenceAttachment(attachment);
				case 5:
					if (type != null && !(type == AttachmentType.EmbeddedMessage))
					{
						return null;
					}
					return new ItemAttachment(attachment);
				case 6:
					if (type != null && !(type == AttachmentType.Ole))
					{
						return null;
					}
					return new OleAttachment(attachment);
				}
			}
			if (type != null && !(type == AttachmentType.Stream))
			{
				return null;
			}
			return new StreamAttachment(attachment);
		}

		internal Attachment Open(AttachmentHandle handle, AttachmentType? type, ICollection<PropertyDefinition> propertyDefinitions)
		{
			Util.ThrowOnNullArgument(handle, "handle");
			if (type != null)
			{
				EnumValidator.ThrowIfInvalid<AttachmentType>(type.Value, "type");
			}
			Attachment attachment = null;
			CoreAttachment coreAttachment = null;
			try
			{
				coreAttachment = this.CoreAttachmentCollection.Open(handle, propertyDefinitions);
				attachment = AttachmentCollection.CreateTypedAttachment(coreAttachment, type);
			}
			finally
			{
				if (attachment == null && coreAttachment != null)
				{
					coreAttachment.Dispose();
				}
			}
			return attachment;
		}

		internal Attachment TryOpenFirstAttachment(AttachmentType attachmentType)
		{
			using (IEnumerator<AttachmentHandle> enumerator = this.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AttachmentHandle handle = enumerator.Current;
					return this.Open(handle, new AttachmentType?(attachmentType), null);
				}
			}
			return null;
		}

		public bool Contains(AttachmentId attachmentId)
		{
			Util.ThrowOnNullArgument(attachmentId, "attachmentId");
			return this.CoreAttachmentCollection.Contains(attachmentId);
		}

		public bool Remove(AttachmentId attachmentId)
		{
			Util.ThrowOnNullArgument(attachmentId, "attachmentId");
			bool result = this.CoreAttachmentCollection.Remove(attachmentId);
			CalendarItemBase calendarItemBase = this.ContainerItem as CalendarItemBase;
			if (calendarItemBase != null)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(63349U, LastChangeAction.AttachmentRemoved);
			}
			return result;
		}

		public bool Remove(AttachmentHandle handle)
		{
			Util.ThrowOnNullArgument(handle, "handle");
			bool result = this.CoreAttachmentCollection.Remove(handle);
			CalendarItemBase calendarItemBase = this.ContainerItem as CalendarItemBase;
			if (calendarItemBase != null)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(38773U, LastChangeAction.AttachmentRemoved);
			}
			return result;
		}

		public void RemoveAll()
		{
			if (this.hideCalendarExceptions)
			{
				IList<AttachmentHandle> handles = this.GetHandles();
				using (IEnumerator<AttachmentHandle> enumerator = handles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AttachmentHandle handle = enumerator.Current;
						this.Remove(handle);
					}
					goto IL_46;
				}
			}
			this.CoreAttachmentCollection.RemoveAll();
			IL_46:
			CalendarItemBase calendarItemBase = this.ContainerItem as CalendarItemBase;
			if (calendarItemBase != null)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(55157U, LastChangeAction.AttachmentRemoved);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.CoreAttachmentCollection.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				if (this.hideCalendarExceptions)
				{
					int num = 0;
					foreach (AttachmentHandle handle in this.CoreAttachmentCollection)
					{
						if (!CoreAttachmentCollection.IsCalendarException(handle))
						{
							num++;
						}
					}
					return num;
				}
				return this.CoreAttachmentCollection.Count;
			}
		}

		public IEnumerator<AttachmentHandle> GetEnumerator()
		{
			if (this.hideCalendarExceptions)
			{
				return new AttachmentCollection.CalendarPublicAttachmentEnumerator(this.CoreAttachmentCollection.GetEnumerator());
			}
			return this.CoreAttachmentCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal Item ContainerItem
		{
			get
			{
				return this.parent;
			}
		}

		public IList<AttachmentHandle> GetHandles()
		{
			if (this.hideCalendarExceptions)
			{
				List<AttachmentHandle> list = new List<AttachmentHandle>(this.CoreAttachmentCollection.Count);
				foreach (AttachmentHandle attachmentHandle in this.CoreAttachmentCollection)
				{
					if (!CoreAttachmentCollection.IsCalendarException(attachmentHandle))
					{
						list.Add(attachmentHandle);
					}
				}
				return list;
			}
			return this.GetAllHandles();
		}

		public IList<AttachmentHandle> GetAllHandles()
		{
			return this.CoreAttachmentCollection.GetAllHandles();
		}

		internal bool IsDirty
		{
			get
			{
				return this.CoreAttachmentCollection.IsDirty;
			}
		}

		public CoreAttachmentCollection CoreAttachmentCollection
		{
			get
			{
				return this.parent.CoreItem.AttachmentCollection;
			}
		}

		private readonly Item parent;

		private readonly bool hideCalendarExceptions;

		private class CalendarPublicAttachmentEnumerator : IEnumerator<AttachmentHandle>, IDisposable, IEnumerator
		{
			public CalendarPublicAttachmentEnumerator(IEnumerator<AttachmentHandle> list)
			{
				this.list = list;
				this.isDisposed = false;
			}

			AttachmentHandle IEnumerator<AttachmentHandle>.Current
			{
				get
				{
					this.CheckDisposed();
					return this.list.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					this.CheckDisposed();
					return this.list.Current;
				}
			}

			public void Reset()
			{
				this.CheckDisposed();
				this.list.Reset();
			}

			public bool MoveNext()
			{
				while (this.list.MoveNext())
				{
					AttachmentHandle handle = this.list.Current;
					if (!CoreAttachmentCollection.IsCalendarException(handle))
					{
						return true;
					}
				}
				return false;
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			internal void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					this.list.Dispose();
				}
				this.isDisposed = true;
			}

			private void CheckDisposed()
			{
				if (this.isDisposed)
				{
					throw new ObjectDisposedException("CoreAttachmentCollection::AttachmentEnumerator");
				}
			}

			private IEnumerator<AttachmentHandle> list;

			private bool isDisposed;
		}
	}
}
