using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrgContainerAmbiguousException : ADOperationException
	{
		public OrgContainerAmbiguousException() : base(DirectoryStrings.OrgContainerAmbiguousException)
		{
		}

		public OrgContainerAmbiguousException(Exception innerException) : base(DirectoryStrings.OrgContainerAmbiguousException, innerException)
		{
		}

		protected OrgContainerAmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
