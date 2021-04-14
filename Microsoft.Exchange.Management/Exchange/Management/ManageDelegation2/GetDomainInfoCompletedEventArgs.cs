using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Management.ManageDelegation2
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class GetDomainInfoCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetDomainInfoCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DomainInfo Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DomainInfo)this.results[0];
			}
		}

		private object[] results;
	}
}
