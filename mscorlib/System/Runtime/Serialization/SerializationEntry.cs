using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public struct SerializationEntry
	{
		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public Type ObjectType
		{
			get
			{
				return this.m_type;
			}
		}

		internal SerializationEntry(string entryName, object entryValue, Type entryType)
		{
			this.m_value = entryValue;
			this.m_name = entryName;
			this.m_type = entryType;
		}

		private Type m_type;

		private object m_value;

		private string m_name;
	}
}
