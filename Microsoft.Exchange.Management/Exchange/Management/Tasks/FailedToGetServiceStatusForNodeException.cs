using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetServiceStatusForNodeException : LocalizedException
	{
		public FailedToGetServiceStatusForNodeException(string server, string error) : base(Strings.FailedToGetServiceStatusForNodeException(server, error))
		{
			this.server = server;
			this.error = error;
		}

		public FailedToGetServiceStatusForNodeException(string server, string error, Exception innerException) : base(Strings.FailedToGetServiceStatusForNodeException(server, error), innerException)
		{
			this.server = server;
			this.error = error;
		}

		protected FailedToGetServiceStatusForNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("error", this.error);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string server;

		private readonly string error;
	}
}
