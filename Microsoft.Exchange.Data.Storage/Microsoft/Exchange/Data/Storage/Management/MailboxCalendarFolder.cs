using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class MailboxCalendarFolder : ConfigurableObject
	{
		internal static object PublishedUrlGetter(IPropertyBag propertyBag, SimpleProviderPropertyDefinition propertyDefinition)
		{
			string text = (string)propertyBag[propertyDefinition];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			PublishingUrl publishingUrl = PublishingUrl.Create(text);
			ObscureUrl obscureUrl = publishingUrl as ObscureUrl;
			if (obscureUrl == null)
			{
				return text;
			}
			return obscureUrl.ChangeToKind(ObscureKind.Normal).ToString();
		}

		internal static void PublishedUrlSetter(object value, IPropertyBag propertyBag, SimpleProviderPropertyDefinition propertyDefinition)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				propertyBag[propertyDefinition] = null;
				return;
			}
			PublishingUrl publishingUrl = PublishingUrl.Create(text);
			ObscureUrl obscureUrl = publishingUrl as ObscureUrl;
			if (obscureUrl == null)
			{
				propertyBag[propertyDefinition] = text;
				return;
			}
			propertyBag[propertyDefinition] = obscureUrl.ChangeToKind(ObscureKind.Restricted).ToString();
		}

		private T EnumGetter<T>(SimpleProviderPropertyDefinition propertyDefinition)
		{
			if (!Enum.IsDefined(typeof(T), this[propertyDefinition]))
			{
				return (T)((object)propertyDefinition.DefaultValue);
			}
			return (T)((object)this[propertyDefinition]);
		}

		private void EnumSetter<T>(T value, SimpleProviderPropertyDefinition propertyDefinition)
		{
			if (!Enum.IsDefined(typeof(T), value))
			{
				throw new ArgumentOutOfRangeException();
			}
			this[propertyDefinition] = value;
		}

		public MailboxCalendarFolder() : base(new SimplePropertyBag(MailboxCalendarFolderSchema.MailboxFolderId, UserConfigurationObjectSchema.ObjectState, UserConfigurationObjectSchema.ExchangeVersion))
		{
		}

		internal sealed override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxCalendarFolder.schema;
			}
		}

		public sealed override ObjectId Identity
		{
			get
			{
				return this.MailboxFolderId;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal MailboxFolderId MailboxFolderId
		{
			get
			{
				return (MailboxFolderId)this[MailboxCalendarFolderSchema.MailboxFolderId];
			}
			set
			{
				this[MailboxCalendarFolderSchema.MailboxFolderId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublishEnabled
		{
			get
			{
				return (bool)this[MailboxCalendarFolderSchema.PublishEnabled];
			}
			set
			{
				this[MailboxCalendarFolderSchema.PublishEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateRangeEnumType PublishDateRangeFrom
		{
			get
			{
				return this.EnumGetter<DateRangeEnumType>(MailboxCalendarFolderSchema.PublishDateRangeFrom);
			}
			set
			{
				this.EnumSetter<DateRangeEnumType>(value, MailboxCalendarFolderSchema.PublishDateRangeFrom);
			}
		}

		[Parameter(Mandatory = false)]
		public DateRangeEnumType PublishDateRangeTo
		{
			get
			{
				return this.EnumGetter<DateRangeEnumType>(MailboxCalendarFolderSchema.PublishDateRangeTo);
			}
			set
			{
				this.EnumSetter<DateRangeEnumType>(value, MailboxCalendarFolderSchema.PublishDateRangeTo);
			}
		}

		[Parameter(Mandatory = false)]
		public DetailLevelEnumType DetailLevel
		{
			get
			{
				return this.EnumGetter<DetailLevelEnumType>(MailboxCalendarFolderSchema.DetailLevel);
			}
			set
			{
				this.EnumSetter<DetailLevelEnumType>(value, MailboxCalendarFolderSchema.DetailLevel);
			}
		}

		[Parameter(Mandatory = false)]
		public bool SearchableUrlEnabled
		{
			get
			{
				return (bool)this[MailboxCalendarFolderSchema.SearchableUrlEnabled];
			}
			set
			{
				this[MailboxCalendarFolderSchema.SearchableUrlEnabled] = value;
			}
		}

		public string PublishedCalendarUrl
		{
			get
			{
				return (string)this[MailboxCalendarFolderSchema.PublishedCalendarUrlCalculated];
			}
			internal set
			{
				this[MailboxCalendarFolderSchema.PublishedCalendarUrlCalculated] = value;
			}
		}

		public string PublishedICalUrl
		{
			get
			{
				return (string)this[MailboxCalendarFolderSchema.PublishedICalUrlCalculated];
			}
			internal set
			{
				this[MailboxCalendarFolderSchema.PublishedICalUrlCalculated] = value;
			}
		}

		internal string PublishedCalendarUrlRaw
		{
			get
			{
				return (string)this[MailboxCalendarFolderSchema.PublishedCalendarUrl];
			}
		}

		internal string PublishedICalUrlRaw
		{
			get
			{
				return (string)this[MailboxCalendarFolderSchema.PublishedICalUrl];
			}
		}

		private static readonly MailboxCalendarFolderSchema schema = ObjectSchema.GetInstance<MailboxCalendarFolderSchema>();

		internal static readonly SimpleProviderPropertyDefinition[] CalendarFolderConfigurationProperties = new SimpleProviderPropertyDefinition[]
		{
			MailboxCalendarFolderSchema.PublishEnabled,
			MailboxCalendarFolderSchema.PublishDateRangeFrom,
			MailboxCalendarFolderSchema.PublishDateRangeTo,
			MailboxCalendarFolderSchema.DetailLevel,
			MailboxCalendarFolderSchema.SearchableUrlEnabled,
			MailboxCalendarFolderSchema.PublishedCalendarUrl,
			MailboxCalendarFolderSchema.PublishedICalUrl
		};
	}
}
