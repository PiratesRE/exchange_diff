using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class EnumMembersCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal EnumMembersCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Member[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Member[])this.results[0];
			}
		}

		private object[] results;
	}
}
