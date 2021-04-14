using System;
using System.Collections;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OwaEventStructAttribute : Attribute
	{
		public OwaEventStructAttribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.fieldInfoTable = new Hashtable();
			this.fieldInfoIndexTable = new OwaEventFieldAttribute[32];
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal uint RequiredMask
		{
			get
			{
				return this.requiredMask;
			}
			set
			{
				this.requiredMask = value;
			}
		}

		internal uint AllFieldsMask
		{
			get
			{
				return this.allFieldsMask;
			}
			set
			{
				this.allFieldsMask = value;
			}
		}

		internal int FieldCount
		{
			get
			{
				return this.fieldCount;
			}
			set
			{
				this.fieldCount = value;
			}
		}

		internal Type StructType
		{
			get
			{
				return this.structType;
			}
			set
			{
				this.structType = value;
			}
		}

		internal void AddFieldInfo(OwaEventFieldAttribute fieldInfo, int index)
		{
			this.fieldInfoTable.Add(fieldInfo.Name, fieldInfo);
			this.fieldInfoIndexTable[index] = fieldInfo;
		}

		internal OwaEventFieldAttribute FindFieldInfo(string name)
		{
			return (OwaEventFieldAttribute)this.fieldInfoTable[name];
		}

		internal Hashtable FieldInfoTable
		{
			get
			{
				return this.fieldInfoTable;
			}
		}

		internal OwaEventFieldAttribute[] FieldInfoIndexTable
		{
			get
			{
				return this.fieldInfoIndexTable;
			}
		}

		private string name;

		private Hashtable fieldInfoTable;

		private OwaEventFieldAttribute[] fieldInfoIndexTable;

		private Type structType;

		private uint requiredMask;

		private uint allFieldsMask;

		private int fieldCount;
	}
}
