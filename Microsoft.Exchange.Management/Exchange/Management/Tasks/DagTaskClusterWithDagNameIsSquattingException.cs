using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskClusterWithDagNameIsSquattingException : LocalizedException
	{
		public DagTaskClusterWithDagNameIsSquattingException(string dagName) : base(Strings.DagTaskClusterWithDagNameIsSquattingException(dagName))
		{
			this.dagName = dagName;
		}

		public DagTaskClusterWithDagNameIsSquattingException(string dagName, Exception innerException) : base(Strings.DagTaskClusterWithDagNameIsSquattingException(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagTaskClusterWithDagNameIsSquattingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string dagName;
	}
}
