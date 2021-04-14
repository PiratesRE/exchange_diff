using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal static class SyncDaemonArbitrationConfigHelper
	{
		internal static string SyncDaemonLeaseShare
		{
			get
			{
				return "SyncDaemonLeaseShare";
			}
		}

		internal static string ServerNameForFakeLock
		{
			get
			{
				return "Fake-Server";
			}
		}

		internal static ArbitrationConfigFromAD GetArbitrationConfigFromAD(string serviceInstanceName)
		{
			IConfigurationSession configurationSession = ForwardSyncDataAccessHelper.CreateSession(true);
			RidMasterInfo ridMasterInfo = SyncDaemonArbitrationConfigHelper.GetRidMasterInfo(configurationSession);
			SyncServiceInstance[] array = configurationSession.Find<SyncServiceInstance>(SyncServiceInstance.GetMsoSyncRootContainer(), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serviceInstanceName), null, 1);
			if (array == null || array.Length != 1)
			{
				throw new SyncDaemonArbitrationConfigException(Strings.ErrorCannotRetrieveSyncDaemonArbitrationConfigContainer((array == null) ? "0" : array.Length.ToString()));
			}
			return new ArbitrationConfigFromAD(array[0], ridMasterInfo);
		}

		internal static RidMasterInfo GetRidMasterInfo(IConfigurationSession session)
		{
			bool useConfigNC = session.UseConfigNC;
			RidMasterInfo result;
			try
			{
				session.UseConfigNC = false;
				RidManagerContainer[] array = session.Find<RidManagerContainer>(null, QueryScope.SubTree, null, null, 1);
				if (array == null || array.Length != 1)
				{
					throw new RidMasterConfigException(Strings.ErrorCannotRetrieveRidManagerContainer((array == null) ? "0" : array.Length.ToString()));
				}
				session.UseConfigNC = true;
				ADObjectId fsmoRoleOwner = array[0].FsmoRoleOwner;
				if (fsmoRoleOwner == null)
				{
					throw new RidMasterConfigException(Strings.ErrorEmptyFsmoRoleOwnerAttribute);
				}
				ADServer adserver = session.Read<ADServer>(fsmoRoleOwner.Parent);
				if (adserver == null)
				{
					throw new RidMasterConfigException(Strings.ErrorCannotRetrieveServer(fsmoRoleOwner.Parent.ToString()));
				}
				string dnsHostName = adserver.DnsHostName;
				int fsmoRoleOwnerVersion = SyncDaemonArbitrationConfigHelper.GetFsmoRoleOwnerVersion(array[0].ReplicationAttributeMetadata);
				result = new RidMasterInfo(dnsHostName, fsmoRoleOwnerVersion);
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
			return result;
		}

		internal static string GetLeaseFileName(string serviceInstanceName)
		{
			return Regex.Replace(serviceInstanceName, "[\\\\/:*?\"<>|]", "_");
		}

		private static int GetFsmoRoleOwnerVersion(MultiValuedProperty<string> attributeMetadataList)
		{
			int num = -1;
			string text = string.Empty;
			Exception ex = null;
			if (attributeMetadataList != null)
			{
				foreach (string text2 in attributeMetadataList)
				{
					if (text2.Contains(">fSMORoleOwner</"))
					{
						text = text2;
						break;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num2 = text.IndexOf("</dwVersion>");
				int num3 = text.IndexOf("<dwVersion>");
				if (num2 != -1 && num3 != -1)
				{
					string value = text.Substring(num3 + "<dwVersion>".Length, num2 - num3 - "<dwVersion>".Length);
					if (!string.IsNullOrEmpty(value))
					{
						try
						{
							num = Convert.ToInt32(value);
						}
						catch (FormatException ex2)
						{
							ex = ex2;
						}
						catch (OverflowException ex3)
						{
							ex = ex3;
						}
					}
				}
			}
			if (num == -1)
			{
				throw new SyncDaemonArbitrationConfigException(Strings.ErrorCannotParseFsmoRoleOwnerVersion(text, (ex == null) ? string.Empty : ex.ToString()));
			}
			return num;
		}

		private const string FsmoRoleOwnerTag = ">fSMORoleOwner</";

		private const string DwVersionStartTag = "<dwVersion>";

		private const string DwVersionEndTag = "</dwVersion>";

		private const string RegExInvalidFileNameCharacters = "[\\\\/:*?\"<>|]";

		private const string InvalidFileNameCharacterReplacement = "_";
	}
}
