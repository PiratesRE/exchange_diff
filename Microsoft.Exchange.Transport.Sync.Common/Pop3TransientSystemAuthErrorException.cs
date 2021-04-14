using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3TransientSystemAuthErrorException : LocalizedException
	{
		public Pop3TransientSystemAuthErrorException() : base(Strings.Pop3TransientSystemAuthErrorException)
		{
		}

		public Pop3TransientSystemAuthErrorException(Exception innerException) : base(Strings.Pop3TransientSystemAuthErrorException, innerException)
		{
		}

		protected Pop3TransientSystemAuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
