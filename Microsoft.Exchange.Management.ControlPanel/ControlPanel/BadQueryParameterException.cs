using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BadQueryParameterException : LocalizedException
	{
		public BadQueryParameterException(string queryParam) : base(Strings.BadQueryParameterError(queryParam))
		{
			this.queryParam = queryParam;
		}

		public BadQueryParameterException(string queryParam, Exception innerException) : base(Strings.BadQueryParameterError(queryParam), innerException)
		{
			this.queryParam = queryParam;
		}

		protected BadQueryParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.queryParam = (string)info.GetValue("queryParam", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("queryParam", this.queryParam);
		}

		public string QueryParam
		{
			get
			{
				return this.queryParam;
			}
		}

		private readonly string queryParam;
	}
}
