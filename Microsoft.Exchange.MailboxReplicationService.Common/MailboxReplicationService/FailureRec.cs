using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class FailureRec : XMLSerializableBase
	{
		public FailureRec()
		{
		}

		private FailureRec(Exception ex)
		{
			this.Timestamp = DateTime.UtcNow;
			this.FailureType = CommonUtils.GetFailureType(ex);
			this.FailureCode = CommonUtils.HrFromException(ex);
			this.MapiLowLevelError = CommonUtils.GetMapiLowLevelError(ex);
			this.FailureSide = CommonUtils.GetExceptionSide(ex);
			this.ExceptionTypes = CommonUtils.ClassifyException(ex);
			this.Message = CommonUtils.FullExceptionMessage(ex).ToString();
			this.DataContext = ExecutionContext.GetDataContext(ex);
			this.StackTrace = CommonUtils.GetStackTrace(ex);
			this.InnerException = FailureRec.Create(ex.InnerException);
		}

		private FailureRec(string failureType, string message, string stackTrace, string dataContext, FailureRec innerFailure)
		{
			this.Timestamp = DateTime.UtcNow;
			this.FailureType = failureType;
			this.FailureCode = 0;
			this.MapiLowLevelError = 0;
			this.FailureSide = null;
			WellKnownException[] exceptionTypes = new WellKnownException[1];
			this.ExceptionTypes = exceptionTypes;
			this.Message = message;
			this.StackTrace = stackTrace;
			this.DataContext = dataContext;
			if (innerFailure != null)
			{
				this.InnerException = innerFailure;
			}
		}

		[XmlElement(ElementName = "Timestamp")]
		public DateTime Timestamp { get; set; }

		[XmlElement(ElementName = "FailureType")]
		public string FailureType { get; set; }

		[XmlElement(ElementName = "FailureCode")]
		public int FailureCode { get; set; }

		[XmlElement(ElementName = "MapiLowLevelError")]
		public int MapiLowLevelError { get; set; }

		[XmlIgnore]
		public ExceptionSide? FailureSide { get; private set; }

		[XmlElement(ElementName = "FailureSide")]
		public int FailureSideInt
		{
			get
			{
				ExceptionSide? failureSide = this.FailureSide;
				if (failureSide == null)
				{
					return 0;
				}
				return (int)failureSide.GetValueOrDefault();
			}
			set
			{
				if (value == 0)
				{
					this.FailureSide = null;
					return;
				}
				this.FailureSide = new ExceptionSide?((ExceptionSide)value);
			}
		}

		[XmlIgnore]
		public WellKnownException[] ExceptionTypes { get; private set; }

		[XmlElement(ElementName = "ExceptionTypes")]
		public int[] ExceptionTypesInt
		{
			get
			{
				if (this.ExceptionTypes == null || this.ExceptionTypes.Length == 0)
				{
					return null;
				}
				int[] array = new int[this.ExceptionTypes.Length];
				for (int i = 0; i < this.ExceptionTypes.Length; i++)
				{
					array[i] = (int)this.ExceptionTypes[i];
				}
				return array;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.ExceptionTypes = null;
					return;
				}
				this.ExceptionTypes = new WellKnownException[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					this.ExceptionTypes[i] = (WellKnownException)value[i];
				}
			}
		}

		[XmlElement(ElementName = "MessageStr")]
		public string Message { get; set; }

		[XmlElement(ElementName = "Message")]
		public byte[] MessageData
		{
			get
			{
				return null;
			}
			set
			{
				LocalizedString localizedString = CommonUtils.ByteDeserialize(value);
				if (!localizedString.IsEmpty && string.IsNullOrEmpty(this.Message))
				{
					this.Message = localizedString.ToString();
				}
			}
		}

		[XmlElement(ElementName = "DataContextStr")]
		public string DataContext { get; set; }

		[XmlElement(ElementName = "DataContext")]
		public byte[] DataContextData
		{
			get
			{
				return null;
			}
			set
			{
				LocalizedString localizedString = CommonUtils.ByteDeserialize(value);
				if (!localizedString.IsEmpty && string.IsNullOrEmpty(this.DataContext))
				{
					this.DataContext = localizedString.ToString();
				}
			}
		}

		[XmlElement(ElementName = "StackTrace")]
		public string StackTrace { get; set; }

		[XmlElement(ElementName = "InnerException")]
		public FailureRec InnerException { get; set; }

		public static FailureRec Create(Exception ex)
		{
			if (ex == null)
			{
				return null;
			}
			return new FailureRec(ex);
		}

		public static FailureRec Create(string failureType, string message, string stackTrace, string dataContext, string innerException)
		{
			FailureRec innerFailure = string.IsNullOrEmpty(innerException) ? null : FailureRec.Create(string.Empty, string.Empty, innerException, string.Empty, string.Empty);
			return new FailureRec(failureType, message, stackTrace, dataContext, innerFailure);
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", this.FailureType, this.Message);
		}

		internal XElement GetDiagnosticData()
		{
			XElement xelement = new XElement("FailureRecord");
			xelement.Add(new XElement("TimeStamp", this.Timestamp));
			xelement.Add(new XElement("DataContext", this.DataContext));
			xelement.Add(new XElement("ExceptionTypes", this.ExceptionTypes));
			xelement.Add(new XElement("FailureType", this.FailureType));
			xelement.Add(new XElement("FailureSide", this.FailureSide));
			xelement.Add(new XElement("FailureCode", this.FailureCode));
			xelement.Add(new XElement("MapiLowLevelError", this.MapiLowLevelError));
			xelement.Add(new XElement("Message", this.Message));
			xelement.Add(new XElement("StackTrace", this.StackTrace));
			if (this.InnerException != null)
			{
				xelement.Add(new XElement("InnerException", this.InnerException.GetDiagnosticData()));
			}
			return xelement;
		}
	}
}
