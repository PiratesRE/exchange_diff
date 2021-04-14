using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class CsvFieldCache
	{
		public CsvFieldCache(CsvTable table, Version requestedSchemaVersion, Stream data, int readSize)
		{
			this.schema = table;
			this.csvFieldCacheImpl = new CsvFieldCache(this.schema.CsvTableImpl, requestedSchemaVersion, data, readSize);
		}

		public CsvFieldCache(CsvTable table, Stream data, int readSize)
		{
			this.schema = table;
			this.csvFieldCacheImpl = new CsvFieldCache(this.schema.CsvTableImpl, data, readSize);
		}

		public bool AtEnd
		{
			get
			{
				return this.csvFieldCacheImpl.AtEnd;
			}
		}

		public long Position
		{
			get
			{
				return this.csvFieldCacheImpl.Position;
			}
		}

		public long Length
		{
			get
			{
				return this.csvFieldCacheImpl.Length;
			}
		}

		public int FieldCount
		{
			get
			{
				return this.csvFieldCacheImpl.FieldCount;
			}
		}

		public int SchemaFieldCount
		{
			get
			{
				return this.csvFieldCacheImpl.SchemaFieldCount;
			}
		}

		public int ReadSize
		{
			get
			{
				return this.csvFieldCacheImpl.ReadSize;
			}
		}

		public CsvTable Schema
		{
			get
			{
				return this.schema;
			}
		}

		internal CsvFieldCache CsvFieldCacheImpl
		{
			get
			{
				return this.csvFieldCacheImpl;
			}
		}

		public static string NormalizeMessageID(string messageId)
		{
			return CsvFieldCache.NormalizeMessageID(messageId);
		}

		public static string NormalizeEmailAddress(string email)
		{
			return CsvFieldCache.NormalizeEmailAddress(email);
		}

		public static FileStream OpenLogFile(string filePath)
		{
			return CsvFieldCache.OpenLogFile(filePath);
		}

		public static FileStream OpenLogFile(string filePath, out string version)
		{
			return CsvFieldCache.OpenLogFile(filePath, out version);
		}

		public static string DecodeString(byte[] src, int offset, int count)
		{
			return CsvFieldCache.DecodeString(src, offset, count);
		}

		public object GetField(int index)
		{
			return this.csvFieldCacheImpl.GetField(index);
		}

		public byte[] GetFieldRaw(int index)
		{
			return this.csvFieldCacheImpl.GetFieldRaw(index);
		}

		public int CopyRow(int srcOffset, byte[] dest, int offset, int count)
		{
			return this.csvFieldCacheImpl.CopyRow(srcOffset, dest, offset, count);
		}

		public bool MoveNext(bool skipHeaders = false)
		{
			return this.csvFieldCacheImpl.MoveNext(skipHeaders);
		}

		public long Seek(long position)
		{
			return this.csvFieldCacheImpl.Seek(position);
		}

		public void SetMappingTableBasedOnCurrentRecord()
		{
			this.csvFieldCacheImpl.SetMappingTableBasedOnCurrentRecord();
		}

		public const int DefaultReadSize = 32768;

		private CsvFieldCache csvFieldCacheImpl;

		private CsvTable schema;
	}
}
