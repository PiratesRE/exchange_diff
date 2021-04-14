using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateInitialDomainException : ADObjectAlreadyExistsException
	{
		public DuplicateInitialDomainException() : base(Strings.DuplicateInitialDomain)
		{
		}

		public DuplicateInitialDomainException(Exception innerException) : base(Strings.DuplicateInitialDomain, innerException)
		{
		}

		protected DuplicateInitialDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
