using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "ScanInfoType", Namespace = "HMSYNC:")]
	[Serializable]
	public class ScanInfoType
	{
		[XmlIgnore]
		public byte Result
		{
			get
			{
				return this.internalResult;
			}
			set
			{
				this.internalResult = value;
				this.internalResultSpecified = true;
			}
		}

		[XmlIgnore]
		public CleanedAttachments CleanedAttachments
		{
			get
			{
				if (this.internalCleanedAttachments == null)
				{
					this.internalCleanedAttachments = new CleanedAttachments();
				}
				return this.internalCleanedAttachments;
			}
			set
			{
				this.internalCleanedAttachments = value;
			}
		}

		[XmlIgnore]
		public InfectedAttachments InfectedAttachments
		{
			get
			{
				if (this.internalInfectedAttachments == null)
				{
					this.internalInfectedAttachments = new InfectedAttachments();
				}
				return this.internalInfectedAttachments;
			}
			set
			{
				this.internalInfectedAttachments = value;
			}
		}

		[XmlIgnore]
		public SuspiciousAttachments SuspiciousAttachments
		{
			get
			{
				if (this.internalSuspiciousAttachments == null)
				{
					this.internalSuspiciousAttachments = new SuspiciousAttachments();
				}
				return this.internalSuspiciousAttachments;
			}
			set
			{
				this.internalSuspiciousAttachments = value;
			}
		}

		[XmlElement(ElementName = "Result", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public byte internalResult;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalResultSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(CleanedAttachments), ElementName = "CleanedAttachments", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public CleanedAttachments internalCleanedAttachments;

		[XmlElement(Type = typeof(InfectedAttachments), ElementName = "InfectedAttachments", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public InfectedAttachments internalInfectedAttachments;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SuspiciousAttachments), ElementName = "SuspiciousAttachments", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public SuspiciousAttachments internalSuspiciousAttachments;
	}
}
