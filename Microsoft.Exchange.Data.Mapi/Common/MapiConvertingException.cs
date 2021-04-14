﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiConvertingException : MapiInvalidOperationException
	{
		public MapiConvertingException(LocalizedString message) : base(message)
		{
		}

		public MapiConvertingException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MapiConvertingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
