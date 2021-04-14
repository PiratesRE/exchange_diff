using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	internal class ThrowTerminatingErrorException : ExCmdletInvocationException
	{
		internal ThrowTerminatingErrorException(ErrorRecord errorRecord) : base(errorRecord)
		{
		}

		protected ThrowTerminatingErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
