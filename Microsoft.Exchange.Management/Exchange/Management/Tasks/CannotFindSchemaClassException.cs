using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindSchemaClassException : LocalizedException
	{
		public CannotFindSchemaClassException(string objclass, string schemaDN, string server) : base(Strings.CannotFindSchemaClassException(objclass, schemaDN, server))
		{
			this.objclass = objclass;
			this.schemaDN = schemaDN;
			this.server = server;
		}

		public CannotFindSchemaClassException(string objclass, string schemaDN, string server, Exception innerException) : base(Strings.CannotFindSchemaClassException(objclass, schemaDN, server), innerException)
		{
			this.objclass = objclass;
			this.schemaDN = schemaDN;
			this.server = server;
		}

		protected CannotFindSchemaClassException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objclass = (string)info.GetValue("objclass", typeof(string));
			this.schemaDN = (string)info.GetValue("schemaDN", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objclass", this.objclass);
			info.AddValue("schemaDN", this.schemaDN);
			info.AddValue("server", this.server);
		}

		public string Objclass
		{
			get
			{
				return this.objclass;
			}
		}

		public string SchemaDN
		{
			get
			{
				return this.schemaDN;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string objclass;

		private readonly string schemaDN;

		private readonly string server;
	}
}
