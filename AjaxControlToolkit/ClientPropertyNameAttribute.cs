using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ClientPropertyNameAttribute : Attribute
	{
		public ClientPropertyNameAttribute()
		{
		}

		public ClientPropertyNameAttribute(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public override bool IsDefaultAttribute()
		{
			return string.IsNullOrEmpty(this.PropertyName);
		}

		private string propertyName;
	}
}
