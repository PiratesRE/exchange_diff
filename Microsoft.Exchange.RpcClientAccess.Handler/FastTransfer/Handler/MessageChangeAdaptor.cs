using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MessageChangeAdaptor : BaseObject, IMessageChange, IDisposable
	{
		internal MessageChangeAdaptor(IPropertyBag messageHeaderPropertyBag, MessageAdaptor message)
		{
			this.messageHeaderPropertyBag = messageHeaderPropertyBag;
			this.message = message;
		}

		public IMessage Message
		{
			get
			{
				base.CheckDisposed();
				return this.message;
			}
		}

		public int MessageSize
		{
			get
			{
				base.CheckDisposed();
				return this.messageHeaderPropertyBag.GetAnnotatedProperty(PropertyTag.MessageSize).PropertyValue.GetServerValue<int>();
			}
		}

		public bool IsAssociatedMessage
		{
			get
			{
				base.CheckDisposed();
				return this.messageHeaderPropertyBag.GetAnnotatedProperty(PropertyTag.Associated).PropertyValue.GetServerValue<bool>();
			}
		}

		public IPropertyBag MessageHeaderPropertyBag
		{
			get
			{
				base.CheckDisposed();
				return this.messageHeaderPropertyBag;
			}
		}

		public IMessageChangePartial PartialChange
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.message);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageChangeAdaptor>(this);
		}

		private readonly IPropertyBag messageHeaderPropertyBag;

		private readonly IMessage message;
	}
}
