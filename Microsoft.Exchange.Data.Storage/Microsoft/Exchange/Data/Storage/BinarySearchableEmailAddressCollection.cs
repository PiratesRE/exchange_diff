using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BinarySearchableEmailAddressCollection : PeopleIKnowEmailAddressCollection
	{
		public BinarySearchableEmailAddressCollection(ICollection<string> strings, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
			this.traceId = traceId;
			this.strings = strings;
		}

		public BinarySearchableEmailAddressCollection(byte[] data, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
			this.traceId = traceId;
			this.data = data;
		}

		protected abstract byte Version { get; }

		protected abstract BinarySearchableEmailAddressCollection.IMetadataSerializer MetadataSerializer { get; }

		private int Count
		{
			get
			{
				if (this.Data == null || this.Data.Length < 2)
				{
					return 0;
				}
				return (int)this.ReadUShortFromDataArray(1);
			}
		}

		public override byte[] Data
		{
			get
			{
				if (this.data == null)
				{
					this.PackStrings(this.strings);
				}
				return this.data;
			}
		}

		public override bool Contains(string s, out PeopleIKnowMetadata metadata)
		{
			this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.Contains: entering");
			metadata = null;
			if (this.Data == null)
			{
				this.tracer.TraceDebug((long)this.traceId, "this.Data is null; returning false");
				return false;
			}
			if (string.IsNullOrEmpty(s))
			{
				this.tracer.TraceDebug((long)this.traceId, "s is null or empty; returning false");
				return false;
			}
			byte[] array = this.NormalizeAndConvertToUTF8(s);
			byte[] array2 = new byte[array.Length + 1];
			Array.Copy(array, 0, array2, 0, array.Length);
			int i = 0;
			int num = this.Count - 1;
			while (i <= num)
			{
				int num2 = (i + num) / 2;
				int num3 = (int)this.ReadUShortFromDataArray(3 + num2 * 2);
				int num4 = BinarySearchableEmailAddressCollection.byteArrayComparer.CompareNullTerminatedByteStrings(this.Data, num3, array2, 0);
				if (num4 < 0)
				{
					i = num2 + 1;
				}
				else
				{
					if (num4 <= 0)
					{
						this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.Contains: search string was found; returning true");
						while (this.Data[num3++] != 0)
						{
						}
						byte[] array3 = new byte[this.MetadataSerializer.SizeOfMetadata];
						Array.Copy(this.Data, num3, array3, 0, this.MetadataSerializer.SizeOfMetadata);
						metadata = this.MetadataSerializer.Deserialize(array3);
						return true;
					}
					num = num2 - 1;
				}
			}
			this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.Contains: search string was not found; returning false");
			return false;
		}

		private void PackStrings(ICollection<string> strings)
		{
			this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: entering");
			List<Tuple<string, byte[]>> list = new List<Tuple<string, byte[]>>(strings.Count);
			int num = 3;
			this.tracer.TraceDebug<int>((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: cumulativeByteCount initialized to {0}", num);
			foreach (string text in strings)
			{
				if (!string.IsNullOrEmpty(text))
				{
					byte[] array = this.NormalizeAndConvertToUTF8(text);
					if (num + array.Length + 1 + 2 + this.MetadataSerializer.SizeOfMetadata > 30000)
					{
						this.tracer.TraceWarning<int>((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: size limit of {0} exceeded; string NOT added and list truncated at this point", 30000);
						break;
					}
					num += array.Length + 1 + 2 + this.MetadataSerializer.SizeOfMetadata;
					list.Add(Tuple.Create<string, byte[]>(text, array));
					this.tracer.TraceDebug<int>((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: string added, cumulativeByteCount = {0}", num);
				}
			}
			this.tracer.TraceDebug<int>((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: sorting {0} entries", list.Count);
			list.Sort((Tuple<string, byte[]> t1, Tuple<string, byte[]> t2) => BinarySearchableEmailAddressCollection.byteArrayComparer.Compare(t1.Item2, t2.Item2));
			this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: writing final packed and sorted list");
			this.data = new byte[num];
			this.data[0] = this.Version;
			this.WriteUShortToDataArray(1, (ushort)list.Count);
			int num2 = 3;
			int num3 = 3 + list.Count * 2;
			foreach (Tuple<string, byte[]> tuple in list)
			{
				string item = tuple.Item1;
				byte[] item2 = tuple.Item2;
				this.WriteUShortToDataArray(num2, (ushort)num3);
				Array.Copy(item2, 0, this.data, num3, item2.Length);
				this.data[num3 + item2.Length] = 0;
				byte[] array2 = this.MetadataSerializer.Serialize(item);
				if (array2.Length != this.MetadataSerializer.SizeOfMetadata)
				{
					throw new InvalidOperationException(string.Format("Metadata length does not match SizeOfMetadata. Actual size: {0}", array2.Length));
				}
				Array.Copy(array2, 0, this.data, num3 + item2.Length + 1, this.MetadataSerializer.SizeOfMetadata);
				num2 += 2;
				num3 += item2.Length + 1 + this.MetadataSerializer.SizeOfMetadata;
			}
			this.tracer.TraceDebug((long)this.traceId, "BinarySearchableEmailAddressCollection.PackStrings: exiting");
		}

		private byte[] NormalizeAndConvertToUTF8(string s)
		{
			string text = s.Normalize().Trim().ToLowerInvariant();
			this.tracer.TraceDebug<string>((long)this.traceId, "BinarySearchableEmailAddressCollection.NormalizeAndConvertToUTF8: normalized string is {0}", text);
			return Encoding.UTF8.GetBytes(text);
		}

		private ushort ReadUShortFromDataArray(int index)
		{
			return BitConverter.ToUInt16(this.data, index);
		}

		private void WriteUShortToDataArray(int index, ushort value)
		{
			ExBitConverter.Write(value, this.data, index);
		}

		public const int MaximumDataSize = 30000;

		private const int NumberOfHeaderBytes = 3;

		private const int NumberOfEntriesOffset = 1;

		private const int SizeOfOffsetRecord = 2;

		private static BinarySearchableEmailAddressCollection.ByteArrayComparer byteArrayComparer = new BinarySearchableEmailAddressCollection.ByteArrayComparer();

		private readonly ITracer tracer;

		private readonly int traceId;

		private readonly ICollection<string> strings;

		private byte[] data;

		protected interface IMetadataSerializer
		{
			int SizeOfMetadata { get; }

			byte[] Serialize(string email);

			PeopleIKnowMetadata Deserialize(byte[] buffer);
		}

		private class ByteArrayComparer : Comparer<byte[]>, IEqualityComparer<byte[]>
		{
			public override int Compare(byte[] a, byte[] b)
			{
				int num = 0;
				while (num < a.Length && num < b.Length && a[num] == b[num])
				{
					num++;
				}
				if (num == a.Length || num == b.Length)
				{
					return a.Length - b.Length;
				}
				return (int)(a[num] - b[num]);
			}

			public int CompareNullTerminatedByteStrings(byte[] a, int ia, byte[] b, int ib)
			{
				while (a[ia] == b[ib])
				{
					if (a[ia] == 0)
					{
						return 0;
					}
					ia++;
					ib++;
				}
				return (int)(a[ia] - b[ib]);
			}

			public bool Equals(byte[] a, byte[] b)
			{
				return this.Compare(a, b) == 0;
			}

			public int GetHashCode(byte[] a)
			{
				return a.GetHashCode();
			}
		}
	}
}
