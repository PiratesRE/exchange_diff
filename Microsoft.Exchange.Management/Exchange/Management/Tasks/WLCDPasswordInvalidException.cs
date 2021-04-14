using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDPasswordInvalidException : WLCDMemberException
	{
		public WLCDPasswordInvalidException(LocalizedString message) : base(message)
		{
		}

		public WLCDPasswordInvalidException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDPasswordInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
