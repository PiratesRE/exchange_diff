using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class CsvTable
	{
		public CsvTable(CsvField[] fields)
		{
			this.fields = fields;
			CsvField[] array = new CsvField[this.fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = this.fields[i].CsvFieldImpl;
			}
			this.csvTableImpl = new CsvTable(array);
		}

		public CsvField[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		internal CsvTable CsvTableImpl
		{
			get
			{
				return this.csvTableImpl;
			}
		}

		public int NameToIndex(string name)
		{
			return this.csvTableImpl.NameToIndex(name);
		}

		public bool TryGetTypeByName(string name, out Type type)
		{
			return this.csvTableImpl.TryGetTypeByName(name, out type);
		}

		public int[] GetFieldsAddedAfterVersion(Version lowerBoundVersion)
		{
			return this.csvTableImpl.GetFieldsAddedAfterVersion(lowerBoundVersion);
		}

		public byte[] SerializeFieldNameList(Version highestBuildToInclude)
		{
			return this.csvTableImpl.SerializeFieldNameList(highestBuildToInclude);
		}

		private CsvTable csvTableImpl;

		private CsvField[] fields;
	}
}
