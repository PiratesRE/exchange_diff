using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class UserChoiceState
	{
		[XmlElement(IsNullable = true)]
		public string CurrentWizardPageName { get; set; }

		[XmlElement(IsNullable = true)]
		public string ExchangeVersionBeingInstalled { get; set; }

		public bool? CustomerFeedbackEnabled { get; set; }

		public bool ErrorReportingEnabled { get; set; }

		public bool TypicalInstallation { get; set; }

		public bool IsAdminToolsChecked { get; set; }

		public bool IsBridgeheadChecked { get; set; }

		public bool IsClientAccessChecked { get; set; }

		public bool IsGatewayChecked { get; set; }

		public bool IsMailboxChecked { get; set; }

		public bool IsUnifiedMessagingChecked { get; set; }

		public bool IsFrontendTransportChecked { get; set; }

		public bool IsCentralAdminChecked { get; set; }

		public bool IsCentralAdminDatabaseChecked { get; set; }

		public bool IsMonitoringChecked { get; set; }

		public bool IsLanguagePacksChecked { get; set; }

		public bool IsCafeChecked { get; set; }

		public bool IsOSPChecked { get; set; }

		[XmlElement(IsNullable = true)]
		public string ProgramFilesPath { get; set; }

		[XmlElement(IsNullable = true)]
		public string OrganizationName { get; set; }

		[XmlElement(IsNullable = true)]
		public string ExternalCASServerDomain { get; set; }

		public bool? ActiveDirectorySplitPermissions { get; set; }

		public static void DeleteFile()
		{
			try
			{
				File.Delete(Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, UserChoiceState.stateFileName));
			}
			catch (IOException e)
			{
				SetupLogger.Log(Strings.ExceptionWhenDeserializingStateFile(e));
			}
		}

		public void SaveToFile()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserChoiceState));
			TextWriter textWriter = new StreamWriter(Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, UserChoiceState.stateFileName));
			xmlSerializer.Serialize(textWriter, this);
			textWriter.Close();
		}

		public static UserChoiceState LoadFromFile()
		{
			try
			{
				string text = File.ReadAllText(Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, UserChoiceState.stateFileName));
				SetupLogger.Log(Strings.DeserializedStateXML(text));
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(text)))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserChoiceState));
					if (xmlSerializer.CanDeserialize(xmlReader))
					{
						return (UserChoiceState)xmlSerializer.Deserialize(xmlReader);
					}
					SetupLogger.Log(Strings.CouldNotDeserializeStateFile);
				}
			}
			catch (IOException e)
			{
				SetupLogger.Log(Strings.ExceptionWhenDeserializingStateFile(e));
			}
			catch (XmlException e2)
			{
				SetupLogger.Log(Strings.ExceptionWhenDeserializingStateFile(e2));
			}
			catch (InvalidOperationException e3)
			{
				SetupLogger.Log(Strings.ExceptionWhenDeserializingStateFile(e3));
			}
			return null;
		}

		internal void WriteToContext(ISetupContext setupContext, ModeDataHandler modeDataHandler)
		{
			if (this.ExchangeVersionBeingInstalled == null || !this.ExchangeVersionBeingInstalled.Equals(setupContext.RunningVersion.ToString()))
			{
				SetupLogger.Log(Strings.StateFileVersionMismatch(this.ExchangeVersionBeingInstalled, setupContext.RunningVersion.ToString()));
				return;
			}
			setupContext.IsRestoredFromPreviousState = true;
			setupContext.CurrentWizardPageName = this.CurrentWizardPageName;
			if (modeDataHandler is InstallModeDataHandler)
			{
				InstallModeDataHandler installModeDataHandler = modeDataHandler as InstallModeDataHandler;
				installModeDataHandler.CustomerFeedbackEnabled = this.CustomerFeedbackEnabled;
				if (setupContext.IsCleanMachine)
				{
					installModeDataHandler.TypicalInstallation = this.TypicalInstallation;
				}
			}
			setupContext.WatsonEnabled = this.ErrorReportingEnabled;
			modeDataHandler.IsAdminToolsChecked = this.IsAdminToolsChecked;
			modeDataHandler.IsBridgeheadChecked = this.IsBridgeheadChecked;
			modeDataHandler.IsClientAccessChecked = this.IsClientAccessChecked;
			modeDataHandler.IsGatewayChecked = this.IsGatewayChecked;
			modeDataHandler.IsMailboxChecked = this.IsMailboxChecked;
			modeDataHandler.IsUnifiedMessagingChecked = this.IsUnifiedMessagingChecked;
			modeDataHandler.IsFrontendTransportChecked = this.IsFrontendTransportChecked;
			modeDataHandler.IsCentralAdminChecked = this.IsCentralAdminChecked;
			modeDataHandler.IsCentralAdminDatabaseChecked = this.IsCentralAdminDatabaseChecked;
			modeDataHandler.IsMonitoringChecked = this.IsMonitoringChecked;
			modeDataHandler.IsLanguagePacksChecked = this.IsLanguagePacksChecked;
			modeDataHandler.IsCafeChecked = this.IsCafeChecked;
			modeDataHandler.IsOSPChecked = this.IsOSPChecked;
			setupContext.ActiveDirectorySplitPermissions = this.ActiveDirectorySplitPermissions;
			if (this.ProgramFilesPath != null)
			{
				setupContext.TargetDir = NonRootLocalLongFullPath.Parse(this.ProgramFilesPath);
			}
			if (this.OrganizationName != null)
			{
				setupContext.OrganizationName = new OrganizationName(this.OrganizationName);
			}
		}

		internal void ReadFromContext(ISetupContext setupContext, ModeDataHandler modeDataHandler)
		{
			this.ExchangeVersionBeingInstalled = ((setupContext.RunningVersion == null) ? null : setupContext.RunningVersion.ToString());
			this.CurrentWizardPageName = setupContext.CurrentWizardPageName;
			if (modeDataHandler is InstallModeDataHandler)
			{
				InstallModeDataHandler installModeDataHandler = modeDataHandler as InstallModeDataHandler;
				this.CustomerFeedbackEnabled = installModeDataHandler.CustomerFeedbackEnabled;
				this.TypicalInstallation = installModeDataHandler.TypicalInstallation;
			}
			this.ErrorReportingEnabled = setupContext.WatsonEnabled;
			this.IsAdminToolsChecked = modeDataHandler.IsAdminToolsChecked;
			this.IsBridgeheadChecked = modeDataHandler.IsBridgeheadChecked;
			this.IsClientAccessChecked = modeDataHandler.IsClientAccessChecked;
			this.IsGatewayChecked = modeDataHandler.IsGatewayChecked;
			this.IsMailboxChecked = modeDataHandler.IsMailboxChecked;
			this.IsUnifiedMessagingChecked = modeDataHandler.IsUnifiedMessagingChecked;
			this.IsFrontendTransportChecked = modeDataHandler.IsFrontendTransportChecked;
			this.IsCentralAdminChecked = modeDataHandler.IsCentralAdminChecked;
			this.IsCentralAdminDatabaseChecked = modeDataHandler.IsCentralAdminDatabaseChecked;
			this.IsMonitoringChecked = modeDataHandler.IsMonitoringChecked;
			this.IsLanguagePacksChecked = modeDataHandler.IsLanguagePacksChecked;
			this.IsCafeChecked = modeDataHandler.IsCafeChecked;
			this.IsOSPChecked = modeDataHandler.IsOSPChecked;
			this.ActiveDirectorySplitPermissions = setupContext.ActiveDirectorySplitPermissions;
			this.ProgramFilesPath = ((setupContext.TargetDir == null) ? null : setupContext.TargetDir.ToString());
			this.OrganizationName = ((setupContext.OrganizationName == null) ? null : setupContext.OrganizationName.UnescapedName);
		}

		private static readonly string stateFileName = "exchangeInstallState.xml";
	}
}
