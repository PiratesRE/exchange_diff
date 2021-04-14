using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FederatedIdentityMisconfiguredException : ADOperationException
	{
		public FederatedIdentityMisconfiguredException() : base(DirectoryStrings.FederatedIdentityMisconfigured)
		{
		}

		public FederatedIdentityMisconfiguredException(Exception innerException) : base(DirectoryStrings.FederatedIdentityMisconfigured, innerException)
		{
		}

		protected FederatedIdentityMisconfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
