using System;
using System.Runtime.InteropServices;
using Microsoft.Internal.ManagedWPP;
using Microsoft.Mce.Interop.Api;
using Microsoft.Win32;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	[Guid("FC3928F9-5887-4D0B-8A5C-0E8973C35D4B")]
	[ComVisible(true)]
	public class RegistryEngineSettings : IPropertyBag
	{
		public int Read(string propertyName, ref object propertyValue, IErrorLog errorLog)
		{
			propertyValue = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ClassificationEngine"))
			{
				if (registryKey != null)
				{
					propertyValue = registryKey.GetValue(propertyName);
				}
			}
			Func<string, string, bool> func = (string l, string r) => 0 == string.Compare(l, r, StringComparison.CurrentCultureIgnoreCase);
			if (propertyValue == null)
			{
				if (func("RulePackageCachePollPeriod", propertyName))
				{
					propertyValue = 1800;
				}
				else if (func("PerformanceMonitorPollPeriod", propertyName))
				{
					propertyValue = 900;
				}
				else if (func("UseMemoryToImprovePerformance", propertyName))
				{
					propertyValue = true;
				}
			}
			if (propertyValue == null)
			{
				string text = string.Format("No value is available for property \"{0}\"", propertyName);
				if (Tracing.tracer.Level >= 5 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(5, 10, this.GetHashCode(), TraceProvider.MakeStringArg(text));
				}
				throw new ArgumentException(text, "propertyName");
			}
			return 0;
		}

		public int Write(string propertyName, ref object propertyValue)
		{
			string text = string.Format("RegistryEngineSettings.Write was called unexpectedly (name={0}, value={1})", propertyName, propertyValue.ToString());
			if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
			{
				WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(5, 11, this.GetHashCode(), TraceProvider.MakeStringArg(text));
			}
			throw new NotImplementedException(text);
		}

		private const string ClassificationEngineRegistryKey = "Software\\Microsoft\\ClassificationEngine";
	}
}
