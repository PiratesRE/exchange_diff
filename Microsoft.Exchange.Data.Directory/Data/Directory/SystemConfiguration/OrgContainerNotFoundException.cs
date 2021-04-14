using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrgContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public OrgContainerNotFoundException() : base(DirectoryStrings.OrgContainerNotFoundException)
		{
		}

		public OrgContainerNotFoundException(Exception innerException) : base(DirectoryStrings.OrgContainerNotFoundException, innerException)
		{
		}

		protected OrgContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
