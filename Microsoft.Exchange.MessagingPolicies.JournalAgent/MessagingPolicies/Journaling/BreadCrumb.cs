using System;
using System.Collections;
using System.Linq;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class BreadCrumb
	{
		internal string FunctionName { get; set; }

		internal object[] Info { get; set; }

		public override string ToString()
		{
			return string.Format("[{0} {1}] {2}", this.createdTime, this.FunctionName, BreadCrumb.GetStringValue(this.Info));
		}

		public string GetAdditionStateInfoString()
		{
			return BreadCrumb.GetStringValue(this.Info);
		}

		private static string GetStringValue(object obj)
		{
			if (obj == null)
			{
				return "<NULL>";
			}
			IEnumerable enumerable = obj as IEnumerable;
			if (enumerable != null && !(obj is string))
			{
				return "(" + string.Join(",", enumerable.Cast<object>().Select(new Func<object, string>(BreadCrumb.GetStringValue))) + ")";
			}
			return obj.ToString();
		}

		private ExDateTime createdTime = ExDateTime.UtcNow;
	}
}
