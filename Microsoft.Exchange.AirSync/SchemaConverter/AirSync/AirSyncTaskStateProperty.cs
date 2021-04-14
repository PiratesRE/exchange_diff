using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncTaskStateProperty : AirSyncProperty, ITaskState, IProperty
	{
		public AirSyncTaskStateProperty(string xmlNodeNamespace, string airSyncCompleteTagName, string airSyncDateCompletedTag, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncCompleteTagName, airSyncDateCompletedTag, requiresClientSupport)
		{
			this.completeTagName = airSyncCompleteTagName;
			this.dateCompletedTag = airSyncDateCompletedTag;
		}

		public bool Complete
		{
			get
			{
				string innerText;
				if ((innerText = base.XmlNode.InnerText) != null)
				{
					if (innerText == "0")
					{
						return false;
					}
					if (innerText == "1")
					{
						return true;
					}
				}
				throw new ConversionException("Incorrectly formatted boolean");
			}
		}

		public ExDateTime? DateCompleted
		{
			get
			{
				XmlNode xmlNode = base.XmlNode.ParentNode[this.dateCompletedTag];
				if (xmlNode == null)
				{
					return null;
				}
				ExDateTime value;
				if (!ExDateTime.TryParseExact(xmlNode.InnerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeInAirSyncTaskState"
					};
				}
				return new ExDateTime?(value);
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			ITaskState taskState = srcProperty as ITaskState;
			if (taskState == null)
			{
				throw new UnexpectedTypeException("ITaskState", srcProperty);
			}
			if (taskState.Complete)
			{
				base.CreateAirSyncNode(this.completeTagName, "1");
				if (taskState.DateCompleted != null)
				{
					base.CreateAirSyncNode(this.dateCompletedTag, taskState.DateCompleted.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo));
					return;
				}
			}
			else
			{
				base.CreateAirSyncNode(this.completeTagName, "0");
			}
		}

		private string completeTagName;

		private string dateCompletedTag;
	}
}
