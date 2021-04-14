using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskNodeNotFoundTryConfigOnlyException : LocalizedException
	{
		public DagTaskNodeNotFoundTryConfigOnlyException(string machineName, string clusterName) : base(Strings.DagTaskNodeNotFoundTryConfigOnlyException(machineName, clusterName))
		{
			this.machineName = machineName;
			this.clusterName = clusterName;
		}

		public DagTaskNodeNotFoundTryConfigOnlyException(string machineName, string clusterName, Exception innerException) : base(Strings.DagTaskNodeNotFoundTryConfigOnlyException(machineName, clusterName), innerException)
		{
			this.machineName = machineName;
			this.clusterName = clusterName;
		}

		protected DagTaskNodeNotFoundTryConfigOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineName = (string)info.GetValue("machineName", typeof(string));
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineName", this.machineName);
			info.AddValue("clusterName", this.clusterName);
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		private readonly string machineName;

		private readonly string clusterName;
	}
}
