using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ProxyPSCommand
	{
		public ProxyPSCommand(RemoteConnectionInfo connectionInfo, PSCommand cmd, Task.TaskWarningLoggingDelegate writeWarning) : this(connectionInfo, cmd, false, writeWarning)
		{
		}

		public ProxyPSCommand(RemoteConnectionInfo connectionInfo, PSCommand cmd, bool asyncInvoke, Task.TaskWarningLoggingDelegate writeWarning)
		{
			if (connectionInfo == null)
			{
				throw new ArgumentNullException("connectionInfo");
			}
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			RemoteRunspaceFactory remoteRunspaceFactory = new RemoteRunspaceFactory(new RunspaceConfigurationFactory(), null, connectionInfo);
			this.runspaceMediator = new RunspaceMediator(remoteRunspaceFactory, new BasicRunspaceCache(1));
			this.cmd = cmd;
			this.asyncInvoke = asyncInvoke;
			this.writeWarning = writeWarning;
		}

		public static Func<RunspaceProxy, PSCommand, IPowerShellProxy> PowerShellProxyFactory
		{
			get
			{
				return ProxyPSCommand.powerShellProxyFactory;
			}
			set
			{
				ProxyPSCommand.powerShellProxyFactory = value;
			}
		}

		public IEnumerable<PSObject> Invoke()
		{
			if (!this.asyncInvoke)
			{
				Collection<PSObject> result = null;
				using (RunspaceProxy runspaceProxy = new RunspaceProxy(this.runspaceMediator, true))
				{
					IPowerShellProxy powerShellProxy = ProxyPSCommand.PowerShellProxyFactory(runspaceProxy, this.cmd);
					result = powerShellProxy.Invoke<PSObject>();
					if (powerShellProxy.Errors != null && powerShellProxy.Errors.Count != 0 && powerShellProxy.Errors[0].Exception != null)
					{
						throw powerShellProxy.Errors[0].Exception;
					}
					if (powerShellProxy.Warnings != null && powerShellProxy.Warnings.Count != 0)
					{
						foreach (WarningRecord warningRecord in powerShellProxy.Warnings)
						{
							this.writeWarning(new LocalizedString(warningRecord.Message));
						}
					}
				}
				return result;
			}
			return new CmdletProxyDataReader(this.runspaceMediator, this.cmd, this.writeWarning);
		}

		private RunspaceMediator runspaceMediator;

		private PSCommand cmd;

		private readonly bool asyncInvoke;

		private readonly Task.TaskWarningLoggingDelegate writeWarning;

		private static Func<RunspaceProxy, PSCommand, IPowerShellProxy> powerShellProxyFactory = (RunspaceProxy runspace, PSCommand cmd) => new PowerShellProxy(runspace, cmd);
	}
}
