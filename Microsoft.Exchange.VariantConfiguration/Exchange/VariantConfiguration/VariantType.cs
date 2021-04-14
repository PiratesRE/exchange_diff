using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantType
	{
		private VariantType(string name, Type type, VariantTypeFlags flags, Func<string, bool> valueValidator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			this.Name = name;
			this.Type = type;
			this.Flags = flags;
			this.ValidateValue = valueValidator;
		}

		public static VariantType Create(string name, Type type, VariantTypeFlags flags)
		{
			return VariantType.Create(name, type, flags, VariantType.GetDefaultValidator(type));
		}

		public static VariantType Create(string name, Type type, VariantTypeFlags flags, Func<string, bool> valueValidator)
		{
			return new VariantType(name, type, flags, valueValidator);
		}

		public static implicit operator string(VariantType variant)
		{
			return variant.Name;
		}

		private static VariantTypeCollection InitializeVariants()
		{
			List<VariantType> list = new List<VariantType>();
			foreach (FieldInfo fieldInfo in typeof(VariantType).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				VariantType variantType = fieldInfo.GetValue(null) as VariantType;
				if (variantType != null)
				{
					list.Add(variantType);
				}
			}
			return VariantTypeCollection.Create(list);
		}

		private static bool ServiceValidator(string value)
		{
			return VariantType.ServiceVariantValues.Contains(value);
		}

		private static bool ModeValidator(string value)
		{
			return VariantType.ModeVariantValues.Contains(value);
		}

		private static bool RegionValidator(string value)
		{
			return value.Length == 3;
		}

		private static bool BoolValidator(string value)
		{
			bool flag;
			return bool.TryParse(value, out flag);
		}

		private static bool GuidValidator(string value)
		{
			return VariantType.GuidRegex.IsMatch(value);
		}

		private static bool VersionValidator(string value)
		{
			return VariantType.VersionRegex.IsMatch(value);
		}

		private static bool DefaultValidator(string value)
		{
			return true;
		}

		private static Func<string, bool> GetDefaultValidator(Type type)
		{
			if (type == typeof(bool))
			{
				return new Func<string, bool>(VariantType.BoolValidator);
			}
			if (type == typeof(Version))
			{
				return new Func<string, bool>(VariantType.VersionValidator);
			}
			if (type == typeof(Guid))
			{
				return new Func<string, bool>(VariantType.GuidValidator);
			}
			return new Func<string, bool>(VariantType.DefaultValidator);
		}

		public static readonly VariantType Organization = VariantType.Create("org", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights | VariantTypeFlags.AllowedInTeams);

		public static readonly VariantType User = VariantType.Create("user", typeof(string), VariantTypeFlags.Public | VariantTypeFlags.AllowedInFlights | VariantTypeFlags.AllowedInTeams);

		public static readonly VariantType Locale = VariantType.Create("loc", typeof(string), VariantTypeFlags.Public | VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Mode = VariantType.Create("mode", typeof(string), VariantTypeFlags.AllowedInSettings, new Func<string, bool>(VariantType.ModeValidator));

		public static readonly VariantType Dogfood = VariantType.Create("dogfood", typeof(bool), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Region = VariantType.Create("region", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights, new Func<string, bool>(VariantType.RegionValidator));

		public static readonly VariantType Process = VariantType.Create("process", typeof(string), VariantTypeFlags.Public | VariantTypeFlags.AllowedInSettings);

		public static readonly VariantType FirstRelease = VariantType.Create("FirstRelease", typeof(bool), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Primary = VariantType.Create("primary", typeof(bool), VariantTypeFlags.AllowedInSettings);

		public static readonly VariantType Test = VariantType.Create("test", typeof(bool), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Machine = VariantType.Create("machine", typeof(string), VariantTypeFlags.Public | VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Dag = VariantType.Create("dag", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Pod = VariantType.Create("pod", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Forest = VariantType.Create("forest", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Service = VariantType.Create("Service", typeof(string), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights, new Func<string, bool>(VariantType.ServiceValidator));

		public static readonly VariantType Flight = VariantType.Create("flt", typeof(bool), VariantTypeFlags.Prefix | VariantTypeFlags.AllowedInSettings);

		public static readonly VariantType Team = VariantType.Create("team", typeof(bool), VariantTypeFlags.Prefix | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType Preview = VariantType.Create("Preview", typeof(bool), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType MdbGuid = VariantType.Create("mdbguid", typeof(Guid), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType MdbName = VariantType.Create("mdbname", typeof(string), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType MdbVersion = VariantType.Create("mdbversion", typeof(Version), VariantTypeFlags.AllowedInFlights);

		public static readonly VariantType AuthMethod = VariantType.Create("AuthMethod", typeof(string), VariantTypeFlags.AllowedInSettings);

		public static readonly VariantType UserType = VariantType.Create("UserType", typeof(VariantConfigurationUserType), VariantTypeFlags.AllowedInSettings | VariantTypeFlags.AllowedInFlights);

		public static readonly VariantTypeCollection Variants = VariantType.InitializeVariants();

		public readonly string Name;

		public readonly Type Type;

		public readonly VariantTypeFlags Flags;

		public readonly Func<string, bool> ValidateValue;

		internal static readonly Regex VersionRegex = new Regex("^\\d{2}\\.\\d{2}\\.\\d{4}\\.\\d{3}$", RegexOptions.Compiled);

		internal static readonly Regex GuidRegex = new Regex("^[\\da-f]{8}(\\-[\\da-f]{4}){3}\\-[\\da-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly HashSet<string> ServiceVariantValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Gallatin",
			"PROD",
			"ServiceDogfood"
		};

		private static readonly HashSet<string> ModeVariantValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"dedicated",
			"datacenter",
			"enterprise"
		};
	}
}
