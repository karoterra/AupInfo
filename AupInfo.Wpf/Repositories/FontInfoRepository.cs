using System.Drawing.Text;
using System.Text.RegularExpressions;
using AupInfo.Core;
using Karoterra.AupDotNet.ExEdit;
using Karoterra.AupDotNet.ExEdit.Effects;

namespace AupInfo.Wpf.Repositories
{
    public class FontInfoRepository
    {
        private static readonly HashSet<string> installedFonts = new();
        private static readonly Regex regex = new Regex(@"<s\d*,([^,>]+)(,[BI]*)?>");

        public FontInfo GetFontInfo(string name)
        {
            return new FontInfo(name, installedFonts.Contains(name));
        }

        public List<FontInfo> GetAllFontInfo(IEnumerable<TimelineObject> timelineObjects)
        {
            var fonts = new HashSet<string>();
            foreach (var obj in timelineObjects)
            {
                if (obj.Chain) continue;
                if (obj.Effects[0] is TextEffect t)
                {
                    if (!string.IsNullOrEmpty(t.Font))
                        fonts.Add(t.Font);
                    foreach (Match m in regex.Matches(t.Text))
                    {
                        fonts.Add(m.Groups[1].Value);
                    }
                }
            }
            return fonts.Select(f => GetFontInfo(f)).ToList();
        }

        static FontInfoRepository()
        {
            InstalledFontCollection ifc = new();
            foreach (var ff in ifc.Families)
            {
                installedFonts.Add(ff.Name);
            }
        }
    }
}
