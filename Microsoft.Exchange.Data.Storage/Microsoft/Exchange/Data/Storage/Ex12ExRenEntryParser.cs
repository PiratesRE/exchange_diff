using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Ex12ExRenEntryParser
	{
		private Ex12ExRenEntryParser(byte[] additionalRenEntryIdsEx)
		{
			this.defaultFolderRenEx = new Dictionary<ushort, Ex12ExRenEntryParser.ExFolderEntry>();
			if (additionalRenEntryIdsEx == null)
			{
				this.entryBlob = Array<byte>.Empty;
				return;
			}
			this.entryBlob = additionalRenEntryIdsEx;
		}

		internal static Ex12ExRenEntryParser FromBytes(byte[] blob)
		{
			Ex12ExRenEntryParser ex12ExRenEntryParser = new Ex12ExRenEntryParser(blob);
			try
			{
				ex12ExRenEntryParser.Parse();
			}
			catch (FormatException)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceWarning<string>(-1L, "Ex12ExRenEntryParser::FromBytes. The blob is not correctly formated. Bytes = {0}.", (blob == null) ? "<null>" : Convert.ToBase64String(blob));
				ex12ExRenEntryParser.entryBlob = Array<byte>.Empty;
				ex12ExRenEntryParser.defaultFolderRenEx = new Dictionary<ushort, Ex12ExRenEntryParser.ExFolderEntry>();
			}
			return ex12ExRenEntryParser;
		}

		internal byte[] GetEntryId(Ex12RenEntryIdStrategy.PersistenceId persistenceId)
		{
			return this.GetExFolderEntry((ushort)persistenceId).EntryId;
		}

		internal void Insert(Ex12RenEntryIdStrategy.PersistenceId persistenceId, byte[] entryId)
		{
			Ex12ExRenEntryParser.ExFolderEntry exFolderEntry = new Ex12ExRenEntryParser.ExFolderEntry(1, entryId, 0, 0);
			if (!this.ReplaceEntryBlock(persistenceId, exFolderEntry))
			{
				this.InsertEntryBlock(persistenceId, exFolderEntry);
			}
		}

		internal bool Remove(Ex12RenEntryIdStrategy.PersistenceId persistenceId)
		{
			Ex12ExRenEntryParser.ExFolderEntry exFolderEntry = null;
			if (this.defaultFolderRenEx.TryGetValue((ushort)persistenceId, out exFolderEntry))
			{
				int start = exFolderEntry.Start - 4;
				this.entryBlob = Ex12ExRenEntryParser.ReplaceBlob(this.entryBlob, start, exFolderEntry.End, Array<byte>.Empty);
				this.defaultFolderRenEx.Clear();
				this.Parse();
				return true;
			}
			return false;
		}

		internal byte[] ToBytes()
		{
			return this.entryBlob;
		}

		[Conditional("DEBUG")]
		private static void DebugCheckReplaceBlob(byte[] newBlob, Ex12RenEntryIdStrategy.PersistenceId persistenceId, byte[] entryId)
		{
			Ex12ExRenEntryParser ex12ExRenEntryParser = new Ex12ExRenEntryParser(newBlob);
			ex12ExRenEntryParser.Parse();
			ex12ExRenEntryParser.GetEntryId(persistenceId);
			for (int i = 0; i < entryId.Length; i++)
			{
			}
		}

		private static byte[] ReplaceBlob(byte[] original, int start, int end, byte[] insertEntry)
		{
			byte[] array = new byte[original.Length - (end - start) + insertEntry.Length];
			Array.Copy(original, 0, array, 0, start);
			Array.Copy(insertEntry, 0, array, start, insertEntry.Length);
			Array.Copy(original, end, array, start + insertEntry.Length, original.Length - end);
			return array;
		}

		private static void ParseEntryBlock(byte[] additionalRenEntryIdsEx, ref int index, ushort blockSize, ref Ex12ExRenEntryParser.ExFolderEntry entry)
		{
			for (;;)
			{
				int start = index;
				uint num = (uint)Ex12ExRenEntryParser.ParseUInt16(additionalRenEntryIdsEx, index);
				index += 2;
				ushort num2 = Ex12ExRenEntryParser.ParseUInt16(additionalRenEntryIdsEx, index);
				index += 2;
				blockSize -= 4;
				if (num2 > blockSize)
				{
					break;
				}
				try
				{
					if (num == 1U)
					{
						entry.EntryId = new byte[(int)num2];
						Array.Copy(additionalRenEntryIdsEx, index, entry.EntryId, 0, (int)num2);
					}
					else if (num == 2U)
					{
						entry.ElementId = Ex12ExRenEntryParser.ParseUInt16(additionalRenEntryIdsEx, index);
					}
					else
					{
						entry.Unknown = new byte[(int)num2];
						Array.Copy(additionalRenEntryIdsEx, index, entry.Unknown, 0, (int)num2);
					}
				}
				catch (ArgumentException innerException)
				{
					throw new FormatException("ParseEntryBlock", innerException);
				}
				index += (int)num2;
				blockSize -= num2;
				entry.Start = start;
				entry.End = index;
				if (blockSize <= 0)
				{
					return;
				}
			}
		}

		private static ushort ParseUInt16(byte[] bytes, int index)
		{
			ushort result;
			try
			{
				result = BitConverter.ToUInt16(bytes, index);
			}
			catch (ArgumentException innerException)
			{
				throw new FormatException("ParseUInt16", innerException);
			}
			return result;
		}

		private void Parse()
		{
			int num = 0;
			if (this.entryBlob != null && this.entryBlob.Length > 1)
			{
				for (;;)
				{
					ushort num2 = Ex12ExRenEntryParser.ParseUInt16(this.entryBlob, num);
					if (num2 == 0)
					{
						break;
					}
					Ex12ExRenEntryParser.ExFolderEntry exFolderEntry = this.GetExFolderEntry(num2);
					num += 2;
					ushort num3 = Ex12ExRenEntryParser.ParseUInt16(this.entryBlob, num);
					num += 2;
					int num4 = num;
					while (num - num4 < (int)num3)
					{
						Ex12ExRenEntryParser.ParseEntryBlock(this.entryBlob, ref num, num3, ref exFolderEntry);
					}
					if (num + 2 >= this.entryBlob.Length)
					{
						return;
					}
				}
				return;
			}
		}

		private bool ReplaceEntryBlock(Ex12RenEntryIdStrategy.PersistenceId persistenceId, Ex12ExRenEntryParser.ExFolderEntry exFolderEntry)
		{
			Ex12ExRenEntryParser.ExFolderEntry exFolderEntry2 = null;
			if (this.defaultFolderRenEx.TryGetValue((ushort)persistenceId, out exFolderEntry2))
			{
				byte[] array = exFolderEntry.ToBytes();
				if (array.Length == exFolderEntry2.End - exFolderEntry2.Start)
				{
					exFolderEntry.Start = exFolderEntry2.Start;
					exFolderEntry.End = array.Length + exFolderEntry.Start;
					this.defaultFolderRenEx[(ushort)persistenceId] = exFolderEntry;
					this.entryBlob = Ex12ExRenEntryParser.ReplaceBlob(this.entryBlob, exFolderEntry2.Start, exFolderEntry2.End, array);
					return true;
				}
			}
			return false;
		}

		private void InsertEntryBlock(Ex12RenEntryIdStrategy.PersistenceId persistenceId, Ex12ExRenEntryParser.ExFolderEntry exFolderEntry)
		{
			this.defaultFolderRenEx[(ushort)persistenceId] = exFolderEntry;
			int num = (this.entryBlob == null) ? 0 : this.entryBlob.Length;
			int num2;
			int num3;
			byte[] array = exFolderEntry.ToBlock(persistenceId, out num2, out num3);
			this.entryBlob = Ex12ExRenEntryParser.ReplaceBlob(this.entryBlob, 0, 0, array);
			exFolderEntry.Start = num + num2;
			exFolderEntry.End = num + array.Length;
		}

		private Ex12ExRenEntryParser.ExFolderEntry GetExFolderEntry(ushort id)
		{
			if (id >= 32769)
			{
			}
			if (!this.defaultFolderRenEx.ContainsKey(id))
			{
				this.defaultFolderRenEx[id] = new Ex12ExRenEntryParser.ExFolderEntry();
			}
			return this.defaultFolderRenEx[id];
		}

		private const int UInt16Size = 2;

		private Dictionary<ushort, Ex12ExRenEntryParser.ExFolderEntry> defaultFolderRenEx;

		private byte[] entryBlob;

		internal enum ElementIdType
		{
			EntryId = 1,
			Header
		}

		internal class ExFolderEntry
		{
			internal ExFolderEntry()
			{
				this.elementId = 0;
				this.entryId = null;
				this.unknown = null;
			}

			internal ExFolderEntry(ushort elementId, byte[] entryId, int start, int end)
			{
				this.elementId = elementId;
				this.entryId = entryId;
				this.unknown = null;
				this.start = start;
				this.end = end;
			}

			internal int Start
			{
				get
				{
					return this.start;
				}
				set
				{
					this.start = value;
				}
			}

			internal int End
			{
				get
				{
					return this.end;
				}
				set
				{
					this.end = value;
				}
			}

			internal byte[] EntryId
			{
				get
				{
					return this.entryId;
				}
				set
				{
					this.entryId = value;
				}
			}

			internal byte[] Unknown
			{
				get
				{
					return this.unknown;
				}
				set
				{
					this.unknown = value;
				}
			}

			internal ushort ElementId
			{
				get
				{
					return this.elementId;
				}
				set
				{
					this.elementId = value;
				}
			}

			internal byte[] ToBytes()
			{
				MemoryStream memoryStream = new MemoryStream();
				Marshal.SizeOf(typeof(ushort));
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.elementId);
					binaryWriter.Write((ushort)this.entryId.Length);
					binaryWriter.Write(this.entryId);
				}
				return memoryStream.ToArray();
			}

			internal byte[] ToBlock(Ex12RenEntryIdStrategy.PersistenceId persistenceId, out int entryStart, out int entryEnd)
			{
				MemoryStream memoryStream = new MemoryStream();
				int num = Marshal.SizeOf(typeof(ushort));
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Seek(2 * num, SeekOrigin.Begin);
					entryStart = (int)binaryWriter.BaseStream.Position;
					binaryWriter.Write(this.ElementId);
					binaryWriter.Write((ushort)this.EntryId.Length);
					binaryWriter.Write(this.EntryId);
					entryEnd = (int)binaryWriter.BaseStream.Position;
					int num2 = (int)binaryWriter.BaseStream.Position - 2 * num;
					binaryWriter.Seek(0, SeekOrigin.Begin);
					binaryWriter.Write((ushort)persistenceId);
					binaryWriter.Write((ushort)num2);
				}
				return memoryStream.ToArray();
			}

			private ushort elementId;

			private byte[] entryId;

			private byte[] unknown;

			private int start;

			private int end;
		}
	}
}
