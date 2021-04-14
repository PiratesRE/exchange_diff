using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class ExchangeCommandResolver : DisposeTrackableBase, IPSCommandResolver
	{
		public ExchangeCommandResolver() : this(ExchangeCommandResolver.powerShellSnapin)
		{
		}

		public ExchangeCommandResolver(IEnumerable<string> snapIns)
		{
			RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
			PSSnapInException ex = null;
			Exception ex2 = null;
			IEnumerable<string> enumerable = (from s in snapIns
			select s.ToLower()).Distinct<string>();
			foreach (string name in enumerable)
			{
				try
				{
					runspaceConfiguration.AddPSSnapIn(name, out ex);
					ex2 = ex;
				}
				catch (PSArgumentException ex3)
				{
					ex2 = ex3;
				}
				if (ex2 != null)
				{
					ReportingWebServiceEventLogConstants.Tuple_LoadReportingschemaFailed.LogEvent(new object[]
					{
						ex2.Message
					});
					ServiceDiagnostics.ThrowError(ReportingErrorCode.ErrorSchemaInitializationFail, Strings.ErrorSchemaInitializationFail, ex2);
				}
			}
			this.runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
			if (this.runspace.RunspaceStateInfo.State != RunspaceState.Opened)
			{
				this.runspace.Open();
			}
		}

		public ReadOnlyCollection<PSTypeName> GetOutputType(string commandName)
		{
			base.CheckDisposed();
			Collection<PSObject> collection = this.runspace.CreatePipeline(string.Format("{0} {1}", "get-command", commandName)).Invoke();
			return ((CommandInfo)collection[0].BaseObject).OutputType;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.runspace != null)
			{
				this.runspace.Dispose();
				this.runspace = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeCommandResolver>(this);
		}

		private static List<string> powerShellSnapin = new List<string>
		{
			"Microsoft.Exchange.Management.PowerShell.E2010"
		};

		private Runspace runspace;
	}
}
