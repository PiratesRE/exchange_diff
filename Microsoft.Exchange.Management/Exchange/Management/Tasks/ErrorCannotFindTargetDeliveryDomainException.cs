﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorCannotFindTargetDeliveryDomainException : LocalizedException
	{
		public ErrorCannotFindTargetDeliveryDomainException() : base(Strings.ErrorCannotFindTargetDeliveryDomain)
		{
		}

		public ErrorCannotFindTargetDeliveryDomainException(Exception innerException) : base(Strings.ErrorCannotFindTargetDeliveryDomain, innerException)
		{
		}

		protected ErrorCannotFindTargetDeliveryDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
