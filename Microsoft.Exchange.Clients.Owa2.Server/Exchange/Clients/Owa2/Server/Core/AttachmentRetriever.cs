using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentRetriever : DisposeTrackableBase, AttachmentHandler.IAttachmentRetriever, IDisposable
	{
		private AttachmentRetriever(string id, IdConverterDependencies converterDependencies)
		{
			bool flag = false;
			try
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string>((long)this.GetHashCode(), "Initialize IdAndSession to get attachment, id = {0}", id);
				List<AttachmentId> attachmentIds = new List<AttachmentId>();
				IdHeaderInformation headerInformation = ServiceIdConverter.ConvertFromConcatenatedId(id, BasicTypes.ItemOrAttachment, attachmentIds);
				Item item = null;
				IdAndSession idAndSession;
				try
				{
					idAndSession = IdConverter.ConvertId(converterDependencies, headerInformation, IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.NoBind, BasicTypes.ItemOrAttachment, attachmentIds, null, this.GetHashCode(), false, ref item);
				}
				finally
				{
					if (item != null)
					{
						item.Dispose();
						item = null;
					}
				}
				this.attachmentHierarchy = new AttachmentHierarchy(idAndSession, false, true);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public BlockStatus BlockStatus
		{
			get
			{
				if (this.blockStatus == null)
				{
					this.blockStatus = new BlockStatus?(BlockStatus.DontKnow);
					object obj = this.attachmentHierarchy.RootItem.TryGetProperty(ItemSchema.BlockStatus);
					if (obj is BlockStatus)
					{
						this.blockStatus = new BlockStatus?((BlockStatus)obj);
					}
				}
				return this.blockStatus.Value;
			}
		}

		public Attachment Attachment
		{
			get
			{
				if (this.attachmentHierarchy.Last != null)
				{
					return this.attachmentHierarchy.Last.Attachment;
				}
				return null;
			}
		}

		public Item RootItem
		{
			get
			{
				return this.attachmentHierarchy.RootItem;
			}
		}

		internal static AttachmentHandler.IAttachmentRetriever CreateInstance(string id, CallContext callContext)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty");
			}
			if (callContext == null)
			{
				throw new ArgumentException("callContext cannot be null");
			}
			IdConverterDependencies converterDependencies = new IdConverterDependencies.FromCallContext(callContext);
			return new AttachmentRetriever(id, converterDependencies);
		}

		internal static AttachmentHandler.IAttachmentRetriever CreateInstance(string id, IdConverterDependencies converterDependencies)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty");
			}
			if (converterDependencies == null)
			{
				throw new ArgumentException("converterDependencies cannot be null");
			}
			return new AttachmentRetriever(id, converterDependencies);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.attachmentHierarchy != null)
			{
				this.attachmentHierarchy.Dispose();
				this.attachmentHierarchy = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AttachmentRetriever>(this);
		}

		private BlockStatus? blockStatus;

		private AttachmentHierarchy attachmentHierarchy;
	}
}
