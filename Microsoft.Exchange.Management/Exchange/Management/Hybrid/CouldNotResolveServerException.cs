using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotResolveServerException : LocalizedException
	{
		public CouldNotResolveServerException(string server, Exception e) : base(HybridStrings.HybridCouldNotResolveServerException(server, e))
		{
			this.server = server;
			this.e = e;
		}

		public CouldNotResolveServerException(string server, Exception e, Exception innerException) : base(HybridStrings.HybridCouldNotResolveServerException(server, e), innerException)
		{
			this.server = server;
			this.e = e;
		}

		protected CouldNotResolveServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.e = (Exception)info.GetValue("e", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("e", this.e);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public Exception E
		{
			get
			{
				return this.e;
			}
		}

		private readonly string server;

		private readonly Exception e;
	}
}
