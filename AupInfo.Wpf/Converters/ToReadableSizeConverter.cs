using System.Globalization;
using System.Windows.Data;

namespace AupInfo.Wpf.Converters
{
    public class ToReadableSizeConverter : IValueConverter
    {
        private static readonly string[] units = { "B", "KB", "MB", "GB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = value switch
            {
                int n => n,
                uint u => u,
                long l => l,
                ulong ul => ul,
                float f => f,
                double lf => lf,
                _ => throw new ArgumentException($"typeof {nameof(value)} is invalid"),
            };

            int scale = 0;
            for (int i = 0; i < units.Length; i++)
            {
                if (size < 1024)
                {
                    scale = i;
                    break;
                }
                size /= 1024;
            }
            return $"{size:#.##} {units[scale]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
