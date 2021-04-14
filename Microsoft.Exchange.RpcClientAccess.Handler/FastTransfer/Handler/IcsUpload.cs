using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal abstract class IcsUpload : ServerObject, IServiceProvider<IcsStateHelper>, IIcsStateCheckpoint
	{
		public IcsUpload(ReferenceCount<CoreFolder> sourceFolder, Logon logon) : base(logon)
		{
			if (logon == null)
			{
				throw new ArgumentNullException("logon");
			}
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.icsStateHelper = new IcsStateHelper(sourceFolder);
				this.coreFolderReference = sourceFolder;
				this.coreFolderReference.AddRef();
				disposeGuard.Success();
			}
		}

		internal IcsStateHelper IcsStateHelper
		{
			get
			{
				base.CheckDisposed();
				return this.icsStateHelper;
			}
		}

		protected CoreFolder CoreFolder
		{
			get
			{
				base.CheckDisposed();
				return this.coreFolderReference.ReferencedObject;
			}
		}

		public static void ValidateSourceKey(byte[] sourceKey, string parameterName)
		{
			Util.ThrowOnNullArgument(sourceKey, parameterName);
			if (sourceKey.Length != 22)
			{
				throw new RopExecutionException(string.Format("Invalid SourceKey[{0}] {1}.", parameterName, new ArrayTracer<byte>(sourceKey)), (ErrorCode)2147942487U);
			}
		}

		public static void ValidateChangeKey(byte[] changeKey, string parameterName)
		{
			Util.ThrowOnNullArgument(changeKey, parameterName);
			if (changeKey.Length < 17)
			{
				throw new RopExecutionException(string.Format("Invalid ChangeKey[{0}] {1}.", parameterName, new ArrayTracer<byte>(changeKey)), (ErrorCode)2147942487U);
			}
		}

		IcsStateHelper IServiceProvider<IcsStateHelper>.Get()
		{
			return this.icsStateHelper;
		}

		IFastTransferProcessor<FastTransferDownloadContext> IIcsStateCheckpoint.CreateIcsStateCheckpointFastTransferObject()
		{
			base.CheckDisposed();
			this.UpdateState();
			return this.icsStateHelper.CreateIcsStateFastTransferObject();
		}

		internal abstract ErrorCode InternalImportDelete(ImportDeleteFlags importDeleteFlags, byte[][] sourceKeys);

		internal ErrorCode ImportDelete(ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges)
		{
			Util.ThrowOnNullArgument(deleteChanges, "deleteChanges");
			RopHandler.CheckEnum<ImportDeleteFlags>(importDeleteFlags);
			if (deleteChanges.Length != 1)
			{
				throw new RopExecutionException("Can only pass one multi-valued property with IDs to delete.", (ErrorCode)2147942487U);
			}
			if (deleteChanges[0].PropertyTag.PropertyId != PropertyId.Null)
			{
				throw new RopExecutionException("Can only pass a specific property tag with IDs to delete.", (ErrorCode)2147942487U);
			}
			return this.InternalImportDelete(importDeleteFlags, deleteChanges[0].GetValue<byte[][]>());
		}

		internal abstract void UpdateState();

		protected static ErrorCode ConvertGroupOperationResultToErrorCode(OperationResult result)
		{
			EnumValidator.ThrowIfInvalid<OperationResult>(result);
			switch (result)
			{
			case OperationResult.Succeeded:
				return ErrorCode.None;
			case OperationResult.Failed:
				return (ErrorCode)2147500037U;
			case OperationResult.PartiallySucceeded:
				return ErrorCode.PartialCompletion;
			default:
				throw new NotSupportedException();
			}
		}

		protected static DeleteItemFlags GetXsoDeleteItemFlags(ImportDeleteFlags importDeleteFlags)
		{
			if ((byte)(importDeleteFlags & ImportDeleteFlags.HardDelete) == 2)
			{
				return DeleteItemFlags.HardDelete;
			}
			return DeleteItemFlags.SoftDelete;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsUpload>(this);
		}

		protected override void InternalDispose()
		{
			ReferenceCount<CoreFolder>.ReleaseIfPresent(this.coreFolderReference);
			Util.DisposeIfPresent(this.icsStateHelper);
			base.InternalDispose();
		}

		private const int SourceKeyLength = 22;

		private const int MinChangeKeyLength = 17;

		private readonly IcsStateHelper icsStateHelper;

		private readonly ReferenceCount<CoreFolder> coreFolderReference;
	}
}
