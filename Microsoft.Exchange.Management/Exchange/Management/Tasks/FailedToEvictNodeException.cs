using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToEvictNodeException : LocalizedException
	{
		public FailedToEvictNodeException(string nodeName, string dagName, string error) : base(Strings.FailedToEvictNodeException(nodeName, dagName, error))
		{
			this.nodeName = nodeName;
			this.dagName = dagName;
			this.error = error;
		}

		public FailedToEvictNodeException(string nodeName, string dagName, string error, Exception innerException) : base(Strings.FailedToEvictNodeException(nodeName, dagName, error), innerException)
		{
			this.nodeName = nodeName;
			this.dagName = dagName;
			this.error = error;
		}

		protected FailedToEvictNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("dagName", this.dagName);
			info.AddValue("error", this.error);
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

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string nodeName;

		private readonly string dagName;

		private readonly string error;
	}
}
