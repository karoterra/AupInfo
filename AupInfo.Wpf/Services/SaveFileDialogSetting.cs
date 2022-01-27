namespace AupInfo.Wpf.Services
{
    public class SaveFileDialogSetting
    {
        public string FileName { get; set; } = string.Empty;
        public string InitialDirectory { get; set; } = string.Empty;
        public string Filter { get; set; } = "すべてのファイル(*.*)|*.*";
        public string Title { get; set; } = "名前を付けて保存";
    }
}
