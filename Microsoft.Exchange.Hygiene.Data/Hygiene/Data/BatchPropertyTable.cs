using System;
using System.Data;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class BatchPropertyTable : PropertyTable
	{
		public BatchPropertyTable()
		{
			this.InitializeSchema();
		}

		public void AddPropertyValue(Guid identity, PropertyDefinition prop, object value)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("prop");
			}
			if (prop.Type.Equals(typeof(DataTable)))
			{
				throw new ArgumentException("Cannot add a property of type DataTable");
			}
			this.rowIdentity = new Guid?(identity);
			base.AddPropertyValue(prop, value);
			this.rowIdentity = null;
		}

		public void AddPropertyValue(ADObjectId identity, PropertyDefinition prop, object value)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.AddPropertyValue(identity.ObjectGuid, prop, value);
		}

		public void AddPropertyValue(ConfigObjectId identity, PropertyDefinition prop, object value)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.AddPropertyValue(Guid.Parse(identity.ToString()), prop, value);
		}

		internal static BatchPropertyTable Deserialize(string tableXmlString)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			if (!string.IsNullOrWhiteSpace(tableXmlString))
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(tableXmlString), BatchPropertyTable.xrs))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "row")
						{
							throw new InvalidOperationException("XML data is invalid. Unable to deserialize xml string.");
						}
						DataRow dataRow = batchPropertyTable.NewRow();
						string value;
						if (BatchPropertyTable.GetAttributeValue(xmlReader, BatchPropertyTable.IdentityColumn, out value))
						{
							dataRow[BatchPropertyTable.IdentityColumn] = value;
						}
						if (BatchPropertyTable.GetAttributeValue(xmlReader, PropertyTable.PropertyNameCol, out value))
						{
							dataRow[PropertyTable.PropertyNameCol] = value;
						}
						if (BatchPropertyTable.GetAttributeValue(xmlReader, PropertyTable.PropertyIndexCol, out value))
						{
							dataRow[PropertyTable.PropertyIndexCol] = value;
						}
						foreach (string text in PropertyTable.PropertyColumnTypes.Keys)
						{
							if (BatchPropertyTable.GetAttributeValue(xmlReader, text, out value))
							{
								dataRow[text] = value;
							}
						}
						batchPropertyTable.Rows.Add(dataRow);
					}
				}
			}
			return batchPropertyTable;
		}

		protected override DataRow AddRow(PropertyDefinition prop, object value, int propertyIndex = 0)
		{
			DataRow dataRow = base.AddRow(prop, value, propertyIndex);
			dataRow[BatchPropertyTable.IdentityColumn] = this.rowIdentity.Value;
			return dataRow;
		}

		private static bool GetAttributeValue(XmlReader reader, string attributeName, out string attributeValue)
		{
			bool result = false;
			string attribute = reader.GetAttribute(attributeName);
			attributeValue = null;
			if (!string.IsNullOrWhiteSpace(attribute))
			{
				attributeValue = attribute;
				result = true;
			}
			return result;
		}

		private void InitializeSchema()
		{
			base.TableName = "BatchPropertyTable";
			DataColumn dataColumn = new DataColumn(BatchPropertyTable.IdentityColumn, typeof(Guid));
			base.Columns.Add(dataColumn);
			dataColumn.SetOrdinal(0);
		}

		internal static readonly string IdentityColumn = "id_Identity";

		private static readonly XmlReaderSettings xrs = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment
		};

		private Guid? rowIdentity;
	}
}
