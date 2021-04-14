using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplicationCheckHighPriorityFailedException : ReplicationCheckException
	{
		public ReplicationCheckHighPriorityFailedException(string checkTitle, string errorMessage) : base(Strings.ReplicationCheckHighPriorityFailedException(checkTitle, errorMessage))
		{
			this.checkTitle = checkTitle;
			this.errorMessage = errorMessage;
		}

		public ReplicationCheckHighPriorityFailedException(string checkTitle, string errorMessage, Exception innerException) : base(Strings.ReplicationCheckHighPriorityFailedException(checkTitle, errorMessage), innerException)
		{
			this.checkTitle = checkTitle;
			this.errorMessage = errorMessage;
		}

		protected ReplicationCheckHighPriorityFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.checkTitle = (string)info.GetValue("checkTitle", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("checkTitle", this.checkTitle);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string CheckTitle
		{
			get
			{
				return this.checkTitle;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string checkTitle;

		private readonly string errorMessage;
	}
}
