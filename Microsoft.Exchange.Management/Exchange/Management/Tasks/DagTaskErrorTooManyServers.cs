using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskErrorTooManyServers : LocalizedException
	{
		public DagTaskErrorTooManyServers(string dagName, int max) : base(Strings.DagTaskErrorTooManyServers(dagName, max))
		{
			this.dagName = dagName;
			this.max = max;
		}

		public DagTaskErrorTooManyServers(string dagName, int max, Exception innerException) : base(Strings.DagTaskErrorTooManyServers(dagName, max), innerException)
		{
			this.dagName = dagName;
			this.max = max;
		}

		protected DagTaskErrorTooManyServers(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.max = (int)info.GetValue("max", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
			info.AddValue("max", this.max);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public int Max
		{
			get
			{
				return this.max;
			}
		}

		private readonly string dagName;

		private readonly int max;
	}
}
