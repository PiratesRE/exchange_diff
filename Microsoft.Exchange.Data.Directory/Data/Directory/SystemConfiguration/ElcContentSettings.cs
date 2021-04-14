using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ElcContentSettings : ADConfigurationObject
	{
		internal ElcContentSettings(IDirectorySession session, ADObjectId elcFolderId, string name)
		{
			this.m_Session = session;
			base.SetId(elcFolderId.GetChildId(name));
		}

		public ElcContentSettings()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ElcContentSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ElcContentSettings.mostDerivedClass;
			}
		}

		public string MessageClassDisplayName
		{
			get
			{
				return (string)this[ElcContentSettingsSchema.MessageClassDisplayName];
			}
		}

		public string MessageClass
		{
			get
			{
				return (string)this[ElcContentSettingsSchema.MessageClass];
			}
			set
			{
				this[ElcContentSettingsSchema.MessageClass] = value;
			}
		}

		public string Description
		{
			get
			{
				return DirectoryStrings.ElcContentSettingsDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetentionEnabled
		{
			get
			{
				return (bool)this[ElcContentSettingsSchema.RetentionEnabled];
			}
			set
			{
				this[ElcContentSettingsSchema.RetentionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RetentionActionType RetentionAction
		{
			get
			{
				return (RetentionActionType)this[ElcContentSettingsSchema.RetentionAction];
			}
			set
			{
				this[ElcContentSettingsSchema.RetentionAction] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? AgeLimitForRetention
		{
			get
			{
				return (EnhancedTimeSpan?)this[ElcContentSettingsSchema.AgeLimitForRetention];
			}
			set
			{
				this[ElcContentSettingsSchema.AgeLimitForRetention] = value;
			}
		}

		public ADObjectId MoveToDestinationFolder
		{
			get
			{
				return (ADObjectId)this[ElcContentSettingsSchema.MoveToDestinationFolder];
			}
			set
			{
				this[ElcContentSettingsSchema.MoveToDestinationFolder] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RetentionDateType TriggerForRetention
		{
			get
			{
				return (RetentionDateType)this[ElcContentSettingsSchema.TriggerForRetention];
			}
			set
			{
				this[ElcContentSettingsSchema.TriggerForRetention] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public JournalingFormat MessageFormatForJournaling
		{
			get
			{
				return (JournalingFormat)this[ElcContentSettingsSchema.MessageFormatForJournaling];
			}
			set
			{
				this[ElcContentSettingsSchema.MessageFormatForJournaling] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalingEnabled
		{
			get
			{
				return (bool)this[ElcContentSettingsSchema.JournalingEnabled];
			}
			set
			{
				this[ElcContentSettingsSchema.JournalingEnabled] = value;
			}
		}

		public ADObjectId AddressForJournaling
		{
			get
			{
				return (ADObjectId)this[ElcContentSettingsSchema.AddressForJournaling];
			}
			set
			{
				this[ElcContentSettingsSchema.AddressForJournaling] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LabelForJournaling
		{
			get
			{
				return (string)this[ElcContentSettingsSchema.LabelForJournaling];
			}
			set
			{
				this[ElcContentSettingsSchema.LabelForJournaling] = value;
			}
		}

		public ADObjectId ManagedFolder
		{
			get
			{
				return (ADObjectId)this[ElcContentSettingsSchema.ManagedFolder];
			}
		}

		public string ManagedFolderName
		{
			get
			{
				return (string)this[ElcContentSettingsSchema.ManagedFolderName];
			}
		}

		internal static object ELCMessageClassGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ElcContentSettingsSchema.MessageClassString];
			if (ElcMessageClass.IsMultiMessageClassDeputy(text))
			{
				StringBuilder stringBuilder = new StringBuilder(128);
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ElcContentSettingsSchema.MessageClassArray];
				multiValuedProperty.Sort();
				foreach (string value in multiValuedProperty)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(ElcMessageClass.MessageClassDelims[0]);
				}
				stringBuilder.Length--;
				text = stringBuilder.ToString();
			}
			return text;
		}

		internal static void ELCMessageClassSetter(object value, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (ElcMessageClass.IsMultiMessageClass(text))
			{
				string[] array = ((string)value).Split(ElcMessageClass.MessageClassDelims, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0)
				{
					text = string.Empty;
				}
				else if (array.Length == 1)
				{
					text = array[0];
				}
				else
				{
					MultiValuedProperty<string> value2 = new MultiValuedProperty<string>(array);
					propertyBag[ElcContentSettingsSchema.MessageClassArray] = value2;
					text = ElcMessageClass.MultiMessageClassDeputy;
				}
			}
			propertyBag[ElcContentSettingsSchema.MessageClassString] = text;
		}

		internal static object ELCFolderGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId != null)
			{
				return adobjectId.Parent;
			}
			return null;
		}

		internal static object ELCFolderNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)ElcContentSettings.ELCFolderGetter(propertyBag);
			if (adobjectId != null)
			{
				return adobjectId.Name;
			}
			return string.Empty;
		}

		internal static object MessageClassDisplayNameGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ElcContentSettingsSchema.MessageClass];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return ElcMessageClass.GetDisplayName(text);
		}

		internal static bool GetValueFromFlags(IPropertyBag propertyBag, ElcContentSettingFlags flag)
		{
			object obj = propertyBag[ElcContentSettingsSchema.ElcFlags];
			return flag == ((ElcContentSettingFlags)obj & flag);
		}

		internal static void SetFlags(IPropertyBag propertyBag, ElcContentSettingFlags flag, bool value)
		{
			ElcContentSettingFlags elcContentSettingFlags = (ElcContentSettingFlags)propertyBag[ElcContentSettingsSchema.ElcFlags];
			ElcContentSettingFlags elcContentSettingFlags2 = value ? (elcContentSettingFlags | flag) : (elcContentSettingFlags & ~flag);
			propertyBag[ElcContentSettingsSchema.ElcFlags] = elcContentSettingFlags2;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			this.ValidateMessageClass(errors);
			if (this.RetentionEnabled)
			{
				if (this.RetentionAction == RetentionActionType.MoveToFolder && this.MoveToDestinationFolder == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMoveToDestinationFolderNotDefined, ElcContentSettingsSchema.MoveToDestinationFolder, this));
				}
				if (this.AgeLimitForRetention == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorAgeLimitExpiration, ElcContentSettingsSchema.AgeLimitForRetention, this));
				}
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			this.ValidateMessageClass(errors);
			if (this.JournalingEnabled && this.AddressForJournaling == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorAddressAutoCopy, ElcContentSettingsSchema.AddressForJournaling, this));
			}
		}

		private void ValidateMessageClass(List<ValidationError> errors)
		{
			bool flag = string.IsNullOrEmpty(this.MessageClass);
			string[] array = null;
			if (!flag)
			{
				array = this.MessageClass.Split(ElcMessageClass.MessageClassDelims, StringSplitOptions.RemoveEmptyEntries);
				flag = (array == null || array.Length == 0);
			}
			if (flag)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMessageClassEmpty, ElcContentSettingsSchema.MessageClass, this));
				return;
			}
			foreach (string text in array)
			{
				int num = text.IndexOf('*');
				if (num != -1 && num < text.Length - 1)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMessageClassHasUnsupportedWildcard, ElcContentSettingsSchema.MessageClass, this));
					return;
				}
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.objectVersion;
			}
		}

		internal override void Initialize()
		{
			if (base.ExchangeVersion == RetentionPolicy.E14RetentionPolicyMajorVersion)
			{
				this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, RetentionPolicy.E14RetentionPolicyFullVersion);
			}
		}

		internal override QueryFilter VersioningFilter
		{
			get
			{
				ExchangeObjectVersion exchange = ExchangeObjectVersion.Exchange2007;
				ExchangeObjectVersion nextMajorVersion = ExchangeObjectVersion.Exchange2010.NextMajorVersion.NextMajorVersion;
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, exchange),
					new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.ExchangeVersion, nextMajorVersion)
				});
			}
		}

		internal bool AppliesToFolder
		{
			set
			{
				this.objectVersion = (value ? ExchangeObjectVersion.Exchange2007 : RetentionPolicy.E14RetentionPolicyFullVersion);
				this.propertyBag.SetField(ADObjectSchema.ExchangeVersion, this.objectVersion);
			}
		}

		private static ElcContentSettingsSchema schema = ObjectSchema.GetInstance<ElcContentSettingsSchema>();

		private static string mostDerivedClass = "msExchELCContentSettings";

		private ExchangeObjectVersion objectVersion = RetentionPolicy.E14RetentionPolicyFullVersion;
	}
}
