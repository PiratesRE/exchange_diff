using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class UMMailboxCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"UMMailboxPolicy",
				"EmailAddresses",
				"AutomaticSpeechRecognitionEnabled",
				"AllowUMCallsFromNonUsers",
				"FaxEnabled",
				"AnonymousCallersCanLeaveMessages",
				"CallAnsweringRulesEnabled",
				"OperatorNumber",
				"UMDialPlan"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "AutomaticSpeechRecognitionEnabled")
			{
				configObject.propertyBag[UMMailboxSchema.ASREnabled] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, UMMailboxSchema.ASREnabled.Type);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
