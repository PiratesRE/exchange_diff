using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RcrExceedDbLimitException : LocalizedException
	{
		public RcrExceedDbLimitException(string server, int limit) : base(Strings.RcrExceedDbLimitException(server, limit))
		{
			this.server = server;
			this.limit = limit;
		}

		public RcrExceedDbLimitException(string server, int limit, Exception innerException) : base(Strings.RcrExceedDbLimitException(server, limit), innerException)
		{
			this.server = server;
			this.limit = limit;
		}

		protected RcrExceedDbLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.limit = (int)info.GetValue("limit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("limit", this.limit);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		private readonly string server;

		private readonly int limit;
	}
}
