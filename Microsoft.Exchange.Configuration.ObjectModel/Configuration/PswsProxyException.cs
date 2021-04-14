﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PswsProxyException : LocalizedException
	{
		public PswsProxyException(LocalizedString message) : base(message)
		{
		}

		public PswsProxyException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PswsProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
