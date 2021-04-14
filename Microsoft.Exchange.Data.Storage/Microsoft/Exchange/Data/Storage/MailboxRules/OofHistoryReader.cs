using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OofHistoryReader
	{
		public int EntryCount
		{
			get
			{
				this.ThrowIfNotInitialized();
				return this.entryCount;
			}
		}

		public bool HasMoreEntries
		{
			get
			{
				this.ThrowIfNotInitialized();
				return this.entriesRead < this.entryCount;
			}
		}

		public IList<byte> CurrentEntryAddressBytes
		{
			get
			{
				this.ThrowIfNotInitialized();
				if (this.currentEntryAddressBytes == null)
				{
					throw new InvalidOperationException("Read an entry first.");
				}
				return this.currentEntryAddressBytes;
			}
		}

		public IList<byte> CurrentEntryRuleIdBytes
		{
			get
			{
				this.ThrowIfNotInitialized();
				if (this.currentEntryRuleIdBytes == null)
				{
					throw new InvalidOperationException("Read an entry first.");
				}
				return this.currentEntryRuleIdBytes;
			}
		}

		public void Initialize(Stream oofHistoryStream)
		{
			this.oofHistoryStream = oofHistoryStream;
			if (this.data != null)
			{
				throw new InvalidOperationException("Initialize can only be called once.");
			}
			this.data = new byte[32768];
			this.dataLength = 0;
			this.dataPosition = 0;
			this.LoadBuffer();
			this.ReadHeader();
			this.initialized = true;
		}

		public void ReadEntry()
		{
			this.ThrowIfNotInitialized();
			if (!this.HasMoreEntries)
			{
				throw new InvalidOperationException("There are no more entries to read.");
			}
			if (this.currentEntryAddressBytes == null)
			{
				this.currentEntryAddressBytes = new VirtualList<byte>(true);
			}
			if (this.currentEntryRuleIdBytes == null)
			{
				this.currentEntryRuleIdBytes = new VirtualList<byte>(true);
			}
			if (!this.EnsureDataSufficiency(1))
			{
				throw new OofHistoryCorruptionException(string.Concat(new object[]
				{
					"Insufficient data in stream to read property count for entry ",
					this.entriesRead,
					" at buffer position ",
					this.dataPosition
				}));
			}
			byte b = this.data[this.dataPosition++];
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				if (!this.EnsureDataSufficiency(3))
				{
					throw new OofHistoryCorruptionException(string.Concat(new object[]
					{
						"Insufficient data in stream to read property id and size. Current entry ",
						this.entriesRead,
						", current property: ",
						b2,
						", total entries: ",
						this.entryCount
					}));
				}
				OofHistory.PropId propId = (OofHistory.PropId)this.data[this.dataPosition];
				ushort num = BitConverter.ToUInt16(this.data, this.dataPosition + 1);
				if (num > 1000)
				{
					throw new OofHistoryCorruptionException(string.Concat(new object[]
					{
						"Current entry: ",
						this.entriesRead,
						"Property with id ",
						propId,
						" has property size: ",
						num,
						", which is over the maximum allowed size of ",
						1000
					}));
				}
				this.dataPosition += 3;
				if (!this.EnsureDataSufficiency((int)num))
				{
					throw new OofHistoryCorruptionException(string.Concat(new object[]
					{
						"Insufficient data in stream to match property values.  Current entry: ",
						this.entriesRead,
						", property with id ",
						this.data[this.dataPosition],
						" has property size: ",
						num
					}));
				}
				switch (propId)
				{
				case OofHistory.PropId.SenderAddress:
					this.currentEntryAddressBytes.SetRange(this.data, this.dataPosition, (int)num);
					break;
				case OofHistory.PropId.GlobalRuleId:
					this.currentEntryRuleIdBytes.SetRange(this.data, this.dataPosition, (int)num);
					break;
				}
				this.dataPosition += (int)num;
			}
			this.entriesRead++;
		}

		private void ReadHeader()
		{
			if (this.dataLength < 6)
			{
				throw new OofHistoryCorruptionException("OOF history initial bytes corrupted.  Must have at least 6 bytes.");
			}
			this.entryCount = BitConverter.ToInt32(this.data, 2);
			if (this.entryCount < 0)
			{
				throw new OofHistoryCorruptionException("OOF history initial bytes corrupted.  Entry count is negative.");
			}
			if (10000 < this.entryCount)
			{
				this.entryCount = 10000;
			}
			this.dataPosition = 6;
		}

		private bool EnsureDataSufficiency(int bytesRequired)
		{
			this.ThrowIfNotInitialized();
			if (this.dataLength < this.dataPosition + bytesRequired)
			{
				if (this.dataLength < 32768)
				{
					return false;
				}
				this.LoadBuffer();
				if (this.dataLength < bytesRequired)
				{
					return false;
				}
				this.dataPosition = 0;
			}
			return true;
		}

		private void LoadBuffer()
		{
			this.dataLength -= this.dataPosition;
			if (this.dataPosition != 0 && this.dataLength != 0)
			{
				if (this.dataPosition < 16384)
				{
					throw new OofHistoryCorruptionException("Remaining valid data is more than half the buffer size.");
				}
				Buffer.BlockCopy(this.data, this.dataPosition, this.data, 0, this.dataLength);
			}
			this.dataLength = this.oofHistoryStream.Read(this.data, this.dataLength, 32768 - this.dataLength) + this.dataLength;
		}

		private void ThrowIfNotInitialized()
		{
			if (!this.initialized)
			{
				throw new InvalidOperationException("Call the Initialize method first.");
			}
		}

		private const int DefaultBufferSize = 32768;

		private byte[] data;

		private int dataPosition;

		private int dataLength;

		private Stream oofHistoryStream;

		private bool initialized;

		private int entryCount;

		private int entriesRead;

		private VirtualList<byte> currentEntryAddressBytes;

		private VirtualList<byte> currentEntryRuleIdBytes;
	}
}
