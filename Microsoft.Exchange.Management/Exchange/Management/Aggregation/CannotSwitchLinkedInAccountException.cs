using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSwitchLinkedInAccountException : LocalizedException
	{
		public CannotSwitchLinkedInAccountException() : base(Strings.CannotSwitchLinkedInAccount)
		{
		}

		public CannotSwitchLinkedInAccountException(Exception innerException) : base(Strings.CannotSwitchLinkedInAccount, innerException)
		{
		}

		protected CannotSwitchLinkedInAccountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
