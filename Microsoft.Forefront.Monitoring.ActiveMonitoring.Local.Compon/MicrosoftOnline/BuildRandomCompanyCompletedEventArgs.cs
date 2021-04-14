using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	public class BuildRandomCompanyCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal BuildRandomCompanyCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Company Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Company)this.results[0];
			}
		}

		private object[] results;
	}
}
