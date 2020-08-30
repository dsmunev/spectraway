using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SpectraWay.Device.Spectrometer.Lotis
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential)]
    public struct TCCDUSBParams
    {
        public Int32 dwDigitCapacity;  //The digit capacity of CCD-camera
        public int nPixelRate;//The pixel rate kHz
        public int nNumPixels;//The number of pixels

        public int nNumReadOuts;//The number of readouts
        public int nExposureTime;//The exposure time
        public Int32 dwSynchr;  //The synchronization mode
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class OrminsCcdDriverWrapper
    {
        struct RECT
        {
            long left;
            long top;
            long right;
            long bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct TCCDUSBExtendParams
        {
            Int32 dwDigitCapacity;  //The digit capacity of CCD-camera
            int nPixelRate;//The pixel rate kHz
            int nNumPixelsH;// The number of pixels on a horizontal (columns number of CCD-array)
            int nNumPixelsV;// The number of pixels on a vertical (rows number of CCD-array)
            Int32 Reserve1; // not used
            Int32 Reserve2; // not used

            int nNumReadOuts; // The number of readouts
            Single sPreBurning; // The Time preliminary burning in seconds.
                                // Really only at synchronization SYNCHR_CONTR_NEG but not for all cameras!!!
                                // Use GetDeviceProperty function to receive properties of the device.
                                // Is used at a spectrum measurements.
            Single sExposureTime; // The exposure time
            Single sTime2; // not used
            Int32 dwSynchr;  // The synchronization mode.
            Boolean bSummingMode; // Turn on(off) summing mode. Not used.

            Int32 dwDeviceMode; // Turn on(off) spectral mode of CCD-array.
                                // Some cameras on the basis of CCD-matrixes have an additional modes. See dwProperty.
                                // DP_MODEA2 It is an additional mode of the matrix registrar. If the device has DP_MODEA2 property it is possible to establish dwDeviceMode in value DEVICEMODEA2.
                                // In mode DEVICEMODES the device works in a spectroscopic mode.
                                // The photosensitive field of a matrix is broken into some strips. Strips are set by parameters nStripCount and rcStrips.
                                // While translating the device in mode DEVICEMODES change nNumPixelsH and nNumPixelsV.
            int nStripCount; // The number of strips for a spectral mode

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 8)]
            RECT[] rcStrips; // The strips for a spectral mode.
            int Reserve11;

            Int32 dwSensitivity; // Turn on (off) a mode of the raised sensitivity of a CCD-sensor control. Actually if dwProperty & DP_SENSIT <> 0.
            Int32 dwProperty; // The device property.
            Single sShuterTime; // Shuter time (ms). Active in minimal exposure time.
                                // Exposure time = MinExp - sShaterTime.
            Int32 Reserve6; // not used
            Int32 Reserve7; // not used
            Int32 Reserve8; // not used
            Int32 Reserve9; // not used
            Int32 Reserve10; // not used
        }



        //The parameter identification number;=

        //The digit capacity of CCD-camera;
        private const int PRM_DIGIT = 1;
        //The pixel rate;
        private const int PRM_PIXELRATE = 2;
        //The number of pixels;
        private const int PRM_NUMPIXELS = 3;
        //The number of readouts;
        private const int PRM_READOUTS = 4;
        //The exposure time;
        private const int PRM_EXPTIME = 5;
        //The synchronization mode;
        private const int PRM_SYNCHR = 6;
        //The number of pixels on a horizontal (columns number of CCD-array);
        private const int PRM_NUMPIXELSH = 7;
        //The number of pixels on a vertical (rows number of CCD-array);
        private const int PRM_NUMPIXELSV = 8;
        //The summing mode;
        private const int PRM_SUMMING = 9;
        //The device mode;
        private const int PRM_DEVICEMODE = 10;
        // DEVICEMODEA1 - Matrix mode #1;
        // DEVICEMODEA2 - Matrix mode #2;
        // DEVICEMODES  - The spectroscope mode without a strips.;
        //                The matrix is submitted as one line;
        //The number of strips for a spectral mode;
        private const int PRM_STRIPCOUNT = 11;
        //The sensitivity;
        private const int PRM_SENSIT = 14;
        //The device property.;
        private const int PRM_DEVICEPROPERTY = 15;
        // The Time preliminary burning in seconds.;
        private const int PRM_PREBURNING = 16;
        // Really only at synchronization SYNCHR_CONTR_NEG but not for all cameras!!!;
        // Use GetDeviceProperty function to receive properties of the device.;
        // Is used  at a spectrum measurements.;
        private const int PRM_SHUTERTIME = 17;//

        // The synchronization mode;
        // Without synchronization.;
        private const int SYNCHR_NONE = 0x01;
        // In the beginning of the first accumulation the positive;
        // pulse of synchronization is formed.;
        private const int SYNCHR_CONTR = 0x20;
        // Clock pulse is formed in the beginning of each accumulation.;
        private const int SYNCHR_CONTR_FRS = 0x04;
        // One pulse of synchronization is formed on all time of registration.;
        // A pulse of negative polarity.;
        private const int SYNCHR_CONTR_NEG = 0x08;

        // The beginning of the first accumulation is adhered to growing;
        // front of external clock pulse.;
        //All other accumulation occur so quickly as it is possible.;
        // In a limit -- without the misses.;
        private const int SYNCHR_EXT = 0x10;

        // The beginning of each accumulation is adhered to growing front of clock pulse.;
        // How much accumulation, so much clock pulses are expected.;
        private const int SYNCHR_EXT_FRS = 0x02;

        //The status of measurement;
        //the measurement in processing;
        private const int STATUS_WAIT_DATA = 1;
        //the waiting of synchronization pulse;
        private const int STATUS_WAIT_TRIG = 2;
        //the measurement has been finished;
        private const int STATUS_DATA_READY = 3;

        private const int MAXSTRIPS = 8;

        // DEVICE MODE;
        // Some cameras on the basis of CCD-matrixes have an additional modes. See dwProperty.;
        // DP_MODEA2 It is an additional mode of the matrix registrar. If the device has DP_MODEA2 property it is possible to establish dwDeviceMode in value DEVICEMODEA2.;
        // In mode DEVICEMODES the device works in a spectroscopic mode.;
        // The photosensitive field of a matrix is broken into some strips. Strips are set by parameters nStripCount and rcStrips.;
        // While translating the device in mode DEVICEMODES change nNumPixelsH and nNumPixelsV.;
        private const int DEVICEMODEA1 = 0x0002;
        private const int DEVICEMODEA2 = 0x0000;
        private const int DEVICEMODES = 0x0003;

        // DEVICEPROPERTY

        // SYNCHR_CONTR is enaible;
        private const int DP_SYNCHR_CONTR = 0x00000001;
        // SYNCHR_CONTR_FRS is enaible;
        private const int DP_SYNCHR_CONTR_FRS = 0x00000002;
        // SYNCHR_CONTR_NEG is enaible;
        private const int DP_SYNCHR_CONTR_NEG = 0x00000004;
        // SYNCHR_EXT is enaible;
        private const int DP_SYNCHR_EXT = 0x00000008;
        // SYNCHR_EXT_FRS is enaible;
        private const int DP_SYNCHR_EXT_FRS = 0x00000010;
        // The sensor has a mode of the raised sensitivity.;
        private const int DP_SENSIT = 0x00000020;
        // Additional matrix mode of the camera.;
        private const int DP_MODEA2 = 0x00000040;
        // Spectroscopic mode of a CCD-matrix.;
        private const int DP_MODES1 = 0x00000080;
        // Spectroscopic mode of a CCD-matrix.;
        private const int DP_MODES2 = 0x00000100;
        // Opportunity to establish preliminary burning.;
        private const int DP_PREBURNING = 0x00000200;
        // Property of an electronic shutter.;
        private const int DP_SHUTER = 0x00000400;
        // Control ADC clock frequency (nPixelRate).;
        private const int DP_CLOCKCONTROL = 0x00000800;

        private const int NCAMMAX = 3;


        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_Init", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_Init(IntPtr ahAppWnd, IntPtr prm, ref Int32 ID);

        public static bool Init(ref int id)
        {
            //var id = 0;
            return CCD_Init(IntPtr.Zero, IntPtr.Zero, ref id);
        }

        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_GetParameters", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_GetParameters(Int32 ID, ref TCCDUSBParams Prms);

        public static TCCDUSBParams GetParameters(int id = 0)
        {
            var @params = new TCCDUSBParams();
            CCD_GetParameters(id, ref @params);
            return @params;
        }

        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_HitTest", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_HitTest(Int32 ID);

        public static bool HitTest(int id = 0)
        {
            return CCD_HitTest(id);
        }

        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_CameraReset", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_CameraReset(Int32 ID);

        public static bool CameraReset(int id = 0)
        {
            return CCD_CameraReset(id);
        }

        //CCD_SetParameter
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_SetParameter", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_SetParameter(Int32 ID, UInt32 dwPrmID, Single Prm);

        public static bool SetExposition(float expos, int id = 0)
        {
            return CCD_SetParameter(id, PRM_EXPTIME, expos);
        }

        //public static void SetParameter(int id = 0)
        //{
        //    CCD_SetParameter(id);
        //}


        //CCD_GetParameter
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_GetParameter", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_GetParameter(Int32 ID, UInt32 dwPrmID, out Single Prm);

        public static float GetExposition(int id = 0)
        {
            float expos;
            if (!CCD_GetParameter(id, PRM_EXPTIME, out expos)) expos = -1;
            return expos;
        }


        //CCD_InitMeasuring
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_InitMeasuring", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_InitMeasuring(Int32 ID);

        public static bool InitMeasuring(int id = 0)
        {
            return CCD_InitMeasuring(id);
        }

        //CCD_InitMeasuringData
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_InitMeasuringData", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_InitMeasuringData(Int32 ID, Int32[] data);

        public static bool InitMeasuringData(Int32[] data, int id = 0)
        {
            return CCD_InitMeasuringData(id, data);
        }

        //CCD_StartWaitMeasuring
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_StartWaitMeasuring", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_StartWaitMeasuring(Int32 ID);

        public static bool StartWaitMeasuring(int id = 0)
        {
            return CCD_StartWaitMeasuring(id);
        }

        //CCD_StartMeasuring
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_StartMeasuring", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_StartMeasuring(Int32 ID);

        public static bool StartMeasuring(int id = 0)
        {
            return CCD_StartMeasuring(id);
        }

        //CCD_GetMeasureStatus
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_GetMeasureStatus", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_GetMeasureStatus(Int32 ID, out Int32 adwStatus);

        public static int GetMeasureStatus(int id = 0)
        {
            int status;
            if (!CCD_GetMeasureStatus(id, out status)) status = -1;
            return status;
        }


        //CCD_GetData
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_GetData", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_GetData(Int32 ID, IntPtr pData);

        public static bool GetData(Int32[] data, int id = 0)
        {
            IntPtr buffer = Marshal.AllocHGlobal(4 * data.Length);
            try
            {
                if (CCD_GetData(id, buffer))
                {
                    Marshal.Copy(buffer, data, 0, data.Length);
                    return true;
                }
                return false;

            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

        }


        //CCD_GetData
        [DllImport("CCDUSBDCOM01.dll", CharSet = CharSet.Unicode, EntryPoint = "CCD_GetSerialNum", CallingConvention = CallingConvention.StdCall)]
        private static extern bool CCD_GetSerialNum(Int32 ID, out IntPtr pData);

        public static string GetSerialNum(int id = 0)
        {
            IntPtr ptr;
            if (CCD_GetSerialNum(id, out ptr))
            {
                // assume returned string is utf-8 encoded
                String str = PtrToStringUtf8(ptr);
                // call native DLL function to free ptr
                // if no function exists, pinvoke C runtime's free()
                return str;
            }
            return null;

        }

        private static string PtrToStringUtf8(IntPtr ptr) // aPtr is nul-terminated
        {
            if (ptr == IntPtr.Zero)
                return "";
            int len = 0;
            while (System.Runtime.InteropServices.Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
                return "";
            byte[] array = new byte[len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, len);
            return System.Text.Encoding.UTF8.GetString(array);
        }
    }
}