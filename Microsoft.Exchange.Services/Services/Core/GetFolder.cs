using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetFolder : MultiStepServiceCommand<GetFolderRequest, BaseFolderType>
	{
		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingRead;
			}
		}

		public GetFolder(CallContext callContext, GetFolderRequest request) : base(callContext, request)
		{
			this.folderIds = base.Request.Ids;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<FolderResponseShape>(base.Request.ShapeName, base.Request.FolderShape, base.CallContext.FeaturesManager);
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.folderIds, "this.folderIds", "GetFolder::PreExecuteCommand");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "GetFolder::PreExecuteCommand");
		}

		internal override ServiceResult<BaseFolderType> Execute()
		{
			FaultInjection.GenerateFault((FaultInjection.LIDs)3116772669U);
			string text = null;
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(4186320189U, ref text);
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && num != 0)
			{
				throw new MapiExceptionMaxObjsExceeded("Fault injection MapiExceptionMaxObjsExceededInGetItem_ChangeValue", 0, num, null, null);
			}
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.folderIds[base.CurrentStep], IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.AllowKnownExternalUsers | (base.Request.IsHierarchicalOperation ? IdConverter.ConvertOption.IsHierarchicalOperation : IdConverter.ConvertOption.None));
			ServiceError serviceError;
			BaseFolderType folderObject = this.GetFolderObject(idAndSession, out serviceError);
			this.objectsChanged++;
			ServiceResult<BaseFolderType> result;
			if (serviceError == null)
			{
				result = new ServiceResult<BaseFolderType>(folderObject);
			}
			else
			{
				result = new ServiceResult<BaseFolderType>(folderObject, serviceError);
			}
			return result;
		}

		private BaseFolderType GetFolderObject(IdAndSession idAndSession, out ServiceError warning)
		{
			warning = null;
			FolderResponseShape folderResponseShape = this.responseShape;
			if (base.CallContext.IsExternalUser)
			{
				folderResponseShape = ExternalUserHandler.FilterFolderResponseShape(idAndSession as ExternalUserIdAndSession, this.responseShape, out warning);
			}
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(idAndSession.Id, idAndSession.Session, folderResponseShape, base.ParticipantResolver);
			BaseFolderType result;
			using (Folder xsoFolder = ServiceCommandBase.GetXsoFolder(idAndSession.Session, idAndSession.Id, ref toServiceObjectPropertyList))
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				if (idAndSession.Session is PublicFolderSession && ClientInfo.OWA.IsMatch(idAndSession.Session.ClientInfoString))
				{
					toServiceObjectPropertyList.CommandOptions |= CommandOptions.ConvertFolderIdToPublicFolderId;
					toServiceObjectPropertyList.CommandOptions |= CommandOptions.ConvertParentFolderIdToPublicFolderId;
				}
				BaseFolderType baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectId.ObjectType);
				ServiceCommandBase.LoadServiceObject(baseFolderType, xsoFolder, idAndSession, this.responseShape, toServiceObjectPropertyList);
				result = baseFolderType;
			}
			return result;
		}

		internal override int StepCount
		{
			get
			{
				return this.folderIds.Count;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetFolderResponse getFolderResponse = new GetFolderResponse();
			getFolderResponse.BuildForResults<BaseFolderType>(base.Results);
			return getFolderResponse;
		}

		private IList<BaseFolderId> folderIds;

		private FolderResponseShape responseShape;
	}
}
