using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskAddClusterNodeUnexpectedlyFailedException : LocalizedException
	{
		public DagTaskAddClusterNodeUnexpectedlyFailedException(string nodeName, string dagName) : base(Strings.DagTaskAddClusterNodeUnexpectedlyFailedException(nodeName, dagName))
		{
			this.nodeName = nodeName;
			this.dagName = dagName;
		}

		public DagTaskAddClusterNodeUnexpectedlyFailedException(string nodeName, string dagName, Exception innerException) : base(Strings.DagTaskAddClusterNodeUnexpectedlyFailedException(nodeName, dagName), innerException)
		{
			this.nodeName = nodeName;
			this.dagName = dagName;
		}

		protected DagTaskAddClusterNodeUnexpectedlyFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("dagName", this.dagName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string nodeName;

		private readonly string dagName;
	}
}
