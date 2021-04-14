using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableOnlineDatabase : QueryableObjectImplBase<QueryableOnlineDatabaseObjectSchema>
	{
		public string DatabaseName
		{
			get
			{
				return (string)this[QueryableOnlineDatabaseObjectSchema.DatabaseName];
			}
			set
			{
				this[QueryableOnlineDatabaseObjectSchema.DatabaseName] = value;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return (Guid)this[QueryableOnlineDatabaseObjectSchema.DatabaseGuid];
			}
			set
			{
				this[QueryableOnlineDatabaseObjectSchema.DatabaseGuid] = value;
			}
		}

		public bool RestartRequired
		{
			get
			{
				return (bool)this[QueryableOnlineDatabaseObjectSchema.RestartRequired];
			}
			set
			{
				this[QueryableOnlineDatabaseObjectSchema.RestartRequired] = value;
			}
		}

		public QueryableEventController EventController
		{
			get
			{
				return (QueryableEventController)this[QueryableOnlineDatabaseObjectSchema.EventController];
			}
			set
			{
				this[QueryableOnlineDatabaseObjectSchema.EventController] = value;
			}
		}
	}
}
