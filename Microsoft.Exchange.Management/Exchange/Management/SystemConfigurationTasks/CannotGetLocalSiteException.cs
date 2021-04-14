using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotGetLocalSiteException : FederationException
	{
		public CannotGetLocalSiteException() : base(Strings.ErrorCannotGetLocalSite)
		{
		}

		public CannotGetLocalSiteException(Exception innerException) : base(Strings.ErrorCannotGetLocalSite, innerException)
		{
		}

		protected CannotGetLocalSiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
