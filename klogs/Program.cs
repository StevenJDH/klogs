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

            string[] kObjects;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                $"klogs_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}");

            try
            {
                kObjects = _k8s.GetObjectList();

                if (kObjects.Length == 0)
                {
                    Console.WriteLine("No objects were detected.\n");
                    PauseConsoleForExit();
                    return;
                }

                Directory.CreateDirectory(path); // Builds any missing folders in path.
            }
            catch (Exception ex) when (ex is StandardErrorException || ex is IOException)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
                PauseConsoleForExit();
                return;
            }
            
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

                Console.Write("\tGetting YAML document...");
                DumpLog(_k8s.GetObjectYAML(kObjects[i]), path, $"{filename}.yaml");

                // Group separator.
                Console.WriteLine();
            }

            bool logsDumped = !IsDirectoryEmpty(path);

            if (args.Length > 0 && args[0].Equals("-a", StringComparison.OrdinalIgnoreCase) && logsDumped)
            {
                bool result = ZipDirectory(path, $"{path}.zip", deleteDirectory: true);
                Console.WriteLine($"Logs successfully saved to: {(result ? $"{path}.zip" : $"{path}")}\n");
            }
            else if (logsDumped)
            {
                Console.WriteLine($"Logs successfully saved to: {path}\n");
            }
            else
            {
                RemoveLogsFolder(path);
                Console.WriteLine("No logs were dumped.\n");
            }

            PauseConsoleForExit();
        }

        /// <summary>
        /// Get a logo that is generated with author and version information.
        /// </summary>
        /// <returns>Text-based application logo.</returns>
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

        /// <summary>
        /// Dumps the logs collected from the output of a command to file.
        /// </summary>
        /// <param name="logData">Output from an executed command containing log info.</param>
        /// <param name="path">Path to the logs folder.</param>
        /// <param name="filename">Filename to use for the dumped log.</param>
        private static void DumpLog(string logData, string path, string filename)
        {
            if (String.IsNullOrWhiteSpace(logData))
            {
                Console.WriteLine("Empty.");
                return;
            }

            try
            {
                File.WriteAllText(Path.Combine(path, filename), logData);
                Console.WriteLine("Saved.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Check to see if a directory contains any files or not.
        /// </summary>
        /// <param name="path">Path to the directory being check.</param>
        /// <returns>True if empty, false if not.</returns>
        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// Archives the logs folder as a zip file and removes and removes the folder.
        /// </summary>
        /// <param name="dirPath">Path to the logs folder.</param>
        /// <param name="zipPath">Path to where to save the zip archive.</param>
        /// <param name="deleteDirectory">True to deleted logs folder after archiving, or false to not.</param>
        /// <returns>True if successful, or false if not. Logs folder is only deleted if successful.</returns>
        private static bool ZipDirectory(string dirPath, string zipPath, bool deleteDirectory = true)
        {
            try
            {
                ZipFile.CreateFromDirectory(dirPath, zipPath,
                    compressionLevel: CompressionLevel.Fastest, includeBaseDirectory: false);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            if (deleteDirectory)
            {
                RemoveLogsFolder(dirPath);
            }

            return true;
        }

        /// <summary>
        /// Removes the logs folder that was created.
        /// </summary>
        /// <param name="dirPath">Path to the logs folder.</param>
        private static void RemoveLogsFolder(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, recursive: true);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Pauses the console execution before exiting until a key is pressed.
        /// </summary>
        private static void PauseConsoleForExit()
        {
            Console.Write("Press any key to exit . . .");
            Console.ReadKey(intercept: true); //Pause before closing workaround.
            Console.WriteLine("\n");
        }
    }
}
