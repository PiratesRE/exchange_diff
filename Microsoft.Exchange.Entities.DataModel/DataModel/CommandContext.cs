using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public sealed class CommandContext
	{
		public string[] Expand { get; set; }

		public string IfMatchETag { get; set; }

		public int PageSizeOnReread
		{
			get
			{
				return this.pageSizeOnReread;
			}
			set
			{
				this.pageSizeOnReread = value;
			}
		}

		public IEnumerable<PropertyDefinition> RequestedProperties { get; set; }

		private Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> result;
				if ((result = this.customParameters) == null)
				{
					result = (this.customParameters = new Dictionary<string, object>());
				}
				return result;
			}
		}

		public void SetCustomParameter(string parameter, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.CustomParameters.Add(parameter, value);
		}

		public bool TryGetCustomParameter<TValue>(string parameter, out TValue value)
		{
			object obj;
			if (!this.CustomParameters.TryGetValue(parameter, out obj))
			{
				value = default(TValue);
				return false;
			}
			Type type = obj.GetType();
			Type typeFromHandle = typeof(TValue);
			if (typeFromHandle.IsAssignableFrom(type))
			{
				value = (TValue)((object)obj);
				return true;
			}
			throw new InvalidCastException(string.Format("Cannot cast the stored value (Type: {0}) for the given parameter ('{1}') to type {2}", type.FullName, parameter, typeFromHandle.FullName));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IfMatchETag != null)
			{
				stringBuilder.AppendFormat("[If-Match:{0}]", this.IfMatchETag);
			}
			if (this.Expand != null)
			{
				stringBuilder.AppendFormat("[Expand:{0}]", string.Join(",", this.Expand));
			}
			IEnumerable<string> values = from param in this.CustomParameters
			select string.Format("[X-{0}:{1}]", param.Key, param.Value);
			stringBuilder.Append(string.Join(",", values));
			return stringBuilder.ToString();
		}

		internal const int DefaultPageSizeOnReread = 20;

		private Dictionary<string, object> customParameters;

		private int pageSizeOnReread = 20;
	}
}
