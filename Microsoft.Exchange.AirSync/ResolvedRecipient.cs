using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.AirSync
{
	internal class ResolvedRecipient : IComparable
	{
		internal ResolvedRecipient(RecipientAddress address)
		{
			this.ResolvedTo = address;
			this.CertificateRetrieval = ResolveRecipientsCommand.CertificateRetrievalType.None;
			this.Certificates = new X509Certificate2Collection();
		}

		internal ResolveRecipientsCommand.CertificateRetrievalType CertificateRetrieval { get; set; }

		internal int CertificateCount { get; set; }

		internal int CertificateRecipientCount { get; set; }

		internal X509Certificate2Collection Certificates { get; set; }

		internal bool GlobalCertLimitWasHit { get; set; }

		internal RecipientAddress ResolvedTo { get; set; }

		internal byte[] Picture { get; set; }

		internal StatusCode AvailabilityStatus { get; set; }

		internal string MergedFreeBusy { get; set; }

		internal PictureOptions PictureOptions { get; set; }

		public int CompareTo(object value)
		{
			ResolvedRecipient resolvedRecipient = value as ResolvedRecipient;
			if (resolvedRecipient != null)
			{
				return this.ResolvedTo.CompareTo(resolvedRecipient.ResolvedTo);
			}
			throw new ArgumentException("object is not an ResolvedRecipient");
		}

		internal void BuildXmlResponse(XmlDocument xmlResponse, XmlNode parentNode, bool pictureLimitReached, out bool pictureWasAdded)
		{
			pictureWasAdded = false;
			XmlNode xmlNode = xmlResponse.CreateElement("Recipient", "ResolveRecipients:");
			parentNode.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlResponse.CreateElement("Type", "ResolveRecipients:");
			xmlNode2.InnerText = ((this.ResolvedTo.AddressOrigin == AddressOrigin.Directory) ? "1" : "2");
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlResponse.CreateElement("DisplayName", "ResolveRecipients:");
			xmlNode3.InnerText = this.ResolvedTo.DisplayName;
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = xmlResponse.CreateElement("EmailAddress", "ResolveRecipients:");
			xmlNode4.InnerText = this.ResolvedTo.SmtpAddress;
			xmlNode.AppendChild(xmlNode4);
			if (this.PictureOptions != null)
			{
				StatusCode statusCode = StatusCode.Success;
				byte[] array = null;
				if (Command.CurrentCommand.User.Features.IsEnabled(EasFeature.HDPhotos) && Command.CurrentCommand.Request.Version >= 160)
				{
					ResolveRecipientsCommand resolveRecipientsCommand = Command.CurrentCommand as ResolveRecipientsCommand;
					if (resolveRecipientsCommand != null && resolveRecipientsCommand.PhotoRetriever != null)
					{
						array = resolveRecipientsCommand.PhotoRetriever.EndGetThumbnailPhotoFromMailbox(this.ResolvedTo.SmtpAddress, GlobalSettings.MaxRequestExecutionTime - ExDateTime.Now.Subtract(Command.CurrentCommand.Context.RequestTime), this.PictureOptions.PhotoSize);
					}
					else
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Error getting PhotoRetriever instance from Command. Command:{0}", (Command.CurrentCommand == null) ? "<null>" : Command.CurrentCommand.Request.CommandType.ToString());
					}
				}
				if (statusCode != StatusCode.Success || array == null)
				{
					array = this.Picture;
				}
				XmlNode newChild = this.PictureOptions.CreatePictureNode(xmlNode.OwnerDocument, "ResolveRecipients:", array, pictureLimitReached, out pictureWasAdded);
				xmlNode.AppendChild(newChild);
			}
			if (this.CertificateRetrieval != ResolveRecipientsCommand.CertificateRetrievalType.None)
			{
				XmlNode xmlNode5 = xmlResponse.CreateElement("Certificates", "ResolveRecipients:");
				xmlNode.AppendChild(xmlNode5);
				XmlNode xmlNode6 = xmlResponse.CreateElement("Status", "ResolveRecipients:");
				xmlNode5.AppendChild(xmlNode6);
				if (!this.GlobalCertLimitWasHit && (this.Certificates == null || this.Certificates.Count == 0))
				{
					xmlNode6.InnerText = 7.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					if (this.GlobalCertLimitWasHit)
					{
						xmlNode6.InnerText = 8.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						xmlNode6.InnerText = 1.ToString(CultureInfo.InvariantCulture);
					}
					XmlNode xmlNode7 = xmlResponse.CreateElement("CertificateCount", "ResolveRecipients:");
					xmlNode7.InnerText = this.CertificateCount.ToString(CultureInfo.InvariantCulture);
					xmlNode5.AppendChild(xmlNode7);
					XmlNode xmlNode8 = xmlResponse.CreateElement("RecipientCount", "ResolveRecipients:");
					xmlNode8.InnerText = this.CertificateRecipientCount.ToString(CultureInfo.InvariantCulture);
					xmlNode5.AppendChild(xmlNode8);
					if (this.CertificateRetrieval == ResolveRecipientsCommand.CertificateRetrievalType.Full)
					{
						foreach (X509Certificate2 x509Certificate in this.Certificates)
						{
							XmlNode xmlNode9 = xmlResponse.CreateElement("Certificate", "ResolveRecipients:");
							xmlNode9.InnerText = Convert.ToBase64String(x509Certificate.RawData);
							xmlNode5.AppendChild(xmlNode9);
						}
					}
					else
					{
						foreach (X509Certificate2 certificate in this.Certificates)
						{
							XmlNode xmlNode10 = xmlResponse.CreateElement("MiniCertificate", "ResolveRecipients:");
							byte[] inArray = X509PartialCertificate.Encode(certificate, true);
							xmlNode10.InnerText = Convert.ToBase64String(inArray);
							xmlNode5.AppendChild(xmlNode10);
						}
					}
				}
			}
			if (this.AvailabilityStatus != StatusCode.None)
			{
				XmlNode xmlNode11 = xmlResponse.CreateElement("Availability", "ResolveRecipients:");
				xmlNode.AppendChild(xmlNode11);
				XmlNode xmlNode12 = xmlResponse.CreateElement("Status", "ResolveRecipients:");
				xmlNode12.InnerText = ((int)this.AvailabilityStatus).ToString(CultureInfo.InvariantCulture);
				xmlNode11.AppendChild(xmlNode12);
				if (!string.IsNullOrEmpty(this.MergedFreeBusy))
				{
					XmlNode xmlNode13 = xmlResponse.CreateElement("MergedFreeBusy", "ResolveRecipients:");
					xmlNode13.InnerText = this.MergedFreeBusy;
					xmlNode11.AppendChild(xmlNode13);
					return;
				}
				if (this.AvailabilityStatus == StatusCode.Success)
				{
					throw new InvalidOperationException("Empty free busy string received!");
				}
			}
		}
	}
}
