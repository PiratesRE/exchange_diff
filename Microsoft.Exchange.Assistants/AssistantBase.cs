using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class AssistantBase
	{
		public AssistantBase(DatabaseInfo databaseInfo, LocalizedString name, string nonLocalizedName)
		{
			if (databaseInfo == null)
			{
				throw new ArgumentNullException("databaseInfo");
			}
			this.databaseInfo = databaseInfo;
			this.Name = name;
			this.NonLocalizedName = nonLocalizedName;
			AssistantBase.Tracer.TraceDebug<AssistantBase>((long)this.GetHashCode(), "{0}: created", this);
		}

		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		public bool Shutdown
		{
			get
			{
				return this.shutdown;
			}
		}

		public LocalizedString Name { get; private set; }

		public string NonLocalizedName { get; private set; }

		public void OnShutdown()
		{
			AssistantBase.Tracer.TraceDebug<AssistantBase>((long)this.GetHashCode(), "{0}: OnShutdown started", this);
			this.shutdown = true;
			this.OnShutdownInternal();
			AssistantBase.Tracer.TraceDebug<AssistantBase>((long)this.GetHashCode(), "{0}: OnShutdown completed", this);
			AssistantBase.TracerPfd.TracePfd<int, AssistantBase>((long)this.GetHashCode(), "PFD IWS {0} {1}: Shutdown", 23319, this);
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = this.NonLocalizedName + " for " + this.DatabaseInfo.ToString();
			}
			return this.toString;
		}

		protected virtual void OnShutdownInternal()
		{
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssistantBaseTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;

		private DatabaseInfo databaseInfo;

		private bool shutdown;

		private string toString;
	}
}
