using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetRoomsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetRoomsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetRoomsResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetRoomsResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
