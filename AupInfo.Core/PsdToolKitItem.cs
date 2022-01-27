using System.Drawing;
using System.Text.Json.Serialization;

namespace AupInfo.Core
{
    public class PsdToolKitItem : IDisposable
    {
        [JsonConverter(typeof(PsdToolKitImageConverter))]
        public string? Image { get; set; }

        public int? Tag { get; set; }

        [JsonConverter(typeof(PsdToolKitThumbnailConverter))]
        public Bitmap? Thumbnail { get; set; }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Thumbnail?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
