using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PswsProxyCmdletException : PswsProxyException
	{
		public PswsProxyCmdletException(string errorMessage) : base(Strings.PswsCmdletError(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public PswsProxyCmdletException(string errorMessage, Exception innerException) : base(Strings.PswsCmdletError(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected PswsProxyCmdletException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string errorMessage;
	}
}
