using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class SetImListMigrationCompletedCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SetImListMigrationCompletedCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SetImListMigrationCompletedResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SetImListMigrationCompletedResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
