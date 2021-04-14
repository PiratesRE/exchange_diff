using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OrganizationRelationshipMissingTargetApplicationUriException : ADOperationException
	{
		public OrganizationRelationshipMissingTargetApplicationUriException() : base(DirectoryStrings.OrganizationRelationshipMissingTargetApplicationUri)
		{
		}

		public OrganizationRelationshipMissingTargetApplicationUriException(Exception innerException) : base(DirectoryStrings.OrganizationRelationshipMissingTargetApplicationUri, innerException)
		{
		}

		protected OrganizationRelationshipMissingTargetApplicationUriException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
