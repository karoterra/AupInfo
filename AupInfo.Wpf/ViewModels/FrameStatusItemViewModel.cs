using Karoterra.AupDotNet;

namespace AupInfo.Wpf.ViewModels
{
    public class FrameStatusItemViewModel
    {
        public int Index { get; }
        public uint Video => frame.Video;
        public uint Audio => frame.Audio;
        public uint Field2 => frame.Field2;
        public uint Field3 => frame.Field3;
        public string Inter => $"0x{(int)frame.Inter:X2}";
        public byte Index24Fps => frame.Index24Fps;
        public string EditFlag => $"0x{(int)frame.EditFlag:X2}";
        public byte Config => frame.Config;
        public byte Vcm => frame.Vcm;
        public byte Clip => frame.Clip;

        private readonly FrameStatus frame;

        public FrameStatusItemViewModel(int index, FrameStatus frame)
        {
            Index = index;
            this.frame = frame;
        }
    }
}
