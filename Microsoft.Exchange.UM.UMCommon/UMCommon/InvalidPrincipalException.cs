﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPrincipalException : LocalizedException
	{
		public InvalidPrincipalException() : base(Strings.InvalidPrincipalException)
		{
		}

		public InvalidPrincipalException(Exception innerException) : base(Strings.InvalidPrincipalException, innerException)
		{
		}

		protected InvalidPrincipalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
