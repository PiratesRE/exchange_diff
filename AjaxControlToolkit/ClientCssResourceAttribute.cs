using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ClientCssResourceAttribute : Attribute
	{
		public ClientCssResourceAttribute(Type baseType, string resourceName)
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
			this.resourcePath = text + '.' + resourceName;
		}

		public ClientCssResourceAttribute(string fullResourceName)
		{
			if (fullResourceName == null)
			{
				throw new ArgumentNullException("fullResourceName");
			}
			this.resourcePath = fullResourceName;
		}

		public string ResourcePath
		{
			get
			{
				return this.resourcePath;
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

		private string resourcePath;

		private int loadOrder;
	}
}
