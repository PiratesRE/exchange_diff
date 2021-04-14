using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferAttachment : FastTransferPropertyBag, IAttachment, IDisposable, IAttachmentHandle
	{
		public FastTransferAttachment(FastTransferDownloadContext downloadContext, MapiAttachment mapiAttachment, bool excludeProps, HashSet<StorePropTag> propList, bool topLevel, FastTransferCopyFlag flags) : base(downloadContext, mapiAttachment, excludeProps, propList)
		{
			this.topLevel = topLevel;
			this.flags = flags;
		}

		public FastTransferAttachment(FastTransferUploadContext uploadContext, MapiAttachment mapiAttachment, bool topLevel, FastTransferCopyFlag flags) : base(uploadContext, mapiAttachment)
		{
			this.topLevel = topLevel;
			this.flags = flags;
		}

		private MapiAttachment MapiAttachment
		{
			get
			{
				return (MapiAttachment)base.MapiPropBag;
			}
			set
			{
				base.MapiPropBag = value;
			}
		}

		public IAttachment GetAttachment()
		{
			return this;
		}

		public IMessage GetEmbeddedMessage()
		{
			if (base.ReadOnly)
			{
				MapiMessage mapiMessage;
				ErrorCode errorCode = this.MapiAttachment.OpenEmbeddedMessage(base.Context.CurrentOperationContext, MessageConfigureFlags.None, this.MapiAttachment.Logon.CodePage, out mapiMessage);
				if (errorCode != ErrorCode.NoError)
				{
					throw new StoreException((LID)63544U, errorCode);
				}
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, "Send Embedded Message");
				return new FastTransferMessage(base.DownloadContext, mapiMessage, true, null, false, false, false, this.flags);
			}
			else
			{
				MapiMessage mapiMessage;
				ErrorCode errorCode2 = this.MapiAttachment.OpenEmbeddedMessage(base.Context.CurrentOperationContext, MessageConfigureFlags.CreateNewMessage, this.MapiAttachment.Logon.CodePage, out mapiMessage);
				if (errorCode2 != ErrorCode.NoError)
				{
					throw new StoreException((LID)38968U, errorCode2);
				}
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, "Receive Embedded Message");
				return new FastTransferMessage(base.UploadContext, mapiMessage, false, this.flags);
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				return this.MapiAttachment.IsEmbeddedMessage(base.Context.CurrentOperationContext);
			}
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this;
			}
		}

		public void Save()
		{
			this.MapiAttachment.SaveChanges(base.Context.CurrentOperationContext);
		}

		public int AttachmentNumber
		{
			get
			{
				return this.MapiAttachment.GetAttachmentNumber();
			}
		}

		protected override List<Property> LoadAllPropertiesImp()
		{
			List<Property> list = base.LoadAllPropertiesImp();
			if (base.ForMoveUser)
			{
				FastTransferPropertyBag.AddNullPropertyIfNotPresent(list, PropTag.Attachment.ReplicaChangeNumber);
				FastTransferPropertyBag.ResetPropertyIfPresent(list, PropTag.Attachment.Inid);
			}
			return list;
		}

		protected override Property GetPropertyImp(StorePropTag propTag)
		{
			if (base.ForMoveUser && propTag == PropTag.Attachment.Inid)
			{
				return new Property(PropTag.Attachment.LTID, this.MapiAttachment.Logon.StoreMailbox.GetNextObjectId(base.Context.CurrentOperationContext).To24ByteArray());
			}
			if (propTag == PropTag.Attachment.Content || propTag == PropTag.Attachment.ContentObj)
			{
				return Property.NotEnoughMemoryError(propTag);
			}
			return base.GetPropertyImp(propTag);
		}

		protected override bool IncludeTag(StorePropTag propTag)
		{
			if (base.ForMoveUser && propTag.IsCategory(4))
			{
				return true;
			}
			if (!base.ForMoveUser && !base.ForUpload && 26112 <= propTag.PropId && propTag.PropId <= 26623)
			{
				return false;
			}
			ushort propId = propTag.PropId;
			if (propId == 3617)
			{
				return false;
			}
			if (propId != 4094)
			{
				if (propId == 14081)
				{
					if (this.IsEmbeddedMessage)
					{
						return false;
					}
				}
				return base.IncludeTag(propTag);
			}
			return false;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferAttachment>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
			if (isCalledFromDispose && this.MapiAttachment != null && !this.topLevel)
			{
				this.MapiAttachment.Dispose();
				this.MapiAttachment = null;
			}
			base.InternalDispose(isCalledFromDispose);
		}

		private bool topLevel;

		private FastTransferCopyFlag flags;
	}
}
