﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncomingUserNameEmpty : LocalizedException
	{
		public IncomingUserNameEmpty() : base(Strings.IncomingUserNameEmpty)
		{
		}

		public IncomingUserNameEmpty(Exception innerException) : base(Strings.IncomingUserNameEmpty, innerException)
		{
		}

		protected IncomingUserNameEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
