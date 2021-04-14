using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class GetDomainInfoExCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetDomainInfoExCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
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
