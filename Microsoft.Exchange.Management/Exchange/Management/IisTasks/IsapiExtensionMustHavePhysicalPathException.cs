using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.IisTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsapiExtensionMustHavePhysicalPathException : LocalizedException
	{
		public IsapiExtensionMustHavePhysicalPathException() : base(Strings.IsapiExtensionMustHavePhysicalPathException)
		{
		}

		public IsapiExtensionMustHavePhysicalPathException(Exception innerException) : base(Strings.IsapiExtensionMustHavePhysicalPathException, innerException)
		{
		}

		protected IsapiExtensionMustHavePhysicalPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
