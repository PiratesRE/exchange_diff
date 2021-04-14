using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendResponse
{
	[XmlType(TypeName = "SendItem", Namespace = "Send:")]
	[Serializable]
	public class SendItem
	{
		[XmlIgnore]
		public int Status
		{
			get
			{
				return this.internalStatus;
			}
			set
			{
				this.internalStatus = value;
				this.internalStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public Fault Fault
		{
			get
			{
				if (this.internalFault == null)
				{
					this.internalFault = new Fault();
				}
				return this.internalFault;
			}
			set
			{
				this.internalFault = value;
			}
		}

		[XmlIgnore]
		public ScanInfoType ScanInfo
		{
			get
			{
				if (this.internalScanInfo == null)
				{
					this.internalScanInfo = new ScanInfoType();
				}
				return this.internalScanInfo;
			}
			set
			{
				this.internalScanInfo = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "Send:")]
		public int internalStatus;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalStatusSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Fault), ElementName = "Fault", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public Fault internalFault;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ScanInfoType), ElementName = "ScanInfo", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public ScanInfoType internalScanInfo;
	}
}
