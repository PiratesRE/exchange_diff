using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WebObjectNotFoundException : DataSourceOperationException
	{
		public WebObjectNotFoundException(LocalizedString message) : base(message)
		{
		}

		public WebObjectNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WebObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
