using System;
using System.Globalization;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	internal class SyncPropertyDefinition : ADPropertyDefinition
	{
		public new SyncPropertyDefinitionFlags Flags
		{
			get
			{
				return (SyncPropertyDefinitionFlags)base.Flags;
			}
		}

		public string MsoPropertyName { get; private set; }

		public ServerVersion SyncPropertyVersionAdded { get; private set; }

		public Func<bool> ShouldProcess { get; set; }

		internal SyncPropertyDefinition(ADPropertyDefinition aDPropertyDefinition, string msoPropertyName, Type externalType, SyncPropertyDefinitionFlags flags, ServerVersion versionAdded) : this(aDPropertyDefinition, msoPropertyName, aDPropertyDefinition.Type, externalType, flags, versionAdded)
		{
		}

		public SyncPropertyDefinition(ADPropertyDefinition aDPropertyDefinition, string msoPropertyName, Type type, Type externalType, SyncPropertyDefinitionFlags flags, ServerVersion versionAdded) : this(aDPropertyDefinition.Name, msoPropertyName, aDPropertyDefinition.VersionAdded, type, externalType, aDPropertyDefinition.LdapDisplayName, aDPropertyDefinition.Flags, flags, versionAdded, aDPropertyDefinition.DefaultValue, SyncPropertyDefinition.ConvertReadOnlyCollectionToArray<ProviderPropertyDefinition>(aDPropertyDefinition.SupportingProperties), aDPropertyDefinition.CustomFilterBuilderDelegate, aDPropertyDefinition.GetterDelegate, aDPropertyDefinition.SetterDelegate, aDPropertyDefinition.ShadowProperty)
		{
		}

		public SyncPropertyDefinition(string name, string msoPropertyName, Type type, Type externalType, SyncPropertyDefinitionFlags flags, ServerVersion versionAdded, object defaultValue) : this(name, msoPropertyName, type, externalType, flags, versionAdded, defaultValue, ProviderPropertyDefinition.None, null, null)
		{
		}

		public SyncPropertyDefinition(string name, string msoPropertyName, Type type, Type externalType, SyncPropertyDefinitionFlags flags, ServerVersion versionAdded, object defaultValue, ProviderPropertyDefinition[] supportingProperties, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : this(name, msoPropertyName, ExchangeObjectVersion.Exchange2010, type, externalType, null, ADPropertyDefinitionFlags.None, flags, versionAdded, defaultValue, supportingProperties, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), getterDelegate, setterDelegate, null)
		{
		}

		private SyncPropertyDefinition(string name, string msoPropertyName, ExchangeObjectVersion versionAdded, Type type, Type externalType, string ldapDisplayName, ADPropertyDefinitionFlags flags, SyncPropertyDefinitionFlags syncFlags, ServerVersion syncVersionAdded, object defaultValue, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate, ADPropertyDefinition shadowProperty) : base(name, versionAdded, type, ldapDisplayName, SyncPropertyDefinition.CalculateFlags(ldapDisplayName, (ADPropertyDefinitionFlags)(syncFlags | (SyncPropertyDefinitionFlags)flags)), defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, supportingProperties, customFilterBuilderDelegate, getterDelegate, setterDelegate, null, null)
		{
			this.externalType = externalType;
			this.MsoPropertyName = msoPropertyName;
			this.SyncPropertyVersionAdded = syncVersionAdded;
			SyncPropertyDefinitionFlags syncPropertyDefinitionFlags = syncFlags & ~(SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.BackSync);
			syncPropertyDefinitionFlags |= SyncPropertyDefinitionFlags.Shadow;
			if (shadowProperty != null)
			{
				Type type2 = shadowProperty.Type;
				if (type2 == typeof(ADObjectId))
				{
					type2 = typeof(SyncLink);
				}
				this.shadowProperty = new SyncPropertyDefinition(shadowProperty, msoPropertyName, type2, externalType, syncPropertyDefinitionFlags, syncVersionAdded);
				return;
			}
			if (this.IsBackSync && base.SupportingProperties.Count == 1 && ((ADPropertyDefinition)base.SupportingProperties[0]).ShadowProperty != null)
			{
				this.shadowProperty = new SyncPropertyDefinition(string.Format(CultureInfo.InvariantCulture, "Shadow{0}", new object[]
				{
					base.Name
				}), msoPropertyName, base.Type, this.ExternalType, syncPropertyDefinitionFlags, syncVersionAdded, base.DefaultValue, new ProviderPropertyDefinition[]
				{
					((ADPropertyDefinition)base.SupportingProperties[0]).ShadowProperty
				}, base.GetterDelegate, base.SetterDelegate);
			}
		}

		private static T[] ConvertReadOnlyCollectionToArray<T>(ReadOnlyCollection<T> collection)
		{
			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		private static ADPropertyDefinitionFlags CalculateFlags(string ldapDisplayName, ADPropertyDefinitionFlags flags)
		{
			if ((flags & ADPropertyDefinitionFlags.Calculated) == ADPropertyDefinitionFlags.None && string.IsNullOrEmpty(ldapDisplayName))
			{
				return flags | ADPropertyDefinitionFlags.TaskPopulated;
			}
			return flags;
		}

		public bool IsForwardSync
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.ForwardSync) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsBackSync
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.BackSync) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsCloud
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.Cloud) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsAlwaysReturned
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.AlwaysReturned) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsNotInMsoDirectory
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.NotInMsoDirectory) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsShadow
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.Shadow) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsFilteringOnly
		{
			get
			{
				return (this.Flags & SyncPropertyDefinitionFlags.FilteringOnly) != (SyncPropertyDefinitionFlags)0;
			}
		}

		public bool IsSyncLink
		{
			get
			{
				return base.Type == typeof(SyncLink);
			}
		}

		public Type ExternalType
		{
			get
			{
				return this.externalType;
			}
		}

		private Type externalType;

		public static ServerVersion InitialSyncPropertySetVersion = new ServerVersion(14, 15, 0, 0);

		public static ServerVersion SyncPropertySetVersion3 = new ServerVersion(15, 0, 377, 0);

		public static ServerVersion SyncPropertySetVersion4 = new ServerVersion(15, 0, 414, 0);

		public static ServerVersion SyncPropertySetVersion6 = new ServerVersion(15, 0, 510, 0);

		public static ServerVersion SyncPropertySetVersion8 = new ServerVersion(15, 0, 548, 0);

		public static ServerVersion SyncPropertySetVersion9 = new ServerVersion(15, 0, 562, 0);

		public static ServerVersion SyncPropertySetVersion10 = new ServerVersion(15, 0, 564, 0);

		public static ServerVersion SyncPropertySetVersion11 = new ServerVersion(15, 0, 565, 0);

		public static ServerVersion SyncPropertySetVersion12 = new ServerVersion(15, 0, 810, 0);

		public static ServerVersion SyncPropertySetVersion13 = new ServerVersion(15, 0, 827, 0);

		public static ServerVersion SyncPropertySetVersion14 = new ServerVersion(15, 0, 842, 0);

		public static ServerVersion SyncPropertySetVersion15 = new ServerVersion(15, 0, 885, 0);

		public static ServerVersion SyncPropertySetVersion16 = new ServerVersion(15, 0, 907, 0);

		public static ServerVersion SyncPropertySetVersion17 = new ServerVersion(15, 0, 946, 0);

		public static ServerVersion SyncPropertySetVersion18 = new ServerVersion(15, 0, 976, 0);

		public static ServerVersion SyncPropertySetVersion19 = new ServerVersion(15, 0, 1000, 0);

		public static ServerVersion IgnoredSyncPropertySetVersion = new ServerVersion(1, 1, 1, 1);
	}
}
