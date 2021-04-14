using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(HiddenParameter))]
	[KnownType(typeof(IncidentReportContentParameter))]
	[DataContract]
	[KnownType(typeof(PeopleParameter))]
	[KnownType(typeof(TimePeriodParameter))]
	[KnownType(typeof(CallerIdsParameter))]
	[KnownType(typeof(ExtensionsDialedParameter))]
	[KnownType(typeof(KeyMappingsParameter))]
	[KnownType(typeof(StringParameter))]
	[KnownType(typeof(EnhancedEnumParameter))]
	[KnownType(typeof(EnumParameter))]
	[KnownType(typeof(NumberRangeParameter))]
	[KnownType(typeof(DateRangeParameter))]
	[KnownType(typeof(StringArrayParameter))]
	[KnownType(typeof(FolderParameter))]
	[KnownType(typeof(OUPickerParameter))]
	[KnownType(typeof(ObjectArrayParameter))]
	[KnownType(typeof(ObjectParameter))]
	[KnownType(typeof(BooleanParameter))]
	[KnownType(typeof(JournalRuleScopeParameter))]
	[KnownType(typeof(NotificationPhoneNumberParameter))]
	[KnownType(typeof(NumberEnumParameter))]
	[KnownType(typeof(NumberParameter))]
	[KnownType(typeof(ByteQuantifiedSizeParameter))]
	[KnownType(typeof(ADAttributeParameter))]
	[KnownType(typeof(ObjectsParameter))]
	[KnownType(typeof(DLPParameter))]
	[KnownType(typeof(SenderNotifyParameter))]
	public abstract class FormletParameter
	{
		public FormletParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, string[] taskParameterNames)
		{
			this.Name = name;
			this.locDialogTitle = dialogTitle;
			this.locDialogLabel = dialogLabel;
			this.TaskParameterNames = taskParameterNames;
			this.EditorType = base.GetType().Name + "Editor";
			this.FormletType = null;
			this.RequiredField = true;
		}

		public FormletParameter(string name, LocalizedString displayName, LocalizedString description) : this(name, displayName, description, new string[]
		{
			name
		})
		{
		}

		[DataMember]
		public string Name { get; private set; }

		public string[] TaskParameterNames { get; private set; }

		[DataMember]
		public virtual string DialogTitle
		{
			get
			{
				return this.locDialogTitle.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string DialogLabel
		{
			get
			{
				return this.locDialogLabel.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EditorType { get; protected set; }

		public Type FormletType { get; protected set; }

		internal static int GetIntFieldValue(Type strongType, string fieldName, int defaultValue)
		{
			FieldInfo field = strongType.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
			if (!(field == null))
			{
				return (int)field.GetValue(null);
			}
			return defaultValue;
		}

		internal static string GetStringFieldValue(Type strongType, string fieldName, string defaultValue)
		{
			FieldInfo field = strongType.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
			if (!(field == null))
			{
				return field.GetValue(null).ToString();
			}
			return defaultValue;
		}

		[DataMember(EmitDefaultValue = false)]
		public string NoSelectionText
		{
			get
			{
				return this.noSelectionText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool ExactMatch { get; protected set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RequiredField { get; set; }

		private LocalizedString locDialogTitle;

		private LocalizedString locDialogLabel;

		protected LocalizedString noSelectionText;
	}
}
