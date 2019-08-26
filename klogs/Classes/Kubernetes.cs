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
    /// <summary>
    /// Provides access to the kubectl command line for the Kubernetes cluster.
    /// </summary>
    public sealed class Kubernetes
    {
        private readonly ICommandable _shell;

        /// <summary>
        /// Initializes a new instance of <see cref="Kubernetes"/>.
        /// </summary>
        /// <param name="shell">Decorate instance with a class with shell access.</param>
        public Kubernetes(ICommandable shell)
        {
            _shell = shell;
        }

        /// <summary>
        /// Gets a list of all the Pods present in the Kubernetes cluster..
        /// </summary>
        /// <returns>Command output containing the list of Pods present.</returns>
        public string[] GetPodList()
        {
            // Use "kubectl get pods --no-headers -o custom-columns=\":metadata.name\"" for pod names without type.
            return ProcessOutput(_shell.Run("kubectl get pods --no-headers -o name"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets a list of all the objects present in the Kubernetes cluster.
        /// </summary>
        /// <returns>Command output containing the list of all the objects present.</returns>
        public string[] GetObjectList()
        {
            return ProcessOutput(_shell.Run("kubectl get all --no-headers -o name"))
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the YAML document of the object defined by <paramref name="fullObjectName"/>.
        /// </summary>
        /// <param name="fullObjectName">Type and name of object.</param>
        /// <returns>Standard output from command containing YAML document of object.</returns>
        public string GetObjectYAML(string fullObjectName)
        {
            return _shell.Run($"kubectl get {fullObjectName} -o yaml").StdOut;
        }

        /// <summary>
        /// Gets the description of the object defined by <paramref name="fullObjectName"/>.
        /// </summary>
        /// <param name="fullObjectName">Type and name of object.</param>
        /// <returns>Standard output from command containing the object description.</returns>
        public string DescribeObject(string fullObjectName)
        {
            return _shell.Run($"kubectl describe {fullObjectName}").StdOut;
        }

        /// <summary>
        /// Gets the logs from the current instance of the object defined by <paramref name="fullObjectName"/>
        /// if present.
        /// </summary>
        /// <param name="fullObjectName">Type and name of object.</param>
        /// <returns>Standard output from command containing log information if present.</returns>
        public string GetObjectLogs(string fullObjectName)
        {
            return _shell.Run($"kubectl logs {fullObjectName} --all-containers").StdOut;
        }

        /// <summary>
        /// Gets the logs from the previous instances of the object defined by <paramref name="fullObjectName"/>
        /// if present.
        /// </summary>
        /// <param name="fullObjectName">Type and name of object.</param>
        /// <returns>Standard output from command containing previous log information if present.</returns>
        public string GetObjectLogsPrevious(string fullObjectName)
        {
            return _shell.Run($"kubectl logs -p {fullObjectName} --all-containers").StdOut;
        }

        /// <summary>
        /// Processes the command output to check if the stream was redirected to the stream to the
        /// standard error output device, and if so, throws it as an exception.
        /// </summary>
        /// <param name="cmdOutput">Output from an executed command.</param>
        /// <returns>Text from the standard output device.</returns>
        /// <exception cref="StandardErrorException">Text from the standard error output device.</exception>
        private static string ProcessOutput(CommandOutput cmdOutput)
        {
            if (cmdOutput.ExitCode > 0)
            {
                throw new StandardErrorException(cmdOutput.StdErr);
            }

            return cmdOutput.StdOut;
        }
    }
}
