using System;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal sealed class EnvironmentAnalyzer
	{
		public static string CheckEnvironment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (EnvironmentAnalyzer.EnvironmentAnalyzerDelegate environmentAnalyzerDelegate in EnvironmentAnalyzer.checkerList)
			{
				string value = environmentAnalyzerDelegate();
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.AppendLine(value);
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetInstallPath()
		{
			return ConfigurationContext.Setup.InstallPath;
		}

		public static string GetLocalServerName()
		{
			if (string.IsNullOrEmpty(EnvironmentAnalyzer.localServerName))
			{
				EnvironmentAnalyzer.localServerName = NativeHelpers.GetLocalComputerFqdn(false);
			}
			return EnvironmentAnalyzer.localServerName;
		}

		public static bool IsWorkGroup()
		{
			EnvironmentAnalyzer.CheckInstalledTopology();
			return EnvironmentAnalyzer.installedTopology == EnvironmentAnalyzer.Topology.WorkGroup;
		}

		private static void CheckInstalledTopology()
		{
			if (EnvironmentAnalyzer.installedTopology == EnvironmentAnalyzer.Topology.Unchecked)
			{
				string path = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange";
				if (WinformsHelper.IsValidRegKey(path))
				{
					EnvironmentAnalyzer.installedTopology = EnvironmentAnalyzer.Topology.WorkGroup;
					return;
				}
				EnvironmentAnalyzer.installedTopology = EnvironmentAnalyzer.Topology.ServerForest;
			}
		}

		private static bool IsExchangeInstalled()
		{
			string path = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";
			return WinformsHelper.IsValidRegKey(path);
		}

		private static string CheckExchangeInstalled()
		{
			string result = string.Empty;
			if (!EnvironmentAnalyzer.IsExchangeInstalled())
			{
				result = Strings.ExchangeIsNotInstalledCorrectly;
			}
			return result;
		}

		private static string CheckDataCenterCompatibility()
		{
			string result = string.Empty;
			if (Datacenter.IsMultiTenancyEnabled())
			{
				result = Strings.BlockEMCInDataCenter;
			}
			return result;
		}

		private static EnvironmentAnalyzer.Topology installedTopology = EnvironmentAnalyzer.Topology.Unchecked;

		private static EnvironmentAnalyzer.EnvironmentAnalyzerDelegate[] checkerList = new EnvironmentAnalyzer.EnvironmentAnalyzerDelegate[]
		{
			new EnvironmentAnalyzer.EnvironmentAnalyzerDelegate(EnvironmentAnalyzer.CheckExchangeInstalled),
			new EnvironmentAnalyzer.EnvironmentAnalyzerDelegate(EnvironmentAnalyzer.CheckDataCenterCompatibility)
		};

		private static string localServerName;

		private enum Topology
		{
			Unchecked,
			WorkGroup,
			ServerForest
		}

		private delegate string EnvironmentAnalyzerDelegate();
	}
}
