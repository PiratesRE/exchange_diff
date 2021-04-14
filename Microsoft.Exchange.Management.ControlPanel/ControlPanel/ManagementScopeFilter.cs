using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ManagementScopeFilter : WebServiceParameters
	{
		public ManagementScopeFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-ManagementScope";
			}
		}

		[DataMember]
		public bool Exclusive
		{
			get
			{
				return (bool)base["Exclusive"];
			}
			set
			{
				base["Exclusive"] = value;
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Exclusive"] = false;
		}

		internal const string GetCmdlet = "Get-ManagementScope";
	}
}
