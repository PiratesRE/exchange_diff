using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ObjectListSource : IListSource
	{
		public ObjectListSource(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.values = values;
		}

		public ObjectListSource(Array values, string displayProperty, string valueProperty) : this(values)
		{
			this.displayProperty = displayProperty;
			this.valueProperty = valueProperty;
		}

		public IList GetList()
		{
			ArrayList arrayList = new ArrayList(this.Values.Length);
			foreach (object objectValue in this.Values)
			{
				arrayList.Add(new ObjectListSourceItem(this.GetValueText(objectValue), this.GetValue(objectValue)));
			}
			if (arrayList.Count >= 13)
			{
				arrayList.Sort(this.comparer);
			}
			return arrayList;
		}

		public IComparer Comparer
		{
			get
			{
				return this.comparer;
			}
			set
			{
				this.comparer = value;
			}
		}

		public bool ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		public Array Values
		{
			get
			{
				return this.values;
			}
		}

		protected virtual string GetValueText(object objectValue)
		{
			string result = string.Empty;
			if (objectValue != null)
			{
				if (!string.IsNullOrEmpty(this.displayProperty))
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectValue)[this.displayProperty];
					result = string.Format("{0}", propertyDescriptor.GetValue(objectValue));
				}
				else
				{
					result = objectValue.ToString();
				}
			}
			return result;
		}

		private object GetValue(object objectValue)
		{
			object result = objectValue;
			if (!string.IsNullOrEmpty(this.valueProperty) && objectValue != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectValue)[this.valueProperty];
				result = propertyDescriptor.GetValue(objectValue);
			}
			return result;
		}

		private const int SortThreshold = 13;

		public const string ValueMemberColumnName = "Value";

		public const string DisplayMemberColumnName = "Text";

		private string displayProperty;

		private string valueProperty;

		private IComparer comparer;

		private Array values;
	}
}
