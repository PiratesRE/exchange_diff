using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public abstract class AcePresentationObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AcePresentationObject.schema;
			}
		}

		public AcePresentationObject(ActiveDirectoryAccessRule ace, ObjectId identity) : this()
		{
			this.realAce = ace;
			this.Identity = identity;
			this.InheritanceType = ace.InheritanceType;
			this.IsInherited = ace.IsInherited;
			this.PopulateCalculatedProperties();
			base.ResetChangeTracking();
		}

		public AcePresentationObject() : base(new SimpleProviderPropertyBag())
		{
		}

		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
		[Parameter(Mandatory = false, ParameterSetName = "AccessRights")]
		public SwitchParameter Deny
		{
			get
			{
				return (bool)this[AcePresentationObjectSchema.Deny];
			}
			set
			{
				this[AcePresentationObjectSchema.Deny] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
		[Parameter(Mandatory = false, ParameterSetName = "AccessRights")]
		public ActiveDirectorySecurityInheritance InheritanceType
		{
			get
			{
				return (ActiveDirectorySecurityInheritance)this[AcePresentationObjectSchema.InheritanceType];
			}
			set
			{
				this[AcePresentationObjectSchema.InheritanceType] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
		[Parameter(Mandatory = true, ParameterSetName = "AccessRights")]
		public SecurityPrincipalIdParameter User
		{
			get
			{
				SecurityPrincipalIdParameter securityPrincipalIdParameter = (SecurityPrincipalIdParameter)this[AcePresentationObjectSchema.User];
				if (securityPrincipalIdParameter != null && SuppressingPiiContext.NeedPiiSuppression)
				{
					securityPrincipalIdParameter = new SecurityPrincipalIdParameter(SuppressingPiiData.Redact(securityPrincipalIdParameter.SecurityIdentifier.Value));
				}
				return securityPrincipalIdParameter;
			}
			set
			{
				this[AcePresentationObjectSchema.User] = value;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				ObjectId objectId = base.Identity;
				if (objectId is ADObjectId && SuppressingPiiContext.NeedPiiSuppression)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, (ADObjectId)objectId);
				}
				return objectId;
			}
			set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		public bool IsInherited
		{
			get
			{
				return (bool)this[AcePresentationObjectSchema.IsInherited];
			}
			internal set
			{
				this[AcePresentationObjectSchema.IsInherited] = value;
			}
		}

		protected ActiveDirectoryAccessRule RealAce
		{
			get
			{
				return this.realAce;
			}
		}

		protected virtual void PopulateCalculatedProperties()
		{
			TaskLogger.Trace("Resolving sid", new object[0]);
			SecurityIdentifier sid = (SecurityIdentifier)this.RealAce.IdentityReference;
			this.User = new SecurityPrincipalIdParameter(sid);
			this.Deny = (this.realAce.AccessControlType == AccessControlType.Deny);
		}

		private static AcePresentationObjectSchema schema = ObjectSchema.GetInstance<AcePresentationObjectSchema>();

		[NonSerialized]
		private ActiveDirectoryAccessRule realAce;
	}
}
