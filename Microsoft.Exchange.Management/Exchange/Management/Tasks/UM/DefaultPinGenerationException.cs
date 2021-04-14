using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultPinGenerationException : LocalizedException
	{
		public DefaultPinGenerationException() : base(Strings.ErrorGeneratingDefaultPassword)
		{
		}

		public DefaultPinGenerationException(Exception innerException) : base(Strings.ErrorGeneratingDefaultPassword, innerException)
		{
		}

		protected DefaultPinGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
