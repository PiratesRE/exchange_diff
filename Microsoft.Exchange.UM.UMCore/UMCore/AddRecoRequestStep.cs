using System;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AddRecoRequestStep : MobileSpeechRecoRequestStep
	{
		public AddRecoRequestStep(Guid requestId, MdbefPropertyCollection args) : base(requestId, args, "Add Reco Request")
		{
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering AddRecoRequestStep constructor", new object[0]);
		}

		protected override void InternalExecuteAsync()
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.RequestId, "Entering AddRecoRequestStep.InternalExecuteAsync", new object[0]);
			base.CollectStartStatisticsLog();
			MobileSpeechRecoRequestType mobileSpeechRecoRequestType;
			CultureInfo cultureInfo;
			Guid guid;
			Guid guid2;
			ExTimeZone exTimeZone;
			this.ExtractArgs(base.Args, out mobileSpeechRecoRequestType, out cultureInfo, out guid, out guid2, out exTimeZone);
			MobileSpeechRecoTracer.TraceDebug(this, base.RequestId, "AddRecoRequestStep.ExecuteAsync requestType='{0}' culture='{1}' userObjectGuid='{2}' tenantGuid='{3}', timeZone='{4}'", new object[]
			{
				mobileSpeechRecoRequestType,
				cultureInfo,
				guid,
				guid2,
				exTimeZone.Id
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoAddRecoRequestRPCParams, null, new object[]
			{
				base.RequestId,
				mobileSpeechRecoRequestType,
				cultureInfo.Name,
				guid,
				guid2,
				exTimeZone.Id
			});
			IMobileSpeechRecoRequestBehavior requestBehavior = MobileSpeechRecoRequestBehavior.Create(mobileSpeechRecoRequestType, base.RequestId, cultureInfo, guid, guid2, exTimeZone);
			MobileSpeechRecoDispatcher.Instance.AddRecoRequestAsync(base.RequestId, requestBehavior, Platform.Builder, new MobileRecoAsyncCompletedDelegate(base.OnStepCompleted));
		}

		private void ExtractArgs(MdbefPropertyCollection inArgs, out MobileSpeechRecoRequestType requestType, out CultureInfo culture, out Guid userObjectGuid, out Guid tenantGuid, out ExTimeZone timeZone)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.RequestId, "Entering AddRecoRequestStep.ExtractArgs", new object[0]);
			object obj = null;
			obj = MobileSpeechRecoRequestStep.ExtractArg(inArgs, 2416050179U, "requestType");
			requestType = (MobileSpeechRecoRequestType)obj;
			if (!EnumValidator.IsValidValue<MobileSpeechRecoRequestType>(requestType))
			{
				MobileSpeechRecoTracer.TraceError(this, base.RequestId, "AddRecoRequestStep.ExtractArgs - Invalid request type='{0}'", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("requestType", obj, "Invalid value");
			}
			obj = MobileSpeechRecoRequestStep.ExtractArg(inArgs, 2416115743U, "cultureName");
			string name = obj as string;
			try
			{
				culture = new CultureInfo(name);
			}
			catch (ArgumentNullException)
			{
				MobileSpeechRecoTracer.TraceError(this, base.RequestId, "AddRecoRequestStep.ExtractArgs -  Culture name is null", new object[0]);
				throw new ArgumentNullException("cultureName");
			}
			catch (ArgumentException)
			{
				MobileSpeechRecoTracer.TraceError(this, base.RequestId, "AddRecoRequestStep.ExtractArgs -  Invalid culture name='{0}'", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("cultureName", obj, "Invalid value");
			}
			obj = MobileSpeechRecoRequestStep.ExtractArg(inArgs, 2416181320U, "userObjectGuid");
			userObjectGuid = (Guid)obj;
			if (userObjectGuid == Guid.Empty)
			{
				MobileSpeechRecoTracer.TraceError(this, base.RequestId, "AddRecoRequestStep.ExtractArgs -  Invalid user object guid='{0}' (empty)", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("userObjectGuid", obj, "User object guid is empty");
			}
			obj = MobileSpeechRecoRequestStep.ExtractArg(inArgs, 2416377928U, "tenantGuid");
			tenantGuid = (Guid)obj;
			obj = MobileSpeechRecoRequestStep.ExtractArg(inArgs, 2416312351U, "timeZone");
			string text = obj as string;
			if (text == null)
			{
				MobileSpeechRecoTracer.TraceError(this, base.RequestId, "AddRecoRequestStep.ExtractArgs -  Invalid time zone ='{0}'", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("timeZone", obj, "Invalid value");
			}
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out timeZone))
			{
				throw new ArgumentOutOfRangeException("timeZone", obj, "Invalid parameter");
			}
		}

		private const string DescriptionStr = "Add Reco Request";
	}
}
