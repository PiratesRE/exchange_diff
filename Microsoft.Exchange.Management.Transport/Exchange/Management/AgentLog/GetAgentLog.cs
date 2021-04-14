using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.AgentLog
{
	[Cmdlet("Get", "AgentLog")]
	public sealed class GetAgentLog : Task
	{
		[Parameter(Mandatory = false)]
		public TransportService TransportService
		{
			get
			{
				return this.transportService.Value;
			}
			set
			{
				this.transportService = new TransportService?(value);
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
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

		protected override void InternalValidate()
		{
			if (this.transportService == null)
			{
				this.transportService = new TransportService?(this.GetDefaultTransportServiceRole());
			}
			string text;
			if (this.location == null || string.IsNullOrEmpty(this.location.PathName))
			{
				LocalLongFullPath agentLogPath = this.GetAgentLogPath(this.transportService.Value);
				text = ((agentLogPath != null) ? agentLogPath.PathName : string.Empty);
			}
			else
			{
				text = this.location.PathName;
			}
			bool flag = File.Exists(text);
			bool flag2 = Directory.Exists(text);
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.AgentLogInvalidLocation(text), "Location"), ErrorCategory.InvalidArgument, null);
			}
			if (this.startTime.CompareTo(this.endTime) > 0)
			{
				base.WriteError(new ArgumentException(Strings.AgentLogInvalidTimeRange, "StartTime, EndTime"), ErrorCategory.InvalidArgument, null);
			}
			if (flag)
			{
				this.files = new string[]
				{
					text
				};
			}
			else
			{
				this.files = Directory.GetFiles(text);
			}
			if (this.GetNextFileIndex() == -1)
			{
				base.WriteError(new ArgumentException(Strings.AgentLogNoLogsAtLocation(text), "Location"), ErrorCategory.InvalidArgument, null);
			}
		}

		private LocalLongFullPath GetAgentLogPath(TransportService transportService)
		{
			GetAgentLog.<>c__DisplayClass3 CS$<>8__locals1 = new GetAgentLog.<>c__DisplayClass3();
			CS$<>8__locals1.session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 182, "GetAgentLogPath", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\AgentLog\\GetAgentLog.cs");
			CS$<>8__locals1.localServer = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				CS$<>8__locals1.localServer = CS$<>8__locals1.session.FindLocalServer();
			});
			if (!adoperationResult.Succeeded)
			{
				base.WriteError(adoperationResult.Exception, ErrorCategory.ReadError, null);
			}
			if (transportService == TransportService.FrontEnd)
			{
				FrontendTransportServer[] feServer = null;
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					feServer = (FrontendTransportServer[])CS$<>8__locals1.session.Find<FrontendTransportServer>(null, CS$<>8__locals1.localServer.Id, true, null);
				});
				if (!adoperationResult.Succeeded)
				{
					base.WriteError(adoperationResult.Exception, ErrorCategory.ReadError, null);
				}
				if (feServer == null || feServer.Length == 0)
				{
					base.WriteError(new LocalServerNotFoundException(string.Format("Local server does not contain transport service {0}", new object[0])), ErrorCategory.InvalidOperation, null);
				}
				return feServer[0].AgentLogPath;
			}
			if (transportService != TransportService.MailboxDelivery && transportService != TransportService.MailboxSubmission)
			{
				return CS$<>8__locals1.localServer.AgentLogPath;
			}
			MailboxTransportServer[] mbxServer = null;
			adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				mbxServer = (MailboxTransportServer[])CS$<>8__locals1.session.Find<MailboxTransportServer>(null, CS$<>8__locals1.localServer.Id, true, null);
			});
			if (!adoperationResult.Succeeded)
			{
				base.WriteError(adoperationResult.Exception, ErrorCategory.ReadError, null);
			}
			if (mbxServer == null || mbxServer.Length == 0)
			{
				base.WriteError(new LocalServerNotFoundException(string.Format("Local server does not contain transport service {0}", new object[0])), ErrorCategory.InvalidOperation, null);
			}
			if (transportService == TransportService.MailboxDelivery)
			{
				return mbxServer[0].MailboxDeliveryAgentLogPath;
			}
			return mbxServer[0].MailboxDeliveryAgentLogPath;
		}

		private TransportService GetDefaultTransportServiceRole()
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 258, "GetDefaultTransportServiceRole", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\AgentLog\\GetAgentLog.cs");
			Server localServer = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localServer = session.FindLocalServer();
			});
			if (!adoperationResult.Succeeded)
			{
				base.WriteError(adoperationResult.Exception, ErrorCategory.ReadError, null);
			}
			ServerRole currentServerRole = localServer.CurrentServerRole;
			if ((currentServerRole & ServerRole.HubTransport) != ServerRole.None)
			{
				return TransportService.Hub;
			}
			if ((currentServerRole & ServerRole.Edge) != ServerRole.None)
			{
				return TransportService.Edge;
			}
			if ((currentServerRole & ServerRole.FrontendTransport) != ServerRole.None)
			{
				return TransportService.FrontEnd;
			}
			if ((currentServerRole & ServerRole.Mailbox) != ServerRole.None)
			{
				return TransportService.MailboxDelivery;
			}
			return TransportService.Hub;
		}

		private bool IsInRange(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				base.WriteError(new LocalizedException(Strings.ExceptionLogFileInvalid(path)), ErrorCategory.InvalidData, path);
				return false;
			}
			string fileName = Path.GetFileName(path);
			string extension = Path.GetExtension(fileName);
			if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
			{
				base.WriteError(new LocalizedException(Strings.ExceptionLogFileInvalid(path)), ErrorCategory.InvalidData, path);
				return false;
			}
			if (!string.Equals(extension, ".LOG", StringComparison.OrdinalIgnoreCase) || !fileName.StartsWith("AGENTLOG", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteError(new LocalizedException(Strings.ExceptionLogFileInvalid(path)), ErrorCategory.InvalidData, path);
				return false;
			}
			DateTime value = DateTime.MinValue;
			DateTime value2 = DateTime.MaxValue;
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				StreamReader streamReader = new StreamReader(fileStream);
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine().ToLowerInvariant();
					if (text.IndexOf("#date:") == 0)
					{
						value = DateTime.Parse(text.Substring("#date:".Length + 1), DateTimeFormatInfo.InvariantInfo);
						break;
					}
				}
			}
			catch (Exception innerException)
			{
				base.WriteError(new LocalizedException(Strings.ExceptionLogFileInvalid(path), innerException), ErrorCategory.InvalidData, path);
				return false;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			value2 = File.GetLastWriteTime(path);
			return this.startTime.CompareTo(value2) <= 0 && this.endTime.CompareTo(value) >= 0;
		}

		private bool IsInRange(DateTime date)
		{
			return this.startTime.CompareTo(date) <= 0 && this.endTime.CompareTo(date) >= 0;
		}

		private int GetNextFileIndex()
		{
			for (int i = this.currentFileIndex + 1; i < this.files.Length; i++)
			{
				if (this.IsInRange(this.files[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private bool OpenNextFile()
		{
			if (this.reader != null)
			{
				this.reader.Close();
			}
			this.currentFileIndex = this.GetNextFileIndex();
			if (this.currentFileIndex != -1)
			{
				try
				{
					FileStream stream = new FileStream(this.files[this.currentFileIndex], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					this.reader = new StreamReader(stream);
					return true;
				}
				catch (Exception innerException)
				{
					base.WriteError(new LocalizedException(Strings.ExceptionLogFileInvalid(this.files[this.currentFileIndex]), innerException), ErrorCategory.InvalidData, this.files[this.currentFileIndex]);
				}
			}
			this.reader = null;
			return false;
		}

		protected override void InternalProcessRecord()
		{
			int num = 0;
			AgentLogEntry agentLogEntry = null;
			while (this.OpenNextFile())
			{
				try
				{
					int num2 = 0;
					while (!this.reader.EndOfStream)
					{
						string text = this.reader.ReadLine();
						num2++;
						try
						{
							if (!this.IsHeader(text))
							{
								string[] array = this.SplitFields(text);
								if (num > 0)
								{
									agentLogEntry.Recipients.Add(new AgentLogEntry.RoutingAddressWrapper((RoutingAddress)array[8]));
									num--;
									if (num == 0)
									{
										base.WriteObject(agentLogEntry);
									}
								}
								else
								{
									DateTime dateTime = DateTime.Parse(array[0], DateTimeFormatInfo.InvariantInfo);
									if (this.IsInRange(dateTime))
									{
										agentLogEntry = new AgentLogEntry();
										agentLogEntry.Timestamp = dateTime;
										agentLogEntry.SessionId = array[1];
										if (!string.IsNullOrEmpty(array[4]))
										{
											agentLogEntry.IPAddress = IPAddress.Parse(array[4]);
										}
										agentLogEntry.MessageId = array[5];
										agentLogEntry.P1FromAddress = new AgentLogEntry.RoutingAddressWrapper((RoutingAddress)array[6]);
										agentLogEntry.P2FromAddresses = this.ParseP2From(array[7]);
										agentLogEntry.Agent = array[10];
										agentLogEntry.Event = array[11];
										agentLogEntry.Action = (AgentAction)Enum.Parse(typeof(AgentAction), array[12]);
										if (!string.IsNullOrEmpty(array[13]) && !SmtpResponse.TryParse(array[13], out agentLogEntry.SmtpResponse))
										{
											this.WriteError(new ArgumentException(Strings.AgentLogInvalidSmtpResponse(num2, this.GetCurrentFilePath()), "smtpResponse"), ErrorCategory.InvalidArgument, null, false);
										}
										else
										{
											agentLogEntry.Reason = array[14];
											agentLogEntry.ReasonData = array[15];
											if (array.Length > 16)
											{
												agentLogEntry.Diagnostics = array[16];
											}
											if (array.Length > 17)
											{
												Guid.TryParse(array[17], out agentLogEntry.NetworkMsgID);
											}
											if (array.Length > 18)
											{
												Guid.TryParse(array[18], out agentLogEntry.TenantID);
											}
											if (array.Length > 19)
											{
												agentLogEntry.Directionality = array[19];
											}
											num = int.Parse(array[9], NumberFormatInfo.InvariantInfo);
											agentLogEntry.Recipients = new List<AgentLogEntry.RoutingAddressWrapper>(num);
											if (num > 0)
											{
												string text2 = array[8];
												string[] array2 = text2.Split(new char[]
												{
													';'
												});
												if (array2.Length != num && array2.Length != 1)
												{
													throw new FormatException("Invalid log format, the recipient count field does not match the actual number of recipients found");
												}
												num -= array2.Length;
												foreach (string address in array2)
												{
													agentLogEntry.Recipients.Add(new AgentLogEntry.RoutingAddressWrapper((RoutingAddress)address));
												}
												if (num > 0)
												{
													continue;
												}
											}
											base.WriteObject(agentLogEntry);
										}
									}
								}
							}
						}
						catch (ArgumentException e)
						{
							this.WriteError(this.CreateEntryException(e, num2), ErrorCategory.InvalidData, text, false);
						}
						catch (FormatException e2)
						{
							this.WriteError(this.CreateEntryException(e2, num2), ErrorCategory.InvalidData, text, false);
						}
						catch (OverflowException e3)
						{
							this.WriteError(this.CreateEntryException(e3, num2), ErrorCategory.InvalidData, text, false);
						}
						catch (IndexOutOfRangeException e4)
						{
							this.WriteError(this.CreateEntryException(e4, num2), ErrorCategory.InvalidData, text, false);
						}
					}
				}
				finally
				{
					if (this.reader != null)
					{
						this.reader.Close();
						this.reader = null;
					}
				}
			}
		}

		private LocalizedException CreateEntryException(Exception e, int lineNumber)
		{
			if (this.currentFileIndex >= 0 && this.currentFileIndex < this.files.Length)
			{
				return new LocalizedException(Strings.ExceptionLogEntryLineInvalidFile(lineNumber, this.files[this.currentFileIndex]), e);
			}
			return new LocalizedException(Strings.ExceptionLogEntryLineInvalid(lineNumber), e);
		}

		private bool IsHeader(string line)
		{
			return line.StartsWith("#");
		}

		private string[] SplitFields(string line)
		{
			string[] array = GetAgentLog.FieldMatcher.Split(line);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].StartsWith("\"") && array[i].EndsWith("\""))
				{
					StringBuilder stringBuilder = new StringBuilder(array[i]);
					stringBuilder.Remove(0, 1);
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Replace("\"\"", "\"");
					array[i] = stringBuilder.ToString();
				}
			}
			return array;
		}

		private List<AgentLogEntry.RoutingAddressWrapper> ParseP2From(string from)
		{
			if (from.EndsWith(';'.ToString()))
			{
				from = from.Substring(0, from.Length - 1);
			}
			string[] array = from.Split(new char[]
			{
				';'
			});
			List<AgentLogEntry.RoutingAddressWrapper> list = new List<AgentLogEntry.RoutingAddressWrapper>(array.Length);
			foreach (string address in array)
			{
				list.Add(new AgentLogEntry.RoutingAddressWrapper((RoutingAddress)address));
			}
			return list;
		}

		private string GetCurrentFilePath()
		{
			if (this.currentFileIndex < 0 || this.currentFileIndex >= this.files.Length)
			{
				return string.Empty;
			}
			return this.files[this.currentFileIndex];
		}

		private const int NoMoreFiles = -1;

		private const string FieldPattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";

		private const string LogfileName = "AGENTLOG";

		private const string LogfileExtension = ".LOG";

		private static readonly Regex FieldMatcher = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

		private TransportService? transportService;

		private LocalLongFullPath location;

		private DateTime startTime = DateTime.MinValue;

		private DateTime endTime = DateTime.MaxValue;

		private string[] files;

		private int currentFileIndex = -1;

		private StreamReader reader;
	}
}
