using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasMissingOrBadUrlOnRedirectException : ConnectionsTransientException
	{
		public EasMissingOrBadUrlOnRedirectException() : base(CXStrings.EasMissingOrBadUrlOnRedirectMsg)
		{
		}

		public EasMissingOrBadUrlOnRedirectException(Exception innerException) : base(CXStrings.EasMissingOrBadUrlOnRedirectMsg, innerException)
		{
		}

		protected EasMissingOrBadUrlOnRedirectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
