using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	public class GetCompanyForeignPrincipalObjectsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetCompanyForeignPrincipalObjectsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ForeignPrincipal[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ForeignPrincipal[])this.results[0];
			}
		}

		private object[] results;
	}
}
