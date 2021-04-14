using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class SyncContentsManifestState : XMLSerializableBase
	{
		public SyncContentsManifestState()
		{
			uint idsGivenPropTag = FastTransferIcsState.IdsetGiven;
			this.folderId = null;
			this.data = null;
			this.idSetGiven = new Lazy<IdSet>(() => MapiStore.GetIdSetFromMapiManifestBlob((PropTag)idsGivenPropTag, this.Data));
		}

		[XmlElement]
		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		[XmlElement]
		public byte[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		[XmlIgnore]
		private IdSet IdSetGiven
		{
			get
			{
				return this.idSetGiven.Value;
			}
		}

		public MemoryStream GetDataStream()
		{
			MemoryStream memoryStream = new MemoryStream(this.data.Length);
			memoryStream.Write(this.data, 0, this.data.Length);
			return memoryStream;
		}

		public bool IdSetGivenContainsEntryId(byte[] entryId)
		{
			try
			{
				if (this.Data == null)
				{
					MrsTracer.Common.Warning("Data not created to generate idset.  default to true", new object[0]);
					return true;
				}
				if (this.IdSetGiven == null)
				{
					MrsTracer.Common.Warning("Couldn't generate idset to check. default to true", new object[0]);
					return true;
				}
				return this.IdSetGiven.Contains(IdConverter.MessageGuidGlobCountFromEntryId(entryId));
			}
			catch (MapiPermanentException ex)
			{
				MrsTracer.Common.Warning("Couldn't generate idset to check. default to true {0}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex)
				});
			}
			catch (MapiRetryableException ex2)
			{
				MrsTracer.Common.Warning("Couldn't generate idset to check. default to true {0}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex2)
				});
			}
			return true;
		}

		private byte[] folderId;

		private byte[] data;

		private Lazy<IdSet> idSetGiven;
	}
}
