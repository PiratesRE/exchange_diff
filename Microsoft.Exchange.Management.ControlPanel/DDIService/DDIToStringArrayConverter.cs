using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIToStringArrayConverter : IOutputConverter, IDDIConverter
	{
		public DDIToStringArrayConverter(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public bool CanConvert(object sourceObject)
		{
			if (sourceObject == null)
			{
				return false;
			}
			IEnumerable enumerable = sourceObject as IEnumerable;
			if (enumerable == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(this.propertyName))
			{
				foreach (object obj in enumerable)
				{
					if (obj.GetType().GetProperty(this.propertyName) == null)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public object Convert(object sourceObject)
		{
			IEnumerable enumerable = sourceObject as IEnumerable;
			List<string> list = new List<string>();
			bool flag = !string.IsNullOrEmpty(this.propertyName);
			foreach (object obj in enumerable)
			{
				if (flag)
				{
					list.Add((string)obj.GetType().GetProperty(this.propertyName).GetValue(obj, null));
				}
				else
				{
					list.Add(obj.ToString());
				}
			}
			return list.ToArray();
		}

		private readonly string propertyName;
	}
}
