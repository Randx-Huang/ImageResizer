using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    partial class ImageProcess
    {

        #region Parallel
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImagesParallel(string sourcePath, string destPath, double scale)
        {
            var allFiles = FindImagesParallel(sourcePath);
            Parallel.ForEach(allFiles, filePath =>
            {
                var imgPhoto = Image.FromFile(filePath);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destionatonWidth = (int)(sourceWidth * scale);
                var destionatonHeight = (int)(sourceHeight * scale);

                var processedImage = processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                var destFile = Path.Combine(destPath, imgName + ".jpg");
                processedImage.Save(destFile, ImageFormat.Jpeg);

            });
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImagesParallel(string srcPath)
        {
            var fileExtenstions = new string[] { "*.png", "*.jpg", "*.jpeg" };
            var files = new ConcurrentQueue<string>();
            Parallel.ForEach(fileExtenstions, fe =>
            {
                foreach (var file in Directory.GetFiles(srcPath, fe, SearchOption.AllDirectories))
                {
                    files.Enqueue(file);
                }
            });

            return files.ToList();
        }

        #endregion

       


    }
}
