using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MessageClassification : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MessageClassification.schema;
			}
		}

		public RawSecurityDescriptor SharedSecurityDescriptor
		{
			get
			{
				if (this.sharedSecurityDescriptor == null)
				{
					RawSecurityDescriptor rawSecurityDescriptor = base.ReadSecurityDescriptor();
					string sddlForm = rawSecurityDescriptor.GetSddlForm(AccessControlSections.Access);
					lock (MessageClassification.securityDescriptorTable)
					{
						if (!MessageClassification.securityDescriptorTable.TryGetValue(sddlForm, out this.sharedSecurityDescriptor))
						{
							MessageClassification.securityDescriptorTable[sddlForm] = rawSecurityDescriptor;
							this.sharedSecurityDescriptor = rawSecurityDescriptor;
						}
					}
				}
				return this.sharedSecurityDescriptor;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MessageClassification.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return MessageClassification.parentPath;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ClassificationID
		{
			get
			{
				return (Guid)this[ClassificationSchema.ClassificationID];
			}
			internal set
			{
				this[ClassificationSchema.ClassificationID] = value;
			}
		}

		public int Version
		{
			get
			{
				return (int)this[ClassificationSchema.Version];
			}
			internal set
			{
				this[ClassificationSchema.Version] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ClassificationDisplayPrecedenceLevel DisplayPrecedence
		{
			get
			{
				return (ClassificationDisplayPrecedenceLevel)this[ClassificationSchema.DisplayPrecedence];
			}
			set
			{
				this[ClassificationSchema.DisplayPrecedence] = value;
			}
		}

		public string Locale
		{
			get
			{
				return (string)this[ClassificationSchema.Locale];
			}
		}

		public bool IsDefault
		{
			get
			{
				return this[ClassificationSchema.Locale] == null;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[ClassificationSchema.DisplayName];
			}
			set
			{
				this[ClassificationSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SenderDescription
		{
			get
			{
				return (string)this[ClassificationSchema.SenderDescription];
			}
			set
			{
				this[ClassificationSchema.SenderDescription] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RecipientDescription
		{
			get
			{
				return (string)this[ClassificationSchema.RecipientDescription];
			}
			set
			{
				this[ClassificationSchema.RecipientDescription] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PermissionMenuVisible
		{
			get
			{
				return (bool)this[ClassificationSchema.PermissionMenuVisible];
			}
			set
			{
				this[ClassificationSchema.PermissionMenuVisible] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetainClassificationEnabled
		{
			get
			{
				return (bool)this[ClassificationSchema.RetainClassificationEnabled];
			}
			set
			{
				this[ClassificationSchema.RetainClassificationEnabled] = value;
			}
		}

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Message Classifications,CN=Transport Settings");

		private static ClassificationSchema schema = ObjectSchema.GetInstance<ClassificationSchema>();

		private static string mostDerivedClass = "msExchMessageClassification";

		private static Dictionary<string, RawSecurityDescriptor> securityDescriptorTable = new Dictionary<string, RawSecurityDescriptor>();

		[NonSerialized]
		private RawSecurityDescriptor sharedSecurityDescriptor;
	}
}
