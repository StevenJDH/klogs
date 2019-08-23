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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using klogs.Interfaces;

namespace klogs.Classes
{
    public class Shell : ICommandable
    {
        public CommandOutput Run(string command)
        {
            var cmdOutput = new CommandOutput();
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var startInfo = new ProcessStartInfo()
            {
                FileName = isWindows ? "cmd.exe" : "/bin/bash",
                Arguments = isWindows ? $"/c \"{command}\"" : $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = Process.Start(startInfo))
                {
                    cmdOutput.StdOut = process?.StandardOutput.ReadToEnd().Trim() ?? "-- No Content --";
                    cmdOutput.StdErr = process?.StandardError.ReadToEnd().Trim() ?? "-- No Content --";
                    process?.WaitForExit();
                    cmdOutput.ExitCode = process?.ExitCode ?? 1;
                }

                return cmdOutput;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
