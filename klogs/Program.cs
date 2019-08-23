/**
 * This file is part of klogs <https://github.com/StevenJDH/klogs>.
 * Copyright (C) 2019 Steven Jenkins De Haro.
 *
 * klogs is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * klogs is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with klogs.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using klogs.Classes;

namespace klogs
{
    class Program
    {
        private static readonly Kubernetes _k8s = new Kubernetes(new Shell());

        static void Main(string[] args)
        {
            Console.WriteLine(GetLogo());

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                $"klogs_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}");
            var kObjects = _k8s.GetObjectList();

            if (kObjects.Length == 0)
            {
                Console.WriteLine("No objects were detected.\n");
                return;
            }

            Directory.CreateDirectory(path); // Builds any missing folders in path.

            for (int i = 0; i < kObjects.Length; i++)
            {
                var filename = $"[{kObjects[i].Replace("/", "]-")}";

                // Group header.
                Console.WriteLine($"{(i + 1):D2} of {kObjects.Length:D2} [{kObjects[i]}]");

                Console.Write("\tGetting logs from current instance...");
                DumpLog(_k8s.GetObjectLogs(kObjects[i]), path, $"{filename}.log");

                Console.Write("\tGetting logs from previous instances...");
                DumpLog(_k8s.GetObjectLogsPrevious(kObjects[i]), path, $"{filename}.previous.log");

                Console.Write("\tGetting description...");
                DumpLog(_k8s.DescribeObject(kObjects[i]), path, $"{filename}.describe.log");

                // Group separator.
                Console.WriteLine();
            }

            bool logsDumped = !IsDirectoryEmpty(path);

            if (args.Length > 0 && args[0].Equals("-a", StringComparison.OrdinalIgnoreCase) && logsDumped)
            {
                ZipDirectory(path, $"{path}.zip");
                Directory.Delete(path, recursive: true);
                Console.WriteLine($"Logs successfully saved to: {path}.zip\n");
            }
            else if (logsDumped)
            {
                Console.WriteLine($"Logs successfully saved to: {path}\n");
            }
            else
            {
                Directory.Delete(path, recursive: true);
                Console.WriteLine("No logs were dumped.\n");
            }
        }

        private static string GetLogo()
        {
            return @"
__   __ _                     
| | / /| |                    
| |/ / | |     ___   __ _ ___ 
|    \ | |    / _ \ / _` / __|
| |\  \| |___| (_) | (_| \__ \
\_| \_/\_____/\___/ \__, |___/ 
Steven Jenkins De Haro_/ |v1.0
                    |___/     
                    ".TrimStart();
        }

        private static void DumpLog(string logData, string path, string filename)
        {
            // Skip log dump if there are no entries.
            if (String.IsNullOrWhiteSpace(logData) == false)
            {
                File.WriteAllText(Path.Combine(path, filename), logData);
                Console.WriteLine("Saved.");

                return;
            }

            Console.WriteLine("Empty.");
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private static void ZipDirectory(string dirPath, string zipPath)
        {
            ZipFile.CreateFromDirectory(dirPath, zipPath, 
                compressionLevel: CompressionLevel.Fastest, includeBaseDirectory: false);
        }
    }
}
