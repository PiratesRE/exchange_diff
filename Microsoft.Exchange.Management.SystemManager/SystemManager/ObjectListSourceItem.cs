using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ObjectListSourceItem : IComparable
	{
		public ObjectListSourceItem(string text, object value)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("Text can not be null or empty", "text");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.value = value;
			this.text = text;
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			ObjectListSourceItem objectListSourceItem = obj as ObjectListSourceItem;
			if (objectListSourceItem == null)
			{
				throw new ArgumentException();
			}
			return this.Text.CompareTo(objectListSourceItem.Text);
		}

		public override bool Equals(object obj)
		{
			ObjectListSourceItem objectListSourceItem = obj as ObjectListSourceItem;
			return objectListSourceItem != null && this.Value.Equals(objectListSourceItem.Value);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		private string text;

		private object value;
	}
}
