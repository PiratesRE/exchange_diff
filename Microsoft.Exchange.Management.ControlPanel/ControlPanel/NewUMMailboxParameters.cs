using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewUMMailboxParameters : UMBasePinSetParameteres
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Enable-UMMailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public Identity Identity
		{
			get
			{
				return (Identity)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		[DataMember]
		public Identity UMMailboxPolicy
		{
			get
			{
				return (Identity)base["UMMailboxPolicy"];
			}
			set
			{
				base["UMMailboxPolicy"] = value;
			}
		}

		[DataMember]
		public string Extension
		{
			get
			{
				return (string)base["Extensions"];
			}
			set
			{
				base["Extensions"] = value;
			}
		}

		[DataMember]
		public string SipResourceIdentifier { get; set; }

		[DataMember]
		public string E164ResourceIdentifier { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (!string.IsNullOrEmpty(this.SipResourceIdentifier))
			{
				base["SipResourceIdentifier"] = this.SipResourceIdentifier;
				return;
			}
			if (!string.IsNullOrEmpty(this.E164ResourceIdentifier))
			{
				base["SipResourceIdentifier"] = this.E164ResourceIdentifier;
			}
		}
	}
}
