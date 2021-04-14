using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniDatabase : MiniObject, IUsnChanged
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniDatabase.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, MailboxDatabase.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, PublicFolderDatabase.MostDerivedClass)
				});
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[MiniDatabaseSchema.Server];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[MiniDatabaseSchema.ServerName];
			}
		}

		public ADObjectId MasterServerOrAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[MiniDatabaseSchema.MasterServerOrAvailabilityGroup];
			}
		}

		public long UsnChanged
		{
			get
			{
				return (long)this[MiniDatabaseSchema.UsnChanged];
			}
		}

		private static MiniDatabaseSchema schema = new MiniDatabaseSchema();

		private static string mostDerivedClass = "msExchMDB";
	}
}
