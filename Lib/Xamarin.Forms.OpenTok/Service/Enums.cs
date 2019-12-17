using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.OpenTok.Service
{
    public class Enums
    {
        public enum CameraCaptureResolution : int
        {
            Low = 1,
            Medium = 2,
            High = 3
        }

        public enum CameraCaptureFrameRate : int
        {
            Fps1 = 1,
            Fps7 = 2,
            Fps15 = 3,
            Fps30 = 4
        }
    }
}
