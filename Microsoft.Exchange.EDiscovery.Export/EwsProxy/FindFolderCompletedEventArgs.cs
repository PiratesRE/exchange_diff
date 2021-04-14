using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class FindFolderCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal FindFolderCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public FindFolderResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (FindFolderResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
