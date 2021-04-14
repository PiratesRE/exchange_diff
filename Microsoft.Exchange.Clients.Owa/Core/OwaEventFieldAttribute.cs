using System;
using System.Reflection;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class OwaEventFieldAttribute : Attribute
	{
		public OwaEventFieldAttribute(string name, bool isOptional, object defaultValue)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.isOptional = isOptional;
			this.defaultValue = defaultValue;
		}

		public OwaEventFieldAttribute(string name) : this(name, false, null)
		{
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal bool IsOptional
		{
			get
			{
				return this.isOptional;
			}
		}

		internal FieldInfo FieldInfo
		{
			get
			{
				return this.fieldInfo;
			}
			set
			{
				this.fieldInfo = value;
			}
		}

		internal Type FieldType
		{
			get
			{
				return this.fieldType;
			}
			set
			{
				this.fieldType = value;
			}
		}

		internal uint FieldMask
		{
			get
			{
				return this.fieldMask;
			}
			set
			{
				this.fieldMask = value;
			}
		}

		internal object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		private string name;

		private bool isOptional;

		private FieldInfo fieldInfo;

		private Type fieldType;

		private uint fieldMask;

		private object defaultValue;
	}
}
