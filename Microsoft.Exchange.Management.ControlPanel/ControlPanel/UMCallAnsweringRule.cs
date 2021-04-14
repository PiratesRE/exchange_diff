using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ControlPanel.DataContracts;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(UMCallAnsweringRule))]
	public class UMCallAnsweringRule : RuleRow
	{
		public UMCallAnsweringRule(UMCallAnsweringRule rule) : base(rule)
		{
			this.Rule = rule;
			base.DescriptionObject = rule.Description;
			base.ConditionDescriptions = base.DescriptionObject.ConditionDescriptions.ToArray();
			base.ActionDescriptions = base.DescriptionObject.ActionDescriptions.ToArray();
			base.ExceptionDescriptions = base.DescriptionObject.ExceptionDescriptions.ToArray();
		}

		public UMCallAnsweringRule Rule { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool? CheckAutomaticReplies
		{
			get
			{
				if (!this.Rule.CheckAutomaticReplies)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public TimeOfDayItem TimeOfDay
		{
			get
			{
				if (this.Rule.TimeOfDay == null)
				{
					return null;
				}
				return new TimeOfDayItem(this.Rule.TimeOfDay);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem[] CallerIds
		{
			get
			{
				return Array.ConvertAll<Microsoft.Exchange.Data.CallerIdItem, Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem>(this.Rule.CallerIds.ToArray(), (Microsoft.Exchange.Data.CallerIdItem x) => new Microsoft.Exchange.Management.ControlPanel.DataContracts.CallerIdItem(x));
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExtensionsDialed
		{
			get
			{
				return this.Rule.ExtensionsDialed.ToArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ScheduleStatus
		{
			get
			{
				FreeBusyStatusEnum scheduleStatus = (FreeBusyStatusEnum)this.Rule.ScheduleStatus;
				if (scheduleStatus == FreeBusyStatusEnum.None)
				{
					return null;
				}
				return scheduleStatus.ToString().Replace(" ", string.Empty).Split(new char[]
				{
					','
				});
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping[] KeyMappings
		{
			get
			{
				return Array.ConvertAll<Microsoft.Exchange.Data.KeyMapping, Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping>(this.Rule.KeyMappings.ToArray(), (Microsoft.Exchange.Data.KeyMapping x) => new Microsoft.Exchange.Management.ControlPanel.DataContracts.KeyMapping(x));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? CallersCanInterruptGreeting
		{
			get
			{
				if (!this.Rule.CallersCanInterruptGreeting)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}
	}
}
