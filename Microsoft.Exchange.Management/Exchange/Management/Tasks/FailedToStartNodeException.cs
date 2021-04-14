using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToStartNodeException : LocalizedException
	{
		public FailedToStartNodeException(string nodeNames, string dagName) : base(Strings.FailedToStartNodeException(nodeNames, dagName))
		{
			this.nodeNames = nodeNames;
			this.dagName = dagName;
		}

		public FailedToStartNodeException(string nodeNames, string dagName, Exception innerException) : base(Strings.FailedToStartNodeException(nodeNames, dagName), innerException)
		{
			this.nodeNames = nodeNames;
			this.dagName = dagName;
		}

		protected FailedToStartNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeNames = (string)info.GetValue("nodeNames", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeNames", this.nodeNames);
			info.AddValue("dagName", this.dagName);
		}

		public string NodeNames
		{
			get
			{
				return this.nodeNames;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string nodeNames;

		private readonly string dagName;
	}
}
