using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleRecipientsFoundException : LocalizedException
	{
		public MultipleRecipientsFoundException(string queryFilter) : base(Strings.MultipleRecipientsFound(queryFilter))
		{
			this.queryFilter = queryFilter;
		}

		public MultipleRecipientsFoundException(string queryFilter, Exception innerException) : base(Strings.MultipleRecipientsFound(queryFilter), innerException)
		{
			this.queryFilter = queryFilter;
		}

		protected MultipleRecipientsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.queryFilter = (string)info.GetValue("queryFilter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("queryFilter", this.queryFilter);
		}

		public string QueryFilter
		{
			get
			{
				return this.queryFilter;
			}
		}

		private readonly string queryFilter;
	}
}
