using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToStartClusSvcException : LocalizedException
	{
		public FailedToStartClusSvcException(string serverName, string state) : base(Strings.FailedToStartClusSvc(serverName, state))
		{
			this.serverName = serverName;
			this.state = state;
		}

		public FailedToStartClusSvcException(string serverName, string state, Exception innerException) : base(Strings.FailedToStartClusSvc(serverName, state), innerException)
		{
			this.serverName = serverName;
			this.state = state;
		}

		protected FailedToStartClusSvcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.state = (string)info.GetValue("state", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("state", this.state);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
		}

		private readonly string serverName;

		private readonly string state;
	}
}
