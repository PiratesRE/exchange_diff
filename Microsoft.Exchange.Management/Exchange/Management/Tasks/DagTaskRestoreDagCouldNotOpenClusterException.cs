﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskRestoreDagCouldNotOpenClusterException : LocalizedException
	{
		public DagTaskRestoreDagCouldNotOpenClusterException() : base(Strings.DagTaskRestoreDagCouldNotOpenCluster)
		{
		}

		public DagTaskRestoreDagCouldNotOpenClusterException(Exception innerException) : base(Strings.DagTaskRestoreDagCouldNotOpenCluster, innerException)
		{
		}

		protected DagTaskRestoreDagCouldNotOpenClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
