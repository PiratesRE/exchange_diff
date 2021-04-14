using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class GetOrganizationRelationshipSettingsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetOrganizationRelationshipSettingsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetOrganizationRelationshipSettingsResponse Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetOrganizationRelationshipSettingsResponse)this.results[0];
			}
		}

		private object[] results;
	}
}
