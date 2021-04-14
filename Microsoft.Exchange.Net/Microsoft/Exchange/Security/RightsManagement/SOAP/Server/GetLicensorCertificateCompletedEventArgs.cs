using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Server
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class GetLicensorCertificateCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetLicensorCertificateCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public LicensorCertChain Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (LicensorCertChain)this.results[0];
			}
		}

		private object[] results;
	}
}
