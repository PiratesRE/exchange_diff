using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class CsvTable
	{
		public CsvTable(CsvField[] fields)
		{
			List<CsvField> list = new List<CsvField>();
			this.fields = (CsvField[])fields.Clone();
			for (int i = 0; i < fields.Length; i++)
			{
				this.nameMap.Add(fields[i].Name, i);
				this.nameToTypeMap.Add(fields[i].Name, fields[i].Type);
				if (this.fields[i].IsIndexed)
				{
					list.Add(this.fields[i]);
				}
			}
			this.indexedFields = list.ToArray();
			this.index = new CsvClusteredIndex(0);
		}

		public CsvField[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		public CsvField[] IndexedFields
		{
			get
			{
				return this.indexedFields;
			}
		}

		public CsvClusteredIndex ClusteredIndex
		{
			get
			{
				return this.index;
			}
		}

		public int NameToIndex(string name)
		{
			int result;
			if (!this.nameMap.TryGetValue(name, out result))
			{
				return -1;
			}
			return result;
		}

		public bool TryGetTypeByName(string name, out Type type)
		{
			return this.nameToTypeMap.TryGetValue(name, out type);
		}

		public int[] GetFieldsAddedAfterVersion(Version lowerBoundVersion)
		{
			List<int> list = new List<int>(this.fields.Length);
			for (int i = 0; i < this.Fields.Length; i++)
			{
				Version buildAdded = this.fields[i].BuildAdded;
				if (!(buildAdded == null) && !(buildAdded <= lowerBoundVersion))
				{
					list.Add(i);
				}
			}
			return list.ToArray();
		}

		public byte[] SerializeFieldNameList(Version highestBuildToInclude)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.Fields[i].BuildAdded <= highestBuildToInclude)
				{
					if (flag)
					{
						stringBuilder.Append(this.fields[i].Name);
						flag = false;
					}
					else
					{
						stringBuilder.AppendFormat(",{0}", this.fields[i].Name);
					}
				}
			}
			stringBuilder.AppendFormat("\r\n", new object[0]);
			return Encoding.ASCII.GetBytes(stringBuilder.ToString());
		}

		private CsvField[] fields;

		private CsvField[] indexedFields;

		private Dictionary<string, int> nameMap = new Dictionary<string, int>();

		private Dictionary<string, Type> nameToTypeMap = new Dictionary<string, Type>();

		private CsvClusteredIndex index;
	}
}
