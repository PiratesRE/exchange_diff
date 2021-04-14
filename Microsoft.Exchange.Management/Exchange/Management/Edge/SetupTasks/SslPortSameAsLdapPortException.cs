using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SslPortSameAsLdapPortException : LocalizedException
	{
		public SslPortSameAsLdapPortException() : base(Strings.SslPortSameAsLdapPort)
		{
		}

		public SslPortSameAsLdapPortException(Exception innerException) : base(Strings.SslPortSameAsLdapPort, innerException)
		{
		}

		protected SslPortSameAsLdapPortException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
