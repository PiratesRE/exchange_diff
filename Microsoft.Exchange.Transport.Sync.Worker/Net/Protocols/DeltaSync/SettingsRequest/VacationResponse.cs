using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "VacationResponse", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class VacationResponse
	{
		[XmlIgnore]
		public VacationResponseMode Mode
		{
			get
			{
				return this.internalMode;
			}
			set
			{
				this.internalMode = value;
				this.internalModeSpecified = true;
			}
		}

		[XmlIgnore]
		public string StartTime
		{
			get
			{
				return this.internalStartTime;
			}
			set
			{
				this.internalStartTime = value;
			}
		}

		[XmlIgnore]
		public string EndTime
		{
			get
			{
				return this.internalEndTime;
			}
			set
			{
				this.internalEndTime = value;
			}
		}

		[XmlIgnore]
		public string Message
		{
			get
			{
				return this.internalMessage;
			}
			set
			{
				this.internalMessage = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Mode", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public VacationResponseMode internalMode;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalModeSpecified;

		[XmlElement(ElementName = "StartTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalStartTime;

		[XmlElement(ElementName = "EndTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalEndTime;

		[XmlElement(ElementName = "Message", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalMessage;
	}
}
