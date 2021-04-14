using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActivityMessageAdapter : DisposableObject, IMessage, IDisposable
	{
		public ActivityMessageAdapter(Action<MemoryPropertyBag> saveAction)
		{
			Util.ThrowOnNullArgument(saveAction, "saveAction");
			this.saveAction = saveAction;
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBagAdapter = new ActivityPropertyBagAdapter(this.propertyBag);
		}

		public ActivityMessageAdapter(MemoryPropertyBag propertyBag)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			this.propertyBag = propertyBag;
			this.propertyBagAdapter = new ActivityPropertyBagAdapter(this.propertyBag);
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBagAdapter;
			}
		}

		public bool IsAssociated
		{
			get
			{
				return false;
			}
		}

		public IEnumerable<IRecipient> GetRecipients()
		{
			yield break;
		}

		public IRecipient CreateRecipient()
		{
			throw new NotSupportedException("Activities are not supposed to have any recipients.");
		}

		public void RemoveRecipient(int rowId)
		{
			throw new NotSupportedException("Cannot remove recipient. Activities are not supposed to have any recipients.");
		}

		public IEnumerable<IAttachmentHandle> GetAttachments()
		{
			yield break;
		}

		public IAttachment CreateAttachment()
		{
			throw new NotSupportedException("Activities are not supposed to have any attachments.");
		}

		public void Save()
		{
			if (this.saveAction != null)
			{
				this.saveAction(this.propertyBag);
			}
		}

		public void SetLongTermId(StoreLongTermId longTermId)
		{
			throw new NotSupportedException("Activities are not supposed to have any long term ID.");
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityMessageAdapter>(this);
		}

		private readonly MemoryPropertyBag propertyBag;

		private readonly ActivityPropertyBagAdapter propertyBagAdapter;

		private readonly Action<MemoryPropertyBag> saveAction;
	}
}
