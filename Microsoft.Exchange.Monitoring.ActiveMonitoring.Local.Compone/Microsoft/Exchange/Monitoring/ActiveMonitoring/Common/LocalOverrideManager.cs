using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class LocalOverrideManager
	{
		public static void LoadLocalOverrides()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(LocalOverrideManager.RegistryPathBase))
			{
				LocalOverrideManager.localOverrideWaterMark = (registryKey.GetValue(LocalOverrideManager.WaterMarkName) as string);
				ServerVersion version = null;
				try
				{
					version = DirectoryAccessor.Instance.Server.AdminDisplayVersion;
				}
				catch (Exception arg)
				{
					WTFDiagnostics.TraceDebug<Exception>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "Failed to get server version, ignore the error {0}", arg, null, "LoadLocalOverrides", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 72);
				}
				ProbeDefinition.LocalOverrides = LocalOverrideManager.LoadLocalOverridesForType<ProbeDefinition>(version, registryKey);
				MonitorDefinition.LocalOverrides = LocalOverrideManager.LoadLocalOverridesForType<MonitorDefinition>(version, registryKey);
				ResponderDefinition.LocalOverrides = LocalOverrideManager.LoadLocalOverridesForType<ResponderDefinition>(version, registryKey);
				MaintenanceDefinition.LocalOverrides = LocalOverrideManager.LoadLocalOverridesForType<MaintenanceDefinition>(version, registryKey);
			}
		}

		public static bool IsLocalOverridesChanged()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(LocalOverrideManager.RegistryPathBase))
			{
				string strB = registryKey.GetValue(LocalOverrideManager.WaterMarkName) as string;
				if (string.Compare(LocalOverrideManager.localOverrideWaterMark, strB, true) != 0)
				{
					return true;
				}
			}
			return false;
		}

		private static List<WorkDefinitionOverride> LoadLocalOverridesForType<TWorkDefinition>(ServerVersion version, RegistryKey root) where TWorkDefinition : WorkDefinition
		{
			string text = typeof(TWorkDefinition).Name;
			text = text.Substring(0, text.IndexOf("Definition"));
			List<WorkDefinitionOverride> result;
			using (RegistryKey registryKey = root.CreateSubKey(text))
			{
				List<WorkDefinitionOverride> list = new List<WorkDefinitionOverride>();
				foreach (string text2 in registryKey.GetSubKeyNames())
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "Found override {0} ", text2, null, "LoadLocalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 123);
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text2))
					{
						string text3 = registryKey2.GetValue("ExpirationTime") as string;
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "Override {0} expiration time {1}", text2, text3, null, "LoadLocalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 128);
						DateTime dateTime = DateTime.MaxValue;
						if (!string.IsNullOrWhiteSpace(text3) && DateTime.TryParse(text3, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
						{
							dateTime = dateTime.ToUniversalTime();
							if (dateTime < DateTime.UtcNow)
							{
								WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "Ignore override {0} as it is expired.", text2, null, "LoadLocalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 138);
								goto IL_23B;
							}
						}
						string text4 = registryKey2.GetValue("ApplyVersion") as string;
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "override {0} apply version is {1} ", text2, text4, null, "LoadLocalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 147);
						ServerVersion left = null;
						if (ServerVersion.TryParseFromSerialNumber(text4, out left) && left != version)
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, LocalOverrideManager.traceContext, "Ignore override {0} because version mismatch ", text2, null, "LoadLocalOverridesForType", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\LocalOverrideManager.cs", 154);
						}
						else
						{
							string[] array = text2.Split(new char[]
							{
								'~'
							});
							WorkDefinitionOverride item = new WorkDefinitionOverride
							{
								WorkDefinitionName = ((array.Length > 1) ? array[1] : string.Empty),
								Scope = ((array.Length > 3) ? array[3] : string.Empty),
								ExpirationDate = dateTime,
								ServiceName = array[0],
								PropertyName = ((array.Length > 2) ? array[2] : string.Empty),
								NewPropertyValue = (registryKey2.GetValue("PropertyValue") as string)
							};
							list.Add(item);
						}
					}
					IL_23B:;
				}
				result = list;
			}
			return result;
		}

		private static readonly string RegistryPathBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\Overrides";

		private static readonly string WaterMarkName = "Watermark";

		private static string localOverrideWaterMark = string.Empty;

		private static TracingContext traceContext = TracingContext.Default;
	}
}
