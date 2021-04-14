using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration
{
	internal static class CmdletFlight
	{
		internal static VariantConfigurationSnapshot GetSnapshot(ADRawEntry executingUser, IEnumerable<KeyValuePair<string, string>> additionalConstraints)
		{
			if (executingUser == null)
			{
				return null;
			}
			VariantConfigurationSnapshot variantConfigurationSnapshot = null;
			MiniRecipient miniRecipient = new MiniRecipient();
			try
			{
				try
				{
					miniRecipient[MiniRecipientSchema.UserPrincipalName] = executingUser[ADUserSchema.UserPrincipalName];
					miniRecipient[ADObjectSchema.OrganizationId] = executingUser[ADObjectSchema.OrganizationId];
					miniRecipient[MiniRecipientSchema.ExternalDirectoryObjectId] = executingUser[ADRecipientSchema.ExternalDirectoryObjectId];
					miniRecipient[MiniRecipientSchema.Languages] = executingUser[ADOrgPersonSchema.Languages];
				}
				catch (DataValidationException ex)
				{
					AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.VariantConfigurationSnapshot, "DataValidationException", ex.Message);
				}
				ConstraintCollection constraintCollection = null;
				if (additionalConstraints != null)
				{
					constraintCollection = ConstraintCollection.CreateEmpty();
					foreach (KeyValuePair<string, string> keyValuePair in additionalConstraints)
					{
						constraintCollection.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				variantConfigurationSnapshot = VariantConfiguration.GetSnapshot(miniRecipient.GetContext(null), constraintCollection, null);
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.VariantConfigurationSnapshot, "Flights", string.Join(" ", variantConfigurationSnapshot.Flights));
			}
			catch (Exception ex2)
			{
				AuthZLogger.SafeAppendGenericError("VariantConfigurationSnapshot.Exception", ex2.Message, false);
			}
			finally
			{
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.VariantConfigurationSnapshot, "User", executingUser[ADUserSchema.UserPrincipalName].ToString());
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.VariantConfigurationSnapshot, "Org", executingUser[ADObjectSchema.OrganizationId].ToString());
			}
			return variantConfigurationSnapshot;
		}

		internal static void FilterCmdletsAndParams(VariantConfigurationSnapshot configurationSnapshot, List<RoleEntryInfo> cmdletList)
		{
			if (configurationSnapshot == null)
			{
				return;
			}
			IDictionary<string, ICmdletSettings> dictionary = null;
			try
			{
				dictionary = configurationSnapshot.GetObjectsOfType<ICmdletSettings>("CmdletInfra.settings.ini");
			}
			catch (KeyNotFoundException ex)
			{
				AuthZLogger.SafeAppendGenericError("VanriantConfigurationSnapshot.KeyNotFoundException", ex.Message, false);
			}
			if (dictionary == null || dictionary.Count == 0)
			{
				return;
			}
			List<RoleEntryInfo> cmdletTobeAdded = new List<RoleEntryInfo>();
			List<RoleEntryInfo> cmdletTobeRemoved = new List<RoleEntryInfo>();
			string value = "";
			ICmdletSettings cmdletSettings = null;
			IList<string> list = null;
			foreach (RoleEntryInfo roleEntryInfo in cmdletList)
			{
				if (dictionary.TryGetValue(roleEntryInfo.RoleEntry.Name, out cmdletSettings))
				{
					bool flag = !roleEntryInfo.RoleEntry.Name.Equals(value, StringComparison.OrdinalIgnoreCase);
					if (flag)
					{
						value = roleEntryInfo.RoleEntry.Name;
					}
					IList<string> list2 = cmdletSettings.AllFlightingParams;
					if (!cmdletSettings.Enabled)
					{
						CmdletFlight.RemoveCmdlet(roleEntryInfo, cmdletTobeRemoved, flag);
					}
					else if (list2 != null && list2.Count > 0)
					{
						if (flag)
						{
							CmdletFlight.BuildParamFlightingList(roleEntryInfo, cmdletSettings, ref list);
						}
						if (list != null)
						{
							IEnumerable<string> source = list2.Except(list);
							list2 = source.ToList<string>();
						}
						CmdletFlight.RemoveCmdletParams(roleEntryInfo, list2, cmdletTobeRemoved, cmdletTobeAdded, flag);
					}
					else if (flag)
					{
						AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.CmdletFlightEnabled, roleEntryInfo.RoleEntry.Name, "*");
					}
				}
			}
			CmdletFlight.CommitChanges(cmdletList, cmdletTobeRemoved, cmdletTobeAdded);
		}

		private static void BuildParamFlightingList(RoleEntryInfo entry, ICmdletSettings cmdletSettings, ref IList<string> paramFlightingList)
		{
			IEnumerable<string> enumerable = cmdletSettings.Params0.Union(cmdletSettings.Params1);
			enumerable = enumerable.Union(cmdletSettings.Params2);
			enumerable = enumerable.Union(cmdletSettings.Params3);
			enumerable = enumerable.Union(cmdletSettings.Params4);
			enumerable = enumerable.Union(cmdletSettings.Params5);
			enumerable = enumerable.Union(cmdletSettings.Params6);
			enumerable = enumerable.Union(cmdletSettings.Params7);
			enumerable = enumerable.Union(cmdletSettings.Params8);
			enumerable = enumerable.Union(cmdletSettings.Params9);
			if (enumerable != null)
			{
				paramFlightingList = enumerable.ToList<string>();
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.CmdletFlightEnabled, entry.RoleEntry.Name, string.Join(" ", paramFlightingList));
			}
		}

		private static void RemoveCmdlet(RoleEntryInfo entry, IList<RoleEntryInfo> cmdletTobeRemoved, bool fNewCmdlet)
		{
			cmdletTobeRemoved.Add(entry);
			if (fNewCmdlet)
			{
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.CmdletFlightDisabled, entry.RoleEntry.Name, "*");
			}
		}

		private static void RemoveCmdletParams(RoleEntryInfo entry, IList<string> paramList, IList<RoleEntryInfo> cmdletTobeRemoved, IList<RoleEntryInfo> cmdletTobeAdded, bool fNewCmdlet)
		{
			if (paramList == null || paramList.Count == 0)
			{
				return;
			}
			if (cmdletTobeRemoved == null)
			{
				return;
			}
			if (cmdletTobeAdded == null)
			{
				return;
			}
			cmdletTobeRemoved.Add(entry);
			IEnumerable<string> source = entry.RoleEntry.Parameters.Except(paramList);
			RoleEntryInfo roleEntryInfo;
			if (entry.RoleAssignment == null)
			{
				roleEntryInfo = new RoleEntryInfo(entry.RoleEntry.Clone(source.ToList<string>()));
			}
			else
			{
				roleEntryInfo = new RoleEntryInfo(entry.RoleEntry.Clone(source.ToList<string>()), entry.RoleAssignment);
			}
			roleEntryInfo.ScopeSet = entry.ScopeSet;
			cmdletTobeAdded.Add(roleEntryInfo);
			if (fNewCmdlet)
			{
				AuthZLogger.SafeAppendColumn(RpsAuthZMetadata.CmdletFlightDisabled, entry.RoleEntry.Name, string.Join(" ", paramList));
			}
		}

		private static void CommitChanges(List<RoleEntryInfo> cmdletList, IList<RoleEntryInfo> cmdletTobeRemoved, IList<RoleEntryInfo> cmdletTobeAdded)
		{
			foreach (RoleEntryInfo item in cmdletTobeRemoved)
			{
				cmdletList.Remove(item);
			}
			foreach (RoleEntryInfo item2 in cmdletTobeAdded)
			{
				cmdletList.Add(item2);
			}
			cmdletList.Sort(RoleEntryInfo.NameAndInstanceHashCodeComparer);
		}

		public const string CmdletInfraSettingsIni = "CmdletInfra.settings.ini";
	}
}
