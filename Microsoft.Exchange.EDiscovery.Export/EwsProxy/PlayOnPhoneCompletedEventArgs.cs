using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class PlayOnPhoneCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal PlayOnPhoneCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public PlayOnPhoneResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (PlayOnPhoneResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
