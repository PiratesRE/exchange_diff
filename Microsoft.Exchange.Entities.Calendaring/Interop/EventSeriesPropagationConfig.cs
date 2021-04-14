using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	public class EventSeriesPropagationConfig : RegistryObject
	{
		public EventSeriesPropagationConfig()
		{
		}

		public EventSeriesPropagationConfig(RegistryObjectId identity) : base(identity)
		{
		}

		public TimeSpan MaxActionLifetime
		{
			get
			{
				return (TimeSpan)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.MaxActionLifetime];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.MaxActionLifetime] = value;
			}
		}

		public uint MaxCollisionRetryCount
		{
			get
			{
				return (uint)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.MaxCollisionRetryCount];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.MaxCollisionRetryCount] = value;
			}
		}

		public uint PropagationCountLimit
		{
			get
			{
				return (uint)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.PropagationCountLimit];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.PropagationCountLimit] = value;
			}
		}

		public TimeSpan PropagationTimeLimit
		{
			get
			{
				return (TimeSpan)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.PropagationTimeLimit];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.PropagationTimeLimit] = value;
			}
		}

		public bool ReversePropagationOrder
		{
			get
			{
				return (bool)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.ReversePropagationOrder];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.ReversePropagationOrder] = value;
			}
		}

		public bool ShouldCleanup
		{
			get
			{
				return (bool)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.ShouldCleanup];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.ShouldCleanup] = value;
			}
		}

		public bool IgnorePropagationErrors
		{
			get
			{
				return (bool)this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.IgnorePropagationErrors];
			}
			set
			{
				this[EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema.IgnorePropagationErrors] = value;
			}
		}

		internal override RegistryObjectSchema RegistrySchema
		{
			get
			{
				return EventSeriesPropagationConfig.Schema;
			}
		}

		public static EventSeriesPropagationConfig GetBackgroundPropagationConfig()
		{
			return EventSeriesPropagationConfig.GetConfig("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop\\BackgroundEventSeriesPropagation", EventSeriesPropagationConfig.DefaultBackgroundPropagationConfig);
		}

		public static EventSeriesPropagationConfig GetInlinePropagationConfig()
		{
			return EventSeriesPropagationConfig.GetConfig("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop\\InlineEventSeriesPropagation", EventSeriesPropagationConfig.DefaultInlinePropagationConfig);
		}

		private static EventSeriesPropagationConfig GetConfig(string configObjectPath, EventSeriesPropagationConfig defaultValue)
		{
			RegistrySession registrySession = new RegistrySession(false);
			EventSeriesPropagationConfig[] array = registrySession.Find<EventSeriesPropagationConfig>(new RegistryObjectId(configObjectPath));
			if (array.Length == 0)
			{
				return defaultValue;
			}
			return array[0];
		}

		public const string BackgroundEventSeriesPropagationSubKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop\\BackgroundEventSeriesPropagation";

		public const string CalendarInteropConfigurationRegistryKeyRoot = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop";

		public const string EventSeriesPropagationConfigName = "EventSeriesPropagationConfig";

		public const string InlineEventSeriesPropagationSubKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop\\InlineEventSeriesPropagation";

		private static readonly EventSeriesPropagationConfig DefaultBackgroundPropagationConfig = new EventSeriesPropagationConfig
		{
			PropagationCountLimit = 50U,
			ReversePropagationOrder = true,
			ShouldCleanup = true,
			PropagationTimeLimit = TimeSpan.FromSeconds(20.0),
			IgnorePropagationErrors = false
		};

		private static readonly EventSeriesPropagationConfig DefaultInlinePropagationConfig = new EventSeriesPropagationConfig
		{
			PropagationCountLimit = 10U,
			ReversePropagationOrder = false,
			ShouldCleanup = false,
			PropagationTimeLimit = TimeSpan.FromSeconds(10.0),
			IgnorePropagationErrors = true
		};

		private static readonly EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema Schema = ObjectSchema.GetInstance<EventSeriesPropagationConfig.EventSeriesPropagationConfigSchema>();

		internal class EventSeriesPropagationConfigSchema : RegistryObjectSchema
		{
			public override string DefaultName
			{
				get
				{
					return "EventSeriesPropagationConfig";
				}
			}

			public override string DefaultRegistryKeyPath
			{
				get
				{
					return "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Calendaring\\Interop";
				}
			}

			public static readonly RegistryPropertyDefinition MaxActionLifetime = new RegistryPropertyDefinition("MaxActionLifetime", typeof(TimeSpan), TimeSpan.FromHours(24.0));

			public static readonly RegistryPropertyDefinition MaxCollisionRetryCount = new RegistryPropertyDefinition("MaxCollisionRetryCount", typeof(uint), 5U);

			public static readonly RegistryPropertyDefinition PropagationCountLimit = new RegistryPropertyDefinition("PropagationCountLimit", typeof(uint), 50U);

			public static readonly RegistryPropertyDefinition PropagationTimeLimit = new RegistryPropertyDefinition("PropagationTimeLimit", typeof(TimeSpan), TimeSpan.FromSeconds(10.0));

			public static readonly RegistryPropertyDefinition ReversePropagationOrder = new RegistryPropertyDefinition("ReversedPropagationOrder", typeof(bool), false);

			public static readonly RegistryPropertyDefinition ShouldCleanup = new RegistryPropertyDefinition("ShouldCleanup", typeof(bool), true);

			public static readonly RegistryPropertyDefinition IgnorePropagationErrors = new RegistryPropertyDefinition("IgnorePropagationErrors", typeof(bool), true);
		}
	}
}
