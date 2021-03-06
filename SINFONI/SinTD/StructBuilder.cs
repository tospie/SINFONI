﻿// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SINFONI
{
    public class StructBuilder
    {
        public StructBuilder()
        {
            assemblyName = new AssemblyName("SINFONIDynamicAssembly-" + assemblyId);
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly
                (assemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("SINFONIDynamicType");
        }

        public TypeBuilder CreateTypeBuilder(string typeName)
        {
            return moduleBuilder.DefineType(typeName,
                TypeAttributes.Public
                | TypeAttributes.Class
                | TypeAttributes.AutoClass );
        }

        private string assemblyId = Guid.NewGuid().ToString();
        private AssemblyName assemblyName;
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
    }
}
