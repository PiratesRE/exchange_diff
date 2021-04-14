using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal class ADDirSyncCookie
	{
		internal ADDirSyncCookie(Guid invocationId, long objectUpdateSequenceNumber, long propertyUpdateSequenceNumber, ADReplicationCursorCollection cursors)
		{
			if (cursors == null)
			{
				throw new ArgumentNullException("cursors");
			}
			this.invocationId = invocationId;
			this.objectUpdateSequenceNumber = objectUpdateSequenceNumber;
			this.propertyUpdateSequenceNumber = propertyUpdateSequenceNumber;
			this.cursors = cursors;
		}

		public Guid InvocationId
		{
			get
			{
				return this.invocationId;
			}
		}

		public long ObjectUpdateSequenceNumber
		{
			get
			{
				return this.objectUpdateSequenceNumber;
			}
		}

		public long PropertyUpdateSequenceNumber
		{
			get
			{
				return this.propertyUpdateSequenceNumber;
			}
		}

		public ADReplicationCursorCollection Cursors
		{
			get
			{
				return this.cursors;
			}
		}

		public bool MoreData
		{
			get
			{
				return this.objectUpdateSequenceNumber != this.propertyUpdateSequenceNumber;
			}
		}

		public static ADDirSyncCookie Parse(byte[] binaryCookie)
		{
			if (binaryCookie == null)
			{
				throw new ArgumentNullException("binaryCookie");
			}
			Exception innerException = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(binaryCookie))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						byte[] first = binaryReader.ReadBytes(4);
						if (first.SequenceEqual(ADDirSyncCookie.Header) && binaryReader.ReadInt32() == 3)
						{
							binaryReader.ReadInt64();
							binaryReader.ReadInt64();
							int num = binaryReader.ReadInt32();
							long num2 = binaryReader.ReadInt64();
							binaryReader.ReadInt64();
							long num3 = binaryReader.ReadInt64();
							Guid guid = new Guid(binaryReader.ReadBytes(16));
							byte[] binary = binaryReader.ReadBytes(num);
							ADReplicationCursorCollection adreplicationCursorCollection = (num == 0) ? new ADReplicationCursorCollection() : ADReplicationCursorCollection.Parse(binary);
							return new ADDirSyncCookie(guid, num2, num3, adreplicationCursorCollection);
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
			throw new FormatException("Unrecognized cookie format.", innerException);
		}

		public byte[] ToByteArray()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					byte[] array = (this.Cursors.Count == 0) ? new byte[0] : this.Cursors.ToByteArray(1);
					binaryWriter.Write(ADDirSyncCookie.Header);
					binaryWriter.Write(3);
					binaryWriter.Write(0L);
					binaryWriter.Write(0L);
					binaryWriter.Write(array.Length);
					binaryWriter.Write(this.ObjectUpdateSequenceNumber);
					binaryWriter.Write(0L);
					binaryWriter.Write(this.PropertyUpdateSequenceNumber);
					binaryWriter.Write(this.InvocationId.ToByteArray());
					binaryWriter.Write(array);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "InvocationId={0} ObjectUsn={1} PropertyUsn={2} MoreData={3} Cursors={{{4}}}", new object[]
			{
				this.InvocationId,
				this.ObjectUpdateSequenceNumber,
				this.PropertyUpdateSequenceNumber,
				this.MoreData,
				this.Cursors
			});
		}

		private const int Version = 3;

		private static readonly byte[] Header = new byte[]
		{
			77,
			83,
			68,
			83
		};

		private readonly Guid invocationId;

		private readonly long objectUpdateSequenceNumber;

		private readonly long propertyUpdateSequenceNumber;

		private readonly ADReplicationCursorCollection cursors;
	}
}
