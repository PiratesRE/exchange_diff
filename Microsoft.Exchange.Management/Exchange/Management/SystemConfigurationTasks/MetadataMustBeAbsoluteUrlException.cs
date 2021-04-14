using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MetadataMustBeAbsoluteUrlException : FederationException
	{
		public MetadataMustBeAbsoluteUrlException() : base(Strings.ErrorMetadataMustBeAbsoluteUrl)
		{
		}

		public MetadataMustBeAbsoluteUrlException(Exception innerException) : base(Strings.ErrorMetadataMustBeAbsoluteUrl, innerException)
		{
		}

		protected MetadataMustBeAbsoluteUrlException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
