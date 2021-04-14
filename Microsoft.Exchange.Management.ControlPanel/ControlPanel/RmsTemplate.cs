using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.RightsManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RmsTemplate : EnumValue
	{
		public RmsTemplate(RmsTemplatePresentation taskRMSTemplate) : base(taskRMSTemplate.Name, taskRMSTemplate.Identity.ToString())
		{
		}
	}
}
