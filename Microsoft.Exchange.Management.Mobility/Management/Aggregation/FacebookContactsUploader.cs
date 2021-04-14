using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Net.Facebook;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FacebookContactsUploader
	{
		internal FacebookContactsUploader(IContactsUploaderPerformanceTracker performanceTracker, IFacebookClient client, IPeopleConnectApplicationConfig configuration, Func<PropertyDefinition[], IEnumerable<IStorePropertyBag>> contactsEnumeratorBuilder)
		{
			ArgumentValidator.ThrowIfNull("performanceTracker", performanceTracker);
			ArgumentValidator.ThrowIfNull("client", client);
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("contactsEnumerator", contactsEnumeratorBuilder);
			this.performanceTracker = performanceTracker;
			this.client = client;
			this.configuration = configuration;
			PropertyDefinition[] arg = PropertyDefinitionCollection.Merge<PropertyDefinition>(FacebookContactsUploader.ContactPropertiesToExport, FacebookContactsUploader.AdditionalContactPropertiesToLoad);
			this.contactsEnumerator = contactsEnumeratorBuilder(arg);
		}

		internal void UploadContacts(string accessToken)
		{
			FacebookContactsUploader.Tracer.TraceFunction((long)this.GetHashCode(), "Entering FacebookContactsUploader.UploadContacts.");
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new FacebookContactUploadException(Strings.FacebookEmptyAccessToken);
			}
			bool continueOnContactUploadFailure = this.configuration.ContinueOnContactUploadFailure;
			try
			{
				this.performanceTracker.Start();
				int maximumContactsToUpload = this.configuration.MaximumContactsToUpload;
				IEnumerable<IStorePropertyBag> enumerable = this.contactsEnumerator.Where(new Func<IStorePropertyBag, bool>(this.ShouldExportContact)).Take(maximumContactsToUpload);
				ContactsExporter contactsExporter = new ContactsExporter(FacebookContactsUploader.ContactPropertiesToExport, enumerable);
				using (Stream streamFromContacts = contactsExporter.GetStreamFromContacts())
				{
					this.performanceTracker.AddTimeBookmark(ContactsUploaderPerformanceTrackerBookmarks.ExportTime);
					using (MultipartFormDataContent multipartFormDataContent = FacebookContactsUploader.CreateMultipartFormDataContent(FacebookContactsUploader.MultipartFormDataBoundary, contactsExporter.ContentType, "contacts", streamFromContacts))
					{
						this.performanceTracker.AddTimeBookmark(ContactsUploaderPerformanceTrackerBookmarks.FormatTime);
						using (Stream result = multipartFormDataContent.ReadAsStreamAsync().Result)
						{
							this.performanceTracker.ExportedDataSize = (double)result.Length;
							this.performanceTracker.ReceivedContactsCount = this.UploadContactsToFacebook(accessToken, contactsExporter.Format, FacebookContactsUploader.GetMultipartFormDataContentType(), result);
						}
						this.performanceTracker.AddTimeBookmark(ContactsUploaderPerformanceTrackerBookmarks.UploadTime);
					}
				}
			}
			catch (TimeoutException exception)
			{
				this.ProcessFacebookFailure(exception, Strings.FacebookTimeoutError, !continueOnContactUploadFailure);
			}
			catch (ProtocolException exception2)
			{
				FacebookClient.AppendDiagnoseDataToException(exception2);
				this.ProcessFacebookFailure(exception2, Strings.FacebookAuthorizationError, !continueOnContactUploadFailure);
			}
			catch (CommunicationException exception3)
			{
				FacebookClient.AppendDiagnoseDataToException(exception3);
				this.ProcessFacebookFailure(exception3, Strings.FacebookCommunicationError, !continueOnContactUploadFailure);
			}
			catch (SerializationException exception4)
			{
				this.ProcessFacebookFailure(exception4, Strings.FacebookCommunicationError, !continueOnContactUploadFailure);
			}
			finally
			{
				this.performanceTracker.Stop();
				FacebookContactsUploader.Tracer.TraceFunction((long)this.GetHashCode(), "Leaving FacebookContactsUploader.UploadContacts.");
			}
		}

		private int UploadContactsToFacebook(string accessToken, string contactsFormat, string contactsContentType, Stream contactStream)
		{
			FacebookContactsUploader.Tracer.TraceDebug((long)this.GetHashCode(), "Calling FacebookClient.UploadContacts.");
			FacebookImportContactsResult facebookImportContactsResult = this.client.UploadContacts(accessToken, this.configuration.NotifyOnEachContactUpload, this.configuration.WaitForContactUploadCommit, "office365", contactsFormat, contactsContentType, contactStream);
			int num;
			if (facebookImportContactsResult != null)
			{
				num = facebookImportContactsResult.ProcessedContacts;
				FacebookContactsUploader.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Return from FacebookClient.UploadContacts. Number of contacts processed by Facebook {0}.", num);
			}
			else
			{
				num = -1;
				FacebookContactsUploader.Tracer.TraceDebug((long)this.GetHashCode(), "Return from FacebookClient.UploadContacts. No FacebookImportContactsResult was found, unkown number of contacts processed.");
			}
			return num;
		}

		private void ProcessFacebookFailure(Exception exception, LocalizedString defaultDescription, bool shouldThrow)
		{
			FacebookContactsUploader.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Found exception while uploading contacts to Facebook. Exception {0}", exception);
			this.performanceTracker.OperationResult = FacebookContactsUploader.SerializeExceptionForOperationResult(exception);
			if (shouldThrow)
			{
				throw new FacebookContactUploadException(defaultDescription, exception);
			}
		}

		private static string SerializeExceptionForOperationResult(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(exception.ToString());
			if (exception.Data != null && exception.Data.Count > 0)
			{
				stringBuilder.Append("-- Data: {");
				foreach (object obj in exception.Data.Keys)
				{
					stringBuilder.Append(string.Format("{0}:{1},", obj, exception.Data[obj]));
				}
				stringBuilder.Append("}");
			}
			return stringBuilder.ToString();
		}

		private static MultipartFormDataContent CreateMultipartFormDataContent(string boundary, string contentType, string fieldName, Stream data)
		{
			return new MultipartFormDataContent(boundary)
			{
				{
					new StreamContent(data)
					{
						Headers = 
						{
							{
								"Content-Type",
								contentType
							}
						}
					},
					fieldName,
					FacebookContactsUploader.GenerateContactsFileName()
				}
			};
		}

		private static string GenerateContactsFileName()
		{
			return string.Format("O365_{0}.csv", Guid.NewGuid().ToString("N"));
		}

		private static string GetMultipartFormDataContentType()
		{
			return string.Format("multipart/form-data; boundary={0}", FacebookContactsUploader.MultipartFormDataBoundary);
		}

		private bool ShouldExportContact(IStorePropertyBag contact)
		{
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			bool flag = string.IsNullOrEmpty(valueOrDefault);
			this.performanceTracker.IncrementContactsRead();
			if (flag)
			{
				this.performanceTracker.IncrementContactsExported();
			}
			return flag;
		}

		private const string ContactsSource = "office365";

		private const string ContentTypeHeaderName = "Content-Type";

		private const string ContactsFileNameFormat = "O365_{0}.csv";

		private const string ContactsFieldName = "contacts";

		private const string MultipartFormDataContentTypeFormat = "multipart/form-data; boundary={0}";

		internal static readonly Trace Tracer = ExTraceGlobals.FacebookTracer;

		private static readonly string MultipartFormDataBoundary = string.Format("------------o365-{0}", Guid.NewGuid().ToString("N"));

		private static readonly PropertyDefinition[] ContactPropertiesToExport = new PropertyDefinition[]
		{
			ContactSchema.GivenName,
			ContactSchema.MiddleName,
			ContactSchema.Surname,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactSchema.HomePhone,
			ContactSchema.HomePhone2,
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.MobilePhone,
			ContactSchema.OtherTelephone,
			ContactSchema.PrimaryTelephoneNumber,
			ContactSchema.HomeStreet,
			ContactSchema.HomeCity,
			ContactSchema.HomeState,
			ContactSchema.HomePostalCode,
			ContactSchema.HomeCountry,
			ContactSchema.OtherStreet,
			ContactSchema.OtherCity,
			ContactSchema.OtherState,
			ContactSchema.OtherPostalCode,
			ContactSchema.OtherCountry,
			ContactSchema.WorkAddressStreet,
			ContactSchema.WorkAddressCity,
			ContactSchema.WorkAddressState,
			ContactSchema.WorkAddressPostalCode,
			ContactSchema.WorkAddressCountry
		};

		private static readonly PropertyDefinition[] AdditionalContactPropertiesToLoad = new PropertyDefinition[]
		{
			ContactSchema.PartnerNetworkId
		};

		private readonly IFacebookClient client;

		private readonly IPeopleConnectApplicationConfig configuration;

		private readonly IEnumerable<IStorePropertyBag> contactsEnumerator;

		private readonly IContactsUploaderPerformanceTracker performanceTracker;
	}
}
