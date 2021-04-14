using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSIPHeaderException : LocalizedException
	{
		public InvalidSIPHeaderException(string request, string header, string value) : base(Strings.InvalidSIPHeader(request, header, value))
		{
			this.request = request;
			this.header = header;
			this.value = value;
		}

		public InvalidSIPHeaderException(string request, string header, string value, Exception innerException) : base(Strings.InvalidSIPHeader(request, header, value), innerException)
		{
			this.request = request;
			this.header = header;
			this.value = value;
		}

		protected InvalidSIPHeaderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.request = (string)info.GetValue("request", typeof(string));
			this.header = (string)info.GetValue("header", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("request", this.request);
			info.AddValue("header", this.header);
			info.AddValue("value", this.value);
		}

		public string Request
		{
			get
			{
				return this.request;
			}
		}

		public string Header
		{
			get
			{
				return this.header;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string request;

		private readonly string header;

		private readonly string value;
	}
}
