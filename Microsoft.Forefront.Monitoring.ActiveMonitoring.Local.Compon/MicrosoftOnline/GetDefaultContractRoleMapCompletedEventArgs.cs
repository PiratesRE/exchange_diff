using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class GetDefaultContractRoleMapCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetDefaultContractRoleMapCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public RoleMap[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (RoleMap[])this.results[0];
			}
		}

		private object[] results;
	}
}
