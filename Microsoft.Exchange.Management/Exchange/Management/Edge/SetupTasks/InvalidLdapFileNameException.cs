using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidLdapFileNameException : LocalizedException
	{
		public InvalidLdapFileNameException() : base(Strings.InvalidLdapFileName)
		{
		}

		public InvalidLdapFileNameException(Exception innerException) : base(Strings.InvalidLdapFileName, innerException)
		{
		}

		protected InvalidLdapFileNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
