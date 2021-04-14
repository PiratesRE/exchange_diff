using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoServersAndOutofServerScopeException : LocalizedException
	{
		public NoServersAndOutofServerScopeException(string databaseid, string serverid) : base(Strings.ErrorNoServersAndOutofServerScope(databaseid, serverid))
		{
			this.databaseid = databaseid;
			this.serverid = serverid;
		}

		public NoServersAndOutofServerScopeException(string databaseid, string serverid, Exception innerException) : base(Strings.ErrorNoServersAndOutofServerScope(databaseid, serverid), innerException)
		{
			this.databaseid = databaseid;
			this.serverid = serverid;
		}

		protected NoServersAndOutofServerScopeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseid = (string)info.GetValue("databaseid", typeof(string));
			this.serverid = (string)info.GetValue("serverid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseid", this.databaseid);
			info.AddValue("serverid", this.serverid);
		}

		public string Databaseid
		{
			get
			{
				return this.databaseid;
			}
		}

		public string Serverid
		{
			get
			{
				return this.serverid;
			}
		}

		private readonly string databaseid;

		private readonly string serverid;
	}
}
