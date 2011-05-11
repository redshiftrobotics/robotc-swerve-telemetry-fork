//
// COM.cs
//
// Miscellaneous glue for talking to COM & OLE

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using COMTypes = System.Runtime.InteropServices.ComTypes;

namespace TelemetryFTC
    {
    static class COM
        {
        public static Guid IIDOf(Type type)
            {
            foreach (object o in type.GetCustomAttributes(false))
                {
                GuidAttribute guidAttr = o as GuidAttribute;
                if (guidAttr != null)
                    {
                    return new Guid(guidAttr.Value);
                    }
                }
            throw new InvalidOperationException();
            }

        //-----------------------------------------------------------------

        [DllImport("ole32.dll")]
        public static extern int CoCreateInstance(
            [In] ref Guid rclsid,
            [In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkOuter,
            [In, MarshalAs(UnmanagedType.U4)] CLSCTX dwClsContext,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out Object ppv);

        //-----------------------------------------------------------------

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(
            [In] uint reserved,
            [Out, MarshalAs(UnmanagedType.Interface)] out COMTypes.IBindCtx pbindContext);


        public static COMTypes.IBindCtx CreateBindContext()
            {
            COMTypes.IBindCtx pbc;
            int hr = COM.CreateBindCtx(0, out pbc);
            if (0==hr)
                {
                COMTypes.BIND_OPTS opts = new COMTypes.BIND_OPTS();
                opts.cbStruct = Marshal.SizeOf(opts);
                opts.grfMode = (int)BIND.MAYBOTHERUSER;
                pbc.SetBindOptions(ref opts);
                return pbc;
                }
            else
                {
	            Marshal.ThrowExceptionForHR(hr);
                return null;
                }
            }

        public enum BIND : int
            {
            MAYBOTHERUSER       = 1,
            JUSTTESTEXISTENCE   = 2 
            }

        //-----------------------------------------------------------------

        [DllImport("ole32.dll")]
        public static extern int MkParseDisplayName(
            [In, MarshalAs(UnmanagedType.Interface)]      COMTypes.IBindCtx pbindContext,
            [In, MarshalAs(UnmanagedType.LPWStr)]         string            sDisplayName,
                                                          IntPtr            pcchEaten,
            [Out, MarshalAs(UnmanagedType.Interface)] out COMTypes.IMoniker ppmk);

        public static COMTypes.IMoniker MkParseDisplayName(string sDisplayName)
            {
            COMTypes.IMoniker ppmk = null;
            COMTypes.IBindCtx pbc = CreateBindContext();
            IntPtr cchEaten = Marshal.AllocCoTaskMem(sizeof(int));
            try {
                int hr = MkParseDisplayName(pbc, sDisplayName, cchEaten, out ppmk);
                if (hr != 0) Marshal.ThrowExceptionForHR(hr);
                }
            finally
                {
                Marshal.FreeCoTaskMem(cchEaten);
                }

            return ppmk;
            }


        //-----------------------------------------------------------------

        [DllImport("ole32.dll")] public static extern int OleInitialize(IntPtr reserved);
        [DllImport("ole32.dll")] public static extern int OleUninitialize();

        //-----------------------------------------------------------------

        public static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        public enum OLEGETMONIKER : uint
            {
            ONLYIFTHERE = 1,
            FORCEASSIGN = 2,
            UNASSIGN    = 3,
            TEMPFORUSER = 4
            };

        public enum OLEWHICHMK : uint
            {
            CONTAINER = 1,
            OBJREL    = 2,
            OBJFULL   = 3
            };

        public enum USERCLASSTYPE : uint
            {
            FULL    = 1,
            SHORT   = 2,
            APPNAME = 3,
            };

        public enum OLEMISC : uint
            {
            RECOMPOSEONRESIZE           = 0x00000001,
            ONLYICONIC                  = 0x00000002,
            INSERTNOTREPLACE            = 0x00000004,
            STATIC                      = 0x00000008,
            CANTLINKINSIDE              = 0x00000010,
            CANLINKBYOLE1               = 0x00000020,
            ISLINKOBJECT                = 0x00000040,
            INSIDEOUT                   = 0x00000080,
            ACTIVATEWHENVISIBLE         = 0x00000100,
            RENDERINGISDEVICEINDEPENDENT= 0x00000200,
            INVISIBLEATRUNTIME          = 0x00000400,
            ALWAYSRUN                   = 0x00000800,
            ACTSLIKEBUTTON              = 0x00001000,
            ACTSLIKELABEL               = 0x00002000,
            NOUIACTIVATE                = 0x00004000,
            ALIGNABLE                   = 0x00008000,
            SIMPLEFRAME                 = 0x00010000,
            SETCLIENTSITEFIRST          = 0x00020000,
            IMEMODE                     = 0x00040000,
            IGNOREACTIVATEWHENVISIBLE   = 0x00080000,
            WANTSTOMENUMERGE            = 0x00100000,
            SUPPORTSMULTILEVELUNDO      = 0x00200000
            };

        public enum OLECLOSE : uint
            {
            SAVEIFDIRTY = 0,
            NOSAVE      = 1,
            PROMPTSAVE  = 2
            };

        public enum OLEIVERB : int
            {
            PRIMARY          =  0,
            SHOW             = -1,
            OPEN             = -2,
            HIDE             = -3,
            UIACTIVATE       = -4,
            INPLACEACTIVATE  = -5,
            DISCARDUNDOSTATE = -6,
            };

        [ComImport, Guid("00000112-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleObject
        // Not yet complete here, just enough now for what we currently need
            {
            void _SetClientSite();
            void _GetClientSite();
            void _SetHostNames();
            void Close(OLECLOSE dwSaveOption);
            void _SetMoniker();
            void _GetMoniker();
            void _InitFromData();
            void _GetClipboardData();
            void DoVerb(
                OLEIVERB iVerb,
                IntPtr lpmsg,           // zero if you don't have one
                [MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite,
                int lindex,             // reserved, must be zero
                IntPtr hwnd,
                IntPtr lprcPosRect
                );
            void _EnumVerbs();
            void Update();
            [PreserveSig] int IsUpToDate();
            void _GetUserClassID();
            void _GetUserType();
            void _SetExtent();
            void _GetExtent();
            void _Advise();
            void _Unadvise();
            void _EnumAdvise();
            void _GetMiscStatus();
            void _SetColorScheme();
            }

        [ComImport, Guid("00000118-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleClientSite
            {
            void SaveObject();
            void GetMoniker(uint dwAssign, uint dwWhichMoniker, [Out,MarshalAs(UnmanagedType.Interface)] out COMTypes.IMoniker ppmk);
            void GetContainer([Out,MarshalAs(UnmanagedType.Interface)] out IOleContainer ppContainer);
            void ShowObject();
            void OnShowWindow([MarshalAs(UnmanagedType.I4)] bool fShow);
            void RequestNewObjectLayout();
            }

        [ComImport, Guid("0000011a-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IParseDisplayName
            {
            void ParseDisplayName(
                [MarshalAs(UnmanagedType.Interface)] COMTypes.IBindCtx pbc,
                [MarshalAs(UnmanagedType.LPWStr)] string sDisplayName,
                IntPtr pcchEaten,
                [Out,MarshalAs(UnmanagedType.Interface)] out COMTypes.IMoniker ppmk);
            }

        [ComImport, Guid("0000011b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer // : IParseDisplayName
            {
            void ParseDisplayName(
                [MarshalAs(UnmanagedType.Interface)] COMTypes.IBindCtx pbc,
                [MarshalAs(UnmanagedType.LPWStr)] string sDisplayName,
                IntPtr pcchEaten,
                [Out,MarshalAs(UnmanagedType.Interface)] out COMTypes.IMoniker ppmk);

            void EnumObjects(uint grfFlags, [Out,MarshalAs(UnmanagedType.Interface)] out object ppenum); // actually IEnumUnknown, not object
            void LockContainer([MarshalAs(UnmanagedType.I4)] bool fLock);
            }

        //-----------------------------------------------------------------

        public class OleClientSite : IOleClientSite
            {
            #region IOleClientSite Members

            void IOleClientSite.SaveObject()
                {
                throw new NotImplementedException();
                }

            void IOleClientSite.GetMoniker(uint dwAssign, uint dwWhichMoniker, out COMTypes.IMoniker ppmk)
                {
                throw new NotImplementedException();
                }

            void IOleClientSite.GetContainer(out IOleContainer ppContainer)
                {
                throw new NotImplementedException();
                }

            void IOleClientSite.ShowObject()
                {
                throw new NotImplementedException();
                }

            void IOleClientSite.OnShowWindow(bool fShow)
                {
                throw new NotImplementedException();
                }

            void IOleClientSite.RequestNewObjectLayout()
                {
                throw new NotImplementedException();
                }

            #endregion
            }

        //-----------------------------------------------------------------

        public const int STG_E_INVALIDFUNCTION = unchecked((int)0x80030001);
        public const int STG_E_FILENOTFOUND    = unchecked((int)0x80030002);
        public const int STG_E_PATHNOTFOUND    = unchecked((int)0x80030003);
        // more to come...

        //-----------------------------------------------------------------

        public enum CLSCTX : uint
            {
            INPROC_SERVER = 0x1,
            INPROC_HANDLER = 0x2,
            LOCAL_SERVER = 0x4,
            INPROC_SERVER16 = 0x8,
            REMOTE_SERVER = 0x10,
            INPROC_HANDLER16 = 0x20,
            RESERVED1 = 0x40,
            RESERVED2 = 0x80,
            RESERVED3 = 0x100,
            RESERVED4 = 0x200,
            NO_CODE_DOWNLOAD = 0x400,
            RESERVED5 = 0x800,
            NO_CUSTOM_MARSHAL = 0x1000,
            ENABLE_CODE_DOWNLOAD = 0x2000,
            NO_FAILURE_LOG = 0x4000,
            DISABLE_AAA = 0x8000,
            ENABLE_AAA = 0x10000,
            FROM_DEFAULT_CONTEXT = 0x20000,
            ACTIVATE_32_BIT_SERVER = 0x40000,
            ACTIVATE_64_BIT_SERVER = 0x80000,
            ENABLE_CLOAKING = 0x100000,
            PS_DLL = 0x80000000,
            }

        //-----------------------------------------------------------------

        // Load a moniker from its serialized representation
        public static COMTypes.IMoniker LoadMoniker(byte[] rgb)
            {
            // The moniker's clsid are the first bytes of the data
            //
            byte[] rgbClsid   = new byte[16];
            byte[] rgbMoniker = new byte[rgb.Length - 16];
            Array.ConstrainedCopy(rgb, 0,  rgbClsid,   0, 16);
            Array.ConstrainedCopy(rgb, 16, rgbMoniker, 0, rgb.Length - 16);
            Guid clsid = new Guid(rgbClsid);
            //
            // Instantiate an instance of that moniker class
            //
            object punk;
            int hr = COM.CoCreateInstance(ref clsid, null, COM.CLSCTX.INPROC_SERVER, ref COM.IID_IUnknown, out punk);
            if (0==hr)
                {
                COMTypes.IMoniker pmk = (COMTypes.IMoniker)punk;
                //
                // Load the moniker from the data
                //
                System.IO.MemoryStream memstm = new System.IO.MemoryStream(rgbMoniker);
                COMTypes.IStream istm = new COMStreamOnSystemIOStream(memstm);
                pmk.Load(istm);
                return pmk;
                }
            else
                {
                Marshal.ThrowExceptionForHR(hr);
                return null;
                }
            }

        public static byte[] GetData(IDataObject oData, string cfFormat)
        // Get this clipboard format out of this data object
            {
            // First, we try using the built-in functionality. Unfortunately, in the TYMED_ISTREAM case
            // they forget to seek the stream to zero, and so aren't successful. We'll take care of that
            // in a moment.
            //
            System.IO.Stream stm = (System.IO.Stream)oData.GetData(cfFormat, false);
            if (null != stm)
                {
                stm.Seek(0, System.IO.SeekOrigin.Begin);
                int    cb  = (int)stm.Length;
                byte[] rgb = new byte[cb];
                int cbRead = stm.Read(rgb, 0, cb);
                if (cbRead == cb)
                    {
                    // The bug is that the data returned is entirely zero. Let's check.
                    //
                    for (int ib=0; ib < cb; ib++)
                        {
                        if (rgb[ib] != 0)
                            return rgb;
                        }
                    }
                }
            //
            // Ok, try to see if we can get it on a stream ourselves
            //
            COMTypes.IDataObject ido       = (COMTypes.IDataObject)oData;
            COMTypes.FORMATETC   formatEtc = new COMTypes.FORMATETC();
            COMTypes.STGMEDIUM   medium    = new COMTypes.STGMEDIUM();
            formatEtc.cfFormat = (short)DataFormats.GetFormat(cfFormat).Id;
            formatEtc.dwAspect = COMTypes.DVASPECT.DVASPECT_CONTENT;
            formatEtc.lindex   = -1;                            // REVIEW: is 0 better?
            formatEtc.tymed    = COMTypes.TYMED.TYMED_ISTREAM;
            //
            ido.GetData(ref formatEtc, out medium);
            //
            if (medium.unionmember != IntPtr.Zero)
                {
                // Get at the IStream and release the ref in the medium
                COMTypes.IStream istm = (COMTypes.IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                Marshal.Release(medium.unionmember);

                // How big is the stream?
                COMTypes.STATSTG statstg = new COMTypes.STATSTG();
                istm.Stat(out statstg, 1);
                int cb = (int)statstg.cbSize;
                byte[] rgb = new byte[cb];

                // Seek the stream to the beginning and read the data
                IntPtr pcbRead = Marshal.AllocCoTaskMem(sizeof(int));
                istm.Seek(0, 0, IntPtr.Zero);
                istm.Read(rgb, cb, pcbRead);
                int cbRead = Marshal.ReadInt32(pcbRead);
                Marshal.FreeCoTaskMem(pcbRead);

                if (cb==cbRead)
                    return rgb;
                }
            //
            // Can't get the data
            //
            return null;
            }

        }

    // Implementation of COM's IStream on a .NET stream
    // Incomplete, just enough for what we currently need.
    public class COMStreamOnSystemIOStream : COMTypes.IStream
        {
        //----------------------------------
        // State
        //----------------------------------

        System.IO.Stream stm;

        //----------------------------------
        // Construction
        //----------------------------------

        public COMStreamOnSystemIOStream(System.IO.Stream stm)
            {
            this.stm = stm;
            }

        //----------------------------------
        // IStream
        //----------------------------------

        public void Clone(out COMTypes.IStream ppstm)
            {
            throw new NotImplementedException();
            }

        public void Commit(int grfCommitFlags)
            {
            // Nothing to do
            }

        public void CopyTo(COMTypes.IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
            {
            throw new NotImplementedException();
            }

        public void LockRegion(long libOffset, long cb, int dwLockType)
            {
            Marshal.ThrowExceptionForHR(COM.STG_E_INVALIDFUNCTION);
            }

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
            {
            int cbRead = this.stm.Read(pv, 0, cb);
            Marshal.WriteInt32(pcbRead, cbRead);
            }

        public void Revert()
            {
            // Nothing to do
            }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
            {
            long ibNew = this.stm.Seek(dlibMove, (System.IO.SeekOrigin)dwOrigin);
            Marshal.WriteInt64(plibNewPosition, ibNew);
            }

        public void SetSize(long libNewSize)
            {
            this.stm.SetLength(libNewSize);
            }

        public void Stat(out COMTypes.STATSTG pstatstg, int grfStatFlag)
            {
            pstatstg = new COMTypes.STATSTG();
            pstatstg.cbSize = this.stm.Length;
            }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
            {
            throw new NotImplementedException();
            }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
            {
            throw new NotImplementedException();
            }
        }
    }
