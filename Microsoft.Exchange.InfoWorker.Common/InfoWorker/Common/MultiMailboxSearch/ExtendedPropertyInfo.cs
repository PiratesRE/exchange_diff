using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class ExtendedPropertyInfo
	{
		public Guid PropertySetId { get; set; }

		public int? PropertyTagId { get; set; }

		public string PropertyName { get; set; }

		public int? PropertyId { get; set; }

		public string PropertyType { get; set; }

		public PropertyDefinition XsoPropertyDefinition { get; set; }

		public MapiPropertyType GetMapiPropertyType()
		{
			MapiPropertyType result = 19;
			if (!string.IsNullOrEmpty(this.PropertyType))
			{
				Enum.TryParse<MapiPropertyType>(this.PropertyType, true, out result);
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Extended property info");
			if (this.PropertySetId != Guid.Empty)
			{
				stringBuilder.AppendFormat(", Property set id: {0}", this.PropertySetId.ToString("D"));
			}
			if (this.PropertyTagId != null && this.PropertyTagId != null)
			{
				stringBuilder.AppendFormat(", Property tag id: {0}", this.PropertyTagId.Value);
			}
			if (!string.IsNullOrEmpty(this.PropertyName))
			{
				stringBuilder.AppendFormat(", Property name: {0}", this.PropertyName);
			}
			if (this.PropertyId != null && this.PropertyId != null)
			{
				stringBuilder.AppendFormat(", Property id: {0}", this.PropertyId.Value);
			}
			if (!string.IsNullOrEmpty(this.PropertyType))
			{
				stringBuilder.AppendFormat(", Property type: {0}", this.PropertyType);
			}
			return stringBuilder.ToString();
		}
	}
}
