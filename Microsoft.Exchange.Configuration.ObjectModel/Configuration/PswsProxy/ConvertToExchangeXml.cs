using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	[Cmdlet("ConvertTo", "ExchangeXml")]
	public class ConvertToExchangeXml : PSCmdlet
	{
		public new ISessionState SessionState
		{
			get
			{
				if (this.sessionState == null)
				{
					this.sessionState = new PSSessionState(base.SessionState);
				}
				return this.sessionState;
			}
		}

		[Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
		[AllowNull]
		public PSObject InputObject { get; set; }

		protected override void BeginProcessing()
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[ConvertToExchangeXml.BeginProcessing] Enter.");
			try
			{
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangePropertyContainer.GetExchangeRunspaceConfiguration(this.SessionState);
				this.serializer = new PSObjectSerializer((exchangeRunspaceConfiguration != null) ? exchangeRunspaceConfiguration.TypeTable : null);
				base.WriteObject("<?xml version=\"1.0\"?>");
				base.WriteObject("<Objs>");
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<Exception>((long)this.GetHashCode(), "[ConvertToExchangeXml.BeginProcessing] Exception: {0}", ex);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PswsPublicAPIFailed, null, new object[]
				{
					"ConvertToExchangeXml.BeginProcessing",
					ex.ToString()
				});
				throw;
			}
			finally
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[ConvertToExchangeXml.BeginProcessing] Exit.");
			}
		}

		protected override void EndProcessing()
		{
			base.WriteObject("</Objs>");
		}

		protected override void ProcessRecord()
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[ConvertToExchangeXml.ProcessRecord] Enter.");
			try
			{
				if (this.InputObject != null)
				{
					base.WriteObject(this.serializer.Serialize(this.InputObject));
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<Exception>((long)this.GetHashCode(), "[ConvertToExchangeXml.ProcessRecord] Exception: {0}", ex);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PswsPublicAPIFailed, null, new object[]
				{
					"ConvertToExchangeXml.ProcessRecord",
					ex.ToString()
				});
				throw;
			}
			finally
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[ConvertToExchangeXml.ProcessRecord] Exit.");
			}
		}

		private PSObjectSerializer serializer;

		private ISessionState sessionState;
	}
}
