using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal sealed class Configuration
	{
		private void Load()
		{
			Exception ex = null;
			try
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 134, "Load", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\attachfilter\\Configuration.cs");
				ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
				AttachmentFilteringConfig[] array = tenantOrTopologyConfigurationSession.Find<AttachmentFilteringConfig>(orgContainerId, QueryScope.OneLevel, null, null, 1);
				if (array.Length != 1)
				{
					throw new InvalidDataException("Corrupt configuration - there are multiple attachment-filtering configuration objects");
				}
				foreach (string storedAttribute in array[0].AttachmentNames)
				{
					AttachmentFilterEntrySpecification attachmentFilterEntrySpecification = AttachmentFilterEntrySpecification.Parse(storedAttribute);
					if (attachmentFilterEntrySpecification.Type == AttachmentType.FileName)
					{
						this.ProcessFileSpec(attachmentFilterEntrySpecification.Name);
					}
					else
					{
						this.blockedContentTypes.Add(attachmentFilterEntrySpecification.Name);
					}
				}
				foreach (ADObjectId entryId in array[0].ExceptionConnectors)
				{
					ReceiveConnector receiveConnector = tenantOrTopologyConfigurationSession.Read<ReceiveConnector>(entryId);
					if (receiveConnector != null)
					{
						this.exceptionConnectors.Add(receiveConnector.Name);
						ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Adding connector {0} as attachment filter exception.", receiveConnector.Name);
					}
				}
				this.action = array[0].Action;
				this.rejectResponse = new SmtpResponse("550", "5.7.1", new string[]
				{
					array[0].RejectResponse
				});
				this.isAdminMessageUsAscii = true;
				foreach (char c in array[0].AdminMessage)
				{
					if (c >= 'Ā')
					{
						this.isAdminMessageUsAscii = false;
						break;
					}
				}
				if (this.isAdminMessageUsAscii)
				{
					this.adminMessage = array[0].AdminMessage.ToCharArray();
				}
				else
				{
					this.adminMessage = new char[array[0].AdminMessage.Length + 1];
					this.adminMessage[0] = '﻿';
					array[0].AdminMessage.CopyTo(0, this.adminMessage, 1, array[0].AdminMessage.Length);
				}
			}
			catch (InvalidDataException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			catch (ExchangeConfigurationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceError<string>((long)this.GetHashCode(), "The current configuration is invalid, it will not be loaded. Error message: {0}", ex.ToString());
				this.validConfig = false;
				return;
			}
			this.validConfig = true;
		}

		private void ProcessFileSpec(string fileSpec)
		{
			string text;
			Regex regex;
			string item;
			AttachmentFilterEntrySpecification.ParseFileSpec(fileSpec, out text, out regex, out item);
			if (text != null)
			{
				this.blockedExtensions.Add(text);
				return;
			}
			if (regex != null)
			{
				this.blockedNameExpressions.Add(regex);
				return;
			}
			this.blockedNames.Add(item);
		}

		internal static Configuration Current
		{
			get
			{
				return Configuration.currentConfig;
			}
		}

		public FilterActions FilterAction
		{
			get
			{
				return this.action;
			}
		}

		internal SmtpResponse RejectResponse
		{
			get
			{
				return this.rejectResponse;
			}
		}

		internal char[] AdminMessage
		{
			get
			{
				return this.adminMessage;
			}
		}

		internal bool IsAdminMessageUsAscii
		{
			get
			{
				return this.isAdminMessageUsAscii;
			}
		}

		public static void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 337, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\attachfilter\\Configuration.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADNotificationAdapter.RegisterChangeNotification<AttachmentFilteringConfig>(orgContainerId, new ADNotificationCallback(Configuration.Configure));
		}

		public static void Configure(ADNotificationEventArgs args)
		{
			Configuration newConfig = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				newConfig = new Configuration();
				newConfig.Load();
			});
			if (!adoperationResult.Succeeded || newConfig == null || !newConfig.validConfig)
			{
				Agent.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_AttachFilterConfigCorrupt, string.Empty, new object[0]);
				return;
			}
			Agent.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_AttachFilterConfigLoaded, string.Empty, new object[0]);
			Configuration.currentConfig = newConfig;
		}

		internal SmtpResponse GetSilentDeleteResponse(string messageId)
		{
			return new SmtpResponse("250", "2.6.0", new string[]
			{
				string.Format("{0} Queued mail for delivery.", messageId)
			});
		}

		internal bool IsBannedName(string name)
		{
			foreach (string text in this.blockedExtensions)
			{
				if (name.EndsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Filename extension {0} is blocked", text);
					return true;
				}
			}
			foreach (string text2 in this.blockedNames)
			{
				if (string.Compare(text2, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Filename matches blocked expression {0}", text2);
					return true;
				}
			}
			foreach (Regex regex in this.blockedNameExpressions)
			{
				if (regex.IsMatch(name))
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Filename matches blocked regex");
					return true;
				}
			}
			return false;
		}

		internal bool IsBannedType(IEnumerable<string> contentTypes)
		{
			foreach (string text in contentTypes)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Checking: {0}", text);
				if (this.IsBannedType(text))
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "The Content-Type is illegal");
					return true;
				}
			}
			return false;
		}

		private bool IsBannedType(string contentType)
		{
			foreach (string text in this.blockedContentTypes)
			{
				if (string.Compare(contentType, text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Content-type {0} matches blocked content-type {1}", contentType, text);
					return true;
				}
			}
			return false;
		}

		public bool IsEnabled(string connectorName)
		{
			foreach (string strB in this.exceptionConnectors)
			{
				if (string.Compare(connectorName, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return false;
				}
			}
			return true;
		}

		public const int MaxNestedAttachmentDepth = 10;

		public const char UnicodeByteOrderMark = '﻿';

		private static Configuration currentConfig;

		private FilterActions action;

		private IList<string> exceptionConnectors = new List<string>();

		private IList<string> blockedExtensions = new List<string>();

		private IList<string> blockedNames = new List<string>();

		private IList<Regex> blockedNameExpressions = new List<Regex>();

		private IList<string> blockedContentTypes = new List<string>();

		private SmtpResponse rejectResponse;

		private char[] adminMessage;

		private bool isAdminMessageUsAscii;

		private bool validConfig;
	}
}
