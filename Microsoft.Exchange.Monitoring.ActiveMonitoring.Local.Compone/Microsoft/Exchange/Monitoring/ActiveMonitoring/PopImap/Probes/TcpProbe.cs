using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public abstract class TcpProbe : ProbeWorkItem
	{
		protected TcpConnection TcpCon { get; set; }

		private protected string UserName { protected get; private set; }

		private protected string Password { protected get; private set; }

		protected bool IsSecure { get; set; }

		protected IPEndPoint EndPoint { get; set; }

		private protected bool IsMbxProbe { protected get; private set; }

		private protected bool IsLocalProbe { protected get; private set; }

		protected abstract Trace Tracer { get; }

		protected abstract string ProbeComponent { get; }

		protected virtual void ParseDefinition()
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "TPDS;";
			IPAddress address;
			if (!IPAddress.TryParse(base.Definition.Endpoint, out address))
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(base.Definition.Endpoint);
				if (hostEntry.AddressList.Length == 0)
				{
					throw new FormatException(string.Format("Unable to resolve ServerName '{0}' to an IpAddress", base.Definition.Endpoint));
				}
				address = hostEntry.AddressList[0];
			}
			int port;
			if (!int.TryParse(base.Definition.SecondaryEndpoint, out port))
			{
				ProbeResult result2 = base.Result;
				object stateAttribute = result2.StateAttribute21;
				result2.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute,
					"TPDE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				throw new FormatException(string.Format("Unable to parse endpoint Port. Recieved: \"{0}\" Expected: int", base.Definition.SecondaryEndpoint));
			}
			if (base.Definition.Attributes.ContainsKey("MbxProbe"))
			{
				bool isMbxProbe;
				bool.TryParse(base.Definition.Attributes["MbxProbe"], out isMbxProbe);
				this.IsMbxProbe = isMbxProbe;
			}
			if (base.Definition.Attributes.ContainsKey("IsLocalProbe"))
			{
				bool isLocalProbe;
				bool.TryParse(base.Definition.Attributes["IsLocalProbe"], out isLocalProbe);
				this.IsLocalProbe = isLocalProbe;
			}
			this.EndPoint = new IPEndPoint(address, port);
			this.UserName = base.Definition.Account;
			this.Password = base.Definition.AccountPassword;
			ProbeResult result3 = base.Result;
			object stateAttribute2 = result3.StateAttribute21;
			result3.StateAttribute21 = string.Concat(new object[]
			{
				stateAttribute2,
				"TPDE:",
				(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
				";"
			});
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "TDWS;";
			this.latencyMeasurementStart = DateTime.UtcNow;
			int num = 110000;
			if (base.Definition.Attributes.ContainsKey("ProbeTimeout"))
			{
				int.TryParse(base.Definition.Attributes["ProbeTimeout"], out num);
			}
			bool skipLogin = false;
			if (base.Definition.Attributes.ContainsKey("LightMode"))
			{
				bool.TryParse(base.Definition.Attributes["LightMode"], out skipLogin);
			}
			try
			{
				this.ParseDefinition();
				ProbeResult result2 = base.Result;
				result2.StateAttribute13 += "R1";
				if (!this.InvokeProbe(skipLogin) && !this.IsLocalProbe)
				{
					ProbeResult result3 = base.Result;
					result3.StateAttribute13 += ":R2";
					this.InvokeProbe(skipLogin);
				}
				if (this.IsMbxProbe && string.IsNullOrEmpty(base.Result.StateAttribute4))
				{
					base.Result.StateAttribute4 = this.EndPoint.Address.ToString();
				}
				if (!this.IsMbxProbe && string.IsNullOrEmpty(base.Result.StateAttribute3))
				{
					base.Result.StateAttribute3 = this.EndPoint.Address.ToString();
				}
			}
			catch (InvalidOperationException ex)
			{
				if (ex.ToString().IndexOf(MonitoringWebClientStrings.NameResolutionFailure(base.Definition.Endpoint), StringComparison.OrdinalIgnoreCase) > -1)
				{
					base.Result.FailureCategory = 3;
					base.Result.StateAttribute1 = PopImapProbeUtil.PopImapFailingComponent.Networking.ToString();
					base.Result.StateAttribute2 = "NameResolution";
				}
				throw;
			}
			finally
			{
				if (this.TcpCon != null)
				{
					this.TcpCon.Close();
				}
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - this.latencyMeasurementStart).TotalMilliseconds);
				ProbeResult result4 = base.Result;
				result4.StateAttribute21 = result4.StateAttribute21 + "TDWE:" + (int)(DateTime.UtcNow - utcNow).TotalMilliseconds;
			}
		}

		protected abstract void TestProtocol();

		protected abstract void InitializeConnection();

		protected abstract void DetermineCapability();

		protected void VerifyResponse(TcpResponse response, string failureMessage)
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "TVRS;";
			string executionContext = string.Format("Probe executed for\r\n\r\nUser: {0}\r\nPassword: {1}\r\nTarget: {2}", base.Definition.Account, base.Definition.AccountPassword, base.Definition.Endpoint);
			base.Result.ExecutionContext = executionContext;
			if (response != null)
			{
				ProbeResult result2 = base.Result;
				result2.StateAttribute12 += response.ResponseString;
				Match match = TcpProbe.ProxyParserSuccessResponse.Match(response.ResponseString);
				if (match.Success && match.Groups["proxy"].Success)
				{
					base.Result.StateAttribute4 = Utils.ExtractServerNameFromFDQN(match.Groups["proxy"].Value);
				}
				if (response.ResponseType != ResponseType.success)
				{
					string error = response.ResponseString;
					Match match2 = TcpProbe.AuthErrorParser.Match(response.ResponseString);
					if (match2.Success && match2.Groups["authError"].Success)
					{
						error = match2.Groups["authError"].Value;
					}
					Match match3 = TcpProbe.ProxyParserFailedResponse.Match(response.ResponseString);
					if (match3.Success && match3.Groups["proxy"].Success)
					{
						base.Result.StateAttribute4 = Utils.ExtractServerNameFromFDQN(match3.Groups["proxy"].Value);
					}
					base.Result.StateAttribute2 = error;
					foreach (string text in this.acceptableErrors)
					{
						if (error.ToLower().Contains(text.ToLower()))
						{
							ProbeResult result3 = base.Result;
							object stateAttribute = result3.StateAttribute21;
							result3.StateAttribute21 = string.Concat(new object[]
							{
								stateAttribute,
								"TVRE:",
								(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
								";"
							});
							return;
						}
					}
					PopImapProbeUtil.PopImapFailingComponent popImapFailingComponent = PopImapProbeUtil.PopImapFailingComponent.PopImap;
					if (PopImapProbeUtil.KnownErrors.ContainsKey(error))
					{
						popImapFailingComponent = PopImapProbeUtil.KnownErrors[error];
					}
					else
					{
						List<string> list = (from key in PopImapProbeUtil.KnownErrors.Keys
						where error.Contains(key)
						select key).ToList<string>();
						if (list.Count == 1)
						{
							popImapFailingComponent = PopImapProbeUtil.KnownErrors[list.First<string>()];
						}
						else
						{
							error = "Unknown Reason: " + error;
						}
					}
					base.Result.FailureCategory = (int)popImapFailingComponent;
					if (popImapFailingComponent == PopImapProbeUtil.PopImapFailingComponent.PopImap)
					{
						base.Result.StateAttribute1 = this.ProbeComponent;
					}
					else
					{
						base.Result.StateAttribute1 = popImapFailingComponent.ToString();
					}
					if (popImapFailingComponent == PopImapProbeUtil.PopImapFailingComponent.Auth && base.Definition.AccountPassword.StartsWith("%") && base.Definition.AccountPassword.EndsWith("%"))
					{
						base.Result.StateAttribute1 = PopImapProbeUtil.PopImapFailingComponent.Monitoring.ToString();
						base.Result.StateAttribute2 = " Bad password for Monitoring Account: " + base.Definition.AccountPassword;
					}
					string message = string.Format("Unexpected Server Response: \"{0}\"", response.ResponseString);
					WTFDiagnostics.TraceError(this.Tracer, base.TraceContext, message, null, "VerifyResponse", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\TcpProbe.cs", 457);
					ProbeResult result4 = base.Result;
					object stateAttribute2 = result4.StateAttribute21;
					result4.StateAttribute21 = string.Concat(new object[]
					{
						stateAttribute2,
						"TVRE:",
						(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
						";"
					});
					throw new InvalidOperationException(failureMessage);
				}
				ProbeResult result5 = base.Result;
				object stateAttribute3 = result5.StateAttribute21;
				result5.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute3,
					"TVRE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				return;
			}
			base.Result.StateAttribute1 = PopImapProbeUtil.PopImapFailingComponent.PopImap.ToString();
			base.Result.StateAttribute2 = "Timedout";
			throw new InvalidOperationException(failureMessage);
		}

		protected bool InvokeProbe(bool skipLogin)
		{
			DateTime utcNow = DateTime.UtcNow;
			ProbeResult result = base.Result;
			result.StateAttribute21 += "TIPS;";
			bool result5;
			try
			{
				ProbeResult result2 = base.Result;
				result2.StateAttribute13 += ":C";
				this.InitializeConnection();
				if (!skipLogin)
				{
					this.TestProtocol();
				}
				ProbeResult result3 = base.Result;
				result3.StateAttribute13 += ":FIN";
				ProbeResult result4 = base.Result;
				object stateAttribute = result4.StateAttribute21;
				result4.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute,
					"TIPE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				result5 = true;
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("TcpConnection Errorcode was not null"))
				{
					this.HandleSocketException(ex);
					ProbeResult result6 = base.Result;
					object stateAttribute2 = result6.StateAttribute21;
					result6.StateAttribute21 = string.Concat(new object[]
					{
						stateAttribute2,
						"TIPE:",
						(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
						";"
					});
					result5 = true;
				}
				else
				{
					if (base.Result.StateAttribute13.Contains("R2") || this.IsLocalProbe || this.isVipProbe)
					{
						ProbeResult result7 = base.Result;
						result7.StateAttribute13 += ":FAIL";
						throw;
					}
					ProbeResult result8 = base.Result;
					object stateAttribute3 = result8.StateAttribute21;
					result8.StateAttribute21 = string.Concat(new object[]
					{
						stateAttribute3,
						"TIPE:",
						(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
						";"
					});
					result5 = false;
				}
			}
			catch (SocketException e)
			{
				this.HandleSocketException(e);
				ProbeResult result9 = base.Result;
				object stateAttribute4 = result9.StateAttribute21;
				result9.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute4,
					"TIPE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				result5 = true;
			}
			catch (ApplicationException ex2)
			{
				if (!ex2.Message.Contains("EndRead() resulted in non-null error code"))
				{
					base.Result.FailureCategory = 9;
					base.Result.StateAttribute1 = this.ProbeComponent;
					base.Result.StateAttribute2 = ex2.Message;
					ProbeResult result10 = base.Result;
					object stateAttribute5 = result10.StateAttribute21;
					result10.StateAttribute21 = string.Concat(new object[]
					{
						stateAttribute5,
						"TIPE:",
						(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
						";"
					});
					throw;
				}
				this.HandleSocketException(ex2);
				ProbeResult result11 = base.Result;
				object stateAttribute6 = result11.StateAttribute21;
				result11.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute6,
					"TIPE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				result5 = true;
			}
			catch (Exception ex3)
			{
				base.Result.FailureCategory = 9;
				base.Result.FailureContext = "Exception: " + ex3.ToString();
				base.Result.StateAttribute1 = this.ProbeComponent;
				base.Result.StateAttribute2 = "Unknown Reason: " + ex3.Message;
				ProbeResult result12 = base.Result;
				object stateAttribute7 = result12.StateAttribute21;
				result12.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute7,
					"TIPE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				throw ex3;
			}
			return result5;
		}

		protected virtual void HandleSocketException(Exception e)
		{
			if (!this.probeTimedOut)
			{
				DateTime utcNow = DateTime.UtcNow;
				ProbeResult result = base.Result;
				result.StateAttribute21 += "THSS;";
				base.Result.FailureCategory = 9;
				base.Result.StateAttribute1 = this.ProbeComponent;
				base.Result.StateAttribute2 = "Infrastructure Failure";
				WTFDiagnostics.TraceError(this.Tracer, base.TraceContext, e.Message, null, "HandleSocketException", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\TcpProbe.cs", 569);
				ProbeResult result2 = base.Result;
				result2.StateAttribute13 += ":FAIL";
				ProbeResult result3 = base.Result;
				object stateAttribute = result3.StateAttribute21;
				result3.StateAttribute21 = string.Concat(new object[]
				{
					stateAttribute,
					"THSE:",
					(int)(DateTime.UtcNow - utcNow).TotalMilliseconds,
					";"
				});
				throw new InvalidOperationException("Unable to initialise TCP Network connection", e);
			}
		}

		internal string DecodeServerName(string banner)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(banner))
			{
				string[] array = banner.Split(new string[]
				{
					" "
				}, StringSplitOptions.None);
				string text2 = array[array.Length - 1];
				if (text2.StartsWith("[") && text2.EndsWith("]"))
				{
					text2 = text2.Substring(1, text2.Length - 2);
					try
					{
						text = Encoding.Unicode.GetString(Convert.FromBase64String(text2));
					}
					catch (FormatException)
					{
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "Unable to retrieve server name";
			}
			return text;
		}

		private const string bannerServerDelimiter = " ";

		public const string MaxCountOfIPs = "MaxCountOfIPs";

		public const string LightModeKey = "LightMode";

		public const string ProbeTimeout = "ProbeTimeout";

		public const string DatabaseGuid = "DatabaseGuid";

		public const string MbxProbeKey = "MbxProbe";

		public const string IsLocalKey = "IsLocalProbe";

		protected const string WrongServerError = "WrongServerException";

		private static readonly Regex AuthErrorParser = new Regex("\\[Error=\"?(?<authError>[^\"]+)\"?( Proxy=(?<proxy>.+))?\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex ProxyParserSuccessResponse = new Regex("\\[Proxy=(?<proxy>.+)?\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex ProxyParserFailedResponse = new Regex("Proxy=(?<proxy>.+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		protected DateTime latencyMeasurementStart;

		protected bool isVipProbe;

		protected List<string> acceptableErrors = new List<string>();

		protected bool probeTimedOut;
	}
}
