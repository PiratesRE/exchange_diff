using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class FindPeopleCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal FindPeopleCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public FindPeopleResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (FindPeopleResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
