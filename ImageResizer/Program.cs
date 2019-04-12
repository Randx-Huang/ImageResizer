using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            var destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;

            var imageProcess = new ImageProcess();
            imageProcess.Clean(destinationPath);

            var sw = new Stopwatch();
            decimal msSync = 0, msParalle = 0, msAsync = 0;
            List<decimal> everySavePercentage = new List<decimal>();

            for (var i = 0; i < 3; i++)
            {
                sw.Restart();
                imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
                sw.Stop();
                msSync = sw.ElapsedMilliseconds;
                Console.WriteLine($"第{i + 1}次 同步花費時間: {msSync} ms");

                sw.Restart();
                imageProcess.ResizeImagesParallel(sourcePath, destinationPath, 2.0);
                sw.Stop();
                msParalle = sw.ElapsedMilliseconds;
                Console.WriteLine($"第{i + 1}次 平行處理花費時間: {msParalle} ms, 節省%: {GetSavePercentage(msSync, msParalle):P}");

                sw.Restart();
                await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
                sw.Stop();
                msAsync = sw.ElapsedMilliseconds;
                everySavePercentage.Add(GetSavePercentage(msSync, msAsync));
                Console.WriteLine($"第{i + 1}次 非同步處理花費時間: {msAsync} ms, 節省%: {everySavePercentage[i]:P}");
            }
            Console.WriteLine($"平均節省時間:{everySavePercentage.Average():P}");
            Console.ReadLine();
        }

        static decimal GetSavePercentage(decimal msSync, decimal msAsync)
        {
            return Math.Abs((msAsync / msSync - 1));
        }
    }
}
