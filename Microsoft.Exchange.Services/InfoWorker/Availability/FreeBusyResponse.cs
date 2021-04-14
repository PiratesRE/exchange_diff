using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class FreeBusyResponse
	{
		[DataMember]
		[XmlElement(IsNullable = false)]
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessage;
			}
			set
			{
				this.responseMessage = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public FreeBusyView FreeBusyView
		{
			get
			{
				return this.freeBusyView;
			}
			set
			{
				this.freeBusyView = value;
			}
		}

		internal static FreeBusyResponse CreateFrom(FreeBusyQueryResult freeBusyResult, int index)
		{
			if (freeBusyResult == null)
			{
				return null;
			}
			return new FreeBusyResponse
			{
				index = index,
				freeBusyView = FreeBusyView.CreateFrom(freeBusyResult),
				ResponseMessage = ResponseMessageBuilder.ResponseMessageFromExchangeException(freeBusyResult.ExceptionInfo)
			};
		}

		internal int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		private FreeBusyResponse()
		{
		}

		private FreeBusyView freeBusyView;

		private ResponseMessage responseMessage;

		private int index;
	}
}
