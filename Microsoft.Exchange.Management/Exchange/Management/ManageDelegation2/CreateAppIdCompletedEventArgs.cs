using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Management.ManageDelegation2
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class CreateAppIdCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal CreateAppIdCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public AppIdInfo Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (AppIdInfo)this.results[0];
			}
		}

		private object[] results;
	}
}
