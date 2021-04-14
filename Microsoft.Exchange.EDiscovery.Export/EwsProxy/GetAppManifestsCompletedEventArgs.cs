using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class GetAppManifestsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetAppManifestsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetAppManifestsResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetAppManifestsResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
