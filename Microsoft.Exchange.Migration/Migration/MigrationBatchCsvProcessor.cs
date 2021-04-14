using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationBatchCsvProcessor
	{
		public MigrationBatchCsvProcessor(MigrationCsvSchemaBase schema)
		{
			this.schema = schema;
		}

		protected virtual bool ValidationWarningAsError
		{
			get
			{
				return false;
			}
		}

		public static MigrationBatchError GetRowValidationError(CsvRow row, MigrationCsvSchemaBase schema)
		{
			LocalizedString localizedString = LocalizedString.Empty;
			foreach (string text in row.Errors.Keys)
			{
				PropertyValidationError propertyValidationError = row.Errors[text];
				LocalizedString localizedString2 = ServerStrings.ColumnError(text, propertyValidationError.Description);
				localizedString = (localizedString.IsEmpty ? localizedString2 : ServerStrings.CompositeError(localizedString, localizedString2));
			}
			if (!localizedString.IsEmpty)
			{
				return schema.CreateValidationError(row, localizedString);
			}
			return null;
		}

		internal LocalizedException ProcessCsv(MigrationBatch batch, byte[] csvData)
		{
			Stream stream2;
			int totalCount;
			MultiValuedProperty<MigrationBatchError> validationWarnings;
			LocalizedException result;
			using (Stream stream = new MemoryStream(csvData))
			{
				stream2 = new MemoryStream(Convert.ToInt32(stream.Length));
				using (StreamWriter streamWriter = new StreamWriter(stream2, MigrationBatchCsvProcessor.UTF8NoBOM, 1024, true))
				{
					this.schema.AllowUnknownColumnsInCSV = batch.AllowUnknownColumnsInCsv;
					result = this.CopyAndValidateCsv(stream, streamWriter, out totalCount, out validationWarnings);
				}
				stream2.Seek(0L, SeekOrigin.Begin);
			}
			batch.CsvStream = stream2;
			batch.TotalCount = totalCount;
			batch.ValidationWarnings = validationWarnings;
			return result;
		}

		internal LocalizedException CopyAndValidateCsv(Stream sourceStream, StreamWriter destinationWriter, out int dataRowCount, out MultiValuedProperty<MigrationBatchError> validationWarnings)
		{
			Dictionary<int, MigrationBatchError> dictionary = new Dictionary<int, MigrationBatchError>();
			dataRowCount = 0;
			validationWarnings = new MultiValuedProperty<MigrationBatchError>();
			try
			{
				foreach (CsvRow row in this.schema.Read(sourceStream))
				{
					bool flag;
					LocalizedException ex = this.ProcessCsvRow(row, dictionary, out flag);
					if (ex != null)
					{
						return ex;
					}
					if (flag)
					{
						dataRowCount++;
					}
					destinationWriter.WriteCsvLine(row.Data);
				}
				if (dataRowCount == 0)
				{
					return new CsvFileIsEmptyException();
				}
			}
			finally
			{
				destinationWriter.Flush();
			}
			validationWarnings = new MultiValuedProperty<MigrationBatchError>(dictionary.Values);
			return this.Validate();
		}

		protected virtual LocalizedException InternalProcessRow(CsvRow row, out bool isDataRow)
		{
			isDataRow = (row.Index != 0);
			return null;
		}

		protected virtual LocalizedException Validate()
		{
			return null;
		}

		private LocalizedException ProcessCsvRow(CsvRow row, Dictionary<int, MigrationBatchError> warnings, out bool isDataRow)
		{
			if (row.Index == 0)
			{
				isDataRow = false;
				return null;
			}
			MigrationBatchError migrationBatchError = MigrationBatchCsvProcessor.GetRowValidationError(row, this.schema);
			MigrationBatchError migrationBatchError2;
			this.schema.ProcessRow(row, out migrationBatchError2);
			migrationBatchError = (migrationBatchError ?? migrationBatchError2);
			if (migrationBatchError != null)
			{
				if (this.ValidationWarningAsError)
				{
					isDataRow = false;
					return new MigrationCSVParsingException(row.Index, migrationBatchError.LocalizedErrorMessage);
				}
				warnings[migrationBatchError.RowIndex] = migrationBatchError;
			}
			return this.InternalProcessRow(row, out isDataRow);
		}

		private const int DefaultBufferSize = 1024;

		private static readonly UTF8Encoding UTF8NoBOM = new UTF8Encoding(false, true);

		private readonly MigrationCsvSchemaBase schema;
	}
}
