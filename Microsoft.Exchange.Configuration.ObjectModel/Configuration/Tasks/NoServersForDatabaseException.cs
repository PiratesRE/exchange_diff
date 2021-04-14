using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoServersForDatabaseException : LocalizedException
	{
		public NoServersForDatabaseException(string id) : base(Strings.ErrorNoServersForDatabase(id))
		{
			this.id = id;
		}

		public NoServersForDatabaseException(string id, Exception innerException) : base(Strings.ErrorNoServersForDatabase(id), innerException)
		{
			this.id = id;
		}

		protected NoServersForDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
