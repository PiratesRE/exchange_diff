using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.AirSync
{
	internal class GalSearchProvider : ISearchProvider
	{
		internal GalSearchProvider(IAirSyncUser user, int lcid, int protocolVersion)
		{
			this.lcid = lcid;
			this.protocolVersion = protocolVersion;
			this.user = user;
			AirSyncCounters.NumberOfGALSearches.Increment();
		}

		public int NumberResponses
		{
			get
			{
				return this.numberResponses;
			}
		}

		public bool RightsManagementSupport
		{
			get
			{
				return false;
			}
		}

		public void BuildResponse(XmlElement responseNode)
		{
			try
			{
				int num = 0;
				this.numberResponses = 0;
				XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Status", "Search:");
				responseNode.AppendChild(xmlNode);
				xmlNode.InnerText = 1.ToString(CultureInfo.InvariantCulture);
				int num2 = this.minRange;
				while (this.addressBookObjects != null && num2 <= this.maxRange && num2 < this.addressBookObjects.Count)
				{
					ABObject abobject = this.addressBookObjects[num2];
					if (abobject != null)
					{
						ABContact abcontact = abobject as ABContact;
						XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Result", "Search:");
						XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("Properties", "Search:");
						xmlNode2.AppendChild(xmlNode3);
						for (int i = 0; i < GalSearchProvider.schema.Length; i++)
						{
							string abproperty;
							if (abcontact == null)
							{
								abproperty = GalSearchProvider.GetABProperty(abobject, GalSearchProvider.schema[i]);
							}
							else
							{
								abproperty = GalSearchProvider.GetABProperty(abcontact, GalSearchProvider.schema[i]);
							}
							if (!string.IsNullOrEmpty(abproperty))
							{
								XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement(GalSearchProvider.schema[i], "Gal:");
								xmlNode4.InnerText = abproperty;
								xmlNode3.AppendChild(xmlNode4);
							}
						}
						if (this.pictureOptions != null && abcontact != null)
						{
							StatusCode statusCode = StatusCode.Success;
							byte[] array = null;
							if (this.user.Features.IsEnabled(EasFeature.HDPhotos) && this.user.Context.Request.Version >= 160)
							{
								array = this.photoRetriever.EndGetThumbnailPhotoFromMailbox(abcontact.EmailAddress, GlobalSettings.MaxRequestExecutionTime - ExDateTime.Now.Subtract(Command.CurrentCommand.Context.RequestTime), this.pictureOptions.PhotoSize);
								AirSyncDiagnostics.TraceDebug<bool, int>(ExTraceGlobals.RequestsTracer, this, "GalSearch Requesting user's HD picture. WasRetrived:{0}, size:{1}", array != null, (array == null) ? 0 : array.Length);
							}
							if (statusCode != StatusCode.Success || array == null)
							{
								AirSyncDiagnostics.TraceDebug<bool, int, StatusCode>(ExTraceGlobals.RequestsTracer, this, "User's HD photo is either null or was not requested, Using contact's picture from AD. :FeatureEnabled:{0}, RequestVersion:{1}, statusCode:{2}", this.user.Features.IsEnabled(EasFeature.HDPhotos), this.user.Context.Request.Version, statusCode);
								array = abcontact.Picture;
							}
							bool flag;
							XmlNode newChild = this.pictureOptions.CreatePictureNode(responseNode.OwnerDocument, "Gal:", array, num >= this.pictureOptions.MaxPictures, out flag);
							xmlNode3.AppendChild(newChild);
							if (flag)
							{
								num++;
							}
						}
						responseNode.AppendChild(xmlNode2);
						this.numberResponses++;
					}
					num2++;
				}
				if (this.numberResponses == 0)
				{
					XmlNode newChild2 = responseNode.OwnerDocument.CreateElement("Result", "Search:");
					responseNode.AppendChild(newChild2);
				}
				else if (this.rangeSpecified)
				{
					XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("Range", "Search:");
					XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("Total", "Search:");
					xmlNode5.InnerText = this.minRange.ToString(CultureInfo.InvariantCulture) + "-" + (this.minRange + this.numberResponses - 1).ToString(CultureInfo.InvariantCulture);
					responseNode.AppendChild(xmlNode5);
					AirSyncDiagnostics.Assert(this.addressBookObjects.Count <= GlobalSettings.MaxGALSearchResults);
					xmlNode6.InnerText = this.addressBookObjects.Count.ToString(CultureInfo.InvariantCulture);
					responseNode.AppendChild(xmlNode6);
				}
			}
			finally
			{
				if (this.photoRetriever != null)
				{
					this.photoRetriever.Dispose();
					this.photoRetriever = null;
				}
			}
		}

		public void Execute()
		{
			Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.SearchQueryLength, this.searchQuery.Length);
			if (this.user.IsConsumerOrganizationUser)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "GalSearch command not supported for consumer users");
				return;
			}
			if (this.minRange >= GlobalSettings.MaxGALSearchResults)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "GalSearch command min range specified is outside our configured maximum. No results will be returned");
				return;
			}
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(this.searchQuery, 0);
			if (this.searchQuery.Length < GlobalSettings.MinGALSearchLength && unicodeCategory != UnicodeCategory.OtherLetter)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "GalSearch search string is shorter than MinGALSearchLength. No results will be returned");
				Command.CurrentCommand.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "SearchStringTooShort");
				return;
			}
			OperationRetryManagerResult operationRetryManagerResult = GalSearchProvider.retryManager.TryRun(delegate
			{
				IABSessionSettings sessionSettings = ABDiscoveryManager.GetSessionSettings(this.user.ExchangePrincipal, new int?(this.lcid), null, GlobalSettings.SyncLog, this.user.ClientSecurityContextWrapper.ClientSecurityContext);
				using (ABSession absession = ADABSession.Create(sessionSettings))
				{
					this.addressBookObjects = absession.FindByANR(this.searchQuery, GlobalSettings.MaxGALSearchResults);
				}
			});
			if (operationRetryManagerResult.Succeeded)
			{
				if (this.pictureOptions != null && this.user.Features.IsEnabled(EasFeature.HDPhotos) && this.user.Context.Request.Version >= 160)
				{
					this.photoRetriever = new AirSyncPhotoRetriever(this.user.Context);
					List<string> list = new List<string>();
					int num = this.minRange;
					while (this.addressBookObjects != null && num <= this.maxRange && num < this.addressBookObjects.Count)
					{
						ABObject abobject = this.addressBookObjects[num];
						if (abobject == null)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "ABSession.FindByAnr returned null  addresBookObject. Continue.");
						}
						else
						{
							ABContact abcontact = abobject as ABContact;
							if (abcontact == null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "ABSession.FindByAnr returned object that is not a \"ABContact\". Continue.");
							}
							else
							{
								list.Add(abcontact.EmailAddress);
							}
						}
						num++;
					}
					this.photoRetriever.BeginGetThumbnailPhotoFromMailbox(list, this.pictureOptions.PhotoSize);
				}
				return;
			}
			if (operationRetryManagerResult.Exception is ABSubscriptionDisabledException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, operationRetryManagerResult.Exception, false)
				{
					ErrorStringForProtocolLogger = "ABSubsDisabled"
				};
			}
			if (operationRetryManagerResult.Exception is DataValidationException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, operationRetryManagerResult.Exception, false)
				{
					ErrorStringForProtocolLogger = "BadADDataInGalSearch"
				};
			}
			if (operationRetryManagerResult.Exception is DataSourceOperationException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, operationRetryManagerResult.Exception, false)
				{
					ErrorStringForProtocolLogger = "BadADDataSource"
				};
			}
			if (operationRetryManagerResult.Exception != null)
			{
				throw operationRetryManagerResult.Exception;
			}
			throw new InvalidOperationException("GalSearch failed with result code: " + operationRetryManagerResult.ResultCode);
		}

		public void ParseOptions(XmlElement optionsNode)
		{
			if (optionsNode != null)
			{
				XmlNode xmlNode = optionsNode["Range", "Search:"];
				if (xmlNode != null)
				{
					string[] array = xmlNode.InnerText.Split(new char[]
					{
						'-'
					});
					AirSyncDiagnostics.Assert(array.Length == 2);
					this.minRange = int.Parse(array[0], CultureInfo.InvariantCulture);
					this.maxRange = int.Parse(array[1], CultureInfo.InvariantCulture);
					if (this.minRange > this.maxRange || (this.protocolVersion < 120 && this.minRange != 0))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
						{
							ErrorStringForProtocolLogger = "MinMoreThanMaxinGalSearch"
						};
					}
					this.rangeSpecified = true;
				}
				XmlNode pictureOptionsNode = optionsNode["Picture", "Search:"];
				this.pictureOptions = PictureOptions.Parse(pictureOptionsNode, StatusCode.Sync_ProtocolVersionMismatch);
			}
			this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.PictureRequested, (this.pictureOptions != null) ? 1 : 0);
		}

		public void ParseQueryNode(XmlElement queryNode)
		{
			if (queryNode == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "NoQueryInGalSearch"
				};
			}
			if (queryNode.InnerText.Length <= 0 || queryNode.InnerText.Length > 256 || queryNode.ChildNodes.Count != 1 || !(queryNode.FirstChild is XmlText))
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "BadQueryInGalSearch"
				};
			}
			this.searchQuery = queryNode.InnerText;
		}

		private static string GetABProperty(ABObject addressBookObject, string airSyncPropertyName)
		{
			switch (airSyncPropertyName)
			{
			case "Alias":
				return addressBookObject.Alias;
			case "DisplayName":
				return addressBookObject.DisplayName;
			case "EmailAddress":
				return addressBookObject.EmailAddress;
			case "Company":
			case "FirstName":
			case "HomePhone":
			case "LastName":
			case "MobilePhone":
			case "Office":
			case "Phone":
			case "Title":
				return string.Empty;
			}
			throw new InvalidOperationException("Unexpected AirSync property name: " + airSyncPropertyName);
		}

		private static string GetABProperty(ABContact contact, string airSyncPropertyName)
		{
			switch (airSyncPropertyName)
			{
			case "Alias":
				return contact.Alias;
			case "DisplayName":
				return contact.DisplayName;
			case "EmailAddress":
				return contact.EmailAddress;
			case "Company":
				return contact.CompanyName;
			case "FirstName":
				return contact.GivenName;
			case "HomePhone":
				return contact.HomePhoneNumber;
			case "LastName":
				return contact.Surname;
			case "MobilePhone":
				return contact.MobilePhoneNumber;
			case "Office":
				return contact.OfficeLocation;
			case "Phone":
				return contact.BusinessPhoneNumber;
			case "Title":
				return contact.Title;
			}
			throw new InvalidOperationException("Unexpected AirSync property name: " + airSyncPropertyName);
		}

		private const int MaxAddressBookRetryCount = 3;

		private static ABOperationRetryManager retryManager = new ABOperationRetryManager(3);

		private static string[] schema = new string[]
		{
			"DisplayName",
			"Phone",
			"Office",
			"Title",
			"Company",
			"Alias",
			"FirstName",
			"LastName",
			"HomePhone",
			"MobilePhone",
			"EmailAddress"
		};

		private IList<ABObject> addressBookObjects;

		private IAirSyncUser user;

		private int lcid;

		private int maxRange = GlobalSettings.MaxGALSearchResults;

		private int minRange;

		private int numberResponses;

		private int protocolVersion;

		private bool rangeSpecified;

		private string searchQuery;

		private PictureOptions pictureOptions;

		private AirSyncPhotoRetriever photoRetriever;
	}
}
