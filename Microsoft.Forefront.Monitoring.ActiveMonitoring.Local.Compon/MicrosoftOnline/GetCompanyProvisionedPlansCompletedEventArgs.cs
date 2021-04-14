using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetCompanyProvisionedPlansCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetCompanyProvisionedPlansCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ProvisionedPlanValue[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ProvisionedPlanValue[])this.results[0];
			}
		}

		private object[] results;
	}
}
