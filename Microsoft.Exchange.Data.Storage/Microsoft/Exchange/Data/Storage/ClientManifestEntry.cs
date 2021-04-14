using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientManifestEntry : ServerManifestEntry
	{
		public ClientManifestEntry()
		{
		}

		public ClientManifestEntry(ISyncItemId id) : base(id)
		{
			this.clientAddId = null;
			base.Watermark = null;
		}

		public string ClientAddId
		{
			get
			{
				return this.clientAddId;
			}
			set
			{
				this.clientAddId = value;
			}
		}

		public bool SoftDeletePending
		{
			get
			{
				return this.softDeletePending;
			}
			set
			{
				this.softDeletePending = value;
			}
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			base.DeserializeData(reader, componentDataPool);
			StringData stringDataInstance = componentDataPool.GetStringDataInstance();
			stringDataInstance.DeserializeData(reader, componentDataPool);
			this.clientAddId = stringDataInstance.Data;
			this.softDeletePending = reader.ReadBoolean();
		}

		public override void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			base.SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.clientAddId).SerializeData(writer, componentDataPool);
			writer.Write(this.softDeletePending);
		}

		private string clientAddId;

		private bool softDeletePending;
	}
}
