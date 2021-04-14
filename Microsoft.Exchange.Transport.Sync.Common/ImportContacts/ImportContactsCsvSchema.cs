using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ImportContactsCsvSchema : CsvSchema
	{
		public ImportContactsCsvSchema(Dictionary<string, ImportContactProperties> columnsMapping, CultureInfo culture) : base(2000, null, null, null)
		{
			SyncUtilities.ThrowIfArgumentNull("columnsMapping", columnsMapping);
			SyncUtilities.ThrowIfArgumentNull("culture", culture);
			this.columnsMapping = columnsMapping;
			this.culture = culture;
		}

		public Dictionary<string, ImportContactProperties> ColumnsMapping
		{
			get
			{
				return this.columnsMapping;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public static CsvRow? ReadCSVHeader(Stream sourceStream, Encoding encoding)
		{
			Stream stream = Streams.CreateSuppressCloseWrapper(sourceStream);
			CsvRow? result;
			using (StreamReader streamReader = new StreamReader(stream, encoding))
			{
				using (CsvReader csvReader = new CsvReader(streamReader, true))
				{
					if (ImportContactsCsvSchema.IsEmptyHeader(csvReader.Headers))
					{
						result = null;
					}
					else
					{
						CsvColumnMap columnMap = new CsvColumnMap(csvReader.Headers, false);
						result = new CsvRow?(new CsvRow(0, csvReader.Headers, columnMap));
					}
				}
			}
			return result;
		}

		protected override CsvRow CreateCsvRow(int index, IList<string> data, CsvColumnMap columnMap)
		{
			int num = columnMap.Count - data.Count;
			if (num > 0)
			{
				List<string> list = new List<string>(data);
				for (int i = 0; i < num; i++)
				{
					list.Add(string.Empty);
				}
				return base.CreateCsvRow(index, list, columnMap);
			}
			if (num == -1 && data[data.Count - 1] == string.Empty)
			{
				List<string> list2 = new List<string>(data);
				list2.RemoveAt(data.Count - 1);
				return base.CreateCsvRow(index, list2, columnMap);
			}
			return base.CreateCsvRow(index, data, columnMap);
		}

		private static bool IsEmptyHeader(string[] header)
		{
			return header == null || header.Length <= 0 || (header.Length == 1 && header[0].Trim() == string.Empty);
		}

		public const int InternalMaximumRowCount = 2000;

		private Dictionary<string, ImportContactProperties> columnsMapping;

		private CultureInfo culture;
	}
}
