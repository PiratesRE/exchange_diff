using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetPasswordExpirationDateCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetPasswordExpirationDateCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetPasswordExpirationDateResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetPasswordExpirationDateResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
