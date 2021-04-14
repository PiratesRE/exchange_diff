using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationObjectNotFoundInADException : StoragePermanentException
	{
		public MigrationObjectNotFoundInADException(string legDN, string server) : base(Strings.MigrationObjectNotFoundInADError(legDN, server))
		{
			this.legDN = legDN;
			this.server = server;
		}

		public MigrationObjectNotFoundInADException(string legDN, string server, Exception innerException) : base(Strings.MigrationObjectNotFoundInADError(legDN, server), innerException)
		{
			this.legDN = legDN;
			this.server = server;
		}

		protected MigrationObjectNotFoundInADException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.legDN = (string)info.GetValue("legDN", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("legDN", this.legDN);
			info.AddValue("server", this.server);
		}

		public string LegDN
		{
			get
			{
				return this.legDN;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string legDN;

		private readonly string server;
	}
}
