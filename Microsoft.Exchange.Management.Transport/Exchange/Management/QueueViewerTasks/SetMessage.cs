using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[LocDescription(QueueViewerStrings.IDs.SetMessageTask)]
	[Cmdlet("Set", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetMessage : MessageActionWithFilter
	{
		[Parameter(Mandatory = true)]
		public int OutboundIPPool
		{
			get
			{
				return (int)base.Fields["OutboundIPPool"];
			}
			set
			{
				base.Fields["OutboundIPPool"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Resubmit
		{
			get
			{
				return (bool)base.Fields["Resubmit"];
			}
			set
			{
				base.Fields["Resubmit"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageSetMessageFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageSetMessageIdentity(base.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.OutboundIPPool < 0 || this.OutboundIPPool > 65535)
			{
				base.WriteError(new LocalizedException(QueueViewerStrings.SetMessageOutboundPoolOutsideRange(this.OutboundIPPool, 0, 65535)), ErrorCategory.InvalidData, this.OutboundIPPool);
			}
			if (base.Fields.IsModified("OutboundIPPool") && (!base.Fields.IsModified("Resubmit") || !this.Resubmit))
			{
				base.WriteError(new LocalizedException(QueueViewerStrings.SetMessageResubmitMustBeTrue), ErrorCategory.InvalidData, "Resubmit");
			}
			long identity = 1L;
			QueueIdentity queueIdentity = QueueIdentity.Empty;
			if (base.Identity != null)
			{
				identity = base.Identity.InternalId;
				queueIdentity = base.Identity.QueueIdentity;
			}
			this.properties = new PropertyBagBasedMessageInfo(identity, queueIdentity);
			this.properties.OutboundIPPool = this.OutboundIPPool;
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)base.Server))
			{
				queueViewerClient.SetMessage(base.Identity, this.innerFilter, this.properties, base.Fields.IsModified("Resubmit") && this.Resubmit);
			}
		}

		private const string ResubmitProperty = "Resubmit";

		private const string OutboundIPPoolProperty = "OutboundIPPool";

		private ExtensibleMessageInfo properties;
	}
}
