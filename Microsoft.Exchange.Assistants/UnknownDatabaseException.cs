using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnknownDatabaseException : LocalizedException
	{
		public UnknownDatabaseException(string databaseId) : base(Strings.descUnknownDatabase(databaseId))
		{
			this.databaseId = databaseId;
		}

		public UnknownDatabaseException(string databaseId, Exception innerException) : base(Strings.descUnknownDatabase(databaseId), innerException)
		{
			this.databaseId = databaseId;
		}

		protected UnknownDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseId = (string)info.GetValue("databaseId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseId", this.databaseId);
		}

		public string DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		private readonly string databaseId;
	}
}
