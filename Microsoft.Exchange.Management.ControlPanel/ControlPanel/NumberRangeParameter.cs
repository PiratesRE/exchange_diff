using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class NumberRangeParameter : FormletParameter
	{
		public NumberRangeParameter(string name, string[] taskParameterNames, LocalizedString dialogTitle, LocalizedString atMostOnlyDisplayTemplate, LocalizedString atLeastAtMostDisplayTemplate) : base(name, dialogTitle, LocalizedString.Empty, taskParameterNames)
		{
			this.locAtMostOnlyDisplayTemplate = atMostOnlyDisplayTemplate;
			this.locAtLeastAtMostDisplayTemplate = atLeastAtMostDisplayTemplate;
			this.MaxValue = 999999;
			this.MinValue = 0;
			base.FormletType = typeof(NumberRangeModalEditor);
		}

		[DataMember]
		public int MaxValue { get; private set; }

		[DataMember]
		public int MinValue { get; private set; }

		[DataMember]
		public string AtMostOnlyDisplayTemplate
		{
			get
			{
				return this.locAtMostOnlyDisplayTemplate.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AtLeastAtMostDisplayTemplate
		{
			get
			{
				return this.locAtLeastAtMostDisplayTemplate.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		internal static int GetMaxValue(Type strongType)
		{
			return FormletParameter.GetIntFieldValue(strongType, "MaxValue", int.MaxValue);
		}

		internal static int GetMinValue(Type strongType)
		{
			return FormletParameter.GetIntFieldValue(strongType, "MinValue", 0);
		}

		private LocalizedString locAtMostOnlyDisplayTemplate;

		private LocalizedString locAtLeastAtMostDisplayTemplate;
	}
}
