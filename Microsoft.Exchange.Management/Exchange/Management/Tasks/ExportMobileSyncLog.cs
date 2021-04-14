using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Export", "ActiveSyncLog", SupportsShouldProcess = true)]
	public sealed class ExportMobileSyncLog : Task
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public string Filename
		{
			get
			{
				if (this.currentFile != null)
				{
					return this.currentFile.FullName;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.currentFile = new FileInfo(value);
					return;
				}
				this.currentFile = null;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime StartDate
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime EndDate
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseGMT
		{
			get
			{
				return this.useGMT;
			}
			set
			{
				this.useGMT = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OutputPrefix
		{
			get
			{
				return this.outputPrefix;
			}
			set
			{
				this.outputPrefix = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OutputPath
		{
			get
			{
				if (this.outputPath != null)
				{
					return this.outputPath.FullName;
				}
				return null;
			}
			set
			{
				this.outputPath = new DirectoryInfo(value);
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationProcessInputLog(this.currentFile.FullName);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.outputPath != null && !Directory.Exists(this.outputPath.FullName))
			{
				base.WriteError(new OutputDirectoryNotExist(this.outputPath.FullName), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.startTime > DateTime.MinValue && this.endTime < DateTime.MaxValue && this.startTime > this.endTime)
			{
				base.WriteError(new ArgumentException(Strings.InvalidTimeRange, "StartDate"), ErrorCategory.InvalidArgument, null);
			}
			this.parser = new ExportMobileSyncLog.AirSyncLogParser();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.useGMT)
			{
				this.parser.StartTime = this.startTime;
				this.parser.EndTime = this.endTime;
			}
			else
			{
				if (this.startTime != DateTime.MinValue)
				{
					this.parser.StartTime = this.startTime.ToUniversalTime();
				}
				if (this.endTime != DateTime.MaxValue)
				{
					this.parser.EndTime = this.endTime.ToUniversalTime();
				}
			}
			base.WriteProgress(Strings.AirSyncReportingProgressActivity, Strings.AirSyncReportingProgressParsing(this.currentFile.FullName), 0);
			base.WriteVerbose(Strings.AirSyncReportingProgressParsing(this.currentFile.FullName));
			this.parser.StartNewFile();
			int num = 0;
			StreamReader streamReader = null;
			try
			{
				FileStream stream = File.Open(this.currentFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				streamReader = new StreamReader(stream);
				long length = streamReader.BaseStream.Length;
				string line;
				while ((line = streamReader.ReadLine()) != null)
				{
					int num2 = (int)((double)streamReader.BaseStream.Position / (double)length * 100.0);
					if (num2 != num)
					{
						base.WriteProgress(Strings.AirSyncReportingProgressActivity, Strings.AirSyncReportingProgressParsing(this.currentFile.FullName), num2);
						num = num2;
					}
					this.parser.ParseLine(line);
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new ExceptionWhileReadingInputFile(this.currentFile.FullName, ex.Message, ex), ErrorCategory.ReadError, this.currentFile);
			}
			catch (UnauthorizedAccessException ex2)
			{
				base.WriteError(new ExceptionWhileReadingInputFile(this.currentFile.FullName, ex2.Message, ex2), ErrorCategory.ReadError, this.currentFile);
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
				streamReader = null;
			}
			base.WriteProgress(Strings.AirSyncReportingProgressActivity, Strings.AirSyncReportingProgressParsing(this.currentFile.FullName), 100);
			this.fileCount++;
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			if (this.fileCount == 0)
			{
				return;
			}
			string[] array = new string[]
			{
				"Users.csv",
				"Servers.csv",
				"Hourly.csv",
				"StatusCodes.csv",
				"PolicyCompliance.csv",
				"UserAgents.csv"
			};
			string text = "";
			if (this.outputPath != null)
			{
				text = this.outputPath.FullName;
				if (!Directory.Exists(text))
				{
					base.WriteError(new OutputDirectoryNotExist(text), ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(text, this.outputPrefix + array[i]));
				if (!fileInfo.Exists || !(this.Force == false) || base.ShouldContinue(Strings.OutputFileExists(fileInfo.FullName)))
				{
					StreamWriter streamWriter = null;
					try
					{
						streamWriter = new StreamWriter(fileInfo.FullName, false, Encoding.UTF8);
						switch (i)
						{
						case 0:
							this.parser.WriteUserResults(streamWriter);
							break;
						case 1:
							this.parser.WriteServerResults(streamWriter);
							break;
						case 2:
							this.parser.WriteHourlyResults(streamWriter);
							break;
						case 3:
							this.parser.WriteStatusCodeResults(streamWriter);
							break;
						case 4:
							this.parser.WritePolicyComplianceResults(streamWriter);
							break;
						case 5:
							this.parser.WriteUserAgentResults(streamWriter);
							break;
						}
					}
					catch (IOException ex)
					{
						base.WriteError(new ExceptionWhileWritingOutputFile(fileInfo.FullName, ex.Message, ex), ErrorCategory.WriteError, this.currentFile);
					}
					catch (UnauthorizedAccessException ex2)
					{
						base.WriteError(new ExceptionWhileWritingOutputFile(fileInfo.FullName, ex2.Message, ex2), ErrorCategory.WriteError, this.currentFile);
					}
					finally
					{
						if (streamWriter != null)
						{
							streamWriter.Close();
						}
						streamWriter = null;
					}
					base.WriteObject(fileInfo);
				}
			}
		}

		private ExportMobileSyncLog.AirSyncLogParser parser;

		private FileInfo currentFile;

		private DateTime startTime = DateTime.MinValue;

		private DateTime endTime = DateTime.MaxValue;

		private SwitchParameter useGMT;

		private string outputPrefix = string.Empty;

		private DirectoryInfo outputPath;

		private SwitchParameter force;

		private int fileCount;

		internal enum PolicyCompliance
		{
			Unknown,
			Full,
			Partial,
			NotCompliant
		}

		internal enum ColumnID
		{
			dateColumn,
			timeColumn,
			userNameColumn,
			uriQueryColumn,
			bytesSentColumn,
			bytesReceivedColumn,
			computerNameColumn,
			uriStemColumn,
			hostColumn,
			ipAddressColumn,
			userAgentColumn,
			statusColumn,
			subStatusColumn,
			maxColumn
		}

		private enum QueryPatternGroup
		{
			Cmd = 1,
			DeviceId,
			DeviceType,
			Ty,
			CliA,
			CliC,
			SrvA = 10,
			SrvC,
			SrvAA = 16,
			Oof = 18,
			Ssp,
			Unc,
			Att,
			Attb,
			Pa
		}

		internal class UserRow
		{
			public string Alias { get; set; }

			public string DeviceID { get; set; }

			public string DeviceType { get; set; }

			public ulong ItemsSent { get; set; }

			public ulong ItemsReceived { get; set; }

			public ulong Hits { get; set; }

			public ulong BytesSent { get; set; }

			public ulong BytesReceived { get; set; }

			public ulong EmailsSent { get; set; }

			public ulong EmailsReceived { get; set; }

			public ulong CalendarsSent { get; set; }

			public ulong CalendarsReceived { get; set; }

			public ulong ContactsSent { get; set; }

			public ulong ContactsReceived { get; set; }

			public ulong TasksSent { get; set; }

			public ulong TasksReceived { get; set; }

			public ulong NotesSent { get; set; }

			public ulong NotesReceived { get; set; }

			public ulong NumberOfOOFSet { get; set; }

			public ulong NumberOfOOFGet { get; set; }

			public ulong SearchRequests { get; set; }

			public ulong SharePointHits { get; set; }

			public ulong UncHits { get; set; }

			public ulong AttachmentHits { get; set; }

			public ulong AttachmentBytes { get; set; }

			public DateTime LastPolicyTime { get; set; }

			public ExportMobileSyncLog.PolicyCompliance PolicyCompliance { get; set; }
		}

		internal class ServerRow
		{
			public string ComputerName { get; set; }

			public string HostName { get; set; }

			public string IPAddress { get; set; }

			public Dictionary<string, Dictionary<string, bool>> DevicesPerDay { get; set; }

			public ulong Hits { get; set; }

			public ulong BytesSent { get; set; }

			public ulong BytesReceived { get; set; }
		}

		internal class HourlyRow
		{
			public DayOfWeek Day { get; set; }

			public uint Hour { get; set; }

			public Dictionary<string, bool> Devices { get; set; }

			public ulong SyncCount { get; set; }
		}

		internal class StatusRow
		{
			public string Status { get; set; }

			public string SubStatus { get; set; }

			public ulong Hits { get; set; }
		}

		internal class UserAgentRow
		{
			public string UserAgent { get; set; }

			public ulong Hits { get; set; }

			public Dictionary<string, bool> Devices { get; set; }
		}

		internal class AirSyncLogParser
		{
			public AirSyncLogParser()
			{
				this.startTime = DateTime.MinValue;
				this.endTime = DateTime.MaxValue;
				this.hourlyTable = new ExportMobileSyncLog.HourlyRow[168];
				for (uint num = 0U; num < 7U; num += 1U)
				{
					for (uint num2 = 0U; num2 < 24U; num2 += 1U)
					{
						ExportMobileSyncLog.HourlyRow hourlyRow = new ExportMobileSyncLog.HourlyRow();
						hourlyRow.Devices = new Dictionary<string, bool>();
						hourlyRow.Day = (DayOfWeek)num;
						hourlyRow.Hour = num2;
						this.hourlyTable[(int)((UIntPtr)(num * 24U + num2))] = hourlyRow;
					}
				}
				this.fieldsPattern = new Regex("^#\\s*Fields\\s*:\\s*(.*?)\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				this.valuesPattern = new Regex("(\\S+)(?:\\s+|$)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				this.queryPattern = new Regex("^(?:(?:^|&)(?:(?i:Cmd)=(?<Cmd>[^&]*)|(?i:DeviceId)=(?<DeviceId>[^&]*)|(?i:DeviceType)=(?<DeviceType>[^&]*)|(?i:Log)=(?:(?:Ty:(?<Ty>[^_]+)|Cli:(?<CliA>[0-9]+)a(?<CliC>[0-9]+)c(?<CliD>[0-9]+)d(?<CliF>[0-9]+)f(?<CliE>[0-9]+)e|Srv:(?<SrvA>[0-9]+)a(?<SrvC>[0-9]+)c(?<SrvD>[0-9]+)d(?<SrvS>[0-9]+)s(?<SrvE>[0-9]+)e(?<SrvR>[0-9]+)r(?<SrvAA>[0-9]+)A(?<SrvSD>[0-9]+)sd|Oof:(?<Oof>[^_]*)|Ssp(?<Ssp>[0-9]+)|Unc(?<Unc>[0-9]+)|Att(?<Att>[0-9]+)|Attb(?<Attb>[0-9]+)|Pa(?<Pa>[0-9]+)|[a-zA-Z]+(?:[0-9]+|:[^_]*))(?:_|$|(?=&)))*|[^=]+=[^&]*))*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				this.columnNameToID = new Dictionary<string, ExportMobileSyncLog.ColumnID>();
				this.columnNameToID.Add("date", ExportMobileSyncLog.ColumnID.dateColumn);
				this.columnNameToID.Add("time", ExportMobileSyncLog.ColumnID.timeColumn);
				this.columnNameToID.Add("cs-username", ExportMobileSyncLog.ColumnID.userNameColumn);
				this.columnNameToID.Add("cs-uri-query", ExportMobileSyncLog.ColumnID.uriQueryColumn);
				this.columnNameToID.Add("sc-bytes", ExportMobileSyncLog.ColumnID.bytesSentColumn);
				this.columnNameToID.Add("cs-bytes", ExportMobileSyncLog.ColumnID.bytesReceivedColumn);
				this.columnNameToID.Add("s-computername", ExportMobileSyncLog.ColumnID.computerNameColumn);
				this.columnNameToID.Add("cs-uri-stem", ExportMobileSyncLog.ColumnID.uriStemColumn);
				this.columnNameToID.Add("cs-host", ExportMobileSyncLog.ColumnID.hostColumn);
				this.columnNameToID.Add("s-ip", ExportMobileSyncLog.ColumnID.ipAddressColumn);
				this.columnNameToID.Add("cs(user-agent)", ExportMobileSyncLog.ColumnID.userAgentColumn);
				this.columnNameToID.Add("sc-status", ExportMobileSyncLog.ColumnID.statusColumn);
				this.columnNameToID.Add("sc-substatus", ExportMobileSyncLog.ColumnID.subStatusColumn);
			}

			public DateTime StartTime
			{
				get
				{
					return this.startTime;
				}
				set
				{
					this.startTime = value;
				}
			}

			public DateTime EndTime
			{
				get
				{
					return this.endTime;
				}
				set
				{
					this.endTime = value;
				}
			}

			public void StartNewFile()
			{
				this.columnCount = 0;
			}

			public void ParseLine(string line)
			{
				Match match = null;
				if (line.Length == 0)
				{
					return;
				}
				if (line[0] == '#')
				{
					match = this.fieldsPattern.Match(line);
					if (match.Success)
					{
						string value = match.Groups[1].Value;
						MatchCollection matchCollection = this.valuesPattern.Matches(value);
						this.columnCount = matchCollection.Count;
						this.columnIndexToID = new ExportMobileSyncLog.ColumnID[this.columnCount];
						for (int i = 0; i < this.columnCount; i++)
						{
							string key = matchCollection[i].Groups[1].Value.ToLower(CultureInfo.InvariantCulture);
							ExportMobileSyncLog.ColumnID columnID;
							if (this.columnNameToID.TryGetValue(key, out columnID))
							{
								this.columnIndexToID[i] = columnID;
							}
							else
							{
								this.columnIndexToID[i] = ExportMobileSyncLog.ColumnID.maxColumn;
							}
						}
					}
					return;
				}
				if (this.columnCount == 0)
				{
					return;
				}
				this.columnValues = new string[13];
				int num = 0;
				int num2 = 0;
				int j = 0;
				while (j < line.Length)
				{
					while (num2 < line.Length && line[num2] == ' ')
					{
						num2++;
					}
					j = num2;
					while (j < line.Length && line[j] != ' ')
					{
						j++;
					}
					if (j == num2)
					{
						break;
					}
					if (num >= this.columnCount)
					{
						return;
					}
					ExportMobileSyncLog.ColumnID columnID2 = this.columnIndexToID[num];
					if (columnID2 != ExportMobileSyncLog.ColumnID.maxColumn)
					{
						string text = line.Substring(num2, j - num2);
						if (text.Length == 1 && text[0] == '-')
						{
							text = null;
						}
						this.columnValues[(int)columnID2] = text;
					}
					num++;
					num2 = j;
				}
				if (num != this.columnCount)
				{
					return;
				}
				string strA = this.columnValues[7];
				if (string.Compare(strA, "/Microsoft-Server-ActiveSync", true, CultureInfo.InvariantCulture) != 0 && string.Compare(strA, "/Microsoft-Server-ActiveSync/default.eas", true, CultureInfo.InvariantCulture) != 0)
				{
					return;
				}
				string text2 = this.columnValues[0];
				string text3 = this.columnValues[1];
				if (text2 == null || text3 == null)
				{
					return;
				}
				DateTime dateTime;
				if (!DateTime.TryParse(text2 + " " + text3, out dateTime))
				{
					return;
				}
				if (dateTime < this.startTime || dateTime > this.endTime)
				{
					return;
				}
				string text4 = this.columnValues[3];
				string text5 = this.columnValues[2];
				string text6 = this.columnValues[6];
				string text7 = this.columnValues[8];
				string text8 = this.columnValues[9];
				string text9 = this.columnValues[11];
				string text10 = this.columnValues[12];
				string text11 = this.columnValues[10];
				ulong num3 = 0UL;
				ulong num4 = 0UL;
				ulong.TryParse(this.columnValues[4], out num3);
				ulong.TryParse(this.columnValues[5], out num4);
				string strA2 = null;
				string text12 = null;
				string deviceType = null;
				uint num5 = 0U;
				uint num6 = 0U;
				uint num7 = 0U;
				uint num8 = 0U;
				string strA3 = null;
				uint num9 = 0U;
				uint num10 = 0U;
				uint num11 = 0U;
				uint num12 = 0U;
				uint num13 = 0U;
				uint num14 = 0U;
				if (text4 != null)
				{
					match = this.queryPattern.Match(text4);
					if (!match.Success)
					{
						return;
					}
					strA2 = match.Groups[1].Value;
					text12 = match.Groups[2].Value;
					deviceType = match.Groups[3].Value;
					strA3 = match.Groups[18].Value;
					uint.TryParse(match.Groups[19].Value, out num9);
					uint.TryParse(match.Groups[20].Value, out num10);
					uint.TryParse(match.Groups[21].Value, out num11);
					uint.TryParse(match.Groups[22].Value, out num12);
					uint.TryParse(match.Groups[23].Value, out num13);
				}
				string text13 = "";
				if (!string.IsNullOrEmpty(text5))
				{
					text13 += text5.ToLower(CultureInfo.InvariantCulture);
				}
				text13 += "_";
				if (!string.IsNullOrEmpty(text12))
				{
					text13 += text12.ToLower(CultureInfo.InvariantCulture);
				}
				if (text13.Length > 1)
				{
					ExportMobileSyncLog.UserRow userRow;
					if (!this.userTable.TryGetValue(text13, out userRow))
					{
						userRow = new ExportMobileSyncLog.UserRow();
						userRow.Alias = text5;
						userRow.DeviceID = text12;
						userRow.DeviceType = deviceType;
						this.userTable.Add(text13, userRow);
					}
					if (text4 != null)
					{
						int count = match.Groups[4].Captures.Count;
						if (count > 0)
						{
							int num15 = match.Groups[10].Captures.Count - 1;
							int num16 = match.Groups[5].Captures.Count - 1;
							for (int k = count - 1; k >= 0; k--)
							{
								num6 = (num5 = (num7 = (num8 = (num14 = 0U))));
								int index = match.Groups[4].Captures[k].Index;
								string value2 = match.Groups[4].Captures[k].Value;
								if (num16 >= 0 && match.Groups[5].Captures.Count > num16 && index < match.Groups[5].Captures[num16].Index)
								{
									uint.TryParse(match.Groups[5].Captures[num16].Value, out num5);
									uint.TryParse(match.Groups[6].Captures[num16].Value, out num6);
									num16--;
								}
								if (num15 >= 0 && match.Groups[10].Captures.Count > num15 && index < match.Groups[10].Captures[num15].Index)
								{
									uint.TryParse(match.Groups[10].Captures[num15].Value, out num7);
									uint.TryParse(match.Groups[11].Captures[num15].Value, out num8);
									uint.TryParse(match.Groups[16].Captures[num15].Value, out num14);
									num15--;
								}
								userRow.ItemsSent += (ulong)(num7 + num8 + num14);
								userRow.ItemsReceived += (ulong)(num5 + num6);
								if (string.Compare(value2, "Em", false, CultureInfo.InvariantCulture) == 0)
								{
									userRow.EmailsSent += (ulong)(num7 + num14 + num8);
								}
								if (string.Compare(value2, "Ca", false, CultureInfo.InvariantCulture) == 0)
								{
									userRow.CalendarsSent += (ulong)(num7 + num8);
									userRow.CalendarsReceived += (ulong)(num5 + num6);
								}
								if (string.Compare(value2, "Co", false, CultureInfo.InvariantCulture) == 0)
								{
									userRow.ContactsSent += (ulong)(num7 + num8);
									userRow.ContactsReceived += (ulong)(num5 + num6);
								}
								if (string.Compare(value2, "Ta", false, CultureInfo.InvariantCulture) == 0)
								{
									userRow.TasksSent += (ulong)(num7 + num8);
									userRow.TasksReceived += (ulong)(num5 + num6);
								}
								if (string.Compare(value2, "Nt", false, CultureInfo.InvariantCulture) == 0)
								{
									userRow.NotesSent += (ulong)(num7 + num8);
									userRow.NotesReceived += (ulong)(num5 + num6);
								}
							}
						}
						else
						{
							uint.TryParse(match.Groups[5].Value, out num5);
							uint.TryParse(match.Groups[6].Value, out num6);
							uint.TryParse(match.Groups[10].Value, out num7);
							uint.TryParse(match.Groups[11].Value, out num8);
							userRow.ItemsSent += (ulong)(num7 + num8);
							userRow.ItemsReceived += (ulong)(num5 + num6);
						}
					}
					if (string.Compare(strA2, "SendMail", true, CultureInfo.InvariantCulture) == 0 || string.Compare(strA2, "SmartReply", true, CultureInfo.InvariantCulture) == 0 || string.Compare(strA2, "SmartForward", true, CultureInfo.InvariantCulture) == 0)
					{
						userRow.EmailsReceived += 1UL;
					}
					if (string.Compare(strA2, "Settings", true, CultureInfo.InvariantCulture) == 0)
					{
						if (string.Compare(strA3, "Set", true, CultureInfo.InvariantCulture) == 0)
						{
							userRow.NumberOfOOFSet += 1UL;
						}
						if (string.Compare(strA3, "Get", true, CultureInfo.InvariantCulture) == 0)
						{
							userRow.NumberOfOOFGet += 1UL;
						}
					}
					if (string.Compare(strA2, "Search", true, CultureInfo.InvariantCulture) == 0)
					{
						userRow.SearchRequests += 1UL;
					}
					userRow.Hits += 1UL;
					userRow.BytesSent += num3;
					userRow.BytesReceived += num4;
					userRow.SharePointHits += (ulong)num9;
					userRow.UncHits += (ulong)num10;
					userRow.AttachmentHits += (ulong)num11;
					userRow.AttachmentBytes += (ulong)num12;
					if (num13 > 0U && dateTime > userRow.LastPolicyTime)
					{
						userRow.LastPolicyTime = dateTime;
						userRow.PolicyCompliance = (ExportMobileSyncLog.PolicyCompliance)num13;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (text6 != null)
				{
					stringBuilder.Append(text6.ToLower(CultureInfo.InvariantCulture));
				}
				stringBuilder.Append('_');
				if (text7 != null)
				{
					stringBuilder.Append(text7.ToLower(CultureInfo.InvariantCulture));
				}
				stringBuilder.Append('_');
				if (text8 != null)
				{
					stringBuilder.Append(text8.ToLower(CultureInfo.InvariantCulture));
				}
				if (stringBuilder.Length > 2)
				{
					string key2 = stringBuilder.ToString();
					ExportMobileSyncLog.ServerRow serverRow;
					if (!this.serverTable.TryGetValue(key2, out serverRow))
					{
						serverRow = new ExportMobileSyncLog.ServerRow();
						serverRow.DevicesPerDay = new Dictionary<string, Dictionary<string, bool>>();
						serverRow.ComputerName = text6;
						serverRow.HostName = text7;
						serverRow.IPAddress = text8;
						this.serverTable.Add(key2, serverRow);
					}
					if (text12 != null)
					{
						Dictionary<string, bool> dictionary;
						if (!serverRow.DevicesPerDay.TryGetValue(text2, out dictionary))
						{
							dictionary = new Dictionary<string, bool>();
							serverRow.DevicesPerDay.Add(text2, dictionary);
						}
						dictionary[text12] = true;
					}
					serverRow.Hits += 1UL;
					serverRow.BytesSent += num3;
					serverRow.BytesReceived += num4;
				}
				ExportMobileSyncLog.HourlyRow hourlyRow = this.hourlyTable[(int)(dateTime.DayOfWeek * (DayOfWeek)24 + dateTime.Hour)];
				if (text12 != null)
				{
					hourlyRow.Devices[text12] = true;
				}
				if (string.Compare(strA2, "Sync", true, CultureInfo.InvariantCulture) == 0 || string.Compare(strA2, "GetItemEstimate", true, CultureInfo.InvariantCulture) == 0)
				{
					hourlyRow.SyncCount += 1UL;
				}
				string text14 = "";
				if (text9 != null)
				{
					text14 = text9.PadLeft(3, '0');
				}
				text14 += "_";
				if (text10 != null)
				{
					text14 += text10.PadLeft(11, '0');
				}
				if (text14.Length > 1)
				{
					ExportMobileSyncLog.StatusRow statusRow;
					if (!this.statusTable.TryGetValue(text14, out statusRow))
					{
						statusRow = new ExportMobileSyncLog.StatusRow();
						statusRow.Status = text9;
						statusRow.SubStatus = text10;
						this.statusTable.Add(text14, statusRow);
					}
					statusRow.Hits += 1UL;
				}
				if (text11 != null)
				{
					ExportMobileSyncLog.UserAgentRow userAgentRow;
					if (!this.userAgentTable.TryGetValue(text11, out userAgentRow))
					{
						userAgentRow = new ExportMobileSyncLog.UserAgentRow();
						userAgentRow.Devices = new Dictionary<string, bool>();
						userAgentRow.UserAgent = text11;
						this.userAgentTable.Add(text11, userAgentRow);
					}
					userAgentRow.Hits += 1UL;
					if (text12 != null)
					{
						userAgentRow.Devices[text12] = true;
					}
				}
			}

			public void WriteUserResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"User Name",
					"Device ID",
					"Device Type",
					"Items Sent",
					"Items Received",
					"Hits",
					"Total Bytes Sent",
					"Total Bytes Received",
					"Total Emails Sent",
					"Total Emails Received",
					"Total Calendar Sent",
					"Total Calendar Received",
					"Total Contacts Sent",
					"Total Contacts Received",
					"Total Tasks Sent",
					"Total Tasks Received",
					"Total Notes Sent",
					"Total Notes Received",
					"Total OOF Messages Set by Client",
					"Total OOF Messages Retrieved from Server",
					"Total Searches requested",
					"Total SharePoint Access",
					"Total UNC Access",
					"Total Attachment Downloads",
					"Total Attachment Size",
					"Currently Compliant with Policy"
				};
				string[] array2 = new string[]
				{
					"Unknown",
					"Compliant",
					"Partially Compliant",
					"Not Compliant"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				string[] array3 = new string[array.Length];
				ulong num = 0UL;
				ulong num2 = 0UL;
				ulong num3 = 0UL;
				ulong num4 = 0UL;
				ulong num5 = 0UL;
				string[] array4 = new string[this.userTable.Count];
				this.userTable.Keys.CopyTo(array4, 0);
				Array.Sort<string>(array4);
				foreach (string key in array4)
				{
					ExportMobileSyncLog.UserRow userRow = this.userTable[key];
					Array.Clear(array3, 0, array3.Length);
					array3[0] = userRow.Alias;
					array3[1] = userRow.DeviceID;
					array3[2] = userRow.DeviceType;
					array3[3] = userRow.ItemsSent.ToString();
					array3[4] = userRow.ItemsReceived.ToString();
					array3[5] = userRow.Hits.ToString();
					array3[6] = userRow.BytesSent.ToString();
					array3[7] = userRow.BytesReceived.ToString();
					array3[8] = userRow.EmailsSent.ToString();
					array3[9] = userRow.EmailsReceived.ToString();
					array3[10] = userRow.CalendarsSent.ToString();
					array3[11] = userRow.CalendarsReceived.ToString();
					array3[12] = userRow.ContactsSent.ToString();
					array3[13] = userRow.ContactsReceived.ToString();
					array3[14] = userRow.TasksSent.ToString();
					array3[15] = userRow.TasksReceived.ToString();
					array3[16] = userRow.NotesSent.ToString();
					array3[17] = userRow.NotesReceived.ToString();
					array3[18] = userRow.NumberOfOOFSet.ToString();
					array3[19] = userRow.NumberOfOOFGet.ToString();
					array3[20] = userRow.SearchRequests.ToString();
					array3[21] = userRow.SharePointHits.ToString();
					array3[22] = userRow.UncHits.ToString();
					array3[23] = userRow.AttachmentHits.ToString();
					array3[24] = userRow.AttachmentBytes.ToString();
					array3[25] = array2[(int)userRow.PolicyCompliance];
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array3);
					num += userRow.ItemsSent;
					num2 += userRow.ItemsReceived;
					num3 += userRow.Hits;
					num4 += userRow.BytesSent;
					num5 += userRow.BytesReceived;
				}
				Array.Clear(array3, 0, array3.Length);
				array3[0] = "*** Total ***";
				array3[3] = num.ToString();
				array3[4] = num2.ToString();
				array3[5] = num3.ToString();
				array3[6] = num4.ToString();
				array3[7] = num5.ToString();
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array3);
			}

			public void WriteServerResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"Server",
					"Host",
					"IP Address",
					"Average Unique Devices",
					"Hits",
					"Total Bytes Sent",
					"Total Bytes Received"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				string[] array2 = new string[array.Length];
				string[] array3 = new string[this.serverTable.Count];
				this.serverTable.Keys.CopyTo(array3, 0);
				Array.Sort<string>(array3);
				foreach (string key in array3)
				{
					ExportMobileSyncLog.ServerRow serverRow = this.serverTable[key];
					Array.Clear(array2, 0, array2.Length);
					uint num = 0U;
					uint num2 = 0U;
					foreach (Dictionary<string, bool> dictionary in serverRow.DevicesPerDay.Values)
					{
						num += 1U;
						num2 += (uint)dictionary.Count;
					}
					uint num3 = 0U;
					if (num > 0U)
					{
						num3 = num2 / num;
					}
					array2[0] = serverRow.ComputerName;
					array2[1] = serverRow.HostName;
					array2[2] = serverRow.IPAddress;
					array2[3] = num3.ToString();
					array2[4] = serverRow.Hits.ToString();
					array2[5] = serverRow.BytesSent.ToString();
					array2[6] = serverRow.BytesReceived.ToString();
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array2);
				}
			}

			public void WriteHourlyResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"Day",
					"Hour",
					"Unique Devices",
					"Sync Related Requests"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				string[] array2 = new string[array.Length];
				foreach (ExportMobileSyncLog.HourlyRow hourlyRow in this.hourlyTable)
				{
					Array.Clear(array2, 0, array2.Length);
					array2[0] = hourlyRow.Day.ToString();
					array2[1] = hourlyRow.Hour.ToString();
					array2[2] = hourlyRow.Devices.Count.ToString();
					array2[3] = hourlyRow.SyncCount.ToString();
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array2);
				}
			}

			public void WriteStatusCodeResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"Status",
					"SubStatus",
					"Hits",
					"Ratio"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				ulong num = 0UL;
				foreach (ExportMobileSyncLog.StatusRow statusRow in this.statusTable.Values)
				{
					num += statusRow.Hits;
				}
				string[] array2 = new string[array.Length];
				string[] array3 = new string[this.statusTable.Count];
				this.statusTable.Keys.CopyTo(array3, 0);
				Array.Sort<string>(array3);
				foreach (string key in array3)
				{
					ExportMobileSyncLog.StatusRow statusRow2 = this.statusTable[key];
					Array.Clear(array2, 0, array2.Length);
					array2[0] = statusRow2.Status;
					array2[1] = statusRow2.SubStatus;
					array2[2] = statusRow2.Hits.ToString();
					if (num > 0UL)
					{
						array2[3] = (statusRow2.Hits / num).ToString();
					}
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array2);
				}
			}

			public void WritePolicyComplianceResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"Compliance Type",
					"Total",
					"Ratio"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				ulong[] array2 = new ulong[this.policyComplianceStrings.Length];
				foreach (ExportMobileSyncLog.UserRow userRow in this.userTable.Values)
				{
					array2[(int)userRow.PolicyCompliance] += 1UL;
				}
				string[] array3 = new string[array.Length];
				for (int i = 0; i < this.policyComplianceStrings.Length; i++)
				{
					Array.Clear(array3, 0, array3.Length);
					array3[0] = this.policyComplianceStrings[i];
					array3[1] = array2[i].ToString();
					if (this.userTable.Count > 0)
					{
						array3[2] = (array2[i] / (float)this.userTable.Count).ToString();
					}
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array3);
				}
			}

			public void WriteUserAgentResults(StreamWriter writer)
			{
				string[] array = new string[]
				{
					"User Agent",
					"Hits",
					"Unique Devices"
				};
				ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array);
				string[] array2 = new string[array.Length];
				string[] array3 = new string[this.userAgentTable.Count];
				this.userAgentTable.Keys.CopyTo(array3, 0);
				Array.Sort<string>(array3);
				foreach (string key in array3)
				{
					ExportMobileSyncLog.UserAgentRow userAgentRow = this.userAgentTable[key];
					Array.Clear(array2, 0, array2.Length);
					array2[0] = userAgentRow.UserAgent;
					array2[1] = userAgentRow.Hits.ToString();
					array2[2] = userAgentRow.Devices.Count.ToString();
					ExportMobileSyncLog.AirSyncLogParser.WriteCSVLine(writer, array2);
				}
			}

			private static void WriteCSVLine(StreamWriter writer, string[] values)
			{
				for (int i = 0; i < values.Length; i++)
				{
					if (i > 0)
					{
						writer.Write(',');
					}
					if (values[i] != null)
					{
						if (values[i].IndexOf(',') == -1)
						{
							writer.Write(values[i]);
						}
						else
						{
							writer.Write('"');
							writer.Write(values[i].Replace("\"", "\"\""));
							writer.Write('"');
						}
					}
				}
				writer.WriteLine();
			}

			private DateTime startTime;

			private DateTime endTime;

			private Dictionary<string, ExportMobileSyncLog.UserRow> userTable = new Dictionary<string, ExportMobileSyncLog.UserRow>();

			private ExportMobileSyncLog.HourlyRow[] hourlyTable;

			private Dictionary<string, ExportMobileSyncLog.ServerRow> serverTable = new Dictionary<string, ExportMobileSyncLog.ServerRow>();

			private Dictionary<string, ExportMobileSyncLog.StatusRow> statusTable = new Dictionary<string, ExportMobileSyncLog.StatusRow>();

			private Dictionary<string, ExportMobileSyncLog.UserAgentRow> userAgentTable = new Dictionary<string, ExportMobileSyncLog.UserAgentRow>();

			private Regex fieldsPattern;

			private Regex valuesPattern;

			private Regex queryPattern;

			private int columnCount;

			private ExportMobileSyncLog.ColumnID[] columnIndexToID;

			private Dictionary<string, ExportMobileSyncLog.ColumnID> columnNameToID;

			private string[] columnValues;

			private string[] policyComplianceStrings = new string[]
			{
				"Unknown",
				"Compliant",
				"Partially Compliant",
				"Not Compliant"
			};
		}
	}
}
