using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SmtpSubmissionException : TransientException
	{
		public SmtpSubmissionException() : base(Strings.SmtpSubmissionFailed)
		{
		}

		public SmtpSubmissionException(Exception innerException) : base(Strings.SmtpSubmissionFailed, innerException)
		{
		}

		protected SmtpSubmissionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
