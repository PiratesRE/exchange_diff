﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncompatibleDatabaseSchemaVersionsInDAGException : LocalizedException
	{
		public IncompatibleDatabaseSchemaVersionsInDAGException() : base(Strings.IncompatibleDatabaseSchemaVersionsInDAG)
		{
		}

		public IncompatibleDatabaseSchemaVersionsInDAGException(Exception innerException) : base(Strings.IncompatibleDatabaseSchemaVersionsInDAG, innerException)
		{
		}

		protected IncompatibleDatabaseSchemaVersionsInDAGException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
