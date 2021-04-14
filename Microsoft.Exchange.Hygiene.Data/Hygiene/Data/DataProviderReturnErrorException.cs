﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Hygiene.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataProviderReturnErrorException : PermanentDALException
	{
		public DataProviderReturnErrorException(LocalizedString message) : base(message)
		{
		}

		public DataProviderReturnErrorException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DataProviderReturnErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
