using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindSchemaAttributeException : LocalizedException
	{
		public CannotFindSchemaAttributeException(string attr, string schemaDN, string server) : base(Strings.CannotFindSchemaAttributeException(attr, schemaDN, server))
		{
			this.attr = attr;
			this.schemaDN = schemaDN;
			this.server = server;
		}

		public CannotFindSchemaAttributeException(string attr, string schemaDN, string server, Exception innerException) : base(Strings.CannotFindSchemaAttributeException(attr, schemaDN, server), innerException)
		{
			this.attr = attr;
			this.schemaDN = schemaDN;
			this.server = server;
		}

		protected CannotFindSchemaAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.attr = (string)info.GetValue("attr", typeof(string));
			this.schemaDN = (string)info.GetValue("schemaDN", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("attr", this.attr);
			info.AddValue("schemaDN", this.schemaDN);
			info.AddValue("server", this.server);
		}

		public string Attr
		{
			get
			{
				return this.attr;
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

		private readonly string attr;

		private readonly string schemaDN;

		private readonly string server;
	}
}
