using System;
using System.Configuration;
using System.Security;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class OverridableSetting<T>
	{
		public OverridableSetting(string name, T defaultValue, Func<string, T> converter, bool allowOverride = false)
		{
			this.traceContext = new TracingContext(null)
			{
				LId = this.GetHashCode(),
				Id = base.GetType().GetHashCode()
			};
			this.Name = name;
			this.Value = defaultValue;
			this.Converter = converter;
			this.AllowOverride = allowOverride;
			string text = ConfigurationManager.AppSettings[name];
			if (text != null)
			{
				try
				{
					this.Value = converter(text);
				}
				catch (FormatException arg)
				{
					WTFDiagnostics.TraceError<string, string, FormatException>(WTFLog.Core, this.traceContext, "[OverridableSetting.OverridableSetting]: incorrect configuration value encountered, {0} : {1} with error:\n{2}", name, text, arg, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\OverridableSetting.cs", 62);
				}
			}
		}

		internal T Value { private get; set; }

		private string Name { get; set; }

		private Func<string, T> Converter { get; set; }

		private bool AllowOverride { get; set; }

		public static implicit operator T(OverridableSetting<T> os)
		{
			if (os.AllowOverride)
			{
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Settings.OverrideRegistryPath))
					{
						if (registryKey != null)
						{
							string text = registryKey.GetValue(os.Name) as string;
							if (!string.IsNullOrWhiteSpace(text))
							{
								try
								{
									os.Value = os.Converter(text);
								}
								catch (FormatException arg)
								{
									WTFDiagnostics.TraceError<string, string, FormatException>(WTFLog.Core, os.traceContext, "[OverridableSetting.Conversion]: incorrect configuration value encountered, {0} : {1}. Error:\n{2}", os.Name, text, arg, null, "op_Implicit", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\OverridableSetting.cs", 133);
								}
							}
						}
					}
				}
				catch (SecurityException arg2)
				{
					WTFDiagnostics.TraceError<string, SecurityException>(WTFLog.Core, os.traceContext, "[OverridableSetting.Conversion]: cannot access registry {0}. Error:\n{1}", Settings.OverrideRegistryPath, arg2, null, "op_Implicit", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\OverridableSetting.cs", 141);
				}
			}
			return os.Value;
		}

		private TracingContext traceContext;
	}
}
