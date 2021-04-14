﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SUC_ExchangePrincipalError : LocalizedException
	{
		public SUC_ExchangePrincipalError() : base(Strings.ExchangePrincipalError)
		{
		}

		public SUC_ExchangePrincipalError(Exception innerException) : base(Strings.ExchangePrincipalError, innerException)
		{
		}

		protected SUC_ExchangePrincipalError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
