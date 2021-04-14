﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNotJoinedException : ClusterException
	{
		public ClusterNotJoinedException(string nodeName) : base(Strings.ClusterNotJoinedException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ClusterNotJoinedException(string nodeName, Exception innerException) : base(Strings.ClusterNotJoinedException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ClusterNotJoinedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		private readonly string nodeName;
	}
}
