using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class StringParameter : FormletParameter
	{
		public StringParameter(string name, LocalizedString displayName, LocalizedString description, Type objectType, bool multiLine) : this(name, displayName, description, objectType, multiLine, string.Empty)
		{
		}

		public StringParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, bool multiLine, string defaultText) : base(name, dialogTitle, dialogLabel)
		{
			this.MaxLength = StringParameter.GetMaxLength(objectType);
			this.MultiLine = multiLine;
			this.ValidatingExpression = StringParameter.GetValidatingExpression(objectType);
			this.DefaultText = defaultText;
			this.noSelectionText = Strings.TransportRuleStringParameterNoSelectionText;
			base.FormletType = (this.MultiLine ? typeof(MultilineStringModalEditor) : typeof(StringModalEditor));
		}

		public StringParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, bool multiLine, string defaultText, LocalizedString noSelectionText) : this(name, dialogTitle, dialogLabel, objectType, multiLine, defaultText)
		{
			this.noSelectionText = noSelectionText;
		}

		[DataMember]
		public int MaxLength { get; private set; }

		[DataMember]
		public bool MultiLine { get; private set; }

		[DataMember]
		public string ValidatingExpression { get; private set; }

		[DataMember]
		public string DefaultText { get; private set; }

		internal static int GetMaxLength(Type strongType)
		{
			return FormletParameter.GetIntFieldValue(strongType, "MaxLength", 0);
		}

		internal static string GetValidatingExpression(Type strongType)
		{
			return FormletParameter.GetStringFieldValue(strongType, "ValidatingExpression", string.Empty);
		}
	}
}
