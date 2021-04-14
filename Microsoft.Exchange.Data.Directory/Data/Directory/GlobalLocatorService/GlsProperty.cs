using System;
using System.Threading;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal abstract class GlsProperty
	{
		protected GlsProperty(string name, Type dataType, object defaultValue)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name");
			}
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			if (defaultValue != null && defaultValue.GetType() != dataType)
			{
				throw new ArgumentException(string.Format("Incompatible type for default Value, expected:{0}, actual:{1}", dataType, defaultValue.GetType()));
			}
			if (defaultValue == null && dataType.IsValueType)
			{
				defaultValue = Activator.CreateInstance(dataType);
			}
			this.name = name;
			this.dataType = dataType;
			this.defaultValue = defaultValue;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type DataType
		{
			get
			{
				return this.dataType;
			}
		}

		internal object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		internal static string ExoPrefix
		{
			get
			{
				if (GlsCallerId.GLSEnvironment != GlsEnvironmentType.Gallatin)
				{
					return "EXO";
				}
				return "EXO-CN";
			}
		}

		internal static string FfoPrefix
		{
			get
			{
				if (GlsCallerId.GLSEnvironment != GlsEnvironmentType.Gallatin)
				{
					return "FFO";
				}
				return "FFO-CN";
			}
		}

		internal static string GlobalPrefix
		{
			get
			{
				return "Global";
			}
		}

		protected static readonly Lazy<GlsEnvironmentType> glsEnvironmentType = new Lazy<GlsEnvironmentType>(() => RegistrySettings.ExchangeServerCurrentVersion.GlsEnvironmentType, LazyThreadSafetyMode.PublicationOnly);

		private readonly string name;

		private readonly Type dataType;

		private readonly object defaultValue;
	}
}
