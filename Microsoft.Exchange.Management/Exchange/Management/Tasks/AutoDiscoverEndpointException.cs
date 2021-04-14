﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoDiscoverEndpointException : TaskException
	{
		public AutoDiscoverEndpointException(LocalizedString message) : base(message)
		{
		}

		public AutoDiscoverEndpointException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AutoDiscoverEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
