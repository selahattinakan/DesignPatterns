
using System.Drawing;

namespace WebApp.Adapter.Services
{
    public class AdvanceImageProcessAdapter : IImageProcess
    {
        private readonly IAdvanceImageProcess _advanceImageProcess;

        public AdvanceImageProcessAdapter(IAdvanceImageProcess advanceImageProcess)
        {
            _advanceImageProcess = advanceImageProcess;
        }

        public void AddWatermark(string text, string fileName, Stream imageStream)
        {
            string path = $"wwwroot/watermarks/{fileName}";
            _advanceImageProcess.AddWatermarkImage(imageStream, text, path, Color.Black, Color.AliceBlue);
        }
    }
}
