using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class ParsedReadOnlyRow
	{
		public ParsedReadOnlyRow(ReadOnlyRow unparsedRow)
		{
			this.table = unparsedRow.Schema;
			this.unparsedRow = unparsedRow;
			this.parsedRow = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		}

		public CsvTable Schema
		{
			get
			{
				return this.table;
			}
		}

		public ReadOnlyRow UnParsedRow
		{
			get
			{
				return this.unparsedRow;
			}
		}

		public static object ConvertToTargetObjectType(string propertyName, object obj, Type targetType)
		{
			if (obj == null)
			{
				return null;
			}
			if (targetType.IsAssignableFrom(obj.GetType()))
			{
				return obj;
			}
			object result;
			try
			{
				if (targetType.IsEnum)
				{
					string value = obj.ToString();
					object obj2 = Enum.Parse(targetType, value);
					if (!Enum.IsDefined(targetType, obj2))
					{
						throw new InvalidCastException(string.Format("Cannot convert object type {0} to {1}, for field:{2}", obj.GetType().Name, targetType.Name, propertyName));
					}
					result = obj2;
				}
				else if (targetType.IsAssignableFrom(typeof(Guid)))
				{
					result = Guid.Parse(obj.ToString());
				}
				else
				{
					result = Convert.ChangeType(obj, targetType);
				}
			}
			catch (InvalidCastException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new InvalidCastException(string.Format("Cannot convert object type {0} to {1}, for field:{2}", obj.GetType().Name, targetType.Name, propertyName), innerException);
			}
			return result;
		}

		public bool HasCustomField(string customfieldName)
		{
			if (this.CustomFieldHasData(customfieldName))
			{
				return true;
			}
			this.ParseField(customfieldName, typeof(KeyValuePair<string, object>[]));
			return this.CustomFieldHasData(customfieldName);
		}

		public T GetField<T>(string fieldName)
		{
			T result;
			try
			{
				result = (T)((object)this.GetField(fieldName, typeof(T)));
			}
			catch (InvalidLogLineException)
			{
				throw;
			}
			catch (Exception inner)
			{
				throw new InvalidLogLineException(Strings.FailedToCastToRequestedType(typeof(T), fieldName), inner);
			}
			return result;
		}

		public object GetField(string fieldName, Type targetType)
		{
			object obj;
			if (this.parsedRow.ContainsKey(fieldName))
			{
				obj = this.parsedRow[fieldName];
			}
			else
			{
				Type fieldType;
				if (!this.unparsedRow.Schema.TryGetTypeByName(fieldName, out fieldType))
				{
					throw new InvalidLogLineException(Strings.UnknownField(fieldName));
				}
				obj = this.ParseField(fieldName, fieldType);
			}
			return ParsedReadOnlyRow.ConvertToTargetObjectType(fieldName, obj, targetType);
		}

		public KeyValuePair<string, object>[] GetCustomFieldData(string customfieldName)
		{
			if (!this.HasCustomField(customfieldName))
			{
				throw new MissingPropertyException(Strings.RequestedCustomDataFieldMissing(customfieldName));
			}
			return (KeyValuePair<string, object>[])this.parsedRow[customfieldName];
		}

		public object GetPropertyValueFromCustomField(string customfieldName, string propertyName)
		{
			if (string.IsNullOrWhiteSpace(customfieldName))
			{
				throw new ArgumentException("customfieldName cannot be null or empty");
			}
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException("propertyName cannot be null or empty");
			}
			object result = null;
			KeyValuePair<string, object>[] field = this.GetField<KeyValuePair<string, object>[]>(customfieldName);
			if (field != null)
			{
				KeyValuePair<string, object> keyValuePair = field.FirstOrDefault((KeyValuePair<string, object> kvp) => kvp.Key.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					result = keyValuePair.Value;
				}
			}
			return result;
		}

		public void ParseAllFields()
		{
			for (int i = 0; i < this.unparsedRow.Schema.Fields.Length; i++)
			{
				CsvField csvField = this.unparsedRow.Schema.Fields[i];
				Type type = csvField.Type;
				string name = csvField.Name;
				this.ParseField(name, type);
			}
		}

		private object ParseField(string fieldName, Type fieldType)
		{
			object result;
			try
			{
				if (fieldType == typeof(string))
				{
					this.parsedRow[fieldName] = this.unparsedRow.GetField<string>(fieldName);
				}
				else if (fieldType == typeof(DateTime))
				{
					this.parsedRow[fieldName] = this.unparsedRow.GetField<DateTime>(fieldName);
				}
				else if (fieldType == typeof(int))
				{
					this.parsedRow[fieldName] = this.unparsedRow.GetField<int>(fieldName);
				}
				else if (fieldType == typeof(KeyValuePair<string, object>[]))
				{
					this.parsedRow[fieldName] = this.unparsedRow.GetField<KeyValuePair<string, object>[]>(fieldName);
				}
				else if (fieldType == typeof(string[]))
				{
					this.parsedRow[fieldName] = this.unparsedRow.GetField<string[]>(fieldName);
				}
				else
				{
					if (!(fieldType == typeof(int[])))
					{
						throw new NotSupportedException(string.Format("Parsing CsvField Type:{0} is not supported, for column name:{1}", fieldType, fieldName));
					}
					this.parsedRow[fieldName] = this.unparsedRow.GetField<int[]>(fieldName);
				}
				result = this.parsedRow[fieldName];
			}
			catch (Exception inner)
			{
				throw new InvalidLogLineException(Strings.FailedToParseField(fieldName, fieldType), inner);
			}
			return result;
		}

		private bool CustomFieldHasData(string customFieldName)
		{
			return this.parsedRow.ContainsKey(customFieldName) && this.parsedRow[customFieldName] != null && ((KeyValuePair<string, object>[])this.parsedRow[customFieldName]).Length != 0;
		}

		private readonly CsvTable table;

		private readonly Dictionary<string, object> parsedRow;

		private readonly ReadOnlyRow unparsedRow;
	}
}
