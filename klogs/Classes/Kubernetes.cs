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
using System.Linq;
using System.Text;
using klogs.Interfaces;

namespace klogs.Classes
{
    sealed class Kubernetes
    {
        private readonly ICommandable _shell;

        public Kubernetes(ICommandable shell)
        {
            _shell = shell;
        }

        public string[] GetPodList()
        {
            // Use "kubectl get pods --no-headers -o custom-columns=\":metadata.name\"" for pod names without type.
            return ProcessOutput(_shell.Run("kubectl get pods --no-headers -o name"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetObjectList()
        {
            return ProcessOutput(_shell.Run("kubectl get all --no-headers -o name"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string DescribeObject(string fullObjectName)
        {
            return _shell.Run($"kubectl describe {fullObjectName}").StdOut;
        }

        public string GetObjectLogs(string fullObjectName)
        {
            return _shell.Run($"kubectl logs {fullObjectName} --all-containers").StdOut;
        }

        public string GetObjectLogsPrevious(string fullObjectName)
        {
            return _shell.Run($"kubectl logs -p {fullObjectName} --all-containers").StdOut;
        }

        private static string ProcessOutput(CommandOutput output)
        {
            if (output.ExitCode > 0)
            {
                throw new StandardErrorException(output.StdErr);
            }

            return output.StdOut;
        }
    }
}
