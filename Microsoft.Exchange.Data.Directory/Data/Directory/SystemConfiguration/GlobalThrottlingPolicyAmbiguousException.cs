using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GlobalThrottlingPolicyAmbiguousException : ADExternalException
	{
		public GlobalThrottlingPolicyAmbiguousException() : base(DirectoryStrings.GlobalThrottlingPolicyAmbiguousException)
		{
		}

		public GlobalThrottlingPolicyAmbiguousException(Exception innerException) : base(DirectoryStrings.GlobalThrottlingPolicyAmbiguousException, innerException)
		{
		}

		protected GlobalThrottlingPolicyAmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
