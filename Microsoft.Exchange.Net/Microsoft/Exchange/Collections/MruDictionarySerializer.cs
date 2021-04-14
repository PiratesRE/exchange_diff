using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MruDictionarySerializer<TKey, TValue>
	{
		public MruDictionarySerializer(string filePath, string fileName, string[] columnNames, MruDictionarySerializerRead<TKey, TValue> callbackReadValues, MruDictionarySerializerWrite<TKey, TValue> callbackWriteValues, IMruDictionaryPerfCounters perfCounters)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			if (columnNames == null || columnNames.Length == 0)
			{
				throw new ArgumentNullException("columnNames");
			}
			if (callbackReadValues == null)
			{
				throw new ArgumentNullException("functionReadValues");
			}
			if (callbackWriteValues == null)
			{
				throw new ArgumentNullException("functionWriteValues");
			}
			this.filePath = filePath;
			this.fileFullName = Path.Combine(this.filePath, fileName);
			this.columnNames = columnNames;
			this.csvTable = this.CreateCsvTable();
			LogSchema schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), fileName, this.columnNames);
			this.headerFormatter = new LogHeaderFormatter(schema);
			this.rowFormatter = new LogRowFormatter(schema);
			this.writeValuesCallback = callbackWriteValues;
			this.readValuesCallback = callbackReadValues;
			this.perfCounters = (perfCounters ?? NoopMruDictionaryPerfCounters.Instance);
		}

		public bool TryWriteToDisk(MruDictionary<TKey, TValue> mruDictionary)
		{
			if (mruDictionary == null)
			{
				return false;
			}
			bool result;
			try
			{
				lock (mruDictionary.SyncRoot)
				{
					using (FileStream fileStream = File.Create(this.CreateDirAndGetFileName(), 4096, FileOptions.SequentialScan))
					{
						if (fileStream == null)
						{
							return false;
						}
						this.headerFormatter.Write(fileStream, DateTime.UtcNow);
						foreach (KeyValuePair<TKey, TValue> keyValuePair in mruDictionary)
						{
							string[] array;
							if (this.writeValuesCallback(keyValuePair.Key, keyValuePair.Value, out array) && array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									this.rowFormatter[i] = (string.IsNullOrEmpty(array[i]) ? "-NA-" : array[i]);
								}
								this.rowFormatter.Write(fileStream);
							}
						}
						fileStream.Flush();
					}
				}
				this.perfCounters.FileWrite(1);
				result = true;
			}
			catch (IOException arg)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<IOException>(0L, "TryWriteToDisk() failed with IOException - {0}.", arg);
				result = false;
			}
			catch (UnauthorizedAccessException arg2)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<UnauthorizedAccessException>(0L, "TryWriteToDisk() failed with UnauthorizedAccessException - {0}.", arg2);
				result = false;
			}
			catch (SecurityException arg3)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<SecurityException>(0L, "TryWriteToDisk() failed with SecurityException - {0}.", arg3);
				result = false;
			}
			return result;
		}

		public bool TryReadFromDisk(MruDictionary<TKey, TValue> mruDictionary)
		{
			if (mruDictionary == null)
			{
				return false;
			}
			bool result;
			try
			{
				using (FileStream fileStream = CsvFieldCache.OpenLogFile(this.CreateDirAndGetFileName()))
				{
					if (fileStream == null)
					{
						return false;
					}
					CsvFieldCache csvFieldCache = new CsvFieldCache(this.csvTable, fileStream, 4096);
					string[] array = new string[this.columnNames.Length];
					while (csvFieldCache.MoveNext(false))
					{
						bool flag = true;
						for (int i = 0; i < this.columnNames.Length; i++)
						{
							if (!MruDictionarySerializer<TKey, TValue>.TryGetString(csvFieldCache, i, out array[i]))
							{
								flag = false;
								break;
							}
						}
						TKey key;
						TValue value;
						if (flag && this.readValuesCallback(array, out key, out value))
						{
							mruDictionary.Add(key, value);
						}
					}
				}
				this.perfCounters.FileRead(1);
				result = true;
			}
			catch (IOException arg)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<IOException>(0L, "TryReadFromDisk() failed with IOException - {0}.", arg);
				result = false;
			}
			catch (UnauthorizedAccessException arg2)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<UnauthorizedAccessException>(0L, "TryReadFromDisk() failed with UnauthorizedAccessException - {0}.", arg2);
				result = false;
			}
			catch (SecurityException arg3)
			{
				MruDictionarySerializer<TKey, TValue>.Tracer.TraceError<SecurityException>(0L, "TryReadFromDisk() failed with SecurityException - {0}.", arg3);
				result = false;
			}
			return result;
		}

		private string CreateDirAndGetFileName()
		{
			if (!Directory.Exists(this.filePath))
			{
				Directory.CreateDirectory(this.filePath);
			}
			return this.fileFullName;
		}

		private CsvTable CreateCsvTable()
		{
			CsvField[] array = new CsvField[this.columnNames.Length];
			for (int i = 0; i < this.columnNames.Length; i++)
			{
				array[i] = new CsvField(this.columnNames[i], typeof(string));
			}
			return new CsvTable(array);
		}

		private static bool TryGetString(CsvFieldCache cursor, int field, out string value)
		{
			value = (cursor.GetField(field) as string);
			if (!string.IsNullOrEmpty(value))
			{
				value = ((value == "-NA-") ? null : value);
				return true;
			}
			return false;
		}

		private const int CsvBufferSize = 4096;

		private const string PlaceHolderForEmptyString = "-NA-";

		private readonly string[] columnNames;

		private readonly CsvTable csvTable;

		private readonly MruDictionarySerializerWrite<TKey, TValue> writeValuesCallback;

		private readonly MruDictionarySerializerRead<TKey, TValue> readValuesCallback;

		private readonly string filePath;

		private readonly string fileFullName;

		private readonly LogHeaderFormatter headerFormatter;

		private readonly LogRowFormatter rowFormatter;

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private IMruDictionaryPerfCounters perfCounters;
	}
}
