using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EndPointNotRegisteredException : LocalizedException
	{
		public EndPointNotRegisteredException() : base(Strings.EndPointNotRegisteredException)
		{
		}

		public EndPointNotRegisteredException(Exception innerException) : base(Strings.EndPointNotRegisteredException, innerException)
		{
		}

		protected EndPointNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
