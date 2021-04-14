using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Monad;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadHost : RunspaceHost
	{
		internal MonadHost(CultureInfo currentCulture, CultureInfo currentUICulture)
		{
			this.currentCulture = currentCulture;
			this.currentUICulture = currentUICulture;
		}

		public static string ServerVersion
		{
			get
			{
				return "Exchange Management Console:" + MonadHost.version;
			}
		}

		public override CultureInfo CurrentCulture
		{
			get
			{
				return this.currentCulture;
			}
		}

		public override CultureInfo CurrentUICulture
		{
			get
			{
				return this.currentUICulture;
			}
		}

		public override Guid InstanceId
		{
			get
			{
				return this.instanceID;
			}
		}

		public override string Name
		{
			get
			{
				return "Exchange Management Console";
			}
		}

		public override Version Version
		{
			get
			{
				return MonadHost.version;
			}
		}

		protected MonadConnection Connection
		{
			get
			{
				if (this.connection == null)
				{
					throw new NotSupportedException();
				}
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		public static void InitializeMonadHostConnection(RunspaceHost runspaceHost, MonadConnection connection)
		{
			MonadHost monadHost = runspaceHost as MonadHost;
			if (monadHost == null)
			{
				return;
			}
			monadHost.connection = connection;
		}

		public override void Deactivate()
		{
			this.connection = null;
		}

		public override void NotifyBeginApplication()
		{
			ExTraceGlobals.HostTracer.Information((long)this.GetHashCode(), "MonadHost.NotifyBeginApplication()");
		}

		public override void NotifyEndApplication()
		{
			ExTraceGlobals.HostTracer.Information((long)this.GetHashCode(), "MonadHost.NotifyEndApplication()");
		}

		public override void SetShouldExit(int exitCode)
		{
			ExTraceGlobals.HostTracer.Information((long)this.GetHashCode(), "MonadHost.SetShouldExit()");
		}

		public override void EnterNestedPrompt()
		{
			throw new NotSupportedException();
		}

		public override void ExitNestedPrompt()
		{
			throw new NotSupportedException();
		}

		public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			return this.Connection.InteractionHandler.Prompt(caption, message, descriptions);
		}

		public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			ExTraceGlobals.HostTracer.Information<string, int>((long)this.GetHashCode(), "MonadHost.PromptForChoice({0}, {1})", message, defaultChoice);
			return (int)this.Connection.InteractionHandler.ShowConfirmationDialog(message, (ConfirmationChoice)defaultChoice);
		}

		public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "MonadHost.Write({0})", value);
		}

		public override void Write(string value)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "MonadGhostUI.Write({0})", value);
		}

		public override void WriteDebugLine(string message)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "MonadGhostUI.WriteDebugLine({0})", message);
		}

		public override void WriteLine(string value)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "MonadGhostUI.WriteLine({0})", value);
		}

		public override void WriteVerboseLine(string message)
		{
			this.Connection.InteractionHandler.ReportVerboseOutput(message);
		}

		public override void WriteErrorLine(string message)
		{
		}

		private const string name = "Exchange Management Console";

		private static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;

		private readonly Guid instanceID = Guid.NewGuid();

		private CultureInfo currentCulture;

		private CultureInfo currentUICulture;

		private MonadConnection connection;
	}
}
