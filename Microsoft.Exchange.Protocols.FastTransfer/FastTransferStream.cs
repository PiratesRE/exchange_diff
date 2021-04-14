using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferStream : MapiPropBagBase
	{
		private FastTransferStream(FastTransferStreamMode mode) : base(MapiObjectType.FastTransferStream)
		{
			this.mode = mode;
		}

		public static FastTransferStream CreateNew(MapiContext context, MapiLogon logon, FastTransferStreamMode mode)
		{
			FastTransferStream fastTransferStream = null;
			bool flag = false;
			FastTransferStream result;
			try
			{
				fastTransferStream = new FastTransferStream(mode);
				fastTransferStream.ConfigureNew(context, logon);
				flag = true;
				result = fastTransferStream;
			}
			finally
			{
				if (!flag && fastTransferStream != null)
				{
					fastTransferStream.Dispose();
				}
			}
			return result;
		}

		public FastTransferState State
		{
			get
			{
				FastTransferDownloadContext fastTransferDownloadContext = this.streamContext as FastTransferDownloadContext;
				if (fastTransferDownloadContext != null)
				{
					return fastTransferDownloadContext.State;
				}
				FastTransferUploadContext fastTransferUploadContext = this.streamContext as FastTransferUploadContext;
				if (fastTransferUploadContext != null)
				{
					return fastTransferUploadContext.State;
				}
				return FastTransferState.Error;
			}
		}

		public int GetNextBuffer(MapiContext operationContext, ArraySegment<byte> buffer)
		{
			base.ThrowIfNotValid(null);
			if (this.mode != FastTransferStreamMode.Download)
			{
				throw new ExExceptionNoSupport((LID)38224U, "Stream was not opened for download");
			}
			if (this.streamContext == null)
			{
				this.streamContext = this.CreateContextForDownload(operationContext);
			}
			return ((FastTransferDownloadContext)this.streamContext).GetNextBuffer(operationContext, buffer);
		}

		public void PutNextBuffer(MapiContext operationContext, ArraySegment<byte> buffer)
		{
			base.ThrowIfNotValid(null);
			if (this.mode != FastTransferStreamMode.Upload)
			{
				throw new ExExceptionNoSupport((LID)54608U, "Stream was not opened for upload");
			}
			if (this.streamContext == null)
			{
				this.streamContext = this.CreateContextForUpload(operationContext);
			}
			((FastTransferUploadContext)this.streamContext).PutNextBuffer(operationContext, buffer);
		}

		public void Flush(MapiContext operationContext)
		{
			base.ThrowIfNotValid(null);
			if (this.mode != FastTransferStreamMode.Upload)
			{
				throw new ExExceptionNoSupport((LID)42320U, "Stream was not opened for upload");
			}
			((FastTransferUploadContext)this.streamContext).Flush(operationContext);
		}

		private void ConfigureNew(MapiContext context, MapiLogon logon)
		{
			this.configPropertyBag = new FastTransferStream.ConfigurationPropertyBag(logon.StoreMailbox);
			base.Logon = logon;
			base.ParentObject = logon;
			base.IsValid = true;
		}

		protected override PropertyBag StorePropertyBag
		{
			get
			{
				if (this.configPropertyBag == null)
				{
					throw new StoreException((LID)58704U, ErrorCodeValue.Rejected, "Configuration information cannot be accessed after upload or download started.");
				}
				return this.configPropertyBag;
			}
		}

		internal override void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid)
		{
		}

		internal override void CheckPropertyRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, AccessCheckOperation operation, LID lid)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferStream>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
			if (isCalledFromDispose && this.streamContext != null)
			{
				this.streamContext.Dispose();
				this.streamContext = null;
			}
			base.InternalDispose(isCalledFromDispose);
		}

		private FastTransferDownloadContext CreateContextForDownload(MapiContext context)
		{
			Guid? guid = base.GetOnePropValue(context, PropTag.FastTransferStream.InstanceGuid) as Guid?;
			if (guid == null)
			{
				throw new ExExceptionNoSupport((LID)34128U, "Invalid stream instance ID");
			}
			if (guid.Value == FastTransferStream.WellKnownIds.InferenceLog)
			{
				FastTransferDownloadContext fastTransferDownloadContext = new FastTransferDownloadContext(Array<PropertyTag>.Empty);
				bool flag = false;
				try
				{
					FastTransferDownloadContext capturedContext = fastTransferDownloadContext;
					fastTransferDownloadContext.Configure(base.Logon, FastTransferSendOption.Unicode, delegate(MapiContext operationContext)
					{
						InferenceLogIterator messageIterator = new InferenceLogIterator(capturedContext);
						return new FastTransferMessageIterator(messageIterator, FastTransferCopyMessagesFlag.None, true);
					});
					flag = true;
					return fastTransferDownloadContext;
				}
				finally
				{
					if (!flag && fastTransferDownloadContext != null)
					{
						fastTransferDownloadContext.Dispose();
					}
				}
			}
			throw new ExExceptionNoSupport((LID)36688U, "Unknown stream instance ID: " + guid.Value.ToString());
		}

		private FastTransferUploadContext CreateContextForUpload(MapiContext context)
		{
			Guid? guid = base.GetOnePropValue(context, PropTag.FastTransferStream.InstanceGuid) as Guid?;
			if (guid == null)
			{
				throw new ExExceptionNoSupport((LID)50512U, "Invalid stream instance ID");
			}
			if (guid.Value == FastTransferStream.WellKnownIds.InferenceLog)
			{
				FastTransferUploadContext fastTransferUploadContext = new FastTransferUploadContext();
				bool flag = false;
				try
				{
					FastTransferUploadContext capturedContext = fastTransferUploadContext;
					fastTransferUploadContext.Configure(base.Logon, delegate(MapiContext operationContext)
					{
						InferenceLogIterator messageIteratorClient = new InferenceLogIterator(capturedContext);
						return new FastTransferMessageIterator(messageIteratorClient, true);
					}, null, null);
					flag = true;
					return fastTransferUploadContext;
				}
				finally
				{
					if (!flag && fastTransferUploadContext != null)
					{
						fastTransferUploadContext.Dispose();
					}
				}
			}
			throw new ExExceptionNoSupport((LID)53072U, "Unknown stream instance ID: " + guid.Value.ToString());
		}

		private FastTransferContext streamContext;

		private FastTransferStream.ConfigurationPropertyBag configPropertyBag;

		private FastTransferStreamMode mode;

		public static class WellKnownIds
		{
			public static readonly Guid InferenceLog = new Guid("8ebdc484-475b-4d27-aaad-647e1cac4144");
		}

		private class ConfigurationPropertyBag : PrivatePropertyBag
		{
			public ConfigurationPropertyBag(Mailbox mailbox) : base(false)
			{
				mailbox.IsValid();
				this.mailbox = mailbox;
			}

			public override ObjectPropertySchema Schema
			{
				get
				{
					return null;
				}
			}

			public override Context CurrentOperationContext
			{
				get
				{
					return this.mailbox.CurrentOperationContext;
				}
			}

			public override ReplidGuidMap ReplidGuidMap
			{
				get
				{
					return null;
				}
			}

			protected override StorePropTag MapPropTag(Context context, uint propertyTag)
			{
				return this.mailbox.GetStorePropTag(context, propertyTag, ObjectType.FastTransferStream);
			}

			private readonly Mailbox mailbox;
		}
	}
}
