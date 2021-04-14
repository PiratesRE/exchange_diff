using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class DateRangeParameter : FormletParameter
	{
		public DateRangeParameter(string name, string[] taskParameterNames, LocalizedString dialogTitle, LocalizedString beforeDateDisplayTemplate, LocalizedString afterDateDisplayTemplate) : base(name, dialogTitle, LocalizedString.Empty, taskParameterNames)
		{
			this.locBeforeDateDisplayTemplate = beforeDateDisplayTemplate;
			this.locAfterDateDisplayTemplate = afterDateDisplayTemplate;
			base.FormletType = typeof(DateRangeModalEditor);
		}

		[DataMember]
		public string BeforeDateDisplayTemplate
		{
			get
			{
				return this.locBeforeDateDisplayTemplate.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AfterDateDisplayTemplate
		{
			get
			{
				return this.locAfterDateDisplayTemplate.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private LocalizedString locBeforeDateDisplayTemplate;

		private LocalizedString locAfterDateDisplayTemplate;
	}
}
