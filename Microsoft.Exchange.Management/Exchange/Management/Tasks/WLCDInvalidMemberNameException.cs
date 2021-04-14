using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDInvalidMemberNameException : WLCDMemberException
	{
		public WLCDInvalidMemberNameException(LocalizedString message) : base(message)
		{
		}

		public WLCDInvalidMemberNameException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDInvalidMemberNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
