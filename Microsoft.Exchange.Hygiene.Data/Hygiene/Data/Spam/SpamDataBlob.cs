using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamDataBlob : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.SpamDataBlobID.ToString());
			}
		}

		internal Guid SpamDataBlobID
		{
			get
			{
				return (Guid)this[SpamDataBlob.SpamDataBlobIDProperty];
			}
			set
			{
				this[SpamDataBlob.SpamDataBlobIDProperty] = value;
			}
		}

		internal SpamDataBlobDataID DataID
		{
			get
			{
				return (SpamDataBlobDataID)this[SpamDataBlob.DataIDProperty];
			}
			set
			{
				this[SpamDataBlob.DataIDProperty] = (byte)value;
			}
		}

		internal byte DataTypeID
		{
			get
			{
				return (byte)this[SpamDataBlob.DataTypeIDProperty];
			}
			set
			{
				this[SpamDataBlob.DataTypeIDProperty] = value;
			}
		}

		internal int MajorVersion
		{
			get
			{
				return (int)this[SpamDataBlob.MajorVersionProperty];
			}
			set
			{
				this[SpamDataBlob.MajorVersionProperty] = value;
			}
		}

		internal int MinorVersion
		{
			get
			{
				return (int)this[SpamDataBlob.MinorVersionProperty];
			}
			set
			{
				this[SpamDataBlob.MinorVersionProperty] = value;
			}
		}

		internal int ChunkID
		{
			get
			{
				return (int)this[SpamDataBlob.ChunkIDProperty];
			}
			set
			{
				this[SpamDataBlob.ChunkIDProperty] = value;
			}
		}

		internal bool IsEndData
		{
			get
			{
				return (bool)this[SpamDataBlob.IsEndDataProperty];
			}
			set
			{
				this[SpamDataBlob.IsEndDataProperty] = value;
			}
		}

		internal byte[] DataChunk
		{
			get
			{
				return (byte[])this[SpamDataBlob.DataChunkProperty];
			}
			set
			{
				this[SpamDataBlob.DataChunkProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition SpamDataBlobIDProperty = new HygienePropertyDefinition("id_SpamDataBlobId", typeof(Guid));

		public static readonly HygienePropertyDefinition DataIDProperty = new HygienePropertyDefinition("ti_DataId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DataTypeIDProperty = new HygienePropertyDefinition("ti_DataTypeId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MajorVersionProperty = new HygienePropertyDefinition("i_MajorVersion", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MinorVersionProperty = new HygienePropertyDefinition("i_MinorVersion", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChunkIDProperty = new HygienePropertyDefinition("i_ChunkId", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsEndDataProperty = new HygienePropertyDefinition("f_IsEndData", typeof(bool));

		public static readonly HygienePropertyDefinition DataChunkProperty = new HygienePropertyDefinition("vb_DataChunk", typeof(byte[]));
	}
}
