using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class OfflineAddressBook : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return OfflineAddressBook.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchOAB";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return OfflineAddressBook.ParentPathInternal;
			}
		}

		internal static object DiffRetentionPeriodGetter(IPropertyBag propertyBag)
		{
			Unlimited<int> unlimited = (Unlimited<int>)propertyBag[OfflineAddressBookSchema.RawDiffRetentionPeriod];
			if (unlimited.IsUnlimited)
			{
				ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)propertyBag[ADObjectSchema.ExchangeVersion];
				if (exchangeObjectVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
				{
					return null;
				}
			}
			return unlimited;
		}

		internal static void DiffRetentionPeriodSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OfflineAddressBookSchema.RawDiffRetentionPeriod] = (value ?? Unlimited<int>.UnlimitedValue);
		}

		internal static object VersionsGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<OfflineAddressBookVersion> multiValuedProperty = new MultiValuedProperty<OfflineAddressBookVersion>();
			object obj = propertyBag[OfflineAddressBookSchema.RawVersion];
			int num = (int)(obj ?? OfflineAddressBookSchema.RawVersion.DefaultValue);
			if ((1 & num) != 0)
			{
				multiValuedProperty.Add(OfflineAddressBookVersion.Version1);
			}
			num = ~num;
			if ((2 & num) != 0)
			{
				multiValuedProperty.Add(OfflineAddressBookVersion.Version2);
			}
			if ((4 & num) != 0)
			{
				multiValuedProperty.Add(OfflineAddressBookVersion.Version3);
			}
			if ((8 & num) != 0)
			{
				multiValuedProperty.Add(OfflineAddressBookVersion.Version4);
			}
			return multiValuedProperty;
		}

		internal static void VersionsSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<OfflineAddressBookVersion> multiValuedProperty = value as MultiValuedProperty<OfflineAddressBookVersion>;
			if (multiValuedProperty != null && 0 < multiValuedProperty.Count)
			{
				object obj = propertyBag[OfflineAddressBookSchema.RawVersion];
				int num = (int)(obj ?? OfflineAddressBookSchema.RawVersion.DefaultValue);
				num |= 14;
				num &= -2;
				foreach (OfflineAddressBookVersion offlineAddressBookVersion in multiValuedProperty)
				{
					if (offlineAddressBookVersion == OfflineAddressBookVersion.Version1)
					{
						num |= 1;
					}
					else
					{
						num &= (int)(~(int)offlineAddressBookVersion);
					}
				}
				propertyBag[OfflineAddressBookSchema.RawVersion] = num;
				return;
			}
			throw new OfflineAddressBookVersionsNullException();
		}

		internal static void ScheduleSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OfflineAddressBookSchema.ScheduleBitmaps] = value;
			if (value == null)
			{
				propertyBag[OfflineAddressBookSchema.ScheduleMode] = ScheduleMode.Never;
				return;
			}
			propertyBag[OfflineAddressBookSchema.ScheduleMode] = ((Schedule)value).Mode;
		}

		internal static object WebDistributionEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[OfflineAddressBookSchema.VirtualDirectories];
			object obj2 = propertyBag[OfflineAddressBookSchema.OabFlags];
			return ((MultiValuedProperty<ADObjectId>)obj).Count != 0 || ((int)obj2 & 2) != 0;
		}

		internal void ResolveConfiguredAttributes()
		{
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.ANRProperties];
			MultiValuedProperty<int> multiValuedProperty2 = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.DetailsProperties];
			MultiValuedProperty<int> multiValuedProperty3 = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.TruncatedProperties];
			MultiValuedProperty<OfflineAddressBookMapiProperty> multiValuedProperty4 = (MultiValuedProperty<OfflineAddressBookMapiProperty>)this.propertyBag[OfflineAddressBookSchema.ConfiguredAttributes];
			bool isReadOnly = multiValuedProperty4.IsReadOnly;
			if (isReadOnly)
			{
				multiValuedProperty4 = new MultiValuedProperty<OfflineAddressBookMapiProperty>();
			}
			foreach (int num in multiValuedProperty)
			{
				if (num != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num, OfflineAddressBookMapiPropertyOption.ANR);
					multiValuedProperty4.Add(oabmapiProperty);
				}
			}
			foreach (int num2 in multiValuedProperty2)
			{
				if (num2 != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num2, OfflineAddressBookMapiPropertyOption.Value);
					if (!multiValuedProperty4.Contains(oabmapiProperty))
					{
						multiValuedProperty4.Add(oabmapiProperty);
					}
				}
			}
			foreach (int num3 in multiValuedProperty3)
			{
				if (num3 != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num3, OfflineAddressBookMapiPropertyOption.Indicator);
					if (!multiValuedProperty4.Contains(oabmapiProperty))
					{
						multiValuedProperty4.Add(oabmapiProperty);
					}
				}
			}
			multiValuedProperty4.ResetChangeTracking();
			if (isReadOnly)
			{
				this.propertyBag.SetField(OfflineAddressBookSchema.ConfiguredAttributes, multiValuedProperty4);
			}
		}

		internal void UpdateRawMapiAttributes(bool movingToPreE14Server)
		{
			MultiValuedProperty<OfflineAddressBookMapiProperty> multiValuedProperty = (MultiValuedProperty<OfflineAddressBookMapiProperty>)this.propertyBag[OfflineAddressBookSchema.ConfiguredAttributes];
			MultiValuedProperty<OfflineAddressBookMapiProperty> multiValuedProperty2 = new MultiValuedProperty<OfflineAddressBookMapiProperty>();
			foreach (OfflineAddressBookMapiProperty offlineAddressBookMapiProperty in multiValuedProperty)
			{
				offlineAddressBookMapiProperty.ResolveMapiPropTag();
				if (multiValuedProperty2.Contains(offlineAddressBookMapiProperty))
				{
					throw new ArgumentException(DirectoryStrings.ErrorDuplicateMapiIdsInConfiguredAttributes);
				}
				multiValuedProperty2.Add(offlineAddressBookMapiProperty);
			}
			MultiValuedProperty<int> multiValuedProperty3 = new MultiValuedProperty<int>();
			MultiValuedProperty<int> multiValuedProperty4 = new MultiValuedProperty<int>();
			MultiValuedProperty<int> multiValuedProperty5 = new MultiValuedProperty<int>();
			foreach (OfflineAddressBookMapiProperty offlineAddressBookMapiProperty2 in multiValuedProperty2)
			{
				switch (offlineAddressBookMapiProperty2.Type)
				{
				case OfflineAddressBookMapiPropertyOption.ANR:
					multiValuedProperty3.Add((int)offlineAddressBookMapiProperty2.PropertyTag);
					break;
				case OfflineAddressBookMapiPropertyOption.Value:
					multiValuedProperty4.Add((int)offlineAddressBookMapiProperty2.PropertyTag);
					break;
				case OfflineAddressBookMapiPropertyOption.Indicator:
					multiValuedProperty5.Add((int)offlineAddressBookMapiProperty2.PropertyTag);
					break;
				}
			}
			if (multiValuedProperty3.Count == 0 && !movingToPreE14Server)
			{
				multiValuedProperty3.Add(0);
			}
			if (multiValuedProperty4.Count == 0 && !movingToPreE14Server)
			{
				multiValuedProperty4.Add(0);
			}
			if (multiValuedProperty5.Count == 0 && !movingToPreE14Server)
			{
				multiValuedProperty5.Add(0);
			}
			this.propertyBag[OfflineAddressBookSchema.ANRProperties] = multiValuedProperty3;
			this.propertyBag[OfflineAddressBookSchema.DetailsProperties] = multiValuedProperty4;
			this.propertyBag[OfflineAddressBookSchema.TruncatedProperties] = multiValuedProperty5;
			if (multiValuedProperty.IsReadOnly)
			{
				this.propertyBag.SetField(OfflineAddressBookSchema.ConfiguredAttributes, new MultiValuedProperty<OfflineAddressBookMapiProperty>());
				return;
			}
			multiValuedProperty.Clear();
			multiValuedProperty.ResetChangeTracking();
		}

		public ADObjectId GeneratingMailbox
		{
			get
			{
				return (ADObjectId)this[OfflineAddressBookSchema.GeneratingMailbox];
			}
			set
			{
				this[OfflineAddressBookSchema.GeneratingMailbox] = value;
			}
		}

		internal ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[OfflineAddressBookSchema.Server];
			}
			set
			{
				this[OfflineAddressBookSchema.Server] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AddressLists
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OfflineAddressBookSchema.AddressLists];
			}
			set
			{
				this[OfflineAddressBookSchema.AddressLists] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<OfflineAddressBookVersion> Versions
		{
			get
			{
				return (MultiValuedProperty<OfflineAddressBookVersion>)this[OfflineAddressBookSchema.Versions];
			}
			set
			{
				this[OfflineAddressBookSchema.Versions] = value;
			}
		}

		[Parameter]
		public bool IsDefault
		{
			get
			{
				return (bool)this[OfflineAddressBookSchema.IsDefault];
			}
			set
			{
				this[OfflineAddressBookSchema.IsDefault] = value;
			}
		}

		public ADObjectId PublicFolderDatabase
		{
			get
			{
				return (ADObjectId)this[OfflineAddressBookSchema.PublicFolderDatabase];
			}
			internal set
			{
				this[OfflineAddressBookSchema.PublicFolderDatabase] = value;
			}
		}

		[Parameter]
		public bool PublicFolderDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookSchema.PublicFolderDistributionEnabled];
			}
			set
			{
				this[OfflineAddressBookSchema.PublicFolderDistributionEnabled] = value;
			}
		}

		[Parameter]
		public bool GlobalWebDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookSchema.GlobalWebDistributionEnabled];
			}
			set
			{
				this[OfflineAddressBookSchema.GlobalWebDistributionEnabled] = value;
			}
		}

		public bool WebDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookSchema.WebDistributionEnabled];
			}
		}

		[Parameter]
		public bool ShadowMailboxDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookSchema.ShadowMailboxDistributionEnabled];
			}
			set
			{
				this[OfflineAddressBookSchema.ShadowMailboxDistributionEnabled] = value;
			}
		}

		public DateTime? LastTouchedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookSchema.LastTouchedTime];
			}
			internal set
			{
				this[OfflineAddressBookSchema.LastTouchedTime] = value;
			}
		}

		public DateTime? LastRequestedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookSchema.LastRequestedTime];
			}
			internal set
			{
				this[OfflineAddressBookSchema.LastRequestedTime] = value;
			}
		}

		public DateTime? LastFailedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookSchema.LastFailedTime];
			}
			internal set
			{
				this[OfflineAddressBookSchema.LastFailedTime] = value;
			}
		}

		public int? LastNumberOfRecords
		{
			get
			{
				return (int?)this[OfflineAddressBookSchema.LastNumberOfRecords];
			}
			internal set
			{
				this[OfflineAddressBookSchema.LastNumberOfRecords] = value;
			}
		}

		public OfflineAddressBookLastGeneratingData LastGeneratingData
		{
			get
			{
				return (OfflineAddressBookLastGeneratingData)this[OfflineAddressBookSchema.LastGeneratingData];
			}
			internal set
			{
				this[OfflineAddressBookSchema.LastGeneratingData] = value;
			}
		}

		[Parameter]
		public int MaxBinaryPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookSchema.MaxBinaryPropertySize];
			}
			set
			{
				this[OfflineAddressBookSchema.MaxBinaryPropertySize] = value;
			}
		}

		[Parameter]
		public int MaxMultivaluedBinaryPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookSchema.MaxMultivaluedBinaryPropertySize];
			}
			set
			{
				this[OfflineAddressBookSchema.MaxMultivaluedBinaryPropertySize] = value;
			}
		}

		[Parameter]
		public int MaxStringPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookSchema.MaxStringPropertySize];
			}
			set
			{
				this[OfflineAddressBookSchema.MaxStringPropertySize] = value;
			}
		}

		[Parameter]
		public int MaxMultivaluedStringPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookSchema.MaxMultivaluedStringPropertySize];
			}
			set
			{
				this[OfflineAddressBookSchema.MaxMultivaluedStringPropertySize] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<OfflineAddressBookMapiProperty> ConfiguredAttributes
		{
			get
			{
				return (MultiValuedProperty<OfflineAddressBookMapiProperty>)this[OfflineAddressBookSchema.ConfiguredAttributes];
			}
			set
			{
				this[OfflineAddressBookSchema.ConfiguredAttributes] = value;
			}
		}

		[Parameter]
		public Unlimited<int>? DiffRetentionPeriod
		{
			get
			{
				return (Unlimited<int>?)this[OfflineAddressBookSchema.DiffRetentionPeriod];
			}
			set
			{
				this[OfflineAddressBookSchema.DiffRetentionPeriod] = value;
			}
		}

		[Parameter]
		public Schedule Schedule
		{
			get
			{
				return (Schedule)this[OfflineAddressBookSchema.Schedule];
			}
			set
			{
				this[OfflineAddressBookSchema.Schedule] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> VirtualDirectories
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OfflineAddressBookSchema.VirtualDirectories];
			}
			set
			{
				this[OfflineAddressBookSchema.VirtualDirectories] = value;
			}
		}

		public new ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[ADObjectSchema.ExchangeVersion];
			}
			internal set
			{
				base.SetExchangeVersion(value);
			}
		}

		internal OfflineAddressBookManifestVersion ManifestVersion
		{
			get
			{
				return (OfflineAddressBookManifestVersion)this[OfflineAddressBookSchema.ManifestVersion];
			}
			set
			{
				this[OfflineAddressBookSchema.ManifestVersion] = value;
			}
		}

		internal bool IsE15OrLater()
		{
			return ExchangeObjectVersion.Exchange2012.CompareTo(this.ExchangeVersion) <= 0;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		internal override SystemFlagsEnum SystemFlags
		{
			get
			{
				return (SystemFlagsEnum)this[OfflineAddressBookSchema.SystemFlags];
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(OfflineAddressBookSchema.OfflineAddressBookFolder))
			{
				PropertyDefinition offlineAddressBookFolder = OfflineAddressBookSchema.OfflineAddressBookFolder;
				byte[] value = new byte[1];
				this[offlineAddressBookFolder] = value;
			}
			if (!base.IsModified(OfflineAddressBookSchema.SiteFolderGuid))
			{
				this[OfflineAddressBookSchema.SiteFolderGuid] = Guid.NewGuid().ToByteArray();
			}
			if (!base.IsModified(OfflineAddressBookSchema.Schedule))
			{
				this.Schedule = Schedule.Daily5AM;
			}
			if (!base.IsModified(OfflineAddressBookSchema.DiffRetentionPeriod))
			{
				this.DiffRetentionPeriod = new Unlimited<int>?(OfflineAddressBook.DefaultDiffRetentionPeriod);
			}
			base.StampPersistableDefaultValues();
		}

		private bool DuplicateConfiguredAttributesDefined()
		{
			if (this.ConfiguredAttributes.Count > 0)
			{
				return false;
			}
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.ANRProperties];
			MultiValuedProperty<int> multiValuedProperty2 = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.DetailsProperties];
			MultiValuedProperty<int> multiValuedProperty3 = (MultiValuedProperty<int>)this.propertyBag[OfflineAddressBookSchema.TruncatedProperties];
			Dictionary<int, OfflineAddressBookMapiProperty> dictionary = new Dictionary<int, OfflineAddressBookMapiProperty>();
			foreach (int num in multiValuedProperty)
			{
				if (num != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num, OfflineAddressBookMapiPropertyOption.ANR);
					if (dictionary.ContainsKey(oabmapiProperty.MapiID))
					{
						return true;
					}
					dictionary.Add(oabmapiProperty.MapiID, oabmapiProperty);
				}
			}
			foreach (int num2 in multiValuedProperty2)
			{
				if (num2 != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num2, OfflineAddressBookMapiPropertyOption.Value);
					if (dictionary.ContainsKey(oabmapiProperty.MapiID))
					{
						return true;
					}
					dictionary.Add(oabmapiProperty.MapiID, oabmapiProperty);
				}
			}
			foreach (int num3 in multiValuedProperty3)
			{
				if (num3 != 0)
				{
					OfflineAddressBookMapiProperty oabmapiProperty = OfflineAddressBookMapiProperty.GetOABMapiProperty((uint)num3, OfflineAddressBookMapiPropertyOption.Indicator);
					if (dictionary.ContainsKey(oabmapiProperty.MapiID))
					{
						return true;
					}
					dictionary.Add(oabmapiProperty.MapiID, oabmapiProperty);
				}
			}
			return false;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (this.DuplicateConfiguredAttributesDefined())
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorDuplicateMapiIdsInConfiguredAttributes, this.Identity, string.Empty));
			}
			if (this.GlobalWebDistributionEnabled && this.VirtualDirectories != null && this.VirtualDirectories.Count > 0)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorGlobalWebDistributionAndVDirsSet(this.Identity.ToString()), this.Identity, string.Empty));
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!this.PublicFolderDistributionEnabled && (this.Versions.Contains(OfflineAddressBookVersion.Version1) || this.Versions.Contains(OfflineAddressBookVersion.Version2) || this.Versions.Contains(OfflineAddressBookVersion.Version3)))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorLegacyVersionOfflineAddressBookWithoutPublicFolderDatabase(this.Identity.ToString()), this.Identity, string.Empty));
			}
			if (this.WebDistributionEnabled && !this.Versions.Contains(OfflineAddressBookVersion.Version4))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorWebDistributionEnabledWithoutVersion4(this.Identity.ToString()), this.Identity, string.Empty));
			}
		}

		internal bool CheckForAssociatedAddressBookPolicies()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(OfflineAddressBookSchema.AssociatedAddressBookPolicies)
			});
			if (base.Session != null)
			{
				OfflineAddressBook[] array = base.Session.Find<OfflineAddressBook>(null, QueryScope.SubTree, filter, null, 1);
				return array != null && array.Length > 0;
			}
			return true;
		}

		public const string MostDerivedClass = "msExchOAB";

		private static OfflineAddressBookSchema schema = ObjectSchema.GetInstance<OfflineAddressBookSchema>();

		internal static readonly ADObjectId RdnContainer = new ADObjectId("CN=Offline Address Lists,CN=Address Lists Container");

		public static readonly ADObjectId ParentPathInternal = new ADObjectId("CN=Offline Address Lists,CN=Address Lists Container");

		public static readonly string DefaultName = DirectoryStrings.DefaultOabName;

		internal static readonly int DefaultDiffRetentionPeriod = 30;
	}
}
