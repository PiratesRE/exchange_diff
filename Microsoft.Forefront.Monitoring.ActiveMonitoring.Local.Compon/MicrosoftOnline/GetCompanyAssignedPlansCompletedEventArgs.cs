using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	public class GetCompanyAssignedPlansCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetCompanyAssignedPlansCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public AssignedPlanValue[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (AssignedPlanValue[])this.results[0];
			}
		}

		private object[] results;
	}
}
