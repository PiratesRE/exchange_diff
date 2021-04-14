using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class MigrationHelper
	{
		public static byte[] ToMigrationCsv(object input, string migrationType)
		{
			if (input != null)
			{
				string[] value = Array.ConvertAll<object, string>((object[])input, (object x) => (string)x);
				string text = string.Join(Environment.NewLine, value);
				text = "EmailAddress" + Environment.NewLine + text;
				return Encoding.UTF8.GetBytes(text);
			}
			return null;
		}

		public static byte[] DecodeCsv(string base64String)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (Stream stream = FileEncodeUploadHandler.DecodeContent(base64String))
				{
					stream.CopyTo(memoryStream);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static Identity[] GetUserSmtpAddress()
		{
			SmtpAddress executingUserPrimarySmtpAddress = EacRbacPrincipal.Instance.ExecutingUserPrimarySmtpAddress;
			string name = EacRbacPrincipal.Instance.Name;
			if (!(executingUserPrimarySmtpAddress == SmtpAddress.Empty))
			{
				return new Identity[]
				{
					new Identity(executingUserPrimarySmtpAddress.ToString(), name)
				};
			}
			return null;
		}

		public static bool TestFlag(int flagValue, int flag)
		{
			return (flagValue & flag) == flag;
		}

		public static string GetMigrationType(object endpoint)
		{
			if (endpoint is MigrationEndpoint)
			{
				MigrationType endpointType = ((MigrationEndpoint)endpoint).EndpointType;
				if (endpointType <= MigrationType.ExchangeOutlookAnywhere)
				{
					if (endpointType == MigrationType.IMAP)
					{
						return "IMAP";
					}
					if (endpointType == MigrationType.ExchangeOutlookAnywhere)
					{
						return "Staged";
					}
				}
				else
				{
					if (endpointType == MigrationType.ExchangeRemoteMove)
					{
						return "RemoteMove";
					}
					if (endpointType == MigrationType.ExchangeLocalMove)
					{
						return "LocalMove";
					}
					if (endpointType == MigrationType.PublicFolder)
					{
						return "PublicFolder";
					}
				}
				throw new Exception("Unexpected endpoint type " + ((MigrationEndpoint)endpoint).EndpointType.ToString());
			}
			return "LocalMove";
		}

		public static string GetCurrentTimeString()
		{
			return ExDateTime.UtcNow.ToUserDateTimeString();
		}

		public static DateTime LocalTimeStringToUniversalTime(object localTimeString)
		{
			ExDateTime? exDateTime = ((string)localTimeString).ToEcpExDateTime("yyyy/MM/dd HH:mm:ss");
			if (exDateTime != null)
			{
				return exDateTime.Value.UniversalTime;
			}
			return DateTime.UtcNow;
		}

		public static List<object> ToMigrationReportEntries(object reports)
		{
			ICollection collection = reports as ICollection;
			List<object> list = null;
			if (collection != null && collection.Count > 0)
			{
				list = new List<object>();
				foreach (object obj in collection)
				{
					MigrationReportSet migrationReportSet = (MigrationReportSet)obj;
					string creationTime = migrationReportSet.CreationTimeUTC.UtcToUserDateTimeString();
					if (!string.IsNullOrEmpty(migrationReportSet.SuccessUrl))
					{
						list.Add(new MigrationReportEntry(creationTime, migrationReportSet.SuccessUrl, Strings.Success));
					}
					if (!string.IsNullOrEmpty(migrationReportSet.ErrorUrl))
					{
						list.Add(new MigrationReportEntry(creationTime, migrationReportSet.ErrorUrl, Strings.Error));
					}
				}
			}
			return list;
		}
	}
}
