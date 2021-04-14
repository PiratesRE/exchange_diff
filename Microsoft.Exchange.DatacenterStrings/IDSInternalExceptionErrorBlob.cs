using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DatacenterStrings
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IDSInternalExceptionErrorBlob : IDSInternalException
	{
		public IDSInternalExceptionErrorBlob(LocalizedString message) : base(message)
		{
		}

		public IDSInternalExceptionErrorBlob(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected IDSInternalExceptionErrorBlob(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
