using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUMMailboxParameters : SetObjectProperties
	{
		public SetUMMailboxParameters()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetUMExtensionParameteres = new SetUMExtensionParameteres();
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMMailbox";
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
				return (Identity)base[UMMailboxSchema.UMMailboxPolicy];
			}
			set
			{
				value.FaultIfNull(Strings.UMMailboxPolicyErrorMessage);
				base[UMMailboxSchema.UMMailboxPolicy] = value;
			}
		}

		[DataMember]
		public string OperatorNumber
		{
			get
			{
				return (string)base[UMMailboxSchema.OperatorNumber];
			}
			set
			{
				base[UMMailboxSchema.OperatorNumber] = value;
			}
		}

		public SetUMExtensionParameteres SetUMExtensionParameteres { get; private set; }

		[DataMember]
		public IEnumerable<UMExtension> SecondaryExtensions
		{
			get
			{
				return this.SetUMExtensionParameteres.SecondaryExtensions;
			}
			set
			{
				this.SetUMExtensionParameteres.SecondaryExtensions = value;
			}
		}
	}
}
