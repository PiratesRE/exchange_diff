using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class GetPersonaCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetPersonaCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetPersonaResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetPersonaResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
