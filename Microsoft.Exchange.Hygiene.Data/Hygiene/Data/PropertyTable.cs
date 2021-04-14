using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class PropertyTable : DataTable
	{
		public PropertyTable()
		{
			this.InitializeSchema();
		}

		public void AddPropertyValue(PropertyDefinition prop, object value)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("prop");
			}
			if (prop.Type.Equals(typeof(DataTable)))
			{
				throw new ArgumentException("Cannot add a property of type DataTable");
			}
			if (value is MultiValuedPropertyBase)
			{
				int num = 0;
				foreach (object value2 in ((IEnumerable)((MultiValuedPropertyBase)value)))
				{
					this.AddRow(prop, value2, num++);
				}
				if (num == 0)
				{
					this.AddRow(prop, null, -1);
					return;
				}
			}
			else
			{
				if (value is MultiValuedQueryFilter)
				{
					int num2 = 0;
					using (HashSet<object>.Enumerator enumerator2 = ((MultiValuedQueryFilter)value).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object value3 = enumerator2.Current;
							this.AddRow(prop, value3, num2++);
						}
						return;
					}
				}
				this.AddRow(prop, value, 0);
			}
		}

		internal static string GetColumn(PropertyDefinition prop)
		{
			string result;
			try
			{
				Type key = DalHelper.ConvertToStoreType(prop);
				string text = null;
				PropertyTable.PropertyValueColumns.TryGetValue(key, out text);
				result = text;
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(HygieneDataStrings.ErrorInvalidColumnType(prop.Name, prop.Type.Name), innerException);
			}
			return result;
		}

		protected virtual DataRow AddRow(PropertyDefinition prop, object value, int propertyIndex = 0)
		{
			DataRow dataRow = base.NewRow();
			DataRow result;
			try
			{
				dataRow[PropertyTable.PropertyNameCol] = prop.Name;
				dataRow[PropertyTable.PropertyIndexCol] = propertyIndex;
				if (value == null)
				{
					result = dataRow;
				}
				else
				{
					string text = PropertyTable.GetColumn(prop);
					if (text == null)
					{
						result = dataRow;
					}
					else
					{
						object obj = DalHelper.ConvertToStoreObject(value, SqlDbType.Variant);
						if (text == PropertyTable.PropertyValueGuidCol && (prop.Type == typeof(ADObjectId) || prop.Type == typeof(Guid)) && obj is string)
						{
							Guid guid;
							if (Guid.TryParse(obj.ToString(), out guid))
							{
								obj = guid;
								dataRow[PropertyTable.PropertyValueStringCol] = obj;
							}
							else
							{
								text = PropertyTable.PropertyValueStringCol;
							}
						}
						if (text == PropertyTable.PropertyValueStringCol && obj.ToString().Length > 255)
						{
							text = PropertyTable.PropertyValueBlobCol;
						}
						if (text == PropertyTable.PropertyValueDateTimeCol && prop.Type == typeof(DateTime) && (DateTime)obj == DateTime.MinValue)
						{
							result = dataRow;
						}
						else
						{
							dataRow[text] = obj;
							if (text == PropertyTable.PropertyValueLongCol && (obj is long || obj is ulong))
							{
								dataRow[PropertyTable.PropertyValueDecimalCol] = obj;
							}
							result = dataRow;
						}
					}
				}
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(string.Format("Error Setting property {0} of type {1} to value {2} of type {3}", new object[]
				{
					prop.Name,
					prop.Type.Name,
					value ?? DalHelper.NullString,
					(value != null) ? value.GetType().Name : DalHelper.NullString
				}), innerException);
			}
			finally
			{
				base.Rows.Add(dataRow);
			}
			return result;
		}

		private void InitializeSchema()
		{
			base.TableName = "PropertyTable";
			base.Columns.Add(new DataColumn(PropertyTable.PropertyNameCol, typeof(string)));
			base.Columns.Add(new DataColumn(PropertyTable.PropertyIndexCol, typeof(int)));
			foreach (KeyValuePair<string, Type> keyValuePair in PropertyTable.PropertyColumnTypes)
			{
				base.Columns.Add(new DataColumn(keyValuePair.Key, keyValuePair.Value));
			}
		}

		internal static readonly string PropertyNameCol = "nvc_PropertyName";

		internal static readonly string PropertyIndexCol = "i_PropertyIndex";

		internal static readonly string PropertyValueGuidCol = "id_PropertyValueGuid";

		internal static readonly string PropertyValueIntegerCol = "i_PropertyValueInteger";

		internal static readonly string PropertyValueLongCol = "bi_PropertyValueLong";

		internal static readonly string PropertyValueStringCol = "nvc_PropertyValueString";

		internal static readonly string PropertyValueDateTimeCol = "dt_PropertyValueDateTime";

		internal static readonly string PropertyValueBitCol = "f_PropertyValueBit";

		internal static readonly string PropertyValueDecimalCol = "d_PropertyValueDecimal";

		internal static readonly string PropertyValueBlobCol = "nvc_PropertyValueBlob";

		internal static readonly Dictionary<Type, string> PropertyValueColumns = new Dictionary<Type, string>
		{
			{
				typeof(byte),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(sbyte),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(short),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(ushort),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(int),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(uint),
				PropertyTable.PropertyValueIntegerCol
			},
			{
				typeof(Guid),
				PropertyTable.PropertyValueGuidCol
			},
			{
				typeof(ADObjectId),
				PropertyTable.PropertyValueGuidCol
			},
			{
				typeof(ConfigObjectId),
				PropertyTable.PropertyValueGuidCol
			},
			{
				typeof(DateTime),
				PropertyTable.PropertyValueDateTimeCol
			},
			{
				typeof(decimal),
				PropertyTable.PropertyValueDecimalCol
			},
			{
				typeof(double),
				PropertyTable.PropertyValueDecimalCol
			},
			{
				typeof(float),
				PropertyTable.PropertyValueDecimalCol
			},
			{
				typeof(bool),
				PropertyTable.PropertyValueBitCol
			},
			{
				typeof(byte[]),
				PropertyTable.PropertyValueBlobCol
			},
			{
				typeof(string),
				PropertyTable.PropertyValueStringCol
			},
			{
				typeof(ulong),
				PropertyTable.PropertyValueLongCol
			},
			{
				typeof(long),
				PropertyTable.PropertyValueLongCol
			}
		};

		internal static readonly Dictionary<string, Type> PropertyColumnTypes = new Dictionary<string, Type>
		{
			{
				PropertyTable.PropertyValueGuidCol,
				typeof(Guid)
			},
			{
				PropertyTable.PropertyValueIntegerCol,
				typeof(int)
			},
			{
				PropertyTable.PropertyValueLongCol,
				typeof(long)
			},
			{
				PropertyTable.PropertyValueStringCol,
				typeof(string)
			},
			{
				PropertyTable.PropertyValueDateTimeCol,
				typeof(DateTime)
			},
			{
				PropertyTable.PropertyValueBitCol,
				typeof(bool)
			},
			{
				PropertyTable.PropertyValueDecimalCol,
				typeof(float)
			},
			{
				PropertyTable.PropertyValueBlobCol,
				typeof(string)
			}
		};
	}
}
