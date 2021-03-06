using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3TransientInUseAuthErrorException : LocalizedException
	{
		public Pop3TransientInUseAuthErrorException() : base(Strings.Pop3TransientInUseAuthErrorException)
		{
		}

		public Pop3TransientInUseAuthErrorException(Exception innerException) : base(Strings.Pop3TransientInUseAuthErrorException, innerException)
		{
		}

		protected Pop3TransientInUseAuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
