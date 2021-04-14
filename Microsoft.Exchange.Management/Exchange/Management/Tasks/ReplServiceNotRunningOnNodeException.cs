using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplServiceNotRunningOnNodeException : LocalizedException
	{
		public ReplServiceNotRunningOnNodeException(string nodeName) : base(Strings.ReplServiceNotRunningOnNodeException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ReplServiceNotRunningOnNodeException(string nodeName, Exception innerException) : base(Strings.ReplServiceNotRunningOnNodeException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ReplServiceNotRunningOnNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
