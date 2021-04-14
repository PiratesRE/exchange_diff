using System;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Outdialing;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class OutdialingDiagnostics
	{
		internal bool HasEvents
		{
			get
			{
				return this.detailBuilder.Length > 0;
			}
		}

		internal static void ValidateProperties()
		{
			Type typeFromHandle = typeof(UMDialPlan);
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			string[] names = Enum.GetNames(typeof(OutdialingDiagnostics.DialPlanProperty));
			foreach (string text in names)
			{
				if (typeFromHandle.GetProperty(text, bindingAttr) == null)
				{
					throw new MissingMemberException(typeFromHandle.Name, text);
				}
			}
		}

		internal void AddSkipDialPlanDetail()
		{
			this.skipDialPlan = true;
		}

		internal void AddPropertyNotSetDetail(UMDialPlan dialPlan, OutdialingDiagnostics.DialPlanProperty property)
		{
			this.detailBuilder.Append("\r\n");
			this.detailBuilder.Append(Strings.DialPlanPropertyNotSet(property.ToString(), dialPlan.Name));
		}

		internal void AddNumberNotInStandardFormat(ADRecipient recipient)
		{
			this.detailBuilder.Append("\r\n");
			if (recipient != null)
			{
				this.detailBuilder.Append(Strings.NumberNotInStandardFormat(recipient.Name));
				return;
			}
			this.detailBuilder.Append(Strings.NumberNotInStandardFormatNoRecipient);
		}

		internal void AddInvalidRecipientPhoneLength(IADRecipient recipient, string dialPlanName)
		{
			string recipient2 = (recipient != null) ? recipient.Name : string.Empty;
			this.detailBuilder.Append("\r\n");
			this.detailBuilder.Append(Strings.InvalidRecipientPhoneLength(recipient2, dialPlanName));
		}

		internal void AddInvalidPlayOnPhoneNumber(string phoneNumber)
		{
			this.detailBuilder.Append("\r\n");
			this.detailBuilder.Append(Strings.InvalidPlayOnPhoneNumber(phoneNumber));
		}

		internal void AddAccessCheckFailed(string phoneNumber)
		{
			this.detailBuilder.Append("\r\n");
			this.detailBuilder.Append(Strings.AccessCheckFailed(phoneNumber));
		}

		internal string GetDetails()
		{
			return this.detailBuilder.ToString();
		}

		internal void LogOutdialingWarning(PhoneNumber inputNumber, PhoneNumber outputNumber)
		{
			if (this.skipDialPlan)
			{
				this.detailBuilder.Append("\r\n");
				this.detailBuilder.Append(Strings.SkippingTargetDialPlan);
			}
			string text;
			if (outputNumber != null)
			{
				text = Strings.CanonicalizationResult(outputNumber.ToDial);
			}
			else
			{
				text = Strings.CanonicalizationFailed;
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_OutdialingConfigurationWarning, null, new object[]
			{
				(inputNumber != null) ? inputNumber.ToDial : string.Empty,
				text,
				this.detailBuilder.ToString()
			});
		}

		private bool skipDialPlan;

		private StringBuilder detailBuilder = new StringBuilder(64);

		internal enum DialPlanProperty
		{
			InternationalAccessCode,
			NationalNumberPrefix,
			InCountryOrRegionNumberFormat,
			InternationalNumberFormat,
			OutsideLineAccessCode,
			CountryOrRegionCode
		}
	}
}
