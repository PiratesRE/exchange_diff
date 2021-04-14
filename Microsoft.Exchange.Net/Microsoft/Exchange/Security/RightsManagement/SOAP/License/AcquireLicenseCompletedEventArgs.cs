using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.License
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class AcquireLicenseCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal AcquireLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public AcquireLicenseResponse[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (AcquireLicenseResponse[])this.results[0];
			}
		}

		private object[] results;
	}
}
