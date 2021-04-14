﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotDisableFederationException : FederationException
	{
		public CannotDisableFederationException() : base(Strings.ErrorCannotDisableFederation)
		{
		}

		public CannotDisableFederationException(Exception innerException) : base(Strings.ErrorCannotDisableFederation, innerException)
		{
		}

		protected CannotDisableFederationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}