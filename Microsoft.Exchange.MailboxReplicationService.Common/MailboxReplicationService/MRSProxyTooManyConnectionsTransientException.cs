using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MRSProxyTooManyConnectionsTransientException : MRSProxyConnectionLimitReachedTransientException
	{
		public MRSProxyTooManyConnectionsTransientException(int activeConnections, int connectionLimit) : base(MrsStrings.MRSProxyConnectionLimitReachedError(activeConnections, connectionLimit))
		{
			this.activeConnections = activeConnections;
			this.connectionLimit = connectionLimit;
		}

		public MRSProxyTooManyConnectionsTransientException(int activeConnections, int connectionLimit, Exception innerException) : base(MrsStrings.MRSProxyConnectionLimitReachedError(activeConnections, connectionLimit), innerException)
		{
			this.activeConnections = activeConnections;
			this.connectionLimit = connectionLimit;
		}

		protected MRSProxyTooManyConnectionsTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.activeConnections = (int)info.GetValue("activeConnections", typeof(int));
			this.connectionLimit = (int)info.GetValue("connectionLimit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("activeConnections", this.activeConnections);
			info.AddValue("connectionLimit", this.connectionLimit);
		}

		public int ActiveConnections
		{
			get
			{
				return this.activeConnections;
			}
		}

		public int ConnectionLimit
		{
			get
			{
				return this.connectionLimit;
			}
		}

		private readonly int activeConnections;

		private readonly int connectionLimit;
	}
}
