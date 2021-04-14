using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	public class CsvFieldCache
	{
		public CsvFieldCache(CsvTable table, Stream data, int readSize)
		{
			this.InitializeMembers(table, CsvFieldCache.LocalVersion, data, readSize);
		}

		public CsvFieldCache(CsvTable table, Version requestedSchemaVersion, Stream data, int readSize)
		{
			this.InitializeMembers(table, requestedSchemaVersion, data, readSize);
		}

		public bool AtEnd
		{
			get
			{
				return this.row.AtEnd;
			}
		}

		public long Position
		{
			get
			{
				return this.position;
			}
		}

		public long Length
		{
			get
			{
				return this.data.Length;
			}
		}

		public int FieldCount
		{
			get
			{
				return this.row.Count;
			}
		}

		public int SchemaFieldCount
		{
			get
			{
				return this.table.Fields.Length - this.unsupportedFields.Length;
			}
		}

		public int ReadSize
		{
			get
			{
				return this.readSize;
			}
		}

		public CsvTable Schema
		{
			get
			{
				return this.table;
			}
		}

		public static string NormalizeMessageID(string messageId)
		{
			if (messageId.Length > 2 && messageId[0] == '<' && messageId[messageId.Length - 1] == '>')
			{
				return messageId.Substring(1, messageId.Length - 2);
			}
			return messageId;
		}

		public static string NormalizeEmailAddress(string email)
		{
			if (email.Length > 2 && email[0] == '<' && email[email.Length - 1] == '>')
			{
				email = email.Substring(1, email.Length - 2);
			}
			return email.ToLower();
		}

		public static FileStream OpenLogFile(string filePath)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = CsvFieldCache.OpenFileAndSkipPreamble(filePath);
				if (fileStream == null)
				{
					return null;
				}
				string text;
				return CsvFieldCache.SeekToDataStartOffset(fileStream, out text);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<string>(0L, "CsvFieldCache OpenLogFile failed with error {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.CommonTracer.TraceError<string>(0L, "CsvFieldCache OpenLogFile failed with error {0}", ex2.Message);
			}
			if (fileStream != null)
			{
				fileStream.Close();
			}
			return null;
		}

		public static FileStream OpenLogFile(string filePath, out string version)
		{
			version = string.Empty;
			FileStream fileStream = null;
			try
			{
				fileStream = CsvFieldCache.OpenFileAndSkipPreamble(filePath);
				if (fileStream == null)
				{
					return null;
				}
				return CsvFieldCache.SeekToDataStartOffset(fileStream, out version);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<string>(0L, "CsvFieldCache OpenLogFile failed with error {0}", ex.Message);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.CommonTracer.TraceError<string>(0L, "CsvFieldCache OpenLogFile failed with error {0}", ex2.Message);
			}
			if (fileStream != null)
			{
				fileStream.Close();
			}
			return null;
		}

		public static string DecodeString(byte[] src, int offset, int count)
		{
			return Encoding.UTF8.GetString(src, offset, count);
		}

		public object GetField(int index)
		{
			int num = index;
			if (this.fieldTranslationTable != null && !this.fieldTranslationTable.TryGetValue(index, out index))
			{
				return null;
			}
			if (index >= this.row.Count)
			{
				return null;
			}
			if (this.cacheVersion[index] == this.version)
			{
				return this.cache[index];
			}
			object obj = this.row.Decode(index, this.decoder[num]);
			this.cache[index] = obj;
			this.cacheVersion[index] = this.version;
			return obj;
		}

		public byte[] GetFieldRaw(int index)
		{
			int index2 = index;
			if (this.fieldTranslationTable != null && !this.fieldTranslationTable.TryGetValue(index, out index))
			{
				return null;
			}
			if (index >= this.row.Count)
			{
				return null;
			}
			return this.row.GetRawFieldValue(index2);
		}

		public int CopyRow(int srcOffset, byte[] dest, int offset, int count)
		{
			return this.row.Copy(srcOffset, dest, offset, count, this.unsupportedFields);
		}

		public bool MoveNext(bool skipHeaders = false)
		{
			this.InvalidateCache();
			while (this.MoveNextRow())
			{
				if (!skipHeaders || !this.row.IsHeaderRow())
				{
					return true;
				}
			}
			return false;
		}

		public long Seek(long position)
		{
			this.position = this.data.Seek(position, SeekOrigin.Begin);
			this.row.Reset();
			return this.position;
		}

		public void SetMappingTableBasedOnCurrentRecord()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>(this.row.Count);
			for (int i = 0; i < this.row.Count; i++)
			{
				string @string = Encoding.ASCII.GetString(this.row.GetRawFieldValue(i));
				int num = this.table.NameToIndex(@string);
				if (num != -1)
				{
					dictionary.Add(num, i);
				}
			}
			this.fieldTranslationTable = dictionary;
		}

		private static FileStream OpenFileAndSkipPreamble(string fileName)
		{
			ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "CsvFieldCache OpenLogFile opens new file {0}", fileName);
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 32768);
			byte[] preamble = Encoding.UTF8.GetPreamble();
			int i = 0;
			while (i < preamble.Length)
			{
				int num = fileStream.ReadByte();
				if (num == -1)
				{
					fileStream.Close();
					return null;
				}
				if (num != (int)preamble[i++])
				{
					fileStream.Seek((long)(-(long)i), SeekOrigin.Current);
					break;
				}
			}
			return fileStream;
		}

		private static FileStream SeekToDataStartOffset(FileStream file, out string version)
		{
			int num = -1;
			version = string.Empty;
			byte[] array = new byte[1024];
			bool flag = true;
			int num2;
			int i;
			for (;;)
			{
				num2 = file.Read(array, 0, array.Length);
				if (num2 == 0)
				{
					break;
				}
				for (i = 0; i < num2; i++)
				{
					byte b = array[i];
					if (flag)
					{
						if (b != 35)
						{
							goto Block_3;
						}
						if (i + "Version:".Length < num2)
						{
							string text = CsvFieldCache.DecodeString(array, i + 1, "Version:".Length);
							if (text.Equals("Version:"))
							{
								num = i + "Version:".Length + 1;
							}
						}
						flag = false;
					}
					else if (b == 10)
					{
						flag = true;
						if (num != -1)
						{
							version = CsvFieldCache.DecodeString(array, num, i - num).Trim();
							num = -1;
						}
					}
				}
			}
			file.Close();
			return null;
			Block_3:
			file.Seek((long)(i - num2), SeekOrigin.Current);
			return file;
		}

		private void InvalidateCache()
		{
			this.version += 1L;
		}

		private bool MoveNextRow()
		{
			long num = this.row.ReadNext(this.data);
			if (num == -1L)
			{
				if (this.data.CanSeek)
				{
					this.position = this.data.Position;
				}
				return false;
			}
			this.position += num;
			return true;
		}

		private void InitializeMembers(CsvTable table, Version schemaVersion, Stream data, int readSize)
		{
			this.table = table;
			this.schemaVersion = schemaVersion;
			this.data = data;
			this.decoder = new CsvDecoderCallback[table.Fields.Length];
			if (this.data.CanSeek)
			{
				this.position = this.data.Position;
			}
			this.readSize = readSize;
			for (int i = 0; i < table.Fields.Length; i++)
			{
				this.decoder[i] = CsvDecoder.GetDecoder(table.Fields[i].Type);
			}
			this.cacheVersion = new long[table.Fields.Length];
			this.cache = new object[table.Fields.Length];
			this.row = new CsvRowBuffer(readSize);
			this.unsupportedFields = table.GetFieldsAddedAfterVersion(this.schemaVersion);
		}

		public const int DefaultReadSize = 32768;

		private const byte Pound = 35;

		private const byte NewLine = 10;

		private const string VersionString = "Version:";

		public static readonly Version LocalVersion = new Version("15.00.1497.015");

		private CsvTable table;

		private CsvDecoderCallback[] decoder;

		private Stream data;

		private object[] cache;

		private long[] cacheVersion;

		private long version;

		private CsvRowBuffer row;

		private long position;

		private int readSize;

		private Version schemaVersion;

		private int[] unsupportedFields;

		private Dictionary<int, int> fieldTranslationTable;
	}
}
