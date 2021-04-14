using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableProperty
	{
		private QueryableProperty(string tag, string name, string type, object value)
		{
			this.tag = tag;
			this.name = name;
			this.type = type;
			this.value = value;
		}

		public string PropertyTag
		{
			get
			{
				return this.tag;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.name;
			}
		}

		public string PropertyType
		{
			get
			{
				return this.type;
			}
		}

		public object PropertyValue
		{
			get
			{
				return this.value;
			}
		}

		public static QueryableProperty Create(string tag, string name, string type, object value)
		{
			return new QueryableProperty(tag, name, type, value);
		}

		private readonly string tag;

		private readonly string name;

		private readonly string type;

		private readonly object value;
	}
}
