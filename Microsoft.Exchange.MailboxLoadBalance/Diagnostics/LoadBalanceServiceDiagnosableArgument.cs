using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceServiceDiagnosableArgument : LoadBalanceDiagnosableArgumentBase
	{
		public bool CleanQueues
		{
			get
			{
				return base.HasArgument("cleanqueues");
			}
		}

		public bool ShowLoadBalancerResults
		{
			get
			{
				return base.HasArgument("loadbalanceresults");
			}
		}

		public bool StartLoadBalance
		{
			get
			{
				return base.HasArgument("startloadbalance");
			}
		}

		public bool ShowQueues
		{
			get
			{
				return base.HasArgument("showqueues");
			}
		}

		public bool ShowQueuedRequests
		{
			get
			{
				return base.HasArgument("showqueuedrequests");
			}
		}

		public bool RemoveSoftDeletedMailbox
		{
			get
			{
				return base.HasArgument("removesoftdeletedmailbox");
			}
		}

		public bool GetMoveHistory
		{
			get
			{
				return base.HasArgument("getmovehistory");
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return base.GetArgument<Guid>("mailboxguid");
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return base.GetArgument<Guid>("databaseguid");
			}
		}

		public Guid TargetDatabaseGuid
		{
			get
			{
				return base.GetArgument<Guid>("targetdatabaseguid");
			}
		}

		public Guid SourceDatabaseGuid
		{
			get
			{
				return base.GetArgument<Guid>("sourcedatabaseguid");
			}
		}

		public bool IsDrainingDatabase
		{
			get
			{
				return base.HasArgument("draindatabase");
			}
		}

		public Guid DatabaseToDrainGuid
		{
			get
			{
				return base.GetArgument<Guid>("draindatabase");
			}
		}

		protected override void ExtendSchema(Dictionary<string, Type> schema)
		{
			schema["startloadbalance"] = typeof(bool);
			schema["loadbalanceresults"] = typeof(bool);
			schema["showqueues"] = typeof(bool);
			schema["showqueuedrequests"] = typeof(bool);
			schema["removesoftdeletedmailbox"] = typeof(bool);
			schema["mailboxguid"] = typeof(Guid);
			schema["databaseguid"] = typeof(Guid);
			schema["getmovehistory"] = typeof(bool);
			schema["sourcedatabaseguid"] = typeof(Guid);
			schema["targetdatabaseguid"] = typeof(Guid);
			schema["draindag"] = typeof(Guid);
			schema["draindatabase"] = typeof(Guid);
			schema["cleanqueues"] = typeof(bool);
		}

		internal const string LoadBalancerResultsArgument = "loadbalanceresults";

		internal const string ShowQueuesArgument = "showqueues";

		private const string StartLoadBalanceArgument = "startloadbalance";

		private const string ShowQueuedRequestsArgument = "showqueuedrequests";

		private const string RemoveSoftDeletedMailboxArgument = "removesoftdeletedmailbox";

		private const string GetMoveHistoryArgument = "getmovehistory";

		private const string MailboxGuidArgument = "mailboxguid";

		private const string DatabaseGuidArgument = "databaseguid";

		private const string TargetDatabaseGuidArgument = "targetdatabaseguid";

		private const string SourceDatabaseGuidArgument = "sourcedatabaseguid";

		private const string DatabaseToDrainArgument = "draindatabase";

		private const string DagToDrainArgument = "draindag";

		private const string CleanQueuesArgument = "cleanqueues";
	}
}
