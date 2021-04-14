using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetManagementCertificateCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetManagementCertificateCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public CertData Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (CertData)this.results[0];
			}
		}

		private object[] results;
	}
}
