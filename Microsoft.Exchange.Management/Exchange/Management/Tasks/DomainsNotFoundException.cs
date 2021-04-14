﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainsNotFoundException : LocalizedException
	{
		public DomainsNotFoundException() : base(Strings.DomainsNotFoundException)
		{
		}

		public DomainsNotFoundException(Exception innerException) : base(Strings.DomainsNotFoundException, innerException)
		{
		}

		protected DomainsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
