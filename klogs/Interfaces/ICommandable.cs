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
using klogs.Classes;

namespace klogs.Interfaces
{
    /// <summary>
    /// Provides an interface that can be used to run shell commands via an implementation.
    /// </summary>
    public interface ICommandable
    {
        /// <summary>
        /// Runs a shell command on a host system.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <returns>Output from the executed command.</returns>
        CommandOutput Run(string command);
    }
}
