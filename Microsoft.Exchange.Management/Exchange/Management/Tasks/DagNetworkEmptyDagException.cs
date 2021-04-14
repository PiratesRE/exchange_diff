using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagNetworkEmptyDagException : LocalizedException
	{
		public DagNetworkEmptyDagException(string dagName) : base(Strings.DagNetworkEmptyDagException(dagName))
		{
			this.dagName = dagName;
		}

		public DagNetworkEmptyDagException(string dagName, Exception innerException) : base(Strings.DagNetworkEmptyDagException(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagNetworkEmptyDagException(SerializationInfo info, StreamingContext context) : base(info, context)
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
