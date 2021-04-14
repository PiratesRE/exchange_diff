using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "Flag", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class Flag
	{
		[XmlIgnore]
		public StateCollection StateCollection
		{
			get
			{
				if (this.internalStateCollection == null)
				{
					this.internalStateCollection = new StateCollection();
				}
				return this.internalStateCollection;
			}
			set
			{
				this.internalStateCollection = value;
			}
		}

		[XmlIgnore]
		public stringWithCharSetTypeCollection TitleCollection
		{
			get
			{
				if (this.internalTitleCollection == null)
				{
					this.internalTitleCollection = new stringWithCharSetTypeCollection();
				}
				return this.internalTitleCollection;
			}
			set
			{
				this.internalTitleCollection = value;
			}
		}

		[XmlIgnore]
		public ReminderDateCollection ReminderDateCollection
		{
			get
			{
				if (this.internalReminderDateCollection == null)
				{
					this.internalReminderDateCollection = new ReminderDateCollection();
				}
				return this.internalReminderDateCollection;
			}
			set
			{
				this.internalReminderDateCollection = value;
			}
		}

		[XmlIgnore]
		public CompletedCollection CompletedCollection
		{
			get
			{
				if (this.internalCompletedCollection == null)
				{
					this.internalCompletedCollection = new CompletedCollection();
				}
				return this.internalCompletedCollection;
			}
			set
			{
				this.internalCompletedCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(byte), ElementName = "State", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMMAIL:")]
		public StateCollection internalStateCollection;

		[XmlElement(Type = typeof(stringWithCharSetType), ElementName = "Title", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public stringWithCharSetTypeCollection internalTitleCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(string), ElementName = "ReminderDate", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMMAIL:")]
		public ReminderDateCollection internalReminderDateCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(bitType), ElementName = "Completed", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		public CompletedCollection internalCompletedCollection;
	}
}
