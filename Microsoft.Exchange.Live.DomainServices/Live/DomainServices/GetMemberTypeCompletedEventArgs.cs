using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Live.DomainServices
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetMemberTypeCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetMemberTypeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public MemberType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (MemberType)this.results[0];
			}
		}

		private object[] results;
	}
}
