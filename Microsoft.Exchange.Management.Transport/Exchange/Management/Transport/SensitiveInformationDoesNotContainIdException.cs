using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SensitiveInformationDoesNotContainIdException : InvalidContentContainsSensitiveInformationException
	{
		public SensitiveInformationDoesNotContainIdException() : base(Strings.SensitiveInformationDoesNotContainId)
		{
		}

		public SensitiveInformationDoesNotContainIdException(Exception innerException) : base(Strings.SensitiveInformationDoesNotContainId, innerException)
		{
		}

		protected SensitiveInformationDoesNotContainIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
