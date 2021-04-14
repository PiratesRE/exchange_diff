using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UpdateDbcDeleteFilesInvalidParametersException : LocalizedException
	{
		public UpdateDbcDeleteFilesInvalidParametersException() : base(Strings.UpdateDbcDeleteFilesInvalidParametersException)
		{
		}

		public UpdateDbcDeleteFilesInvalidParametersException(Exception innerException) : base(Strings.UpdateDbcDeleteFilesInvalidParametersException, innerException)
		{
		}

		protected UpdateDbcDeleteFilesInvalidParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
