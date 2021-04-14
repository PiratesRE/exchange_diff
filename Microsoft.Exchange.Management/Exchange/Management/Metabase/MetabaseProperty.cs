using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal class MetabaseProperty
	{
		public MetabaseProperty()
		{
		}

		public MetabaseProperty(string name, object value) : this(name, value, true)
		{
		}

		public MetabaseProperty(string name, object value, bool eraseOldValue)
		{
			this.propertyName = name;
			this.propertyValue = value;
			this.eraseOldValue = eraseOldValue;
		}

		public string Name
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
			}
		}

		public object Value
		{
			get
			{
				return this.propertyValue;
			}
			set
			{
				this.propertyValue = value;
			}
		}

		public bool EraseOldValue
		{
			get
			{
				return this.eraseOldValue;
			}
			set
			{
				this.eraseOldValue = value;
			}
		}

		private string propertyName;

		private object propertyValue;

		private bool eraseOldValue;
	}
}
