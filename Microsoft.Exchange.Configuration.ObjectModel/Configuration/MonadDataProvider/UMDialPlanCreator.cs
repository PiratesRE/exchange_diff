using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class UMDialPlanCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"FaxEnabled",
				"CallAnsweringRulesEnabled",
				"VoIPSecurity",
				"UMServers",
				"URIType",
				"HuntGroups",
				"NumberOfDigitsInExtension",
				"WhenChanged",
				"WelcomeGreetingFilename",
				"WelcomeGreetingEnabled",
				"InfoAnnouncementFilename",
				"InfoAnnouncementEnabled",
				"AccessTelephoneNumbers",
				"OutsideLineAccessCode",
				"InternationalAccessCode",
				"NationalNumberPrefix",
				"CountryOrRegionCode",
				"InCountryOrRegionNumberFormat",
				"InternationalNumberFormat",
				"CallSomeoneEnabled",
				"SendVoiceMsgEnabled",
				"ContactScope",
				"Extension",
				"ContactAddressList",
				"UMAutoAttendant",
				"MatchedNameSelectionMethod",
				"UMAutoAttendants",
				"DialByNamePrimary",
				"DialByNameSecondary",
				"AudioCodec",
				"OperatorExtension",
				"LogonFailuresBeforeDisconnect",
				"MaxCallDuration",
				"MaxRecordingDuration",
				"RecordingIdleTimeout",
				"InputFailuresBeforeDisconnect",
				"AvailableLanguages",
				"DefaultLanguage",
				"ConfiguredInCountryOrRegionGroups",
				"ConfiguredInternationalGroups",
				"AllowDialPlanSubscribers",
				"AllowExtensions",
				"AllowedInCountryOrRegionGroups",
				"AllowedInternationalGroups",
				"PilotIdentifierList"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "CallAnsweringRulesEnabled")
			{
				PropertyInfo property = configObject.GetType().GetProperty(propertyName);
				property.SetValue(configObject, MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, property.PropertyType), null);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
