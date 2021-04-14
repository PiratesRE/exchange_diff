using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ParticipantEntryId
	{
		internal ParticipantEntryId()
		{
		}

		internal virtual bool? IsDL
		{
			get
			{
				return null;
			}
		}

		public static ParticipantEntryId TryFromEntryId(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			ParticipantEntryId result;
			using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader(bytes))
			{
				result = ParticipantEntryId.TryFromEntryId(reader);
			}
			return result;
		}

		public static ParticipantEntryId FromParticipant(Participant participant, ParticipantEntryIdConsumer consumer)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			EnumValidator.ThrowIfInvalid<ParticipantEntryIdConsumer>(consumer);
			ParticipantEntryId participantEntryId = null;
			foreach (ParticipantEntryId.TryFromParticipantDelegate tryFromParticipantDelegate in ParticipantEntryId.tryFromParticipantChain)
			{
				participantEntryId = tryFromParticipantDelegate(participant, consumer);
				if (participantEntryId != null)
				{
					break;
				}
			}
			return participantEntryId;
		}

		public byte[] ToByteArray()
		{
			byte[] bytes;
			using (ParticipantEntryId.Writer writer = new ParticipantEntryId.Writer())
			{
				this.Serialize(writer);
				bytes = writer.GetBytes();
			}
			return bytes;
		}

		internal static ParticipantEntryId FromEntryId(byte[] bytes)
		{
			return ParticipantEntryId.TranslateExceptions<ParticipantEntryId>(delegate
			{
				ParticipantEntryId result;
				using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader(bytes))
				{
					result = ParticipantEntryId.FromEntryId(null, reader);
				}
				return result;
			});
		}

		internal static IList<ParticipantEntryId> FromFlatEntryList(byte[] bytes)
		{
			return ParticipantEntryId.TranslateExceptions<IList<ParticipantEntryId>>(delegate
			{
				IList<ParticipantEntryId> result;
				using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader(bytes))
				{
					result = ParticipantEntryId.FromFlatEntryList(reader);
				}
				return result;
			});
		}

		internal static byte[] ToFlatEntryList(ICollection<ParticipantEntryId> entryIds)
		{
			return ParticipantEntryId.TranslateExceptions<byte[]>(delegate
			{
				byte[] bytes;
				using (ParticipantEntryId.Writer writer = new ParticipantEntryId.Writer())
				{
					writer.Write(entryIds.Count);
					ParticipantEntryId.Bookmark bookmark = writer.PlaceBookmark<int>(new ParticipantEntryId.WriterMethod<int>(writer.Write), 0);
					ParticipantEntryId.Bookmark currentBookmark = writer.CurrentBookmark;
					foreach (ParticipantEntryId participantEntryId in entryIds)
					{
						if (participantEntryId == null)
						{
							throw new ArgumentNullException("entryIds");
						}
						ParticipantEntryId.Bookmark bookmark2 = writer.PlaceBookmark<int>(new ParticipantEntryId.WriterMethod<int>(writer.Write), 0);
						ParticipantEntryId.Bookmark currentBookmark2 = writer.CurrentBookmark;
						participantEntryId.Serialize(writer);
						int num = (int)(writer.CurrentBookmark - currentBookmark2);
						writer.WriteBookmark<int>(bookmark2, num);
						writer.WritePadding(num, 4);
					}
					writer.WriteBookmark<int>(bookmark, (int)(writer.CurrentBookmark - currentBookmark));
					bytes = writer.GetBytes();
				}
				return bytes;
			});
		}

		internal abstract IEnumerable<PropValue> GetParticipantProperties();

		internal virtual ParticipantOrigin GetParticipantOrigin()
		{
			return null;
		}

		protected static T TranslateExceptions<T>(ParticipantEntryId.SimpleAction<T> action)
		{
			T result;
			try
			{
				result = action();
			}
			catch (ArgumentException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId, innerException);
			}
			catch (EndOfStreamException innerException2)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId, innerException2);
			}
			return result;
		}

		protected abstract void Serialize(ParticipantEntryId.Writer writer);

		private static ParticipantEntryId FromEntryId(ParticipantEntryId.WabEntryFlag? wrapperFlags, ParticipantEntryId.Reader reader)
		{
			Guid a = reader.ReadEntryHeader();
			if (a == ParticipantEntryId.OneOffProviderGuid)
			{
				using (ParticipantEntryId.Reader reader2 = reader.TearRest())
				{
					return new OneOffParticipantEntryId(reader2);
				}
			}
			if (a == ParticipantEntryId.WabProviderGuid)
			{
				ParticipantEntryId.WabEntryFlag wabEntryFlag = (ParticipantEntryId.WabEntryFlag)reader.ReadByte();
				switch ((byte)(wabEntryFlag & ParticipantEntryId.WabEntryFlag.ObjectTypeMask))
				{
				case 0:
				case 5:
				case 6:
					break;
				case 1:
				case 2:
					goto IL_D0;
				case 3:
				case 4:
					using (ParticipantEntryId.Reader reader3 = reader.TearRest())
					{
						return new StoreParticipantEntryId(wabEntryFlag, reader3);
					}
					break;
				default:
					goto IL_D0;
				}
				if (wrapperFlags != null)
				{
					throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
				}
				using (ParticipantEntryId.Reader reader4 = reader.TearRest())
				{
					return ParticipantEntryId.FromEntryId(new ParticipantEntryId.WabEntryFlag?(wabEntryFlag), reader4);
				}
				IL_D0:
				throw new NotSupportedException(ServerStrings.ExInvalidWABObjectType(wabEntryFlag & ParticipantEntryId.WabEntryFlag.ObjectTypeMask));
			}
			if (a == ParticipantEntryId.ExchangeProviderGuid)
			{
				using (ParticipantEntryId.Reader reader5 = reader.TearRest())
				{
					return new ADParticipantEntryId(wrapperFlags, reader5);
				}
			}
			if (a == ParticipantEntryId.OlabProviderGuid)
			{
				using (ParticipantEntryId.Reader reader6 = reader.TearRest())
				{
					return new StoreParticipantEntryId(reader6);
				}
			}
			throw new NotSupportedException(ServerStrings.ExUnsupportedABProvider(a.ToString(), string.Empty));
		}

		private static IList<ParticipantEntryId> FromFlatEntryList(ParticipantEntryId.Reader reader)
		{
			int num = reader.ReadInt32();
			reader.ReadInt32();
			IList<ParticipantEntryId> list = new List<ParticipantEntryId>();
			for (int i = 0; i < num; i++)
			{
				int num2 = reader.ReadInt32();
				list.Add(ParticipantEntryId.TryFromEntryId(reader.TearNext(num2)));
				if (reader.BytesRemaining != 0)
				{
					reader.ReadPadding(num2, 4);
				}
			}
			reader.EnsureEnd();
			return list;
		}

		private static ParticipantEntryId TryFromEntryId(ParticipantEntryId.Reader reader)
		{
			try
			{
				return ParticipantEntryId.TranslateExceptions<ParticipantEntryId>(() => ParticipantEntryId.FromEntryId(null, reader));
			}
			catch (NotSupportedException)
			{
			}
			catch (CorruptDataException)
			{
			}
			reader.BaseStream.Position = 0L;
			return new UnrecognizedParticipantEntryId(reader.ReadBytes(reader.BytesRemaining));
		}

		protected static readonly Guid ExchangeProviderGuid = new Guid("c840a7dc-42c0-1a10-b4b9-08002b2fe182");

		protected static readonly Guid OlabProviderGuid = new Guid("0aaa42fe-c718-101a-e885-0b651c240000");

		protected static readonly Guid OneOffProviderGuid = new Guid("a41f2b81-a3be-1910-9d6e-00dd010f5402");

		protected static readonly Guid WabProviderGuid = new Guid("d3ad91c0-9d51-11cf-a4a9-00aa0047faa4");

		private static ParticipantEntryId.TryFromParticipantDelegate[] tryFromParticipantChain = new ParticipantEntryId.TryFromParticipantDelegate[]
		{
			new ParticipantEntryId.TryFromParticipantDelegate(StoreParticipantEntryId.TryFromParticipant),
			new ParticipantEntryId.TryFromParticipantDelegate(ADParticipantEntryId.TryFromParticipant),
			new ParticipantEntryId.TryFromParticipantDelegate(OneOffParticipantEntryId.TryFromParticipant)
		};

		[Flags]
		protected internal enum WabEntryFlag : byte
		{
			Envelope = 0,
			WabMember = 2,
			ContactPerson = 3,
			ContactDL = 4,
			DirectoryPerson = 5,
			DirectoryDL = 6,
			BusinessFax = 0,
			HomeFax = 16,
			OtherFax = 32,
			NoEmailIndex = 48,
			EmailIndex1 = 64,
			EmailIndex2 = 80,
			EmailIndex3 = 96,
			Outlook = 128,
			EmailIndexMask = 112,
			ObjectTypeMask = 15
		}

		public delegate void WriterMethod<T>(T value);

		protected delegate ParticipantEntryId TryFromParticipantDelegate(Participant participant, ParticipantEntryIdConsumer consumer);

		protected delegate T SimpleAction<T>();

		protected internal struct Bookmark
		{
			public static long operator -(ParticipantEntryId.Bookmark end, ParticipantEntryId.Bookmark start)
			{
				return end.Position - start.Position;
			}

			public long Position;

			public long Size;

			public Delegate WriterMethod;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct LTId
		{
			public const int Size = 24;

			[FieldOffset(0)]
			public Guid Guid;

			[FixedBuffer(typeof(byte), 6)]
			[FieldOffset(16)]
			public ParticipantEntryId.LTId.<GlobCnt>e__FixedBuffer0 GlobCnt;

			[FieldOffset(22)]
			public ushort Level;

			[FixedBuffer(typeof(byte), 24)]
			[FieldOffset(0)]
			private ParticipantEntryId.LTId.<bytes>e__FixedBuffer1 bytes;

			[FieldOffset(16)]
			public ulong GlobCntLong;

			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, Size = 6)]
			public struct <GlobCnt>e__FixedBuffer0
			{
				public byte FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 24)]
			public struct <bytes>e__FixedBuffer1
			{
				public byte FixedElementField;
			}
		}

		internal struct LTEntryId
		{
			public const int Size = 70;

			public uint ABFlags;

			public Guid StoreGuid;

			public ushort Eit;

			public ParticipantEntryId.LTId FolderId;

			public ParticipantEntryId.LTId MessageId;
		}

		protected internal class Reader : BinaryReader
		{
			public Reader(byte[] bytes) : this(bytes, 0, bytes.Length)
			{
			}

			public int BytesRemaining
			{
				get
				{
					return (int)(this.BaseStream.Length - this.BaseStream.Position);
				}
			}

			public bool IsEnd
			{
				get
				{
					return this.BytesRemaining == 0;
				}
			}

			protected int BufferPointer
			{
				get
				{
					return this.Origin + (int)this.BaseStream.Position;
				}
			}

			public void EnsureEnd()
			{
				if (this.BytesRemaining != 0)
				{
					throw new EndOfStreamException(ServerStrings.ExInvalidParticipantEntryId);
				}
			}

			public Guid ReadEntryHeader()
			{
				if (this.ReadUInt32() != 0U)
				{
					throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
				}
				return this.ReadGuid();
			}

			public byte[] ReadExactBytes(int count)
			{
				byte[] array = this.ReadBytes(count);
				if (array.Length != count)
				{
					throw new EndOfStreamException(ServerStrings.ExInvalidParticipantEntryId);
				}
				return array;
			}

			public Guid ReadGuid()
			{
				return new Guid(this.ReadExactBytes(sizeof(Guid)));
			}

			public ParticipantEntryId.LTEntryId ReadLTEntryId()
			{
				ParticipantEntryId.LTEntryId result = default(ParticipantEntryId.LTEntryId);
				result.ABFlags = this.ReadUInt32();
				result.StoreGuid = this.ReadGuid();
				if (this.BytesRemaining != 4)
				{
					result.Eit = this.ReadUInt16();
					result.FolderId = this.ReadLTId();
					result.MessageId = this.ReadLTId();
					return result;
				}
				throw new NotSupportedException(ServerStrings.ExUnsupportedABProvider("PST", string.Empty));
			}

			public ParticipantEntryId.LTId ReadLTId()
			{
				return new ParticipantEntryId.LTId
				{
					Guid = this.ReadGuid(),
					GlobCntLong = this.ReadUInt64()
				};
			}

			public void ReadPadding(int size, int granularity)
			{
				int num = size % granularity;
				if (num != 0)
				{
					this.ReadExactBytes(granularity - num);
				}
			}

			public string ReadZString(Encoding encoding)
			{
				char[] chars = encoding.GetChars(this.GetBuffer(), this.BufferPointer, this.BytesRemaining);
				int num = ParticipantEntryId.Reader.IndexOf(chars, '\0');
				if (num != -1)
				{
					string result = new string(chars, 0, num);
					this.BaseStream.Position += (long)encoding.GetByteCount(chars, 0, num + 1);
					return result;
				}
				throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
			}

			public ParticipantEntryId.Reader TearNext(int count)
			{
				ParticipantEntryId.Reader result = new ParticipantEntryId.Reader(this, (int)this.BaseStream.Position, count);
				this.BaseStream.Position += (long)count;
				return result;
			}

			public ParticipantEntryId.Reader TearRest()
			{
				return this.TearNext(this.BytesRemaining);
			}

			private static int IndexOf(char[] array, char ch)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == ch)
					{
						return i;
					}
				}
				return -1;
			}

			public Reader(byte[] bytes, int index, int count) : base(new MemoryStream(bytes, index, count, false, true))
			{
			}

			public Reader(ParticipantEntryId.Reader parentReader, int index, int count) : this(((MemoryStream)parentReader.BaseStream).GetBuffer(), index + parentReader.Origin, count)
			{
				this.Origin = index + parentReader.Origin;
			}

			protected byte[] GetBuffer()
			{
				return ((MemoryStream)this.BaseStream).GetBuffer();
			}

			protected readonly int Origin;
		}

		protected internal class Writer : BinaryWriter
		{
			public Writer() : base(new MemoryStream())
			{
			}

			public ParticipantEntryId.Bookmark CurrentBookmark
			{
				get
				{
					ParticipantEntryId.Bookmark result;
					result.Position = this.Position;
					result.Size = 0L;
					result.WriterMethod = null;
					return result;
				}
			}

			public long Position
			{
				get
				{
					this.Flush();
					return this.BaseStream.Position;
				}
			}

			public byte[] GetBytes()
			{
				this.Flush();
				return ((MemoryStream)this.BaseStream).ToArray();
			}

			public ParticipantEntryId.Bookmark PlaceBookmark<T>(ParticipantEntryId.WriterMethod<T> writerMethod, T initialValue)
			{
				ParticipantEntryId.Bookmark result;
				result.Position = this.Position;
				writerMethod(initialValue);
				result.Size = this.BaseStream.Position - result.Position;
				result.WriterMethod = writerMethod;
				return result;
			}

			public void WriteEntryHeader(Guid providerGuid)
			{
				this.Write(0U);
				this.Write(providerGuid);
			}

			public void Write(Guid guid)
			{
				byte[] array = guid.ToByteArray();
				this.Write(array, 0, array.Length);
			}

			public void Write(ParticipantEntryId.LTEntryId ltEntryId)
			{
				this.Write(ltEntryId.ABFlags);
				this.Write(ltEntryId.StoreGuid);
				this.Write(ltEntryId.Eit);
				this.Write(ltEntryId.FolderId);
				this.Write(ltEntryId.MessageId);
			}

			public void Write(ParticipantEntryId.LTId ltid)
			{
				this.Write(ltid.Guid);
				this.Write(ltid.GlobCntLong);
			}

			public void WriteBookmark<T>(ParticipantEntryId.Bookmark bookmark, T value)
			{
				long position = this.Position;
				this.BaseStream.Position = bookmark.Position;
				((ParticipantEntryId.WriterMethod<T>)bookmark.WriterMethod)(value);
				this.BaseStream.Position = position;
			}

			public void WritePadding(int size, int granularity)
			{
				int num = size % granularity;
				if (num != 0)
				{
					this.Write(new byte[granularity - num]);
				}
			}

			public void WriteZString(string value, Encoding encoding)
			{
				byte[] bytes = encoding.GetBytes(value + '\0');
				this.Write(bytes, 0, bytes.Length);
			}
		}
	}
}
