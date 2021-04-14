using System;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class PictureOptions
	{
		internal PictureOptions()
		{
			this.MaxSize = int.MaxValue;
			this.MaxPictures = int.MaxValue;
			this.MaxResolution = GlobalSettings.HDPhotoDefaultSupportedResolution;
			this.userPhotoSize = PictureOptions.GetUserPhotoSize(this.MaxResolution);
		}

		internal int MaxSize { get; set; }

		internal int MaxPictures { get; set; }

		internal UserPhotoResolution MaxResolution { get; set; }

		internal UserPhotoSize PhotoSize
		{
			get
			{
				return this.userPhotoSize;
			}
		}

		internal static PictureOptions Parse(XmlNode pictureOptionsNode, StatusCode protocolError)
		{
			if (pictureOptionsNode == null)
			{
				return null;
			}
			PictureOptions pictureOptions = new PictureOptions();
			foreach (object obj in pictureOptionsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string localName;
				if ((localName = xmlNode.LocalName) != null)
				{
					if (!(localName == "MaxSize"))
					{
						if (!(localName == "MaxPictures"))
						{
							if (localName == "MaxResolution")
							{
								UserPhotoResolution userPhotoResolution;
								if (!Enum.TryParse<UserPhotoResolution>(xmlNode.InnerText, out userPhotoResolution))
								{
									throw new AirSyncPermanentException(protocolError, false)
									{
										ErrorStringForProtocolLogger = "InvalidMaxResolution"
									};
								}
								pictureOptions.MaxResolution = userPhotoResolution;
								pictureOptions.userPhotoSize = PictureOptions.GetUserPhotoSize(userPhotoResolution);
								continue;
							}
						}
						else
						{
							int num;
							if (!int.TryParse(xmlNode.InnerText, out num))
							{
								throw new AirSyncPermanentException(protocolError, false)
								{
									ErrorStringForProtocolLogger = "InvalidMaxPictures"
								};
							}
							pictureOptions.MaxPictures = num;
							continue;
						}
					}
					else
					{
						int num;
						if (!int.TryParse(xmlNode.InnerText, out num))
						{
							throw new AirSyncPermanentException(protocolError, false)
							{
								ErrorStringForProtocolLogger = "InvalidMaxSize"
							};
						}
						pictureOptions.MaxSize = num;
						continue;
					}
				}
				throw new AirSyncPermanentException(protocolError, false)
				{
					ErrorStringForProtocolLogger = "UnexpectedPictureOption:" + xmlNode.Name
				};
			}
			return pictureOptions;
		}

		private static UserPhotoSize GetUserPhotoSize(UserPhotoResolution resolution)
		{
			switch (resolution)
			{
			case UserPhotoResolution.HR48x48:
				return UserPhotoSize.HR48x48;
			case UserPhotoResolution.HR64x64:
				return UserPhotoSize.HR64x64;
			case UserPhotoResolution.HR96x96:
				return UserPhotoSize.HR96x96;
			case UserPhotoResolution.HR120x120:
				return UserPhotoSize.HR120x120;
			case UserPhotoResolution.HR240x240:
				return UserPhotoSize.HR240x240;
			case UserPhotoResolution.HR360x360:
				return UserPhotoSize.HR360x360;
			case UserPhotoResolution.HR432x432:
				return UserPhotoSize.HR432x432;
			case UserPhotoResolution.HR504x504:
				return UserPhotoSize.HR504x504;
			case UserPhotoResolution.HR648x648:
				return UserPhotoSize.HR648x648;
			default:
				throw new ArgumentOutOfRangeException("Invalid value for UserPhotoResolution parameter.");
			}
		}

		internal XmlNode CreatePictureNode(XmlDocument document, string namespaceName, byte[] picture, bool pictureLimitReached, out bool pictureWasAdded)
		{
			pictureWasAdded = false;
			XmlNode xmlNode = document.CreateElement("Picture", namespaceName);
			StatusCode statusCode = StatusCode.Success;
			if (picture == null || picture.Length == 0)
			{
				statusCode = StatusCode.NoPicture;
			}
			else if (picture.Length > this.MaxSize)
			{
				statusCode = StatusCode.PictureTooLarge;
			}
			else if (pictureLimitReached)
			{
				statusCode = StatusCode.PictureLimitReached;
			}
			XmlNode xmlNode2 = document.CreateElement("Status", namespaceName);
			XmlNode xmlNode3 = xmlNode2;
			int num = (int)statusCode;
			xmlNode3.InnerText = num.ToString();
			xmlNode.AppendChild(xmlNode2);
			if (statusCode == StatusCode.Success)
			{
				xmlNode.AppendChild(new AirSyncBlobXmlNode(null, "Data", namespaceName, document)
				{
					ByteArray = picture
				});
				pictureWasAdded = true;
			}
			return xmlNode;
		}

		private UserPhotoSize userPhotoSize;
	}
}
