using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class IcsState : DisposableBase, IIcsState, IDisposable, ISession
	{
		public IcsState()
		{
			this.statePropBag = new MemoryPropertyBag(this, ConfigurationSchema.MaxIcsStatePropertySize.Value);
			this.idsetGiven = new IcsState.IdSetWrapper(new PropertyTag(PropTag.IcsState.IdsetGiven.PropTag));
			this.cnsetSeen = new IcsState.IdSetWrapper(new PropertyTag(PropTag.IcsState.CnsetSeen.PropTag));
			this.cnsetSeenAssociated = new IcsState.IdSetWrapper(new PropertyTag(PropTag.IcsState.CnsetSeenFAI.PropTag));
			this.cnsetRead = new IcsState.IdSetWrapper(new PropertyTag(PropTag.IcsState.CnsetRead.PropTag));
		}

		public IdSet IdsetGiven
		{
			get
			{
				if (this.idsetGivenNeedsReload)
				{
					this.idsetGiven.Load(this.statePropBag);
					this.idsetGivenNeedsReload = false;
				}
				return this.idsetGiven.IdSet;
			}
		}

		public IdSet CnsetSeen
		{
			get
			{
				if (this.cnsetSeenNeedsReload)
				{
					this.cnsetSeen.Load(this.statePropBag);
					this.cnsetSeenNeedsReload = false;
				}
				return this.cnsetSeen.IdSet;
			}
		}

		public IdSet CnsetSeenAssociated
		{
			get
			{
				if (this.cnsetSeenAssociatedNeedsReload)
				{
					this.cnsetSeenAssociated.Load(this.statePropBag);
					this.cnsetSeenAssociatedNeedsReload = false;
				}
				return this.cnsetSeenAssociated.IdSet;
			}
		}

		public IdSet CnsetRead
		{
			get
			{
				if (this.cnsetReadNeedsReload)
				{
					this.cnsetRead.Load(this.statePropBag);
					this.cnsetReadNeedsReload = false;
				}
				return this.cnsetRead.IdSet;
			}
			set
			{
				this.cnsetRead.IdSet = value;
			}
		}

		public bool StateUploadStarted
		{
			get
			{
				return this.statePropertyUploadStream != null;
			}
		}

		IPropertyBag IIcsState.PropertyBag
		{
			get
			{
				this.Checkpoint();
				return this.statePropBag;
			}
		}

		void IIcsState.Flush()
		{
		}

		bool ISession.TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty)
		{
			namedProperty = null;
			return false;
		}

		bool ISession.TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag)
		{
			return false;
		}

		public ErrorCode BeginUploadStateProperty(StorePropTag propTag, uint size)
		{
			if (this.StateUploadStarted)
			{
				return ErrorCode.CreateCallFailed((LID)41112U);
			}
			if (propTag != PropTag.IcsState.IdsetGiven && propTag != PropTag.IcsState.IdsetGivenInt32 && propTag != PropTag.IcsState.CnsetSeen && propTag != PropTag.IcsState.CnsetSeenFAI && propTag != PropTag.IcsState.CnsetRead)
			{
				return ErrorCode.CreateNotSupported((LID)57496U);
			}
			this.statePropertyUploadStream = this.OpenPropertyWriteStream(propTag, size);
			this.declaredStatePropertySize = size;
			return ErrorCode.NoError;
		}

		public ErrorCode ContinueUploadStateProperty(ArraySegment<byte> data)
		{
			if (!this.StateUploadStarted)
			{
				return ErrorCode.CreateCallFailed((LID)32920U);
			}
			if (this.statePropertyUploadStream.Length + (long)data.Count > (long)((ulong)this.declaredStatePropertySize))
			{
				return ErrorCode.CreateNotSupported((LID)49304U);
			}
			this.statePropertyUploadStream.Write(data.Array, data.Offset, data.Count);
			return ErrorCode.NoError;
		}

		public ErrorCode EndUploadStateProperty()
		{
			if (!this.StateUploadStarted)
			{
				return ErrorCode.CreateCallFailed((LID)48792U);
			}
			ErrorCode result = ErrorCode.NoError;
			if (this.statePropertyUploadStream.Length != (long)((ulong)this.declaredStatePropertySize))
			{
				result = ErrorCode.CreateCallFailed((LID)65176U);
			}
			this.statePropertyUploadStream.Dispose();
			this.statePropertyUploadStream = null;
			return result;
		}

		public ErrorCode OpenIcsStateDownloadContext(MapiLogon logon, out FastTransferDownloadContext outputContext)
		{
			if (this.statePropertyUploadStream != null)
			{
				throw new ExExceptionNoSupport((LID)60216U, "Unfinished state property upload.");
			}
			FastTransferDownloadContext fastTransferDownloadContext = new FastTransferDownloadContext(Array<PropertyTag>.Empty);
			ErrorCode errorCode = fastTransferDownloadContext.Configure(logon, FastTransferSendOption.UseMAPI, (MapiContext operationContext) => new FastTransferIcsState(this));
			if (errorCode != ErrorCode.NoError)
			{
				fastTransferDownloadContext.Dispose();
				fastTransferDownloadContext = null;
			}
			outputContext = fastTransferDownloadContext;
			return errorCode;
		}

		public Stream OpenPropertyWriteStream(StorePropTag propTag, uint size)
		{
			ushort propId = propTag.PropId;
			if (propId <= 26518)
			{
				if (propId != 16407)
				{
					if (propId == 26518)
					{
						this.cnsetSeenNeedsReload = true;
					}
				}
				else
				{
					this.idsetGivenNeedsReload = true;
					if (propTag == PropTag.IcsState.IdsetGivenInt32)
					{
						propTag = PropTag.IcsState.IdsetGiven;
					}
				}
			}
			else if (propId != 26578)
			{
				if (propId == 26586)
				{
					this.cnsetSeenAssociatedNeedsReload = true;
				}
			}
			else
			{
				this.cnsetReadNeedsReload = true;
			}
			return this.statePropBag.SetPropertyStream(new PropertyTag(propTag.PropTag), (long)((ulong)size));
		}

		public void Checkpoint()
		{
			if (this.idsetGiven.IsDirty)
			{
				this.idsetGiven.Save(this.statePropBag);
			}
			if (this.cnsetSeen.IsDirty)
			{
				this.cnsetSeen.Save(this.statePropBag);
			}
			if (this.cnsetSeenAssociated.IsDirty)
			{
				this.cnsetSeenAssociated.Save(this.statePropBag);
			}
			if (this.cnsetRead.IsDirty)
			{
				this.cnsetRead.Save(this.statePropBag);
			}
		}

		public void ReloadIfNecessary()
		{
			if (this.idsetGivenNeedsReload)
			{
				this.idsetGiven.Load(this.statePropBag);
				this.idsetGivenNeedsReload = false;
			}
			if (this.cnsetSeenNeedsReload)
			{
				this.cnsetSeen.Load(this.statePropBag);
				this.cnsetSeenNeedsReload = false;
			}
			if (this.cnsetSeenAssociatedNeedsReload)
			{
				this.cnsetSeenAssociated.Load(this.statePropBag);
				this.cnsetSeenAssociatedNeedsReload = false;
			}
			if (this.cnsetReadNeedsReload)
			{
				this.cnsetRead.Load(this.statePropBag);
				this.cnsetReadNeedsReload = false;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("IdsetGiven:");
			stringBuilder.Append(this.idsetGiven.IdSet.ToString());
			stringBuilder.Append("  CnsetSeen:");
			stringBuilder.Append(this.cnsetSeen.IdSet.ToString());
			stringBuilder.Append("  CnsetSeenAssoc:");
			stringBuilder.Append(this.cnsetSeenAssociated.IdSet.ToString());
			stringBuilder.Append("  CnsetRead:");
			stringBuilder.Append(this.cnsetRead.IdSet.ToString());
			return stringBuilder.ToString();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IcsState>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.statePropertyUploadStream != null)
			{
				this.statePropertyUploadStream.Dispose();
				this.statePropertyUploadStream = null;
			}
		}

		public static readonly ExchangeId PerUserIdsetIndicator = ExchangeId.Create(ReplidGuidMap.ReservedPerUserIndicatorGuid, 1UL, 1);

		public static readonly ExchangeId NonPerUserIdsetIndicator = ExchangeId.Create(ReplidGuidMap.ReservedNonPerUserIndicatorGuid, 1UL, 1);

		private MemoryPropertyBag statePropBag;

		private IcsState.IdSetWrapper idsetGiven;

		private bool idsetGivenNeedsReload;

		private IcsState.IdSetWrapper cnsetSeen;

		private bool cnsetSeenNeedsReload;

		private IcsState.IdSetWrapper cnsetSeenAssociated;

		private bool cnsetSeenAssociatedNeedsReload;

		private IcsState.IdSetWrapper cnsetRead;

		private bool cnsetReadNeedsReload;

		private Stream statePropertyUploadStream;

		private uint declaredStatePropertySize;

		internal class IdSetWrapper
		{
			public IdSetWrapper(PropertyTag propertyTag)
			{
				this.propertyTag = propertyTag;
			}

			public bool IsDirty
			{
				get
				{
					return this.idset != null && this.idset.IsDirty;
				}
			}

			public IdSet IdSet
			{
				get
				{
					if (this.idset == null)
					{
						this.idset = new IdSet();
					}
					return this.idset;
				}
				set
				{
					this.idset = value;
					this.idset.IsDirty = true;
				}
			}

			public void Load(IPropertyBag propertyBag)
			{
				IdSet idSet = null;
				if (!propertyBag.GetAnnotatedProperty(this.propertyTag).PropertyValue.IsNotFound)
				{
					using (Stream propertyStream = propertyBag.GetPropertyStream(this.propertyTag))
					{
						using (Reader reader = Reader.CreateStreamReader(propertyStream))
						{
							try
							{
								idSet = IdSet.ThrowableParse(reader);
							}
							catch (StoreException innerException)
							{
								throw new RopExecutionException("Invalid IdSet format.", ErrorCode.IdSetFormatError, innerException);
							}
							long num = propertyStream.Length - propertyStream.Position;
							if (num != 0L)
							{
								throw new RopExecutionException(string.Format("Property stream contained {0} more bytes after parsing an IdSet", num), ErrorCode.IdSetFormatError);
							}
						}
					}
				}
				this.idset = idSet;
			}

			public void Save(IPropertyBag propertyBag)
			{
				if (this.idset != null)
				{
					using (Stream stream = propertyBag.SetPropertyStream(this.propertyTag, 512L))
					{
						using (Writer writer = new StreamWriter(stream))
						{
							this.idset.Serialize(writer);
						}
						goto IL_5D;
					}
				}
				propertyBag.SetProperty(new PropertyValue(this.propertyTag, new byte[0]));
				IL_5D:
				this.idset.IsDirty = false;
			}

			public const int DefaultIcsStateStreamCapacity = 512;

			private PropertyTag propertyTag;

			private IdSet idset;
		}
	}
}
