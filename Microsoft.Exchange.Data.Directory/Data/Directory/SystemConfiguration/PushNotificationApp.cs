using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class PushNotificationApp : ADConfigurationObject, IPushNotificationRawSettings, IEquatable<IPushNotificationRawSettings>, IEquatable<PushNotificationApp>
	{
		internal override ADObjectId ParentPath
		{
			get
			{
				return PushNotificationApp.RdnContainer;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return PushNotificationApp.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchPushNotificationsApp";
			}
		}

		public DateTime? LastUpdateTimeUtc
		{
			get
			{
				return (DateTime?)this[PushNotificationAppSchema.LastUpdateTimeUtc];
			}
			set
			{
				this[PushNotificationAppSchema.LastUpdateTimeUtc] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter]
		public string DisplayName
		{
			get
			{
				return (string)this[PushNotificationAppSchema.DisplayName];
			}
			set
			{
				this[PushNotificationAppSchema.DisplayName] = value;
			}
		}

		public PushNotificationPlatform Platform
		{
			get
			{
				return (PushNotificationPlatform)this[PushNotificationAppSchema.Platform];
			}
			set
			{
				this[PushNotificationAppSchema.Platform] = value;
			}
		}

		[Parameter]
		public bool? Enabled
		{
			get
			{
				return (bool?)this[PushNotificationAppSchema.Enabled];
			}
			set
			{
				this[PushNotificationAppSchema.Enabled] = value;
			}
		}

		[Parameter]
		public Version ExchangeMinimumVersion
		{
			get
			{
				return (Version)this[PushNotificationAppSchema.ExchangeMinimumVersion];
			}
			set
			{
				this[PushNotificationAppSchema.ExchangeMinimumVersion] = value;
			}
		}

		[Parameter]
		public Version ExchangeMaximumVersion
		{
			get
			{
				return (Version)this[PushNotificationAppSchema.ExchangeMaximumVersion];
			}
			set
			{
				this[PushNotificationAppSchema.ExchangeMaximumVersion] = value;
			}
		}

		[Parameter]
		public int? QueueSize
		{
			get
			{
				return (int?)this[PushNotificationAppSchema.QueueSize];
			}
			set
			{
				this[PushNotificationAppSchema.QueueSize] = value;
			}
		}

		[Parameter]
		public int? NumberOfChannels
		{
			get
			{
				return (int?)this[PushNotificationAppSchema.NumberOfChannels];
			}
			set
			{
				this[PushNotificationAppSchema.NumberOfChannels] = value;
			}
		}

		public string AuthenticationId
		{
			get
			{
				return (string)this[PushNotificationAppSchema.AuthenticationId];
			}
			set
			{
				this[PushNotificationAppSchema.AuthenticationId] = value;
			}
		}

		public string AuthenticationKey
		{
			get
			{
				return (string)this[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				this[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		public string AuthenticationKeyFallback
		{
			get
			{
				return (string)this[PushNotificationAppSchema.AuthenticationKeyFallback];
			}
			set
			{
				this[PushNotificationAppSchema.AuthenticationKeyFallback] = value;
			}
		}

		public bool? IsAuthenticationKeyEncrypted
		{
			get
			{
				return (bool?)this[PushNotificationAppSchema.IsAuthenticationKeyEncrypted];
			}
			set
			{
				this[PushNotificationAppSchema.IsAuthenticationKeyEncrypted] = value;
			}
		}

		public string Url
		{
			get
			{
				return (string)this[PushNotificationAppSchema.Url];
			}
			set
			{
				this[PushNotificationAppSchema.Url] = value;
			}
		}

		public string UriTemplate
		{
			get
			{
				return (string)this[PushNotificationAppSchema.UriTemplate];
			}
			set
			{
				this[PushNotificationAppSchema.UriTemplate] = value;
			}
		}

		public int? Port
		{
			get
			{
				return (int?)this[PushNotificationAppSchema.Port];
			}
			set
			{
				this[PushNotificationAppSchema.Port] = value;
			}
		}

		public string SecondaryUrl
		{
			get
			{
				return (string)this[PushNotificationAppSchema.SecondaryUrl];
			}
			set
			{
				this[PushNotificationAppSchema.SecondaryUrl] = value;
			}
		}

		public int? SecondaryPort
		{
			get
			{
				return (int?)this[PushNotificationAppSchema.SecondaryPort];
			}
			set
			{
				this[PushNotificationAppSchema.SecondaryPort] = value;
			}
		}

		[Parameter]
		public int? BackOffTimeInSeconds
		{
			get
			{
				return (int?)this[PushNotificationAppSchema.BackOffTimeInSeconds];
			}
			set
			{
				this[PushNotificationAppSchema.BackOffTimeInSeconds] = value;
			}
		}

		public int? AddTimeout
		{
			get
			{
				return null;
			}
		}

		public int? ConnectStepTimeout
		{
			get
			{
				return null;
			}
		}

		public int? ConnectTotalTimeout
		{
			get
			{
				return null;
			}
		}

		public int? ConnectRetryDelay
		{
			get
			{
				return null;
			}
		}

		public int? AuthenticateRetryMax
		{
			get
			{
				return null;
			}
		}

		public int? ConnectRetryMax
		{
			get
			{
				return null;
			}
		}

		public int? ReadTimeout
		{
			get
			{
				return null;
			}
		}

		public int? WriteTimeout
		{
			get
			{
				return null;
			}
		}

		public bool? IgnoreCertificateErrors
		{
			get
			{
				return null;
			}
		}

		public int? MaximumCacheSize
		{
			get
			{
				return null;
			}
		}

		public string RegistrationTemplate
		{
			get
			{
				return (string)this[PushNotificationAppSchema.RegistrationTemplate];
			}
			set
			{
				this[PushNotificationAppSchema.RegistrationTemplate] = value;
			}
		}

		public bool? RegistrationEnabled
		{
			get
			{
				return (bool?)this[PushNotificationAppSchema.RegistrationEnabled];
			}
			set
			{
				this[PushNotificationAppSchema.RegistrationEnabled] = value;
			}
		}

		public bool? MultifactorRegistrationEnabled
		{
			get
			{
				return (bool?)this[PushNotificationAppSchema.MultifactorRegistrationEnabled];
			}
			set
			{
				this[PushNotificationAppSchema.MultifactorRegistrationEnabled] = value;
			}
		}

		public string PartitionName
		{
			get
			{
				return (string)this[PushNotificationAppSchema.PartitionName];
			}
			set
			{
				this[PushNotificationAppSchema.PartitionName] = value;
			}
		}

		public bool? IsDefaultPartitionName
		{
			get
			{
				return (bool?)this[PushNotificationAppSchema.IsDefaultPartitionName];
			}
			set
			{
				this[PushNotificationAppSchema.IsDefaultPartitionName] = value;
			}
		}

		public string ToFullString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ADPropertyDefinition adpropertyDefinition in PushNotificationApp.ComparableProperties.Value)
			{
				object obj = null;
				this.propertyBag.TryGetField(adpropertyDefinition, ref obj);
				string text = null;
				if (obj != null)
				{
					if (!adpropertyDefinition.IsMultivalued)
					{
						text = ADValueConvertor.ConvertValueToString(obj, adpropertyDefinition.FormatProvider);
					}
					else
					{
						MultiValuedPropertyBase multiValuedPropertyBase = obj as MultiValuedPropertyBase;
						StringBuilder stringBuilder2 = new StringBuilder();
						foreach (object originalValue in ((IEnumerable)multiValuedPropertyBase))
						{
							if (stringBuilder2.Length > 0)
							{
								stringBuilder2.Append(", ");
							}
							stringBuilder2.AppendFormat("{0}", ADValueConvertor.ConvertValueToString(originalValue, adpropertyDefinition.FormatProvider));
						}
						text = string.Format("@({0})", stringBuilder2.ToString());
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("; ");
				}
				stringBuilder.AppendFormat("{0}:{1}", adpropertyDefinition.Name, (text != null) ? text : "<null>");
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj is PushNotificationApp && this.Equals((PushNotificationApp)obj);
		}

		public bool Equals(IPushNotificationRawSettings other)
		{
			return other is PushNotificationApp && this.Equals((PushNotificationApp)other);
		}

		public bool Equals(PushNotificationApp other)
		{
			return DeepADObjectEqualityComparer.Instance.Equals(this, other, PushNotificationApp.ComparableProperties.Value);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.Platform == PushNotificationPlatform.None)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorInvalidPushNotificationPlatform, PushNotificationAppSchema.Platform, this.Platform));
			}
		}

		private static IEnumerable<ADPropertyDefinition> CreateComparableProperties()
		{
			List<ADPropertyDefinition> list = new List<ADPropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in PushNotificationApp.schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.LdapDisplayName != null && !adpropertyDefinition.IsCalculated && !PushNotificationApp.ExcludedProperties.Contains(adpropertyDefinition.LdapDisplayName, StringComparer.Ordinal))
				{
					list.Add(adpropertyDefinition);
				}
			}
			return list;
		}

		private const string mostDerivedClass = "msExchPushNotificationsApp";

		public const string PushNotificationAppContainerName = "Push Notifications Settings";

		private static readonly string[] ExcludedProperties = new string[]
		{
			ADObjectSchema.Id.LdapDisplayName,
			ADObjectSchema.WhenChangedRaw.LdapDisplayName,
			ADObjectSchema.WhenCreatedRaw.LdapDisplayName
		};

		private static PushNotificationAppSchema schema = ObjectSchema.GetInstance<PushNotificationAppSchema>();

		private static Lazy<IEnumerable<ADPropertyDefinition>> ComparableProperties = new Lazy<IEnumerable<ADPropertyDefinition>>(() => PushNotificationApp.CreateComparableProperties());

		internal static readonly ADObjectId RdnContainer = new ADObjectId(string.Format("CN={0}", "Push Notifications Settings"));
	}
}
