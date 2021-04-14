using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StopDagFailedException : LocalizedException
	{
		public StopDagFailedException(string errorServers, string dagName) : base(Strings.StopDagFailedException(errorServers, dagName))
		{
			this.errorServers = errorServers;
			this.dagName = dagName;
		}

		public StopDagFailedException(string errorServers, string dagName, Exception innerException) : base(Strings.StopDagFailedException(errorServers, dagName), innerException)
		{
			this.errorServers = errorServers;
			this.dagName = dagName;
		}

		protected StopDagFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorServers = (string)info.GetValue("errorServers", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorServers", this.errorServers);
			info.AddValue("dagName", this.dagName);
		}

		public string ErrorServers
		{
			get
			{
				return this.errorServers;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string errorServers;

		private readonly string dagName;
	}
}
