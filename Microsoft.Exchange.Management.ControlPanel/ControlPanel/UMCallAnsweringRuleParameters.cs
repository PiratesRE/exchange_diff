using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ControlPanel.DataContracts;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class UMCallAnsweringRuleParameters : SetObjectProperties
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[DataMember]
		public bool? CheckAutomaticReplies
		{
			get
			{
				return (bool?)base["CheckAutomaticReplies"];
			}
			set
			{
				base["CheckAutomaticReplies"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] ScheduleStatus
		{
			get
			{
				string text = (string)base["ScheduleStatus"];
				if (text == null)
				{
					return null;
				}
				string[] array = text.Split(new char[]
				{
					','
				});
				if (array.Length == 1 && array[0] == FreeBusyStatusEnum.None.ToString())
				{
					return null;
				}
				return array;
			}
			set
			{
				if (value != null)
				{
					FreeBusyStatusEnum freeBusyStatusEnum = FreeBusyStatusEnum.None;
					for (int i = 0; i < value.Length; i++)
					{
						string value2 = value[i];
						freeBusyStatusEnum |= (FreeBusyStatusEnum)Enum.Parse(typeof(FreeBusyStatusEnum), value2);
					}
					base["ScheduleStatus"] = freeBusyStatusEnum;
					return;
				}
				base["ScheduleStatus"] = null;
			}
		}

		[DataMember]
		public Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem[] CallerIds
		{
			get
			{
				Microsoft.Exchange.Data.CallerIdItem[] array = (Microsoft.Exchange.Data.CallerIdItem[])base["CallerIds"];
				if (array != null)
				{
					return Array.ConvertAll<Microsoft.Exchange.Data.CallerIdItem, Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem>(array, (Microsoft.Exchange.Data.CallerIdItem x) => new Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem(x));
				}
				return null;
			}
			set
			{
				string cmdletParameterName = "CallerIds";
				object value2;
				if (value == null)
				{
					value2 = null;
				}
				else
				{
					value2 = Array.ConvertAll<Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem, Microsoft.Exchange.Data.CallerIdItem>(value, (Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem x) => x.ToTaskObject());
				}
				base[cmdletParameterName] = value2;
			}
		}

		[DataMember]
		public string[] ExtensionsDialed
		{
			get
			{
				return (string[])base["ExtensionsDialed"];
			}
			set
			{
				base["ExtensionsDialed"] = value;
			}
		}

		[DataMember]
		public TimeOfDayItem TimeOfDay
		{
			get
			{
				TimeOfDay timeOfDay = (TimeOfDay)base["TimeOfDay"];
				if (timeOfDay != null)
				{
					return new TimeOfDayItem(timeOfDay);
				}
				return null;
			}
			set
			{
				base["TimeOfDay"] = ((value != null) ? value.ToTaskObject() : null);
			}
		}

		[DataMember]
		public Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping[] KeyMappings
		{
			get
			{
				Microsoft.Exchange.Data.KeyMapping[] array = (Microsoft.Exchange.Data.KeyMapping[])base["KeyMappings"];
				if (array != null)
				{
					return Array.ConvertAll<Microsoft.Exchange.Data.KeyMapping, Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping>(array, (Microsoft.Exchange.Data.KeyMapping x) => new Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping(x));
				}
				return null;
			}
			set
			{
				string cmdletParameterName = "KeyMappings";
				object value2;
				if (value == null)
				{
					value2 = null;
				}
				else
				{
					value2 = Array.ConvertAll<Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping, Microsoft.Exchange.Data.KeyMapping>(value, (Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping x) => x.ToTaskObject());
				}
				base[cmdletParameterName] = value2;
			}
		}

		[DataMember]
		public bool? CallersCanInterruptGreeting
		{
			get
			{
				return (bool?)base["CallersCanInterruptGreeting"];
			}
			set
			{
				base["CallersCanInterruptGreeting"] = (value ?? false);
			}
		}
	}
}
