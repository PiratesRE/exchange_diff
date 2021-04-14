using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class OBDUploaderProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("OBDUploaderProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			OBDUploaderProbeDefinition obduploaderProbeDefinition = new OBDUploaderProbeDefinition(base.Definition.ExtensionAttributes, base.TraceContext);
			DateTime utcNow = DateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				if (!Directory.Exists(obduploaderProbeDefinition.RawLogFileFolder))
				{
					ProbeResult result = base.Result;
					result.ExecutionContext += string.Format("OBDUploaderProbe complete: raw log folder {0} doesn't exist {1}", obduploaderProbeDefinition.RawLogFileFolder, Environment.NewLine);
					return;
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(obduploaderProbeDefinition.RawLogFileFolder);
				IEnumerable<FileInfo> logfiles = from f in directoryInfo.GetFiles(obduploaderProbeDefinition.RawLogFileNamePattern)
				orderby f.LastWriteTimeUtc descending
				select f;
				if (!Directory.Exists(obduploaderProbeDefinition.ProgressFileFolder))
				{
					this.HandleError(string.Format("Progress file folder {0} doesn't exist {1}", obduploaderProbeDefinition.ProgressFileFolder, Environment.NewLine));
				}
				string pattern = "([0-9]{4})(0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])([0-1][0-9]|2[0-3])$";
				Regex re = new Regex(pattern);
				DirectoryInfo directoryInfo2 = new DirectoryInfo(obduploaderProbeDefinition.ProgressFileFolder);
				FileInfo fileInfo = (from f in directoryInfo2.GetFiles(obduploaderProbeDefinition.ProgressFileNamePattern)
				where re.Match(f.Name.Split(new char[]
				{
					'-'
				}).First<string>()).Success
				orderby f.LastWriteTimeUtc descending
				select f).FirstOrDefault<FileInfo>();
				if (fileInfo == null)
				{
					this.HandleError(string.Format("There is no valid progress file in folder {0}{1}", obduploaderProbeDefinition.ProgressFileFolder, Environment.NewLine));
				}
				string[] array = fileInfo.Name.Split(new char[]
				{
					'-'
				});
				string s = array[0].Substring(array[0].Length - 10, 10);
				DateTime dateTime = DateTime.ParseExact(s, "yyyyMMddHH", CultureInfo.InvariantCulture);
				TimeSpan timeSpan = DateTime.UtcNow.Subtract(dateTime);
				if (timeSpan.TotalHours > (double)obduploaderProbeDefinition.SLAThresholdInHours)
				{
					stringBuilder.AppendFormat("Progress file {0} violates SLA, the delay is {1} hours {2}", fileInfo, timeSpan.TotalHours, Environment.NewLine);
					dateTime = dateTime.AddHours(1.0);
					string value = this.GenerateFileList(logfiles, dateTime);
					stringBuilder.AppendLine(value);
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendFormat("Unexpected error, please check the detail, this could be a bug for the probe itself: {0}.{1}", ex.ToString(), Environment.NewLine);
			}
			finally
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += string.Format("OBDUploaderProbe ended at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			}
			if (stringBuilder.Length > 0)
			{
				this.HandleError(stringBuilder.ToString());
			}
		}

		private void HandleError(string error)
		{
			base.Result.FailureContext = error;
			throw new Exception(error);
		}

		private string GenerateFileList(IEnumerable<FileInfo> logfiles, DateTime datetimeFromFileNameUTC)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FileInfo fileInfo in logfiles.Reverse<FileInfo>())
			{
				try
				{
					if (fileInfo.CreationTimeUtc >= datetimeFromFileNameUTC)
					{
						stringBuilder.AppendFormat("Log file {0} hasn't been uploaded: the log file's create time(UTC) is {1} {2}", fileInfo, fileInfo.CreationTimeUtc, Environment.NewLine);
						num++;
					}
					if (num >= 10)
					{
						break;
					}
				}
				catch (FileNotFoundException)
				{
					ProbeResult result = base.Result;
					result.ExecutionContext += string.Format("{0} is not found.", fileInfo);
				}
				catch (IOException ex)
				{
					ProbeResult result2 = base.Result;
					result2.ExecutionContext += string.Format("Get exception when processing {0}. The error is {1}", fileInfo, ex.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
