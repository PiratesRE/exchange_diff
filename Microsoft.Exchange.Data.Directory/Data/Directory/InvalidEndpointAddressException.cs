using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEndpointAddressException : LocalizedException
	{
		public InvalidEndpointAddressException(string exceptionType, string wcfEndpoint) : base(DirectoryStrings.InvalidEndpointAddressErrorMessage(exceptionType, wcfEndpoint))
		{
			this.exceptionType = exceptionType;
			this.wcfEndpoint = wcfEndpoint;
		}

		public InvalidEndpointAddressException(string exceptionType, string wcfEndpoint, Exception innerException) : base(DirectoryStrings.InvalidEndpointAddressErrorMessage(exceptionType, wcfEndpoint), innerException)
		{
			this.exceptionType = exceptionType;
			this.wcfEndpoint = wcfEndpoint;
		}

		protected InvalidEndpointAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exceptionType = (string)info.GetValue("exceptionType", typeof(string));
			this.wcfEndpoint = (string)info.GetValue("wcfEndpoint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exceptionType", this.exceptionType);
			info.AddValue("wcfEndpoint", this.wcfEndpoint);
		}

		public string ExceptionType
		{
			get
			{
				return this.exceptionType;
			}
		}

		public string WcfEndpoint
		{
			get
			{
				return this.wcfEndpoint;
			}
		}

		private readonly string exceptionType;

		private readonly string wcfEndpoint;
	}
}
