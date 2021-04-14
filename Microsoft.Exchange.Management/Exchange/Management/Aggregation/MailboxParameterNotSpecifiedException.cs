using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxParameterNotSpecifiedException : LocalizedException
	{
		public MailboxParameterNotSpecifiedException() : base(Strings.MailboxParameterNotSpecified)
		{
		}

		public MailboxParameterNotSpecifiedException(Exception innerException) : base(Strings.MailboxParameterNotSpecified, innerException)
		{
		}

		protected MailboxParameterNotSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
