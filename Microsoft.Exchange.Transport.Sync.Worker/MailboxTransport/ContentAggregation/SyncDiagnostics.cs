using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncDiagnostics : IDiagnosable
	{
		public SyncDiagnostics(AggregationScheduler aggregationScheduler, RemoteServerHealthManager remoteServerHealthManager)
		{
			SyncUtilities.ThrowIfArgumentNull("aggregationScheduler", aggregationScheduler);
			SyncUtilities.ThrowIfArgumentNull("remoteServerHealthManager", remoteServerHealthManager);
			this.aggregationScheduler = aggregationScheduler;
			this.remoteServerHealthManager = remoteServerHealthManager;
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "SyncWorker";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement("SyncWorker");
			if (string.IsNullOrEmpty(parameters.Argument) || string.Equals(parameters.Argument, "help", StringComparison.OrdinalIgnoreCase))
			{
				xelement.Add(new XElement("help", "Supported argument(s): basic, verbose, help, sub-components:scheduler, remoteserverhealthmanager. Specifying a component will only return info for that component. E.g. verbose scheduler. That will only return verbose information for scheduler and nothing for the other components."));
			}
			else
			{
				bool verbose = 0 <= parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase);
				bool flag = 0 <= parameters.Argument.IndexOf("scheduler", StringComparison.OrdinalIgnoreCase);
				bool flag2 = 0 <= parameters.Argument.IndexOf("remoteserverhealthmanager", StringComparison.OrdinalIgnoreCase);
				bool flag3 = flag || flag2;
				if (!flag3 || flag)
				{
					xelement.Add(this.aggregationScheduler.GetDiagnosticInfo(verbose));
				}
				if (!flag3 || flag2)
				{
					xelement.Add(this.remoteServerHealthManager.GetDiagnosticInfo(verbose));
				}
			}
			return xelement;
		}

		internal void Register()
		{
			ProcessAccessManager.RegisterComponent(this);
		}

		internal void Unregister()
		{
			ProcessAccessManager.UnregisterComponent(this);
		}

		private const string ProcessAccessManagerComponentName = "SyncWorker";

		private readonly AggregationScheduler aggregationScheduler;

		private readonly RemoteServerHealthManager remoteServerHealthManager;
	}
}
