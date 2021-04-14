using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class SchemaState
	{
		protected SchemaState()
		{
		}

		protected List<IProperty>[] ConversionTable
		{
			get
			{
				return this.conversionTable;
			}
			set
			{
				this.conversionTable = value;
			}
		}

		protected int LinkedSchemas
		{
			get
			{
				return this.linkedSchemas;
			}
		}

		public void AddProperty(IProperty[] linkedPropertyList)
		{
			if (linkedPropertyList == null)
			{
				throw new ArgumentNullException("linkedPropertyList");
			}
			if (this.linkedSchemas != linkedPropertyList.Length)
			{
				throw new ArgumentException("linkedPropertyList supports a const number of linked schemas");
			}
			int count = this.conversionTable[0].Count;
			for (int i = 0; i < linkedPropertyList.Length; i++)
			{
				linkedPropertyList[i].SchemaLinkId = count;
				this.conversionTable[i].Add(linkedPropertyList[i]);
			}
		}

		public List<IProperty> GetSchema(int schemaNumber)
		{
			return this.conversionTable[schemaNumber];
		}

		protected void InitConversionTable(int linkedSchemas)
		{
			if (linkedSchemas <= 0)
			{
				throw new ArgumentException("cLinkedSchemas must be > 0");
			}
			this.linkedSchemas = linkedSchemas;
			this.conversionTable = new List<IProperty>[this.linkedSchemas];
			for (int i = 0; i < this.linkedSchemas; i++)
			{
				this.conversionTable[i] = new List<IProperty>();
			}
		}

		private List<IProperty>[] conversionTable;

		private int linkedSchemas = -1;
	}
}
