using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ClientIntentQuery
	{
		public GlobalObjectId GlobalObjectId { get; private set; }

		public ICalendarItemStateDefinition TargetState { get; private set; }

		protected ClientIntentQuery(GlobalObjectId globalObjectId, ICalendarItemStateDefinition targetState)
		{
			Util.ThrowOnNullArgument(targetState, "targetState");
			this.GlobalObjectId = globalObjectId;
			this.TargetState = targetState;
		}

		public static bool CheckDesiredClientIntent(ClientIntentFlags? changeIntent, params ClientIntentFlags[] desiredClientIntent)
		{
			if (desiredClientIntent == null || desiredClientIntent.Length == 0)
			{
				throw new InvalidOperationException("At least one intent should be desired from the query.");
			}
			return changeIntent != null && desiredClientIntent.Any((ClientIntentFlags flag) => ClientIntentQuery.CheckDesiredClientIntent(changeIntent.Value, flag));
		}

		public abstract ClientIntentQuery.QueryResult Execute(MailboxSession session, CalendarVersionStoreGateway cvsGateway);

		protected ClientIntentFlags GetClientIntentFromPropertyBag(PropertyBag propertyBag)
		{
			return propertyBag.GetValueOrDefault<ClientIntentFlags>(CalendarItemBaseSchema.ClientIntent, ClientIntentFlags.None);
		}

		protected VersionedId GetIdFromPropertyBag(PropertyBag propertyBag)
		{
			return propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id);
		}

		protected void RunQuery(MailboxSession session, CalendarVersionStoreGateway cvsGateway, Func<PropertyBag, bool> processRecord)
		{
			if (this.TargetState.IsRecurringMasterSpecific)
			{
				cvsGateway.QueryByCleanGlobalObjectId(session, this.GlobalObjectId, this.TargetState.SchemaKey, this.TargetState.RequiredProperties, processRecord, true, ClientIntentQuery.CalendarItemClassArray, null, null);
				return;
			}
			cvsGateway.QueryByGlobalObjectId(session, this.GlobalObjectId, this.TargetState.SchemaKey, this.TargetState.RequiredProperties, processRecord, true, ClientIntentQuery.CalendarItemClassArray, null, null);
		}

		private static bool CheckDesiredClientIntent(ClientIntentFlags changeIntent, ClientIntentFlags desiredFlag)
		{
			if (desiredFlag != ClientIntentFlags.None)
			{
				return changeIntent.Includes(desiredFlag);
			}
			return changeIntent == ClientIntentFlags.None;
		}

		protected static readonly string[] CalendarItemClassArray = new string[]
		{
			"IPM.Appointment"
		};

		internal struct QueryResult
		{
			public QueryResult(ClientIntentFlags? intent, VersionedId sourceVersionId)
			{
				this.Intent = intent;
				this.SourceVersionId = sourceVersionId;
			}

			public ClientIntentFlags? Intent;

			public VersionedId SourceVersionId;
		}
	}
}
