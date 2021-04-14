using System;

namespace System.Runtime.Serialization
{
	internal class TypeLoadExceptionHolder
	{
		internal TypeLoadExceptionHolder(string typeName)
		{
			this.m_typeName = typeName;
		}

		internal string TypeName
		{
			get
			{
				return this.m_typeName;
			}
		}

		private string m_typeName;
	}
}
