﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.DatacenterStrings
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IDSWebException : RecipientTaskException
	{
		public IDSWebException(LocalizedString message) : base(message)
		{
		}

		public IDSWebException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected IDSWebException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
