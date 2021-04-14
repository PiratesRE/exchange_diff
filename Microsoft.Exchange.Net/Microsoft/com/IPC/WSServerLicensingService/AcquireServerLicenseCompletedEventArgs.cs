using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	public class AcquireServerLicenseCompletedEventArgs : AsyncCompletedEventArgs
	{
		public AcquireServerLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public BatchLicenseResponses AcquireServerLicenseResponses
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (BatchLicenseResponses)this.results[0];
			}
		}

		public VersionData Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (VersionData)this.results[1];
			}
		}

		private object[] results;
	}
}
