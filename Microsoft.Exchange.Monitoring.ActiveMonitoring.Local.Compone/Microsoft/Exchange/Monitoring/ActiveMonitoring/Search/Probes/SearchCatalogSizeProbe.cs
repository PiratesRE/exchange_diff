using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchCatalogSizeProbe : SearchProbeBase
	{
		protected override bool SkipOnNonHealthyCatalog
		{
			get
			{
				return true;
			}
		}

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			double @double = base.AttributeHelper.GetDouble("SizePercentageThreshold", true, 0.0, null, null);
			double double2 = base.AttributeHelper.GetDouble("MinimumCheckSizeGb", true, 0.0, null, null);
			string targetResource = base.Definition.TargetResource;
			CopyStatusClientCachedEntry cachedLocalDatabaseCopyStatus = SearchMonitoringHelper.GetCachedLocalDatabaseCopyStatus(targetResource);
			if (cachedLocalDatabaseCopyStatus == null || cachedLocalDatabaseCopyStatus.CopyStatus == null)
			{
				base.Result.StateAttribute1 = "CopyStatus is null.";
				return;
			}
			if (cachedLocalDatabaseCopyStatus.CopyStatus.ReplayLagEnabled == ReplayLagEnabledEnum.Enabled)
			{
				base.Result.StateAttribute1 = "Lag copy.";
				return;
			}
			MailboxDatabaseInfo databaseInfo = SearchMonitoringHelper.GetDatabaseInfo(targetResource);
			MailboxDatabase mailboxDatabaseFromGuid = DirectoryAccessor.Instance.GetMailboxDatabaseFromGuid(databaseInfo.MailboxDatabaseGuid);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("***SearchCatalogSizeProbe/{0} diagnostic info***", targetResource);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("FullName,LastWriteTime,Length");
			if (mailboxDatabaseFromGuid.EdbFilePath == null)
			{
				base.Result.StateAttribute1 = "EdbFilePath is null.";
				return;
			}
			FileInfo fileInfo = new FileInfo(mailboxDatabaseFromGuid.EdbFilePath.PathName);
			if (!fileInfo.Exists)
			{
				base.Result.StateAttribute1 = string.Format("EDB file '{0}' does not exist.", mailboxDatabaseFromGuid.EdbFilePath.PathName);
				return;
			}
			double num = (double)fileInfo.Length;
			base.Result.StateAttribute2 = num.ToString();
			DirectoryInfo directory = fileInfo.Directory;
			DirectoryInfo[] directories = directory.GetDirectories(string.Format("{0}*.Single", mailboxDatabaseFromGuid.Guid));
			base.Result.StateAttribute4 = directories.Length.ToString();
			double num2 = 0.0;
			foreach (DirectoryInfo directoryInfo in directories)
			{
				FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
				foreach (FileInfo fileInfo2 in files)
				{
					stringBuilder.AppendFormat("{0},{1},{2}", fileInfo2.FullName, fileInfo2.LastWriteTime.ToString(), fileInfo2.Length);
					stringBuilder.AppendLine();
					num2 += (double)fileInfo2.Length;
				}
			}
			base.Result.StateAttribute3 = num2.ToString();
			if (num2 / 1073741824.0 < double2)
			{
				base.Result.StateAttribute1 = "Catalog is too small for check.";
				return;
			}
			if (num2 / num > @double)
			{
				double num3 = num2 / 1073741824.0;
				double num4 = num / 1073741824.0;
				SearchMonitoringHelper.LogInfo(stringBuilder.ToString(), new object[0]);
				throw new SearchProbeFailureException(Strings.SearchCatalogTooBig(targetResource, num4.ToString("N"), num3.ToString("N"), @double.ToString("P")));
			}
		}

		private const string CatalogFolderWildcard = "{0}*.Single";

		private const string CatalogFileWildcard = "*.*";

		private const int GB = 1073741824;
	}
}
