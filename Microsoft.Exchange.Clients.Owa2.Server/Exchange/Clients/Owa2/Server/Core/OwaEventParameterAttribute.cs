using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class OwaEventParameterAttribute : Attribute
	{
		public OwaEventParameterAttribute(string name, Type type, bool isArray, bool isOptional)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.name = name;
			this.isOptional = isOptional;
			this.isArray = isArray;
			this.paramType = type;
		}

		public OwaEventParameterAttribute(string name, Type type, bool isArray) : this(name, type, isArray, false)
		{
		}

		public OwaEventParameterAttribute(string name, Type type) : this(name, type, false, false)
		{
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type Type
		{
			get
			{
				return this.paramType;
			}
		}

		internal bool IsOptional
		{
			get
			{
				return this.isOptional;
			}
		}

		internal bool IsArray
		{
			get
			{
				return this.isArray;
			}
		}

		internal ulong ParameterMask
		{
			get
			{
				return this.parameterMask;
			}
			set
			{
				this.parameterMask = value;
			}
		}

		private string name;

		private Type paramType;

		private bool isOptional;

		private bool isArray;

		private ulong parameterMask;
	}
}
