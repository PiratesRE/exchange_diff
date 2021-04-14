using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GlobalThrottlingPolicyNotFoundException : ADExternalException
	{
		public GlobalThrottlingPolicyNotFoundException() : base(DirectoryStrings.GlobalThrottlingPolicyNotFoundException)
		{
		}

		public GlobalThrottlingPolicyNotFoundException(Exception innerException) : base(DirectoryStrings.GlobalThrottlingPolicyNotFoundException, innerException)
		{
		}

		protected GlobalThrottlingPolicyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
