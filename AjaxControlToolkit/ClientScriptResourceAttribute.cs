using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ClientScriptResourceAttribute : Attribute
	{
		public string ComponentType
		{
			get
			{
				return this.componentType;
			}
			set
			{
				this.componentType = value;
			}
		}

		public int LoadOrder
		{
			get
			{
				return this.loadOrder;
			}
			set
			{
				this.loadOrder = value;
			}
		}

		public string ResourcePath
		{
			get
			{
				return this.resourcePath;
			}
			set
			{
				this.resourcePath = value;
			}
		}

		public ClientScriptResourceAttribute()
		{
		}

		public ClientScriptResourceAttribute(string componentType)
		{
			this.componentType = componentType;
		}

		public ClientScriptResourceAttribute(string componentType, Type baseType, string resourceName)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			string text = baseType.FullName;
			int num = text.LastIndexOf('.');
			if (num != -1)
			{
				text = text.Substring(0, num);
			}
			this.ResourcePath = text + "." + resourceName;
			this.componentType = componentType;
		}

		public ClientScriptResourceAttribute(string componentType, string fullResourceName) : this(componentType)
		{
			if (fullResourceName == null)
			{
				throw new ArgumentNullException("fullResourceName");
			}
			this.ResourcePath = fullResourceName;
		}

		public override bool IsDefaultAttribute()
		{
			return this.ComponentType == null && this.ResourcePath == null;
		}

		private string resourcePath;

		private string componentType;

		private int loadOrder;
	}
}
