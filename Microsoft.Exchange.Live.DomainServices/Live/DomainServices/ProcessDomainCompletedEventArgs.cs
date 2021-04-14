using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class ProcessDomainCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal ProcessDomainCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DomainInfoEx Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DomainInfoEx)this.results[0];
			}
		}

		private object[] results;
	}
}
