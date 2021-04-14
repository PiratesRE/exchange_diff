using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	public class CertifyCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal CertifyCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public CertifyResponse Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (CertifyResponse)this.results[0];
			}
		}

		private object[] results;
	}
}
