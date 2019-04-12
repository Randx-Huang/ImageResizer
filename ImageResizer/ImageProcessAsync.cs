using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    partial class ImageProcess
    {
        #region Async
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            var allFiles = FindImages(sourcePath);

            var tasks = new List<Task>();
            foreach (var filePath in allFiles)
            {
                tasks.Add(Task.Run(() =>
                {
                    var imgPhoto = Image.FromFile(filePath);

                    var sourceWidth = imgPhoto.Width;
                    var sourceHeight = imgPhoto.Height;

                    var destionatonWidth = (int)(sourceWidth * scale);
                    var destionatonHeight = (int)(sourceHeight * scale);

                    var processedImage = processBitmap((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight);

                    var imgName = Path.GetFileNameWithoutExtension(filePath);
                    var destFile = Path.Combine(destPath, imgName + ".jpg");
                    processedImage.Save(destFile, ImageFormat.Jpeg);
                }));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public async Task<List<string>> FindImagesAsync(string srcPath)
        {
            var fileExtenstions = new string[] { "*.png", "*.jpg", "*.jpeg" };
            var tasks = new List<Task>();
            var files = new List<string>();
            foreach (var fileExtenstion in fileExtenstions)
            {
                tasks.Add(Task.Run(() =>
                files.AddRange(Directory.GetFiles(srcPath, fileExtenstion, SearchOption.AllDirectories))));
            }
            await Task.WhenAll(tasks);
            return files;
        }
        #endregion
    }
}
