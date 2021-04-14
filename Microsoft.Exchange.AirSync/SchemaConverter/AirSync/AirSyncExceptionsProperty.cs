using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncExceptionsProperty : AirSyncProperty, IExceptionsProperty, IMultivaluedProperty<ExceptionInstance>, IProperty, IEnumerable<ExceptionInstance>, IEnumerable, IDataObjectGeneratorContainer
	{
		public AirSyncExceptionsProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public int Count
		{
			get
			{
				return base.XmlNode.ChildNodes.Count;
			}
		}

		public IDataObjectGenerator SchemaState
		{
			get
			{
				return this.schemaState;
			}
			set
			{
				this.schemaState = (value as IAirSyncDataObjectGenerator);
			}
		}

		public IEnumerator<ExceptionInstance> GetEnumerator()
		{
			foreach (object obj in base.XmlNode.ChildNodes)
			{
				XmlNode exceptionNode = (XmlNode)obj;
				XmlNamespaceManager mgr = new XmlNamespaceManager(base.XmlNode.OwnerDocument.NameTable);
				mgr.AddNamespace("X", exceptionNode.NamespaceURI);
				XmlNode startTimeNode = exceptionNode.SelectSingleNode("X:ExceptionStartTime", mgr);
				ExDateTime startTime;
				if (!ExDateTime.TryParseExact(startTimeNode.InnerText, "yyyyMMdd\\THHmmss\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out startTime))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeInAirSyncException"
					};
				}
				XmlNode deletedNode = exceptionNode.SelectSingleNode("X:Deleted", mgr);
				if (deletedNode != null && deletedNode.InnerText == "1")
				{
					ExceptionInstance returnVal = new ExceptionInstance(startTime, 1);
					yield return returnVal;
				}
				else
				{
					exceptionNode.RemoveChild(startTimeNode);
					if (deletedNode != null)
					{
						exceptionNode.RemoveChild(deletedNode);
					}
					AirSyncDataObject exceptionContainer = this.schemaState.GetInnerAirSyncDataObject(this.missingPropertyStrategy);
					try
					{
						exceptionContainer.Bind(exceptionNode);
						yield return new ExceptionInstance(startTime, 0)
						{
							ModifiedException = exceptionContainer
						};
						exceptionNode.AppendChild(startTimeNode);
						if (deletedNode != null)
						{
							exceptionNode.AppendChild(deletedNode);
						}
					}
					finally
					{
						exceptionContainer.Unbind();
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IExceptionsProperty exceptionsProperty = srcProperty as IExceptionsProperty;
			if (exceptionsProperty == null)
			{
				throw new UnexpectedTypeException("IExceptionsProperty", srcProperty);
			}
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			foreach (ExceptionInstance exceptionInstance in exceptionsProperty)
			{
				XmlNode xmlNode = base.XmlParentNode.OwnerDocument.CreateElement("Exception", base.Namespace);
				AirSyncDataObject innerAirSyncDataObject = this.schemaState.GetInnerAirSyncDataObject(this.missingPropertyStrategy);
				try
				{
					innerAirSyncDataObject.Bind(xmlNode);
					if (exceptionInstance.Deleted == 1)
					{
						XmlNode xmlNode2 = base.XmlParentNode.OwnerDocument.CreateElement("Deleted", base.Namespace);
						xmlNode2.InnerText = "1";
						xmlNode.AppendChild(xmlNode2);
						XmlNode xmlNode3 = base.XmlParentNode.OwnerDocument.CreateElement("ExceptionStartTime", base.Namespace);
						xmlNode3.InnerText = exceptionInstance.ExceptionStartTime.ToString("yyyyMMdd\\THHmmss\\Z", DateTimeFormatInfo.InvariantInfo);
						xmlNode.AppendChild(xmlNode3);
					}
					else
					{
						innerAirSyncDataObject.CopyFrom(exceptionInstance.ModifiedException);
					}
				}
				finally
				{
					innerAirSyncDataObject.Unbind();
				}
				base.XmlNode.AppendChild(xmlNode);
			}
			if (base.XmlNode.HasChildNodes)
			{
				base.XmlParentNode.AppendChild(base.XmlNode);
			}
		}

		private const string Format = "yyyyMMdd\\THHmmss\\Z";

		private IAirSyncMissingPropertyStrategy missingPropertyStrategy = new AirSyncSetToUnmodifiedStrategy();

		private IAirSyncDataObjectGenerator schemaState;
	}
}
