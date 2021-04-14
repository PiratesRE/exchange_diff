﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskInvalidOperationException : LocalizedException
	{
		public TaskInvalidOperationException(LocalizedString message) : base(message)
		{
		}

		public TaskInvalidOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TaskInvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
