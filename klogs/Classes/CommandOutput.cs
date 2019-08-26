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
using System.Text;

namespace klogs.Classes
{
    /// <summary>
    /// Contains the output and status information for an executed command.
    /// </summary>
    public class CommandOutput
    {
        /// <summary>
        /// Contains the output from the standard error output device.
        /// </summary>
        public string StdOut { get; set; }

        /// <summary>
        /// Contains the output from the standard error output device. Check <see cref="ExitCode"/> to see
        /// if the output stream has been redirected here.
        /// </summary>
        public string StdErr { get; set; }

        /// <summary>
        /// Contains the status of the executed command with 0 meaning successful and 1 or more meaning it failed.
        /// </summary>
        public int ExitCode { get; set; }
    }

}
