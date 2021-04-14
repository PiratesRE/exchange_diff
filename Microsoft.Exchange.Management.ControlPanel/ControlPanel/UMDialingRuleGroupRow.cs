using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMDialingRuleGroupRow : BaseRow
	{
		public UMDialingRuleGroupRow(string name) : base(new Identity(name, name), null)
		{
		}

		public UMDialingRuleGroupRow(Identity id) : base(id, null)
		{
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return base.Identity.RawIdentity;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
