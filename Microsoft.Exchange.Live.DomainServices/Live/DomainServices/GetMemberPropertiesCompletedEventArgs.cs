using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetMemberPropertiesCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetMemberPropertiesCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Property[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Property[])this.results[0];
			}
		}

		private object[] results;
	}
}
