using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class DDINoDuplicationAttribute : DDIValidateAttribute
	{
		public DDINoDuplicationAttribute() : base("DDINoDuplicationAttribute")
		{
			this.PropertyName = "Name";
		}

		public string PropertyName { get; set; }

		public override List<string> Validate(object target, Service profile)
		{
			List<string> list = new List<string>();
			if (target != null)
			{
				IEnumerable enumerable = target as IEnumerable;
				if (enumerable == null)
				{
					throw new ArgumentException("DDINoDuplicationAttribute can only be applied to Collection object");
				}
				if (enumerable.OfType<object>().Count<object>() > 0)
				{
					PropertyInfo propertyInfo = null;
					if (!string.IsNullOrEmpty(this.PropertyName))
					{
						object obj = enumerable.OfType<object>().First<object>();
						propertyInfo = obj.GetType().GetPropertyEx(this.PropertyName);
						if (propertyInfo == null)
						{
							throw new ArgumentException(string.Format("{0} is not a valid property in class {1}", this.PropertyName, obj.GetType().FullName));
						}
						enumerable.GetEnumerator().Reset();
					}
					Dictionary<object, int> dictionary = new Dictionary<object, int>();
					foreach (object obj2 in enumerable)
					{
						object obj3 = (propertyInfo != null) ? propertyInfo.GetValue(obj2, null) : obj2;
						if (obj3 != null)
						{
							dictionary[obj3] = (dictionary.ContainsKey(obj3) ? (dictionary[obj3] + 1) : 1);
						}
					}
					foreach (object obj4 in dictionary.Keys)
					{
						if (dictionary[obj4] > 1)
						{
							list.Add(string.Format("Duplicated element {0} found in collection", obj4));
						}
					}
				}
			}
			return list;
		}
	}
}
