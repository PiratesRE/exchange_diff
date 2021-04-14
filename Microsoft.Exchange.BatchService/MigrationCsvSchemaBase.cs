using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationCsvSchemaBase : CsvSchema
	{
		protected MigrationCsvSchemaBase(int maximumRowCount, Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(maximumRowCount, requiredColumns, optionalColumns, prohibitedColumns)
		{
			this.csvHeaders = null;
			if (requiredColumns == null)
			{
				if (optionalColumns == null)
				{
					throw new ArgumentException("need to pass in either required and/or optional columns");
				}
				this.allColumns = new List<string>(optionalColumns.Keys);
				return;
			}
			else
			{
				if (optionalColumns == null)
				{
					this.allColumns = new List<string>(requiredColumns.Keys);
					return;
				}
				this.allColumns = new List<string>(from columnName in requiredColumns.Keys.Union(optionalColumns.Keys)
				orderby columnName
				select columnName);
				return;
			}
		}

		public bool AllowUnknownColumnsInCSV { get; set; }

		public static ProviderPropertyDefinition GetDefaultPropertyDefinition(string propertyName, PropertyDefinitionConstraint[] constraints)
		{
			return MigrationCsvSchemaBase.GetDefaultPropertyDefinition<string>(propertyName, constraints);
		}

		public static ProviderPropertyDefinition GetDefaultPropertyDefinition<T>(string propertyName, PropertyDefinitionConstraint[] constraints)
		{
			if (constraints == null)
			{
				constraints = PropertyDefinitionConstraint.None;
			}
			PropertyDefinitionFlags propertyDefinitionFlags = PropertyDefinitionFlags.None;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int) || typeFromHandle == typeof(uint) || typeFromHandle == typeof(long) || typeFromHandle == typeof(ulong) || typeFromHandle == typeof(ByteQuantifiedSize) || typeFromHandle == typeof(EnhancedTimeSpan) || typeFromHandle == typeof(DateTime))
			{
				propertyDefinitionFlags |= PropertyDefinitionFlags.PersistDefaultValue;
			}
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, typeFromHandle, propertyDefinitionFlags, default(T), constraints, constraints);
		}

		public static Dictionary<string, ProviderPropertyDefinition> GenerateDefaultColumnInfo(string[] columnNames = null)
		{
			Dictionary<string, ProviderPropertyDefinition> dictionary = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase);
			if (columnNames != null)
			{
				foreach (string text in columnNames)
				{
					dictionary[text] = MigrationCsvSchemaBase.GetDefaultPropertyDefinition(text, null);
				}
			}
			return dictionary;
		}

		public MigrationBatchError CreateValidationError(CsvRow row, LocalizedString errorMessage)
		{
			return new MigrationBatchError
			{
				RowIndex = row.Index,
				EmailAddress = this.GetIdentifier(row),
				LocalizedErrorMessage = errorMessage
			};
		}

		public virtual string GetIdentifier(CsvRow row)
		{
			return row["EmailAddress"];
		}

		public virtual void ProcessRow(CsvRow row, out MigrationBatchError error)
		{
			error = null;
		}

		public IEnumerable<string> GetOrderedHeaders()
		{
			if (this.csvHeaders == null)
			{
				throw new InvalidOperationException("expect to have headers before returning ordered row data");
			}
			return this.csvHeaders.OrderedHeaders;
		}

		public IEnumerable<string> GetOrderedRowData(CsvRow row)
		{
			if (this.csvHeaders == null)
			{
				throw new InvalidOperationException("expect to have headers before returning ordered row data");
			}
			return this.csvHeaders.GetOrderedRowData(row);
		}

		public override CsvColumnMap CreateColumnMap(IList<string> columnNames, bool throwOnUnknownColumns, bool throwOnDuplicateColumns)
		{
			CsvColumnMap csvColumnMap = base.CreateColumnMap(columnNames, !this.AllowUnknownColumnsInCSV, throwOnDuplicateColumns);
			this.csvHeaders = new MigrationCsvSchemaBase.CsvHeaders(this.allColumns, csvColumnMap);
			return csvColumnMap;
		}

		public virtual void WriteHeaders(StreamWriter streamWriter)
		{
			this.CreateColumnMap(base.RequiredColumns.Keys.Concat(base.OptionalColumns.Keys).ToList<string>(), true, true);
			streamWriter.WriteCsvLine(this.GetOrderedHeaders());
		}

		public const string EmailColumnName = "EmailAddress";

		internal static readonly PropertyDefinitionConstraint[] EmailAddressConstraint = new PropertyDefinitionConstraint[]
		{
			new ValidSmtpAddressConstraint(),
			new StringLengthConstraint(1, 1024)
		};

		private MigrationCsvSchemaBase.CsvHeaders csvHeaders;

		private List<string> allColumns;

		protected class CsvRangedValueConstraint<T> : RangedValueConstraint<T> where T : struct, IComparable
		{
			public CsvRangedValueConstraint(T minValue, T maxValue) : base(minValue, maxValue)
			{
			}

			public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
			{
				if (!(value is T))
				{
					Exception ex = null;
					try
					{
						TypeConverter converter = TypeDescriptor.GetConverter(propertyDefinition.Type);
						if (value is string)
						{
							value = converter.ConvertFromInvariantString((string)value);
						}
						else
						{
							value = converter.ConvertFrom(value);
						}
					}
					catch (ArgumentException ex2)
					{
						ex = ex2;
					}
					catch (FormatException ex3)
					{
						ex = ex3;
					}
					catch (NotSupportedException ex4)
					{
						ex = ex4;
					}
					catch (Exception ex5)
					{
						if (ex5.InnerException == null || !(ex5.InnerException is FormatException))
						{
							throw;
						}
						ex = ex5;
					}
					if (ex != null)
					{
						return new PropertyConstraintViolationError(DataStrings.PropertyTypeMismatch(value.GetType().ToString(), propertyDefinition.Type.ToString()), propertyDefinition, value, this);
					}
				}
				return base.Validate(value, propertyDefinition, propertyBag);
			}
		}

		private class CsvHeaders
		{
			public CsvHeaders(IList<string> headers, CsvColumnMap columnMap)
			{
				this.headers = headers;
				this.writeIndex = new List<int>();
				this.writeIndex.Capacity = headers.Count;
				foreach (string columnName in headers)
				{
					int item = columnMap.Contains(columnName) ? columnMap[columnName] : -1;
					this.writeIndex.Add(item);
				}
			}

			public IEnumerable<string> OrderedHeaders
			{
				get
				{
					return this.headers;
				}
			}

			public IEnumerable<string> GetOrderedRowData(CsvRow row)
			{
				foreach (int index in this.writeIndex)
				{
					yield return (index >= 0) ? row.Data[index] : string.Empty;
				}
				yield break;
			}

			private List<int> writeIndex;

			private IList<string> headers;
		}
	}
}
