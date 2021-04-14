using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data
{
	internal class CsvSchema
	{
		public CsvSchema(int maximumRowCount, Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns)
		{
			this.MaximumRowCount = maximumRowCount;
			this.RequiredColumns = (requiredColumns ?? new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase));
			this.OptionalColumns = (optionalColumns ?? new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase));
			this.ProhibitedColumns = (prohibitedColumns ?? ((IEnumerable<string>)new string[0]));
			if (this.RequiredColumns.Comparer != StringComparer.OrdinalIgnoreCase)
			{
				throw new ArgumentOutOfRangeException("requiredColumns.Comparer", this.RequiredColumns.Comparer, "requiredColumns.Comparer must be StringComparer.OrdinalIgnoreCase");
			}
			if (this.OptionalColumns.Comparer != StringComparer.OrdinalIgnoreCase)
			{
				throw new ArgumentOutOfRangeException("optionalColumns.Comparer", this.OptionalColumns.Comparer, "optionalColumns.Comparer must be StringComparer.OrdinalIgnoreCase");
			}
		}

		private protected int MaximumRowCount { protected get; private set; }

		private protected Dictionary<string, ProviderPropertyDefinition> OptionalColumns { protected get; private set; }

		private protected IEnumerable<string> ProhibitedColumns { protected get; private set; }

		private protected Dictionary<string, ProviderPropertyDefinition> RequiredColumns { protected get; private set; }

		public IEnumerable<CsvRow> Read(Stream sourceStream)
		{
			return this.Read(sourceStream, null, true, true);
		}

		public IEnumerable<CsvRow> Read(Stream sourceStream, Encoding encoding, bool throwOnUnknownColumns, bool throwOnDuplicateColumns)
		{
			StreamReader sourceReader;
			if (encoding != null)
			{
				sourceReader = new StreamReader(sourceStream, encoding);
			}
			else
			{
				sourceReader = new StreamReader(sourceStream, true);
			}
			using (sourceReader)
			{
				using (CsvReader csvReader = new CsvReader(sourceReader, true))
				{
					if (csvReader.Headers != null)
					{
						CsvColumnMap columnMap = this.CreateColumnMap(csvReader.Headers, throwOnUnknownColumns, throwOnDuplicateColumns);
						yield return this.CreateCsvRow(0, csvReader.Headers, columnMap);
						int index = 0;
						foreach (string[] data in csvReader.ReadRows())
						{
							index++;
							CsvRow row = this.CreateCsvRow(index, data, columnMap);
							this.ValidateRow(row);
							yield return row;
						}
					}
				}
			}
			yield break;
		}

		public int Copy(Stream sourceStream, StreamWriter destinationWriter)
		{
			return this.Copy(sourceStream, destinationWriter, null);
		}

		public int Copy(Stream sourceStream, StreamWriter destinationWriter, Converter<CsvRow, CsvRow> rowProcessor)
		{
			int result;
			try
			{
				int num = 0;
				foreach (CsvRow csvRow in this.Read(sourceStream))
				{
					num = csvRow.Index;
					CsvRow csvRow2 = csvRow;
					if (rowProcessor != null)
					{
						csvRow2 = rowProcessor(csvRow);
					}
					destinationWriter.WriteCsvLine(csvRow2.Data);
				}
				if (num == 0)
				{
					throw new CsvFileIsEmptyException();
				}
				result = num;
			}
			finally
			{
				destinationWriter.Flush();
			}
			return result;
		}

		public virtual CsvColumnMap CreateColumnMap(IList<string> columnNames, bool throwOnUnknownColumns, bool throwOnDuplicateColumns)
		{
			CsvColumnMap result = new CsvColumnMap(columnNames, throwOnDuplicateColumns);
			foreach (string text in this.RequiredColumns.Keys)
			{
				if (!result.Contains(text))
				{
					throw new CsvRequiredColumnMissingException(text);
				}
			}
			foreach (string text2 in this.ProhibitedColumns)
			{
				if (result.Contains(text2))
				{
					throw new CsvProhibitedColumnPresentException(text2);
				}
			}
			if (throwOnUnknownColumns)
			{
				HashSet<string> hashSet = new HashSet<string>(columnNames, StringComparer.OrdinalIgnoreCase);
				hashSet.ExceptWith(this.RequiredColumns.Keys);
				hashSet.ExceptWith(this.OptionalColumns.Keys);
				if (hashSet.Count > 0)
				{
					string columns = '"' + string.Join("\", \"", hashSet.ToArray<string>()) + '"';
					throw new CsvUnknownColumnsException(columns, hashSet);
				}
			}
			return result;
		}

		public virtual void ValidateRow(CsvRow row)
		{
			if (row.Index > this.MaximumRowCount)
			{
				throw new CsvTooManyRowsException(this.MaximumRowCount);
			}
			foreach (KeyValuePair<string, ProviderPropertyDefinition> keyValuePair in this.RequiredColumns)
			{
				string key = keyValuePair.Key;
				string value = row[key];
				ProviderPropertyDefinition value2 = keyValuePair.Value;
				if (value.IsNullOrBlank())
				{
					PropertyValidationError error = new PropertyValidationError(DataStrings.PropertyIsMandatory, value2, null);
					this.OnPropertyValidationError(row, key, error);
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in row.GetExistingValues())
			{
				string key2 = keyValuePair2.Key;
				string value3 = keyValuePair2.Value ?? "";
				ProviderPropertyDefinition propertyDefinition = this.GetPropertyDefinition(key2);
				if (propertyDefinition != null)
				{
					foreach (PropertyDefinitionConstraint propertyDefinitionConstraint in propertyDefinition.AllConstraints)
					{
						PropertyConstraintViolationError propertyConstraintViolationError = propertyDefinitionConstraint.Validate(value3, propertyDefinition, null);
						if (propertyConstraintViolationError != null)
						{
							this.OnPropertyValidationError(row, key2, propertyConstraintViolationError);
						}
					}
				}
			}
		}

		public event Action<CsvRow, string, PropertyValidationError> PropertyValidationError;

		protected virtual CsvRow CreateCsvRow(int index, IList<string> data, CsvColumnMap columnMap)
		{
			return new CsvRow(index, data, columnMap);
		}

		protected void OnPropertyValidationError(CsvRow row, string columnName, PropertyValidationError error)
		{
			row.SetError(columnName, error);
			Action<CsvRow, string, PropertyValidationError> propertyValidationError = this.PropertyValidationError;
			if (propertyValidationError != null)
			{
				propertyValidationError(row, columnName, error);
			}
		}

		private ProviderPropertyDefinition GetPropertyDefinition(string columnName)
		{
			ProviderPropertyDefinition result = null;
			if (!this.OptionalColumns.TryGetValue(columnName, out result))
			{
				this.RequiredColumns.TryGetValue(columnName, out result);
			}
			return result;
		}
	}
}
