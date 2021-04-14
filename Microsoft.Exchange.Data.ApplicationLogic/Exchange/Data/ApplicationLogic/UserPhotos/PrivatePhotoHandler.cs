using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PrivatePhotoHandler : IPhotoHandler
	{
		public PrivatePhotoHandler(PhotosConfiguration configuration, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.xsoFactory = xsoFactory;
			this.tracer = upstreamTracer;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("PrivateHandlerTotal", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("PrivateHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.Private))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else if (request.RequestorMailboxSession == null)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: skipped because a session to the requestor's mailbox has NOT been initialized.");
						result = response;
					}
					else
					{
						response.PrivateHandlerProcessed = true;
						request.PerformanceLogger.Log("PrivateHandlerProcessed", string.Empty, 1U);
						try
						{
							result = this.FindContactOrPersonAndServePhoto(request, response);
						}
						catch (ObjectNotFoundException arg)
						{
							this.tracer.TraceDebug<ObjectNotFoundException>((long)this.GetHashCode(), "PRIVATE HANDLER: photo not found.  Exception: {0}", arg);
							result = response;
						}
						catch (StorageTransientException arg2)
						{
							this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "PRIVATE HANDLER: transient exception at reading photo.  Exception: {0}", arg2);
							throw;
						}
						catch (StoragePermanentException arg3)
						{
							this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "PRIVATE HANDLER: permanent exception at reading photo.  Exception: {0}", arg3);
							throw;
						}
					}
				}
			}
			return result;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private PhotoResponse FindContactOrPersonAndServePhoto(PhotoRequest request, PhotoResponse response)
		{
			StoreObjectId storeObjectId = this.FindContactId(request);
			if (storeObjectId == null)
			{
				return this.FindPersonAndServePhoto(request, response);
			}
			return this.ReadPhotoOffOfContactAndServe(storeObjectId, request, response);
		}

		private PhotoResponse FindPersonAndServePhoto(PhotoRequest request, PhotoResponse response)
		{
			Person person = this.FindPerson(request);
			if (person == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: person could not be found.");
				return this.ServePhotoNotFound(response);
			}
			return this.ReadPhotoOffOfPersonAndServe(person, request, response);
		}

		private StoreObjectId FindContactId(PhotoRequest request)
		{
			return request.TargetContactId;
		}

		private Person FindPerson(PhotoRequest request)
		{
			PersonId personId = this.FindPersonId(request);
			if (personId == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: person ID is NOT available and could NOT be computed.");
				return null;
			}
			this.tracer.TraceDebug<PersonId>((long)this.GetHashCode(), "PRIVATE HANDLER: loading person with ID '{0}' from mailbox.", personId);
			Person result;
			using (new StopwatchPerformanceTracker("GetPersonFromPersonId", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("GetPersonFromPersonId", request.PerformanceLogger))
				{
					result = Person.Load((MailboxSession)request.RequestorMailboxSession, personId, PrivatePhotoHandler.PersonPropertiesToLoad);
				}
			}
			return result;
		}

		private PersonId FindPersonId(PhotoRequest request)
		{
			if (request.TargetPersonId != null)
			{
				return request.TargetPersonId;
			}
			if (string.IsNullOrEmpty(request.TargetSmtpAddress))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: target person ID has NOT been initialized and email address is NOT available.");
				return null;
			}
			PersonId result;
			using (new StopwatchPerformanceTracker("FindPersonIdByEmailAddress", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("FindPersonIdByEmailAddress", request.PerformanceLogger))
				{
					result = Person.FindPersonIdByEmailAddress(request.RequestorMailboxSession, this.xsoFactory, request.TargetSmtpAddress);
				}
			}
			return result;
		}

		private PhotoResponse ReadPhotoOffOfPersonAndServe(Person person, PhotoRequest request, PhotoResponse response)
		{
			string preferredThirdPartyPhotoUrl = person.PreferredThirdPartyPhotoUrl;
			if (!string.IsNullOrEmpty(preferredThirdPartyPhotoUrl))
			{
				return this.ServePhotoWithRedirect(request, response, preferredThirdPartyPhotoUrl, null, string.Empty);
			}
			StoreObjectId photoSourceContactId;
			string photoSourceNetworkId;
			PhotoResponse result;
			using (Stream photoStreamFromPerson = this.GetPhotoStreamFromPerson(request, person, out photoSourceContactId, out photoSourceNetworkId))
			{
				if (photoStreamFromPerson == null)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: person doesn't have a photo.");
					result = this.ServePhotoNotFound(response);
				}
				else if (photoStreamFromPerson.Length == 0L)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: photo attached to person is empty.");
					result = this.ServePhotoNotFound(response);
				}
				else
				{
					result = this.ServePhotoContent(request, response, photoStreamFromPerson, photoSourceContactId, photoSourceNetworkId);
				}
			}
			return result;
		}

		private Stream GetPhotoStreamFromPerson(PhotoRequest request, Person person, out StoreObjectId photoSourceContactId, out string photoSourceNetworkId)
		{
			Stream attachedPhoto;
			using (new StopwatchPerformanceTracker("GetPhotoStreamFromPerson", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("GetPhotoStreamFromPerson", request.PerformanceLogger))
				{
					attachedPhoto = person.GetAttachedPhoto(out photoSourceNetworkId, out photoSourceContactId);
				}
			}
			return attachedPhoto;
		}

		private PhotoResponse ReadPhotoOffOfContactAndServe(StoreObjectId contactId, PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (IContact contact = this.xsoFactory.BindToContact(request.RequestorMailboxSession, contactId, PrivatePhotoHandler.ContactPropertiesToLoad))
			{
				string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.PartnerNetworkThumbnailPhotoUrl, null);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					result = this.ServePhotoWithRedirect(request, response, valueOrDefault, contactId, string.Empty);
				}
				else
				{
					using (Stream photoStream = contact.GetPhotoStream())
					{
						if (photoStream == null)
						{
							result = this.ServePhotoNotFound(response);
						}
						else if (photoStream.Length == 0L)
						{
							result = this.ServePhotoNotFound(response);
						}
						else
						{
							result = this.ServePhotoContent(request, response, photoStream, contactId, string.Empty);
						}
					}
				}
			}
			return result;
		}

		private PhotoResponse ServePhotoNotFound(PhotoResponse response)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: no photo could be found.");
			return response;
		}

		private PhotoResponse ServePhotoContent(PhotoRequest request, PhotoResponse response, Stream photo, StoreObjectId photoSourceContactId, string photoSourceNetworkId)
		{
			this.tracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "PRIVATE HANDLER: serving photo found in contact with ID: '{0}';  source network: {1}", photoSourceContactId, photoSourceNetworkId);
			response.Served = true;
			response.Status = HttpStatusCode.OK;
			using (new StopwatchPerformanceTracker("PrivateHandlerReadPhotoStream", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("PrivateHandlerReadPhotoStream", request.PerformanceLogger))
				{
					photo.CopyTo(response.OutputPhotoStream);
				}
			}
			response.ContentType = "image/jpeg";
			response.ContentLength = photo.Length;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.OK, this.configuration);
			response.Thumbprint = null;
			this.TraceInformationAboutContact(photoSourceContactId, request);
			request.PerformanceLogger.Log("PrivateHandlerServedContent", string.Empty, 1U);
			return response;
		}

		private PhotoResponse ServePhotoWithRedirect(PhotoRequest request, PhotoResponse response, string photoUrl, StoreObjectId photoSourceContactId, string photoSourceNetworkId)
		{
			this.tracer.TraceDebug<string, StoreObjectId, string>((long)this.GetHashCode(), "PRIVATE HANDLER: serving photo at '{0}';  found in contact with ID: '{1}';  source network: '{2}'", photoUrl, photoSourceContactId, photoSourceNetworkId);
			response.Served = true;
			response.Status = HttpStatusCode.Found;
			response.PhotoUrl = photoUrl;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.Found, this.configuration);
			response.Thumbprint = null;
			this.TraceInformationAboutContact(photoSourceContactId, request);
			request.PerformanceLogger.Log("PrivateHandlerServedRedirect", string.Empty, 1U);
			return response;
		}

		private void TraceInformationAboutContact(StoreObjectId photoSourceContactId, PhotoRequest request)
		{
			if (!request.Trace)
			{
				return;
			}
			if (photoSourceContactId == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: cannot trace additional information about contact because its ID is unknown.");
				return;
			}
			using (IContact contact = this.xsoFactory.BindToContact(request.RequestorMailboxSession, photoSourceContactId, PrivatePhotoHandler.ContactPropertiesToLoadForDiagnostics))
			{
				using (IFolder folder = this.xsoFactory.BindToFolder(request.RequestorMailboxSession, contact.ParentId))
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "PRIVATE HANDLER: contact with chosen photo has ID: '{0}';  Display name: '{1}';  Parent folder ID: '{2}';  Parent folder display name: '{3}'", new object[]
					{
						photoSourceContactId,
						contact.DisplayName,
						contact.ParentId,
						folder.DisplayName
					});
				}
			}
		}

		private const string ContactPhotoImageType = "image/jpeg";

		private static readonly PropertyDefinition[] PersonPropertiesToLoad = new PropertyDefinition[]
		{
			PersonSchema.Id,
			PersonSchema.PhotoContactEntryId,
			PersonSchema.EmailAddress,
			PersonSchema.ThirdPartyPhotoUrls
		};

		private static readonly PropertyDefinition[] ContactPropertiesToLoad = new PropertyDefinition[]
		{
			ContactSchema.PartnerNetworkThumbnailPhotoUrl
		};

		private static readonly PropertyDefinition[] ContactPropertiesToLoadForDiagnostics = new PropertyDefinition[]
		{
			StoreObjectSchema.DisplayName,
			StoreObjectSchema.ParentItemId
		};

		private readonly PhotosConfiguration configuration;

		private readonly IXSOFactory xsoFactory;

		private readonly ITracer tracer;
	}
}
