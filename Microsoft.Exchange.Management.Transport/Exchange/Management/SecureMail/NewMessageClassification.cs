using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SecureMail
{
	[Cmdlet("New", "MessageClassification", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewMessageClassification : NewMultitenancySystemConfigurationObjectTask<MessageClassification>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		public NewMessageClassification()
		{
			this.ClassificationID = Guid.NewGuid();
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Guid ClassificationID
		{
			get
			{
				return this.DataObject.ClassificationID;
			}
			set
			{
				this.DataObject.ClassificationID = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string DisplayName
		{
			get
			{
				return this.DataObject.DisplayName;
			}
			set
			{
				this.DataObject.DisplayName = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string SenderDescription
		{
			get
			{
				return this.DataObject.SenderDescription;
			}
			set
			{
				this.DataObject.SenderDescription = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RecipientDescription
		{
			get
			{
				return this.DataObject.RecipientDescription;
			}
			set
			{
				this.DataObject.RecipientDescription = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ClassificationDisplayPrecedenceLevel DisplayPrecedence
		{
			get
			{
				return this.DataObject.DisplayPrecedence;
			}
			set
			{
				this.DataObject.DisplayPrecedence = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool PermissionMenuVisible
		{
			get
			{
				return this.DataObject.PermissionMenuVisible;
			}
			set
			{
				this.DataObject.PermissionMenuVisible = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool RetainClassificationEnabled
		{
			get
			{
				return this.DataObject.RetainClassificationEnabled;
			}
			set
			{
				this.DataObject.RetainClassificationEnabled = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Localized")]
		public CultureInfo Locale
		{
			get
			{
				return this.locale;
			}
			set
			{
				this.locale = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMessageClassificationName(base.Name.ToString(), this.DisplayName.ToString(), this.SenderDescription.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return MessageClassificationIdParameter.DefaultRoot(base.DataSession);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			MessageClassification messageClassification = (MessageClassification)base.PrepareDataObject();
			if (!messageClassification.IsModified(ClassificationSchema.RecipientDescription))
			{
				this.RecipientDescription = this.SenderDescription;
			}
			OrFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.Name),
				new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, this.ClassificationID)
			});
			MessageClassification[] array = configurationSession.Find<MessageClassification>((ADObjectId)this.RootId, QueryScope.SubTree, filter, null, 1);
			ADObjectId adobjectId = configurationSession.GetOrgContainerId().GetDescendantId(MessageClassificationIdParameter.MessageClassificationRdn);
			if (this.Locale != null)
			{
				if (array == null || array.Length == 0)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorDefaultLocaleClassificationDoesNotExist(base.Name)), ErrorCategory.ObjectNotFound, messageClassification);
				}
				else
				{
					MessageClassification messageClassification2 = array[0];
					this.DataObject.ClassificationID = messageClassification2.ClassificationID;
					this.DataObject.DisplayPrecedence = messageClassification2.DisplayPrecedence;
					this.DataObject.PermissionMenuVisible = messageClassification2.PermissionMenuVisible;
					this.DataObject.RetainClassificationEnabled = messageClassification2.RetainClassificationEnabled;
					adobjectId = adobjectId.GetChildId(this.Locale.ToString());
				}
			}
			else
			{
				if (array.Length > 0)
				{
					string name = array[0].Name;
					base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorClassificationAlreadyDefined(name, this.ClassificationID)), ErrorCategory.InvalidOperation, base.Name);
				}
				adobjectId = (ADObjectId)this.RootId;
			}
			messageClassification.SetId(adobjectId.GetChildId(base.Name));
			return messageClassification;
		}

		protected override void InternalValidate()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			MessageClassification[] array = configurationSession.Find<MessageClassification>(configurationSession.GetOrgContainerId().GetDescendantId(MessageClassificationIdParameter.MessageClassificationRdn), QueryScope.SubTree, null, null, NewMessageClassification.MaxMessageClassificationsPerTenant);
			if (base.OrganizationId != OrganizationId.ForestWideOrgId && array.Length == NewMessageClassification.MaxMessageClassificationsPerTenant)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorExceededMaxClassificationsLimit(NewMessageClassification.MaxMessageClassificationsPerTenant)), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (base.DataSession.Read<Container>(this.DataObject.Id.Parent) == null)
			{
				Container container = new Container();
				container.SetId(this.DataObject.Id.Parent);
				base.DataSession.Save(container);
			}
			base.InternalProcessRecord();
		}

		private const string DefaultParameterSet = "Identity";

		private const string LocalizedParameterSet = "Localized";

		internal static readonly int MaxMessageClassificationsPerTenant = 15;

		private CultureInfo locale;
	}
}
