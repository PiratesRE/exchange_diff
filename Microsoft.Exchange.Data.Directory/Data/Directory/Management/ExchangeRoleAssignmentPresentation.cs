using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ExchangeRoleAssignmentPresentation : ADPresentationObject
	{
		public ExchangeRoleAssignmentPresentation()
		{
		}

		public ExchangeRoleAssignmentPresentation(ExchangeRoleAssignment dataObject, ADObjectId userId, AssignmentMethod assignmentMethod, string userName, ADObjectId assigneeId, OrganizationId sharedOrgId) : base(dataObject)
		{
			if (assigneeId != null && null == sharedOrgId)
			{
				throw new ArgumentException("AssigneeID isnt null and sharedOrgId is null. sharedOrgId cannot be null if assigneeId isn't null.");
			}
			this.User = userId;
			this.assignmentMethod = assignmentMethod;
			if (!string.IsNullOrEmpty(userName))
			{
				this.EffectiveUserName = userName;
			}
			if (assigneeId != null)
			{
				this.roleAssignee = assigneeId;
			}
		}

		public ExchangeRoleAssignmentPresentation(ExchangeRoleAssignment dataObject, ADObjectId userId, AssignmentMethod assignmentMethod) : this(dataObject, userId, assignmentMethod, null, null, null)
		{
		}

		public ExchangeRoleAssignmentPresentation(ExchangeRoleAssignment dataObject, ADObjectId userId, AssignmentMethod assignmentMethod, string userName) : this(dataObject, userId, assignmentMethod, userName, null, null)
		{
		}

		public new ADObject DataObject
		{
			get
			{
				return base.DataObject;
			}
		}

		public ADObjectId User
		{
			get
			{
				ADObjectId adobjectId = this.user;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					adobjectId = (ADObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, adobjectId);
				}
				return adobjectId;
			}
			private set
			{
				this.user = value;
			}
		}

		public AssignmentMethod AssignmentMethod
		{
			get
			{
				return this.assignmentMethod;
			}
			internal set
			{
				this.assignmentMethod = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				if (this.isCompositeIdentity)
				{
					return this.compositeIdentity;
				}
				return base.Identity;
			}
		}

		public string EffectiveUserName
		{
			get
			{
				string text = this.effectiveUserName;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					text = (string)SuppressingPiiProperty.TryRedact(ExchangeRoleAssignmentPresentationSchema.RoleAssigneeName, text);
				}
				return text;
			}
			internal set
			{
				this.effectiveUserName = value;
			}
		}

		public MultiValuedProperty<FormattedADObjectIdCollection> AssignmentChain
		{
			get
			{
				return this.assignmentChain;
			}
			internal set
			{
				this.assignmentChain = value;
			}
		}

		public RoleAssigneeType RoleAssigneeType
		{
			get
			{
				return (RoleAssigneeType)this[ExchangeRoleAssignmentPresentationSchema.RoleAssigneeType];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.RoleAssigneeType] = value;
			}
		}

		public ADObjectId RoleAssignee
		{
			get
			{
				if (this.roleAssignee == null)
				{
					return (ADObjectId)this[ExchangeRoleAssignmentPresentationSchema.RoleAssignee];
				}
				return this.roleAssignee;
			}
			protected set
			{
				this[ExchangeRoleAssignmentPresentationSchema.RoleAssignee] = value;
			}
		}

		public ADObjectId Role
		{
			get
			{
				return (ADObjectId)this[ExchangeRoleAssignmentPresentationSchema.Role];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.Role] = value;
			}
		}

		public RoleAssignmentDelegationType RoleAssignmentDelegationType
		{
			get
			{
				return (RoleAssignmentDelegationType)this[ExchangeRoleAssignmentPresentationSchema.RoleAssignmentDelegationType];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.RoleAssignmentDelegationType] = value;
			}
		}

		public ADObjectId CustomRecipientWriteScope
		{
			get
			{
				return (ADObjectId)this[ExchangeRoleAssignmentPresentationSchema.CustomRecipientWriteScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.CustomRecipientWriteScope] = value;
			}
		}

		public ADObjectId CustomConfigWriteScope
		{
			get
			{
				return (ADObjectId)this[ExchangeRoleAssignmentPresentationSchema.CustomConfigWriteScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.CustomConfigWriteScope] = value;
			}
		}

		public ScopeType RecipientReadScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleAssignmentPresentationSchema.RecipientReadScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.RecipientReadScope] = value;
			}
		}

		public ScopeType ConfigReadScope
		{
			get
			{
				return (ScopeType)this[ExchangeRoleAssignmentPresentationSchema.ConfigReadScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.ConfigReadScope] = value;
			}
		}

		public RecipientWriteScopeType RecipientWriteScope
		{
			get
			{
				return (RecipientWriteScopeType)this[ExchangeRoleAssignmentPresentationSchema.RecipientWriteScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.RecipientWriteScope] = value;
			}
		}

		public ConfigWriteScopeType ConfigWriteScope
		{
			get
			{
				return (ConfigWriteScopeType)this[ExchangeRoleAssignmentPresentationSchema.ConfigWriteScope];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.ConfigWriteScope] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[ExchangeRoleAssignmentPresentationSchema.Enabled];
			}
			internal set
			{
				this[ExchangeRoleAssignmentPresentationSchema.Enabled] = value;
			}
		}

		public string RoleAssigneeName
		{
			get
			{
				return (string)this[ExchangeRoleAssignmentPresentationSchema.RoleAssigneeName];
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ExchangeRoleAssignmentPresentation.schema;
			}
		}

		internal void UpdatePresentationObjectWithEffectiveUser(ADObjectId effectiveUser, MultiValuedProperty<FormattedADObjectIdCollection> assignmentChains, bool isCompositeidentity, AssignmentMethod assignmentMethod)
		{
			this.EffectiveUserName = effectiveUser.Name;
			this.AssignmentChain = assignmentChains;
			this.isCompositeIdentity = isCompositeidentity;
			this.AssignmentMethod = assignmentMethod;
			this.User = effectiveUser;
			if (this.isCompositeIdentity)
			{
				this.compositeIdentity = new EffectiveUserObjectId(base.OriginalId, this.User);
			}
		}

		internal const char ElementSeparatorChar = '\\';

		private static ExchangeRoleAssignmentPresentationSchema schema = ObjectSchema.GetInstance<ExchangeRoleAssignmentPresentationSchema>();

		private AssignmentMethod assignmentMethod;

		private ADObjectId user;

		private string effectiveUserName;

		private bool isCompositeIdentity;

		private ObjectId compositeIdentity;

		private MultiValuedProperty<FormattedADObjectIdCollection> assignmentChain;

		private ADObjectId roleAssignee;
	}
}
