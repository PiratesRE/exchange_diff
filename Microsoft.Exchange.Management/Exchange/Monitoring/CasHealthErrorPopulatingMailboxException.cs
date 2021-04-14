using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthErrorPopulatingMailboxException : LocalizedException
	{
		public CasHealthErrorPopulatingMailboxException(string exceptionMessage) : base(Strings.CasHealthErrorPopulatingMailbox(exceptionMessage))
		{
			this.exceptionMessage = exceptionMessage;
		}

		public CasHealthErrorPopulatingMailboxException(string exceptionMessage, Exception innerException) : base(Strings.CasHealthErrorPopulatingMailbox(exceptionMessage), innerException)
		{
			this.exceptionMessage = exceptionMessage;
		}

		protected CasHealthErrorPopulatingMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exceptionMessage = (string)info.GetValue("exceptionMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string exceptionMessage;
	}
}
