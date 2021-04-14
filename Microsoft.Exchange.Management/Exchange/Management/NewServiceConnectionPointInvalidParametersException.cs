using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewServiceConnectionPointInvalidParametersException : LocalizedException
	{
		public NewServiceConnectionPointInvalidParametersException() : base(Strings.NewServiceConnectionPointInvalidParameters)
		{
		}

		public NewServiceConnectionPointInvalidParametersException(Exception innerException) : base(Strings.NewServiceConnectionPointInvalidParameters, innerException)
		{
		}

		protected NewServiceConnectionPointInvalidParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
