using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Template
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	public class AcquireTemplateInformationCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal AcquireTemplateInformationCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public TemplateInformation Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (TemplateInformation)this.results[0];
			}
		}

		private object[] results;
	}
}
