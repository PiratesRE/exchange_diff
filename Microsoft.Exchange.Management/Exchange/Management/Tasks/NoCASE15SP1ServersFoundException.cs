using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoCASE15SP1ServersFoundException : LocalizedException
	{
		public NoCASE15SP1ServersFoundException() : base(Strings.NoCASE15SP1ServersFoundException)
		{
		}

		public NoCASE15SP1ServersFoundException(Exception innerException) : base(Strings.NoCASE15SP1ServersFoundException, innerException)
		{
		}

		protected NoCASE15SP1ServersFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
