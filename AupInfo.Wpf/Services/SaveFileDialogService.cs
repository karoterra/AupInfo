using Microsoft.Win32;

namespace AupInfo.Wpf.Services
{
    public class SaveFileDialogService
    {
        private readonly SaveFileDialog dialog = new();

        public bool? ShowDialog(SaveFileDialogSetting setting)
        {
            dialog.FileName = setting.FileName;
            dialog.InitialDirectory = setting.InitialDirectory;
            dialog.Filter = setting.Filter;
            dialog.Title = setting.Title;
            var result = dialog.ShowDialog();
            setting.FileName = dialog.FileName;
            return result;
        }
    }
}
