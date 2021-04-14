using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal class ADReplicationCursorCollection : KeyedCollection<Guid, ReplicationCursor>
	{
		public ADReplicationCursorCollection()
		{
		}

		public ADReplicationCursorCollection(IEnumerable<ReplicationCursor> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (ReplicationCursor item in collection)
			{
				base.Add(item);
			}
		}

		public static ADReplicationCursorCollection Parse(byte[] binary)
		{
			if (binary == null)
			{
				throw new ArgumentNullException("binary");
			}
			ADReplicationCursorCollection adreplicationCursorCollection = new ADReplicationCursorCollection();
			Exception innerException = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(binary))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num = binaryReader.ReadInt32();
						if (num == 1)
						{
							binaryReader.ReadInt32();
							int num2 = binaryReader.ReadInt32();
							binaryReader.ReadInt32();
							while (num2-- > 0)
							{
								Guid sourceInvocationId = new Guid(binaryReader.ReadBytes(16));
								long upToDatenessUsn = binaryReader.ReadInt64();
								adreplicationCursorCollection.Add(new ReplicationCursor(sourceInvocationId, upToDatenessUsn, DateTime.MinValue, null));
							}
							return adreplicationCursorCollection;
						}
					}
				}
			}
			catch (ArgumentException ex)
			{
				innerException = ex;
			}
			catch (IOException ex2)
			{
				innerException = ex2;
			}
			throw new FormatException("Unrecognized format.", innerException);
		}

		public byte[] ToByteArray(int version)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (version == 1)
					{
						binaryWriter.Write(version);
						binaryWriter.Write(0);
						binaryWriter.Write(base.Count);
						binaryWriter.Write(0);
						foreach (ReplicationCursor replicationCursor in this)
						{
							binaryWriter.Write(replicationCursor.SourceInvocationId.ToByteArray());
							binaryWriter.Write(replicationCursor.UpToDatenessUsn);
						}
						return memoryStream.ToArray();
					}
				}
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Version '{0}' is not supported.", new object[]
			{
				version
			}));
		}

		public long GetNextUpdateSequenceNumber(Guid invocationId)
		{
			if (!base.Contains(invocationId))
			{
				return 0L;
			}
			return 1L + base[invocationId].UpToDatenessUsn;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ReplicationCursor replicationCursor in this)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(replicationCursor.ToString());
			}
			return stringBuilder.ToString();
		}

		protected override Guid GetKeyForItem(ReplicationCursor item)
		{
			return item.SourceInvocationId;
		}
	}
}
