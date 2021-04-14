using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public abstract class ExchangeDiagnosableWrapper<T> : IDiagnosableExtraData, IDiagnosable
	{
		protected virtual string UsageText
		{
			get
			{
				return "Returns diagnostics information about specified process.";
			}
		}

		protected virtual string UsageSample
		{
			get
			{
				return "Get-ExchangeDiagnosticInfo -Server <TargetServer> -Process <ProcessName> -Component <ComponentName> -Argument <Argument>";
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return this.ComponentName;
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters arguments)
		{
			XElement xelement = new XElement(this.ComponentName);
			if (arguments.Argument.Equals("?", StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(this.GetUsage());
				return xelement;
			}
			try
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string, bool, string>((long)this.GetHashCode(), "ExchangeDiagnosableWrapper::GetDIagnosticsInfo called. Argument:{0}, AllowRestrictedData:{1}, User:{2}", arguments.Argument, arguments.AllowRestrictedData, arguments.UserIdentity);
				T exchangeDiagnosticsInfoData = this.GetExchangeDiagnosticsInfoData(arguments);
				if (typeof(XElement) == typeof(T))
				{
					xelement.Add(exchangeDiagnosticsInfoData);
				}
				else
				{
					Type type = (exchangeDiagnosticsInfoData != null) ? exchangeDiagnosticsInfoData.GetType() : typeof(T);
					XmlSerializer xmlSerializer = new XmlSerializer(type);
					using (MemoryStream memoryStream = new MemoryStream())
					{
						xmlSerializer.Serialize(memoryStream, exchangeDiagnosticsInfoData);
						memoryStream.Position = 0L;
						xelement.Add(XElement.Load(memoryStream));
					}
				}
			}
			catch (Exception ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				Exception ex2 = ex;
				do
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("\r\n");
					}
					stringBuilder.Append(ex2.Message);
					ex2 = ex2.InnerException;
				}
				while (ex2 != null);
				FaultDiagnosticsInfo faultDiagnosticsInfo = new FaultDiagnosticsInfo(-999, string.Format("Type:{0},Ex:{1},Stack:{2}", ex.GetType(), stringBuilder, ex.StackTrace));
				xelement.Add(faultDiagnosticsInfo);
				xelement.Add(faultDiagnosticsInfo.ErrorText);
				ExTraceGlobals.CommonTracer.TraceError<string>((long)this.GetHashCode(), "ExchangeDiagnosableWrapper::GetDiagnosticsInfo Exception occurred. Exception:{0}", ex.ToString());
			}
			return xelement;
		}

		void IDiagnosableExtraData.SetData(XElement dataElement)
		{
			this.InternalSetData(dataElement);
		}

		void IDiagnosableExtraData.OnStop()
		{
			this.InternalOnStop();
		}

		protected abstract string ComponentName { get; }

		protected virtual void InternalSetData(XElement dataElement)
		{
		}

		protected virtual void InternalOnStop()
		{
		}

		protected virtual void OnPreProcess()
		{
			ExTraceGlobals.CommonTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "ExchangeDiagnosableWrapper::OnPreProcess called. Component:{0}", (this.ComponentName == null) ? "<null>" : this.ComponentName);
		}

		protected virtual string GetUsage()
		{
			StringBuilder stringBuilder = new StringBuilder(this.UsageText);
			stringBuilder.AppendLine(this.UsageSample);
			return stringBuilder.ToString();
		}

		internal abstract T GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments);

		internal string RemoveQuotes(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			return input.Replace("'", string.Empty).Replace("\"", string.Empty);
		}

		private const int Version = 1;

		private const string HelpArgument = "?";
	}
}
