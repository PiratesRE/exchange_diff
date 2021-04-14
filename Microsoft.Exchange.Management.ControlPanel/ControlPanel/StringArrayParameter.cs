using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class StringArrayParameter : FormletParameter
	{
		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText, Type objectType) : this(name, dialogTitle, dialogLabel, noSelectionText)
		{
			if (objectType.IsArray)
			{
				this.MaxLength = StringParameter.GetMaxLength(objectType.GetElementType());
			}
		}

		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType) : this(name, dialogTitle, dialogLabel, LocalizedString.Empty, objectType)
		{
		}

		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel) : this(name, dialogTitle, dialogLabel, LocalizedString.Empty)
		{
		}

		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText) : base(name, dialogTitle, dialogLabel)
		{
			this.MaxLength = 255;
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRuleStringArrayParameterNoSelectionText;
			}
			else
			{
				this.noSelectionText = noSelectionText;
			}
			base.FormletType = typeof(StringArrayModalEditor);
		}

		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, int maxLength, LocalizedString noSelectionText, string inputWaterMarkText, string validationExpression, string validationErrorMessage) : base(name, dialogTitle, dialogLabel, null)
		{
			this.MaxLength = maxLength;
			this.noSelectionText = noSelectionText;
			this.InputWaterMarkText = inputWaterMarkText;
			this.ValidationExpression = validationExpression;
			this.ValidationErrorMessage = validationErrorMessage;
			base.FormletType = typeof(StringArrayModalEditor);
		}

		public StringArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, int maxLength, LocalizedString noSelectionText, string inputWaterMarkText, string validationExpression, string validationErrorMessage, DuplicateHandlingType duplicateHandlingType) : this(name, dialogTitle, dialogLabel, maxLength, noSelectionText, inputWaterMarkText, validationExpression, validationErrorMessage)
		{
			this.DuplicateHandlingType = duplicateHandlingType;
		}

		[DataMember]
		public int MaxLength { get; private set; }

		[DataMember]
		public string InputWaterMarkText { get; private set; }

		[DataMember]
		public string ValidationExpression { get; private set; }

		[DataMember]
		public string ValidationErrorMessage { get; private set; }

		[DataMember]
		public DuplicateHandlingType DuplicateHandlingType { get; private set; }
	}
}
