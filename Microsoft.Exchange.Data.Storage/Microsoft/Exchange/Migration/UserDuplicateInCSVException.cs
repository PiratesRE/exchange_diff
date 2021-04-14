using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UserDuplicateInCSVException : MigrationPermanentException
	{
		public UserDuplicateInCSVException(string alias) : base(Strings.UserDuplicateInCSV(alias))
		{
			this.alias = alias;
		}

		public UserDuplicateInCSVException(string alias, Exception innerException) : base(Strings.UserDuplicateInCSV(alias), innerException)
		{
			this.alias = alias;
		}

		protected UserDuplicateInCSVException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.alias = (string)info.GetValue("alias", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("alias", this.alias);
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
		}

		private readonly string alias;
	}
}
