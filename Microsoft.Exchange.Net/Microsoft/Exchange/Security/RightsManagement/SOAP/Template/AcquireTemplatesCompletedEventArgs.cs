using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Template
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class AcquireTemplatesCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal AcquireTemplatesCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GuidTemplate[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GuidTemplate[])this.results[0];
			}
		}

		private object[] results;
	}
}
