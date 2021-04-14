using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class RequiredScriptAttribute : Attribute
	{
		public Type ExtenderType
		{
			get
			{
				return this.extenderType;
			}
		}

		public string ScriptName
		{
			get
			{
				return this.scriptName;
			}
		}

		public int LoadOrder
		{
			get
			{
				return this.order;
			}
		}

		public RequiredScriptAttribute()
		{
		}

		public RequiredScriptAttribute(string scriptName)
		{
			this.scriptName = scriptName;
		}

		public RequiredScriptAttribute(Type extenderType) : this(extenderType, 0)
		{
		}

		public RequiredScriptAttribute(Type extenderType, int loadOrder)
		{
			this.extenderType = extenderType;
			this.order = loadOrder;
		}

		public override bool IsDefaultAttribute()
		{
			return this.extenderType == null;
		}

		private int order;

		private Type extenderType;

		private string scriptName;
	}
}
