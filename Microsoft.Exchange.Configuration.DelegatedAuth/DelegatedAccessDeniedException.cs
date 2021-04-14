﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DelegatedAccessDeniedException : LocalizedException
	{
		public DelegatedAccessDeniedException() : base(Strings.DelegatedAccessDeniedException)
		{
		}

		public DelegatedAccessDeniedException(Exception innerException) : base(Strings.DelegatedAccessDeniedException, innerException)
		{
		}

		protected DelegatedAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
