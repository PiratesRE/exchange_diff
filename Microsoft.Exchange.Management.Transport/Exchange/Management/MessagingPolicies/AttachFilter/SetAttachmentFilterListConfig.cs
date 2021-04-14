using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("set", "attachmentfilterlistconfig", SupportsShouldProcess = true)]
	public class SetAttachmentFilterListConfig : SetSingletonSystemConfigurationObjectTask<AttachmentFilteringConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAttachmentfilterlistconfig;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			}
		}

		[Parameter]
		public MultiValuedProperty<ReceiveConnectorIdParameter> ExceptionConnectors
		{
			get
			{
				return (MultiValuedProperty<ReceiveConnectorIdParameter>)base.Fields["ExceptionConnectors"];
			}
			set
			{
				base.Fields["ExceptionConnectors"] = value;
			}
		}

		public SetAttachmentFilterListConfig()
		{
			base.Fields["ExceptionConnectors"] = null;
			base.Fields.ResetChangeTracking();
		}

		protected override void InternalProcessRecord()
		{
			MultiValuedProperty<string> attachmentNames = this.DataObject.AttachmentNames;
			if (attachmentNames != null)
			{
				foreach (string text in attachmentNames)
				{
					if (text != null && (!text.StartsWith(AttachmentType.ContentType.ToString() + ":", StringComparison.InvariantCulture) || text.Length < AttachmentType.ContentType.ToString().Length + 2) && (!text.StartsWith(AttachmentType.FileName.ToString() + ":", StringComparison.InvariantCulture) || text.Length < AttachmentType.FileName.ToString().Length + 2))
					{
						base.WriteError(new InvalidDataException(DirectoryStrings.AttachmentFilterEntryInvalid.ToString()), ErrorCategory.InvalidData, null);
						return;
					}
				}
			}
			if (base.Fields.IsModified("ExceptionConnectors"))
			{
				this.DataObject.ExceptionConnectors = base.ResolveIdParameterCollection<ReceiveConnectorIdParameter, ReceiveConnector, ADObjectId>(this.ExceptionConnectors, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorReceiveConnectorNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorReceiveConnectorNotUnique), null, null);
			}
			else if (this.DataObject.IsChanged(AttachmentFilteringConfigSchema.ExceptionConnectors) && this.DataObject.ExceptionConnectors != null && this.DataObject.ExceptionConnectors.Added.Length != 0)
			{
				foreach (ADObjectId adObjectId in this.DataObject.ExceptionConnectors.Added)
				{
					ReceiveConnectorIdParameter receiveConnectorIdParameter = new ReceiveConnectorIdParameter(adObjectId);
					base.GetDataObject<ReceiveConnector>(receiveConnectorIdParameter, base.DataSession, null, new LocalizedString?(Strings.ErrorReceiveConnectorNotFound(receiveConnectorIdParameter)), new LocalizedString?(Strings.ErrorReceiveConnectorNotUnique(receiveConnectorIdParameter)));
				}
			}
			base.InternalProcessRecord();
		}

		private const string ExceptionConnectorsKey = "ExceptionConnectors";
	}
}
