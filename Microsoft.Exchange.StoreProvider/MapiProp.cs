using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MapiProp : MapiUnk
	{
		protected MapiProp(IExMapiProp iMAPIProp, object externalIMAPIProp, MapiStore mapiStore, Guid[] interfaceIds) : base(iMAPIProp, externalIMAPIProp, mapiStore)
		{
			bool flag = false;
			try
			{
				if (interfaceIds == null || interfaceIds.Length != 1)
				{
					throw new ArgumentException("interfaceIds");
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8210, 22, (long)this.GetHashCode(), "MapiProp.MapiProp: this={0}", TraceUtils.MakeHash(this));
				}
				this.iMAPIProp = iMAPIProp;
				this.externalIMAPIProp = (IMAPIProp)externalIMAPIProp;
				this.InterfaceGuids = interfaceIds;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		private Guid[] InterfaceGuids
		{
			get
			{
				return this.interfaceGuids;
			}
			set
			{
				this.interfaceGuids = value;
			}
		}

		protected override void MapiInternalDispose()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(12306, 22, (long)this.GetHashCode(), "MapiProp.InternalDispose: this={0}", TraceUtils.MakeHash(this));
			}
			this.iMAPIProp = null;
			this.externalIMAPIProp = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiProp>(this);
		}

		internal static PropTag SecurityPropToPropTag(SecurityProp secPropType)
		{
			switch (secPropType)
			{
			case SecurityProp.NTSD:
				return PropTag.NTSD;
			case SecurityProp.AdminNTSD:
				return PropTag.AdminNTSD;
			case SecurityProp.FreeBusyNTSD:
				return PropTag.FreeBusyNTSD;
			default:
				throw MapiExceptionHelper.ArgumentException("secPropType", "Invalid SecurityProp value");
			}
		}

		internal virtual NamedProp[] GetNamesFromIDs(Guid guidPropSet, int ulFlags, ICollection<PropTag> tags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			NamedProp[] result;
			try
			{
				NamedProp[] array = null;
				int namesFromIDs = this.iMAPIProp.GetNamesFromIDs(tags, guidPropSet, ulFlags, out array);
				if (namesFromIDs != 0)
				{
					base.ThrowIfError("Unable to get names from property IDs.", namesFromIDs);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8914, 22, (long)this.GetHashCode(), "MapiProp.GetNamesFromIDs results: ppa={0}", TraceUtils.DumpNamedPropsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public PropValue GetProp(PropTag tag)
		{
			PropValue propValue = this.GetProps(new PropTag[]
			{
				tag
			})[0];
			if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(9170, 22, (long)this.GetHashCode(), "MapiProp.GetProp: tag={0}, value={1}", TraceUtils.DumpPropTag(tag), TraceUtils.DumpPropVal(propValue));
			}
			return propValue;
		}

		public PropValue[] GetProps(ICollection<PropTag> propTagsRequested)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (propTagsRequested == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("propTagsRequested");
			}
			base.LockStore();
			PropValue[] result;
			try
			{
				int count = propTagsRequested.Count;
				PropValue[] array = new PropValue[count];
				if (count > 0)
				{
					int props = this.iMAPIProp.GetProps(propTagsRequested, 0, out array);
					if (props < 0)
					{
						base.ThrowIfError("Unable to get properties on object.", props);
					}
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(13266, 22, (long)this.GetHashCode(), "MapiProp.GetProps: propTagsRequested={0}, propsToReturn={1}", TraceUtils.DumpPropTagsArray(propTagsRequested), TraceUtils.DumpPropValsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public PropValue[] GetAllProps()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			PropValue[] result;
			try
			{
				PropValue[] array = null;
				int props = this.iMAPIProp.GetProps(null, int.MinValue, out array);
				if (props != 0)
				{
					base.ThrowIfError("Unable to get all properties on object.", props);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11218, 22, (long)this.GetHashCode(), "MapiProp.GetAllProps: propsToReturn={0}", TraceUtils.DumpPropValsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		protected virtual int SetPropsCall(ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems, bool trackChanges)
		{
			return this.iMAPIProp.SetProps(lpPropArray, out lppProblems);
		}

		public PropProblem[] SetProps(params PropValue[] pva)
		{
			return this.SetProps((ICollection<PropValue>)pva);
		}

		public PropProblem[] SetProps(ICollection<PropValue> pva)
		{
			return this.SetProps(pva, true);
		}

		public PropProblem[] SetProps(ICollection<PropValue> pva, bool trackChanges)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (pva == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("pva");
			}
			if (pva.Count <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("pva", "pva must contain at least 1 element");
			}
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15314, 22, (long)this.GetHashCode(), "MapiProp.SetProps params: pva={0}", TraceUtils.DumpPropValsArray(pva));
				}
				PropProblem[] array = null;
				int num = this.SetPropsCall(pva, out array, trackChanges);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set properties on object.", num);
				}
				if (array != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(10194, 22, (long)this.GetHashCode(), "MapiProp.SetProps problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public RawSecurityDescriptor GetSecurityDescriptor(SecurityProp secPropType)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			PropValue prop = this.GetProp(MapiProp.SecurityPropToPropTag(secPropType));
			if (prop.IsError() && prop.GetErrorValue() == 2409)
			{
				throw MapiExceptionHelper.NonCanonicalACLException("Copying non-canonical folder ACLs is not allowed during mailbox move.");
			}
			byte[] array = (byte[])prop.Value;
			if (array == null || array.Length <= 8)
			{
				throw MapiExceptionHelper.NotSupportedException("Security descriptor is empty");
			}
			int num = (int)BitConverter.ToInt16(array, 2);
			if (num != 3)
			{
				throw MapiExceptionHelper.NotSupportedException("Unsupported store security descriptor header - this is not a version 3 SD; the version found was 0x" + num.ToString("x") + ".");
			}
			int num2 = (int)BitConverter.ToInt16(array, 0);
			if (num2 < 8)
			{
				throw MapiExceptionHelper.NotSupportedException("Unsupported store security descriptor header - header is not at least 8 bytes long; the header length found was 0x" + num2.ToString("x") + ".");
			}
			RawSecurityDescriptor rawSecurityDescriptor = null;
			try
			{
				rawSecurityDescriptor = new RawSecurityDescriptor(array, num2);
			}
			catch (ArgumentNullException ex)
			{
				throw MapiExceptionHelper.ArgumentNullException(ex.ParamName);
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException(ex2.ParamName, ex2.Message);
			}
			catch (ArgumentException ex3)
			{
				throw MapiExceptionHelper.ArgumentException(ex3.ParamName, ex3.Message);
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(14290, 22, (long)this.GetHashCode(), "MapiProp.GetSecurityDescriptor results: secPropType={0}, sd={1}", secPropType.ToString(), (rawSecurityDescriptor == null) ? "null" : rawSecurityDescriptor.ToString());
			}
			return rawSecurityDescriptor;
		}

		public PropProblem[] SetSecurityDescriptor(SecurityProp secPropType, RawSecurityDescriptor sd)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(12242, 22, (long)this.GetHashCode(), "MapiProp.SetSecurityDescriptor params: secPropType={0}, sd={1}", secPropType.ToString(), (sd == null) ? "null" : sd.ToString());
				}
				byte[] array = new byte[sd.BinaryLength + 8];
				ExBitConverter.Write(196616, array, 0);
				ExBitConverter.Write(0, array, 4);
				sd.GetBinaryForm(array, 8);
				PropValue[] array2 = new PropValue[]
				{
					new PropValue(MapiProp.SecurityPropToPropTag(secPropType), array)
				};
				PropProblem[] array3 = this.SetProps((ICollection<PropValue>)array2);
				if (array3 != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(16338, 22, (long)this.GetHashCode(), "MapiProp.SetSecurityDescriptor problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array3));
				}
				result = array3;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void SaveChanges()
		{
			this.SaveChanges(SaveChangesFlags.KeepOpenReadWrite);
		}

		public void SaveChanges(SaveChangesFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace(8658, 22, (long)this.GetHashCode(), "MapiProp.SaveChanges");
				}
				int num = this.iMAPIProp.SaveChanges((int)flags);
				if (num != 0)
				{
					base.ThrowIfError("Unable to save changes.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public PropProblem[] CopyTo(MapiProp destProp, params PropTag[] excludeTags)
		{
			return this.CopyTo(destProp, (ICollection<PropTag>)excludeTags);
		}

		public PropProblem[] CopyTo(MapiProp destProp, ICollection<PropTag> excludeTags)
		{
			return this.CopyTo(destProp, false, CopyPropertiesFlags.None, true, excludeTags);
		}

		public PropProblem[] CopyTo(MapiProp destProp, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, bool copySubObjects, ICollection<PropTag> excludeTags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (destProp == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destProp");
			}
			if (reportProgress)
			{
				throw MapiExceptionHelper.NotSupportedException("reportProgress is not supported yet.");
			}
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(12754, 22, (long)this.GetHashCode(), "MapiProp.CopyTo params: destProp={0}, excludeTags={1}", TraceUtils.MakeHash(destProp), TraceUtils.DumpPropTagsArray(excludeTags));
				}
				int num = (int)copyPropertiesFlags;
				if (destProp is MapiMessage)
				{
					num |= -2147474432;
				}
				Guid lpInterface = this.InterfaceGuids[0];
				Guid[] array = (!copySubObjects) ? this.InterfaceGuids : Array<Guid>.Empty;
				PropTag[] lpExcludeProps = (excludeTags != null && excludeTags.Count > 0) ? PropTagHelper.SPropTagArray(excludeTags) : null;
				PropProblem[] array2 = null;
				destProp.LockStore();
				try
				{
					destProp.CheckDisposed();
					int hr;
					if (destProp.IsExternal)
					{
						hr = this.iMAPIProp.CopyTo_External(array.Length, array, lpExcludeProps, IntPtr.Zero, IntPtr.Zero, lpInterface, destProp.ExternalIMAPIProp, num, out array2);
					}
					else
					{
						hr = this.iMAPIProp.CopyTo(array.Length, array, lpExcludeProps, IntPtr.Zero, IntPtr.Zero, lpInterface, destProp.IMAPIProp, num, out array2);
					}
					base.ThrowIfErrorOrWarning("Unable to copy to target.", hr);
				}
				finally
				{
					destProp.UnlockStore();
				}
				if (array2 != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(10706, 22, (long)this.GetHashCode(), "MapiProp.CopyTo problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array2));
				}
				result = array2;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public PropProblem[] CopyProps(MapiProp destProp, params PropTag[] includeTags)
		{
			return this.CopyProps(destProp, CopyPropertiesFlags.None, (ICollection<PropTag>)includeTags);
		}

		public PropProblem[] CopyProps(MapiProp destProp, ICollection<PropTag> includeTags)
		{
			return this.CopyProps(destProp, CopyPropertiesFlags.None, includeTags);
		}

		public PropProblem[] CopyProps(MapiProp destProp, CopyPropertiesFlags copyPropertiesFlags, ICollection<PropTag> includeTags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (destProp == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destProp");
			}
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(14802, 22, (long)this.GetHashCode(), "MapiProp.CopyProps params: destProp={0}, includeTags={1}", TraceUtils.MakeHash(destProp), TraceUtils.DumpPropTagsArray(includeTags));
				}
				PropTag[] lpIncludeProps = (includeTags != null && includeTags.Count > 0) ? PropTagHelper.SPropTagArray(includeTags) : null;
				PropProblem[] array = null;
				destProp.CheckDisposed();
				Guid lpInterface = destProp.InterfaceGuids[0];
				destProp.LockStore();
				try
				{
					int hr;
					if (destProp.IsExternal)
					{
						hr = this.iMAPIProp.CopyProps_External(lpIncludeProps, IntPtr.Zero, IntPtr.Zero, lpInterface, destProp.ExternalIMAPIProp, (int)copyPropertiesFlags, out array);
					}
					else
					{
						hr = this.iMAPIProp.CopyProps(lpIncludeProps, IntPtr.Zero, IntPtr.Zero, lpInterface, destProp.IMAPIProp, (int)copyPropertiesFlags, out array);
					}
					base.ThrowIfErrorOrWarning("Unable to copy properties to target.", hr);
				}
				finally
				{
					destProp.UnlockStore();
				}
				if (array != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9682, 22, (long)this.GetHashCode(), "MapiProp.CopyProps problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public PropTag[] GetPropList()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			PropTag[] result;
			try
			{
				PropTag[] array = null;
				int propList = this.iMAPIProp.GetPropList(0, out array);
				if (propList != 0)
				{
					base.ThrowIfError("Unable to get property list.", propList);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13778, 22, (long)this.GetHashCode(), "MapiProp.GetPropList results: tagsRet={0}", TraceUtils.DumpPropTagsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		protected virtual int DeletePropsCall(ICollection<PropTag> tags, out PropProblem[] problemProps, bool trackChanges)
		{
			return this.iMAPIProp.DeleteProps(tags, out problemProps);
		}

		public PropProblem[] DeleteProps(params PropTag[] tags)
		{
			return this.DeleteProps((ICollection<PropTag>)tags);
		}

		public PropProblem[] DeleteProps(ICollection<PropTag> tags)
		{
			return this.DeleteProps(tags, true);
		}

		public PropProblem[] DeleteProps(ICollection<PropTag> tags, bool trackChanges)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (tags == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("tags");
			}
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11730, 22, (long)this.GetHashCode(), "MapiProp.DeleteProps params: tags={0}", TraceUtils.DumpPropTagsArray(tags));
				}
				PropProblem[] array = null;
				int num = this.DeletePropsCall(tags, out array, trackChanges);
				if (num != 0)
				{
					base.ThrowIfError("Unable to delete properties.", num);
				}
				if (array != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15826, 22, (long)this.GetHashCode(), "MapiProp.DeleteProps problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public NamedProp[] GetNamesFromIDs(ICollection<PropTag> tags)
		{
			if (tags == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("tags");
			}
			if (tags.Count <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("tags", "tags must contain at least 1 value");
			}
			return this.GetNamesFromIDs(Guid.Empty, 0, tags);
		}

		public NamedProp[] GetNamesFromIDs(Guid guidPropSet)
		{
			return this.GetNamesFromIDs(guidPropSet, GetNamesFromIDsFlags.None);
		}

		public NamedProp[] GetNamesFromIDs(Guid guidPropSet, GetNamesFromIDsFlags flags)
		{
			return this.GetNamesFromIDs(guidPropSet, (int)flags, null);
		}

		public PropTag[] GetIDsFromNames(bool fCreateIfNotExists, params NamedProp[] np)
		{
			return this.GetIDsFromNames(fCreateIfNotExists, (ICollection<NamedProp>)np);
		}

		public PropTag[] GetIDsFromNames(bool fCreateIfNotExists, ICollection<NamedProp> np)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (np == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("np");
			}
			if (np.Count <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("np", "Cannot have empty list of named props");
			}
			base.LockStore();
			PropTag[] result;
			try
			{
				int ulFlags = fCreateIfNotExists ? 2 : 0;
				PropTag[] array = null;
				int idsFromNames = this.iMAPIProp.GetIDsFromNames(np, ulFlags, out array);
				if (idsFromNames != 0)
				{
					base.ThrowIfError("Unable to get IDs from property names.", idsFromNames);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13010, 22, (long)this.GetHashCode(), "MapiProp.GetIDsFromNames results: ppa={0}", TraceUtils.DumpArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiStream OpenStream(PropTag propTag, OpenPropertyFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiStream result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(10962, 22, (long)this.GetHashCode(), "MapiProp.OpenStream params: propTag={0}, flags={1}", TraceUtils.DumpPropTag(propTag), flags.ToString());
				}
				IExInterface iExInterface = null;
				MapiStream mapiStream;
				try
				{
					iExInterface = this.InternalOpenProperty(propTag, InterfaceIds.IStreamGuid, 0, flags);
					mapiStream = new MapiStream(iExInterface.ToInterface<IExMapiStream>(), base.MapiStore);
					iExInterface = null;
				}
				finally
				{
					iExInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15058, 22, (long)this.GetHashCode(), "MapiProp.OpenStream results: stream={0}", TraceUtils.MakeHash(mapiStream));
				}
				result = mapiStream;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public object OpenProperty(PropTag propTag, Guid interfaceId, int interfaceOptions, OpenPropertyFlags flags)
		{
			base.CheckDisposed();
			if (propTag == PropTag.RulesTable && !base.MapiStore.ClassicRulesInterfaceAvailable)
			{
				MapiFolder mapiFolder = this as MapiFolder;
				if (mapiFolder != null)
				{
					return mapiFolder.GetRulesModifyTable();
				}
			}
			base.LockStore();
			object result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace(9938, 22, (long)this.GetHashCode(), "MapiProp.OpenProperty params: propTag={0}, interfaceId={1}, interfaceOptions={2}, flags={3}", new object[]
					{
						TraceUtils.DumpPropTag(propTag),
						interfaceId.ToString(),
						interfaceOptions.ToString(),
						flags.ToString()
					});
				}
				object obj = null;
				IExInterface exInterface = null;
				object obj2;
				try
				{
					if (base.IsExternal)
					{
						obj = this.InternalOpenPropertyExternal(propTag, interfaceId, interfaceOptions, flags);
						obj2 = MapiProp.WrapExternal(obj, interfaceId, base.MapiStore);
						obj = null;
					}
					else
					{
						exInterface = this.InternalOpenProperty(propTag, interfaceId, interfaceOptions, flags);
						obj2 = MapiProp.Wrap(exInterface, interfaceId, interfaceOptions, this, base.MapiStore);
						exInterface = null;
					}
				}
				finally
				{
					exInterface.DisposeIfValid();
					MapiUnk.ReleaseObject(obj);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14034, 22, (long)this.GetHashCode(), "MapiProp.OpenProperty results: resultObject={0}", TraceUtils.MakeHash(obj2));
				}
				result = obj2;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiFxProxy GetFxProxyCollector()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			IExInterface iExInterface = null;
			MapiFxCollector mapiFxCollector = null;
			MapiFxProxy mapiFxProxy = null;
			bool flag = false;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace(48845, 22, (long)this.GetHashCode(), "MapiProp.GetFxProxyCollector");
				}
				iExInterface = this.InternalOpenProperty(PropTag.FastTransfer, InterfaceIds.IExchangeFastTransferEx, 0, OpenPropertyFlags.None);
				mapiFxCollector = new MapiFxCollector(iExInterface.ToInterface<IExFastTransferEx>(), null, base.MapiStore);
				iExInterface = null;
				mapiFxProxy = new MapiFxProxy(mapiFxCollector);
				mapiFxCollector = null;
				if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(65229, 22, (long)this.GetHashCode(), "MapiProp.GetFxProxyCollector results: {0}", TraceUtils.MakeHash(mapiFxProxy));
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					iExInterface.DisposeIfValid();
					if (mapiFxCollector != null)
					{
						mapiFxCollector.Dispose();
					}
					if (mapiFxProxy != null)
					{
						mapiFxProxy.Dispose();
						mapiFxProxy = null;
					}
				}
			}
			return mapiFxProxy;
		}

		private PropProblem[] ExportObjectHelper(IMapiFxProxy dest, PropTag[] tags, CopyPropertiesFlags copyPropertiesFlags, bool useCopyProps)
		{
			if (dest == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("dest");
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(22))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(44749, 22, (long)this.GetHashCode(), "MapiProp.ExportObjectHelper params: dest={0}, tags={1}, fUseCopyProps={2}", TraceUtils.MakeHash(dest), TraceUtils.DumpPropTagsArray(tags), useCopyProps.ToString());
			}
			MapiStore mapiStore = (this as MapiStore) ?? base.MapiStore;
			FxProxyWrapper fxProxyWrapper;
			PropProblem[] array;
			using (MapiProp mapiProp = FxProxyWrapper.CreateFxProxyWrapper(dest, mapiStore, out fxProxyWrapper))
			{
				try
				{
					if (useCopyProps)
					{
						array = this.CopyProps(mapiProp, copyPropertiesFlags, (ICollection<PropTag>)tags);
					}
					else
					{
						array = this.CopyTo(mapiProp, false, copyPropertiesFlags, true, (ICollection<PropTag>)tags);
					}
				}
				catch (MapiPermanentException)
				{
					if (fxProxyWrapper.LastException != null)
					{
						MapiExceptionHelper.ThrowImportFailureException("Data import failed", fxProxyWrapper.LastException);
					}
					throw;
				}
				catch (MapiRetryableException)
				{
					if (fxProxyWrapper.LastException != null)
					{
						MapiExceptionHelper.ThrowImportFailureException("Data import failed", fxProxyWrapper.LastException);
					}
					throw;
				}
			}
			if (array != null && ComponentTrace<MapiNetTags>.CheckEnabled(22))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(61133, 22, (long)this.GetHashCode(), "MapiProp.ExportObjectHelper problems: {0}", TraceUtils.DumpPropProblemsArray(array));
			}
			return array;
		}

		public PropProblem[] ExportProps(IMapiFxProxy fxProxy, params PropTag[] includeTags)
		{
			return this.ExportObjectHelper(fxProxy, includeTags, CopyPropertiesFlags.None, true);
		}

		public PropProblem[] ExportProps(IMapiFxProxy fxProxy, CopyPropertiesFlags copyPropertiesFlags, params PropTag[] includeTags)
		{
			return this.ExportObjectHelper(fxProxy, includeTags, copyPropertiesFlags, true);
		}

		public PropProblem[] ExportObject(IMapiFxProxy fxProxy, params PropTag[] excludeTags)
		{
			return this.ExportObjectHelper(fxProxy, excludeTags, CopyPropertiesFlags.None, false);
		}

		public PropProblem[] ExportObject(IMapiFxProxy fxProxy, CopyPropertiesFlags copyPropertiesFlags, params PropTag[] excludeTags)
		{
			return this.ExportObjectHelper(fxProxy, excludeTags, copyPropertiesFlags, false);
		}

		protected IntPtr IMAPIProp
		{
			get
			{
				return ((SafeExInterfaceHandle)this.iMAPIProp).DangerousGetHandle();
			}
		}

		protected IMAPIProp ExternalIMAPIProp
		{
			get
			{
				return this.externalIMAPIProp;
			}
		}

		protected IExInterface InternalOpenProperty(PropTag propTag, Guid interfaceId, int interfaceOptions, OpenPropertyFlags flags)
		{
			base.BlockExternalObjectCheck();
			IExInterface exInterface = null;
			bool flag = false;
			try
			{
				int num = this.iMAPIProp.OpenProperty((int)propTag, interfaceId, interfaceOptions, (int)flags, out exInterface);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to open property 0x{0:X} (Interface {1}, Options {2}) with flags {3}.", new object[]
					{
						(int)propTag,
						interfaceId,
						interfaceOptions,
						flags
					}), num);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					exInterface.DisposeIfValid();
					exInterface = null;
				}
			}
			return exInterface;
		}

		protected object InternalOpenPropertyExternal(PropTag propTag, Guid interfaceId, int interfaceOptions, OpenPropertyFlags flags)
		{
			object obj = null;
			bool flag = false;
			try
			{
				int num = this.ExternalIMAPIProp.OpenProperty((int)propTag, interfaceId, interfaceOptions, (int)flags, out obj);
				if (num != 0)
				{
					base.ThrowIfError(string.Format("Unable to open property 0x{0:X} (Interface {1}, Options {2}) with flags {3}.", new object[]
					{
						(int)propTag,
						interfaceId,
						interfaceOptions,
						flags
					}), num);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					MapiUnk.ReleaseObject(obj);
					obj = null;
				}
			}
			return obj;
		}

		private static object Wrap(IExInterface obj, Guid interfaceId, int interfaceOptions, MapiProp mapiProp, MapiStore mapiStore)
		{
			if (interfaceId.Equals(InterfaceIds.IStreamGuid))
			{
				return new MapiStream(obj.ToInterface<IExMapiStream>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IMessageGuid))
			{
				return new MapiMessage(obj.ToInterface<IExMapiMessage>(), null, mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeModifyTable))
			{
				return new MapiModifyTable(obj.ToInterface<IExModifyTable>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeExportChanges))
			{
				return new MapiSynchronizer(obj.ToInterface<IExExportChanges>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeImportContentsChanges))
			{
				return new MapiCollector(obj.ToInterface<IExImportContentsChanges>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeImportContentsChanges3))
			{
				return new MapiCollector(obj.ToInterface<IExImportContentsChanges>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeImportHierarchyChanges))
			{
				return new MapiHierarchyCollector(obj.ToInterface<IExImportHierarchyChanges>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeExportManifest))
			{
				return new MapiManifest(obj.ToInterface<IExExportManifest>(), mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IExchangeFastTransferEx))
			{
				return new MapiFxCollector(obj.ToInterface<IExFastTransferEx>(), null, mapiStore);
			}
			if (interfaceId.Equals(InterfaceIds.IFastTransferStream))
			{
				return new MapiFastTransferStream(new SafeExFastTransferStreamHandle((SafeExInterfaceHandle)obj), (MapiFastTransferStreamMode)interfaceOptions, mapiStore);
			}
			throw MapiExceptionHelper.NotSupportedException("Unsupported interface for OpenProperty");
		}

		private static object WrapExternal(object obj, Guid interfaceId, MapiStore mapiStore)
		{
			if (interfaceId.Equals(InterfaceIds.IExchangeFastTransferEx))
			{
				return new MapiFxCollector(null, obj, mapiStore);
			}
			throw MapiExceptionHelper.NotSupportedException("Unsupported interface for OpenProperty");
		}

		internal const int StoreNTSDHeaderRev3 = 196616;

		private IExMapiProp iMAPIProp;

		private IMAPIProp externalIMAPIProp;

		private Guid[] interfaceGuids;
	}
}
