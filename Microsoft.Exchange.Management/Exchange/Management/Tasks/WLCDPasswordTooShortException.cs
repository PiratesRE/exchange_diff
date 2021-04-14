using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDPasswordTooShortException : WLCDMemberException
	{
		public WLCDPasswordTooShortException(LocalizedString message) : base(message)
		{
		}

		public WLCDPasswordTooShortException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDPasswordTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
