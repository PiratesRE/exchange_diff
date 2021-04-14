using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	public class AcquirePreLicenseCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal AcquirePreLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public AcquirePreLicenseResponse[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (AcquirePreLicenseResponse[])this.results[0];
			}
		}

		private object[] results;
	}
}
