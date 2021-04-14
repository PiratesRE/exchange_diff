using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Threading;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal class RemotePowershellHost : PSHost
	{
		public RemotePowershellHost(PowershellHostUI hostUI)
		{
			this.hostUI = hostUI;
		}

		public override CultureInfo CurrentCulture
		{
			get
			{
				return this.originalCultureInfo;
			}
		}

		public override CultureInfo CurrentUICulture
		{
			get
			{
				return this.originalUICultureInfo;
			}
		}

		public override Guid InstanceId
		{
			get
			{
				return this.myId;
			}
		}

		public override string Name
		{
			get
			{
				return "HybridHost";
			}
		}

		public override PSHostUserInterface UI
		{
			get
			{
				return this.hostUI;
			}
		}

		public override Version Version
		{
			get
			{
				return new Version(1, 0, 0, 0);
			}
		}

		public override void EnterNestedPrompt()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override void ExitNestedPrompt()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override void NotifyBeginApplication()
		{
		}

		public override void NotifyEndApplication()
		{
		}

		public override void SetShouldExit(int exitCode)
		{
		}

		private readonly Guid myId = Guid.NewGuid();

		private CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;

		private CultureInfo originalUICultureInfo = Thread.CurrentThread.CurrentUICulture;

		private PowershellHostUI hostUI;
	}
}
