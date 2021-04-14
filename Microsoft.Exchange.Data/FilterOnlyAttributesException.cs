using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FilterOnlyAttributesException : LocalizedException
	{
		public FilterOnlyAttributesException(string attributeName) : base(DataStrings.FilterOnlyAttributes(attributeName))
		{
			this.attributeName = attributeName;
		}

		public FilterOnlyAttributesException(string attributeName, Exception innerException) : base(DataStrings.FilterOnlyAttributes(attributeName), innerException)
		{
			this.attributeName = attributeName;
		}

		protected FilterOnlyAttributesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.attributeName = (string)info.GetValue("attributeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("attributeName", this.attributeName);
		}

		public string AttributeName
		{
			get
			{
				return this.attributeName;
			}
		}

		private readonly string attributeName;
	}
}
