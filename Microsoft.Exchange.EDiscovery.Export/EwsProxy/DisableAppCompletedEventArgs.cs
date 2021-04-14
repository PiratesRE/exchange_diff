using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class DisableAppCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DisableAppCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DisableAppResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DisableAppResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
