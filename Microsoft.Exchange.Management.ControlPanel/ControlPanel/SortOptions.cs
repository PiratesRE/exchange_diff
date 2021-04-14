using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SortOptions
	{
		[DataMember]
		public string PropertyName { get; set; }

		[DataMember]
		public SortDirection Direction { get; set; }

		public Func<L[], L[]> GetSortFunction<L>()
		{
			if (string.IsNullOrEmpty(this.PropertyName))
			{
				return null;
			}
			PropertyInfo property = typeof(L).GetProperty(this.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			if (property == null)
			{
				throw new FaultException(Strings.InvalidSortProperty);
			}
			MethodInfo method = typeof(SortOptions).GetMethod("GetSortFunctionByProperty", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo methodInfo = method.MakeGenericMethod(new Type[]
			{
				typeof(L),
				property.PropertyType
			});
			return (Func<L[], L[]>)methodInfo.Invoke(this, new object[]
			{
				property
			});
		}

		private Func<L[], L[]> GetSortFunctionByProperty<L, P>(PropertyInfo sortProperty)
		{
			Func<L, P> keySelector = (L item) => (P)((object)sortProperty.GetValue(item, null));
			if (this.Direction == SortDirection.Ascending)
			{
				return (L[] array) => array.OrderBy(keySelector).ToArray<L>();
			}
			return (L[] array) => array.OrderByDescending(keySelector).ToArray<L>();
		}

		public Func<JsonDictionary<object>[], JsonDictionary<object>[]> GetDDISortFunction()
		{
			Func<JsonDictionary<object>, object> keySelector = (JsonDictionary<object> item) => item[this.PropertyName];
			if (this.Direction == SortDirection.Ascending)
			{
				return (JsonDictionary<object>[] array) => array.OrderBy(keySelector).ToArray<JsonDictionary<object>>();
			}
			return (JsonDictionary<object>[] array) => array.OrderByDescending(keySelector).ToArray<JsonDictionary<object>>();
		}
	}
}
