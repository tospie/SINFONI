// This file is part of SINFONI.
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
using System.Text;
using System.Text.RegularExpressions;
using SINFONI.Exceptions;

namespace SINFONI
{
    /// <summary>
    /// Implements methods to parse service definitions from a SINFONI IDL. Creates service objects that are stored in
    /// SINFONI's service registry, and parses service functions defined for the different services. All of them are
    /// stored in a representation to allow lookup by name and compare their return types and parameters via SinTD
    /// Type objects.
    /// </summary>
    internal class ServiceParser
    {
        internal static ServiceParser Instance = new ServiceParser();

        /// <summary>
        /// Reads the name of a service when IDL parser encounters a service definition and creates a service
        /// of respective name. Puts IDL parser in service parsing mode so that every following line will be
        /// treated as service function definition
        /// </summary>
        /// <param name="serviceDefinition">Line of the IDL defining a new service</param>
        internal void startServiceParsing(string serviceDefinition)
        {
            Regex nameRegEx = new Regex("service ([A-Za-z0-9_]*)[{}._<,>; ]*");
            Match nameMatch = nameRegEx.Match(serviceDefinition);
            string name = nameMatch.Groups[1].Value;
            currentlyParsedService = new SINFONIService(name);
        }

        /// <summary>
        /// Parses a single line of a service definition. At this point, each line corresponds to a service
        /// function definition and is parsed as such
        /// </summary>
        /// <param name="line">Line of the IDL defining an entry within an service, i.e. a service function</param>
        /// <param name="lineNumber">Current line in IDL</param>
        internal void parseLineOfService(string line, int lineNumber)
        {
            // Opening bracket for service may have slipped to new line. Split the line containing the
            // opening bracket and only consider the remaining line, if there is already the first content
            if (line.Contains('{'))
                line = line.Split('{')[1].Trim();
            if (line.Length == 0)
                return;

            // Closing bracket finishes parsing of the service. The bracket may be ending the line after
            // the last service function definition. This case is checked before finalization
            if (line.Contains('}'))
                finalizeServiceParsing(line, lineNumber);
            else
                parseServiceFunctionDefinition(line, lineNumber);
        }

        /// <summary>
        /// Interprets a line of the IDL as service function definition and creates one of it from the entry
        /// </summary>
        /// <param name="line">Line as read from the IDL</param>
        /// <param name="lineNumber">Line number within IDL</param>
        private void parseServiceFunctionDefinition(string line, int lineNumber)
        {
            // Check syntax of the line. If the parameter list cannot be retrieved correctly from the
            // paranthese, or the definition is not finished by a semicolon, throw exception
            if (!(line.Contains(';')
                && line.Contains('(')
                && line.Contains(')')))
                throw new IDLParseException(line, lineNumber);

            int indexOfOpenPar = line.IndexOf('(');
            int indexOfClosePar = line.IndexOf(')');

            string parameterDefinition = line.Substring(indexOfOpenPar + 1, indexOfClosePar - (indexOfOpenPar + 1));
            string nameAndType = line.Substring(0, indexOfOpenPar);

            ServiceFunctionDescription newServiceFunction = createTypedServiceFunction(nameAndType);
            parseParameters(parameterDefinition, newServiceFunction);
            currentlyParsedService.serviceFunctions.Add(newServiceFunction.Name, newServiceFunction);
        }

        /// <summary>
        /// Creates a new service function definition from an entry in the IDL. Reads type and name of the service
        /// function and creates a respective object from these
        /// </summary>
        /// <param name="nameAndType">Content of IDL defining name and type of a service function</param>
        /// <returns>ServiceFunction object with corresponding name and type</returns>
        private ServiceFunctionDescription createTypedServiceFunction(string nameAndType)
        {
            string[] values = splitDeclarationInNameAndType(nameAndType);

            SinTDType returnType;
            if (values[0].Trim() == "void")
                returnType = new SinTDType("void");
            else
                returnType = getSinTDType(values[0].Trim());

            return new ServiceFunctionDescription(values[1], returnType);
        }

        /// <summary>
        /// Parses parameters given for a service function in the IDL.
        /// </summary>
        /// <param name="parameterDefinition">String defining all parameters of a service funtion in the IDL</param>
        /// <param name="functionDescription">The ServiceFunction object for which the parameters are created</param>
        private void parseParameters(string parameterDefinition, ServiceFunctionDescription functionDescription)
        {
            if (parameterDefinition.Length == 0)
                return;

            // Both different parameters and key-and-value definitions of maps are separated by commata.
            // Replace the commata separating the parameters by another delimiter to avoid ambiguity and still
            // allow the MapParser to work correctly
            parameterDefinition = replaceMapKeyValueDelimiter(parameterDefinition);
            string[] parameters = parameterDefinition.Split(';');
            foreach (string param in parameters)
            {
                createParameterForServiceFunction(param.Trim(), functionDescription);
            }
        }

        /// <summary>
        /// Creates a SinTDTyped object and stores it under the name specified in the IDL for the corresponding parameter.
        /// The created Parameter is added to the list of parameters for the currently parsed service function object.
        /// </summary>
        /// <param name="param">String defining name and type of the parameter</param>
        /// <param name="functionDescription">Service Function object to which the parameter should be added</param>
        private void createParameterForServiceFunction(string param, ServiceFunctionDescription functionDescription)
        {
            string[] values = splitDeclarationInNameAndType(param);

            SinTDType paramType = getSinTDType(values[0].Trim());
            string paramName = values[1].Trim();
            functionDescription.Parameters.Add(paramName, paramType);
        }

        /// <summary>
        /// Splits the string that declares name and type of a parameter in a type- and a name part. This is done
        /// by splitting by space (' ') for non-complex objects. Array- or Map-Typed parameters may contain additional
        /// spaces, so we need to collapse those.
        /// </summary>
        /// <param name="declaration">String declaring type and name of a parameter</param>
        /// <returns>Parameter declaration split in type and name part</returns>
        private string[] splitDeclarationInNameAndType(string declaration)
        {
            string[] values;
            if (declaration.Contains("array<") || declaration.Contains("map<"))
            {
                declaration = Regex.Replace(declaration, @"\s+", "");
                values = new string[2];

                int lastClosingBracket = declaration.LastIndexOf('>');
                values[0] = declaration.Substring(0, lastClosingBracket + 1);
                values[1] = declaration.Substring(lastClosingBracket + 1, declaration.Length - (lastClosingBracket+1));
            }
            else
            {
                values = declaration.Split(' ');
            }

            return values;
        }

        /// <summary>
        /// Replaces the ',' that separates parameters in the service function definition to avoid ambiguity with
        /// ',' separating key and value definitions of a map-typed parameter
        /// </summary>
        /// <param name="paramDefinition">String defining parameters for a Service function in the IDL</param>
        /// <returns>Parameter definition with replaced delimiteres</returns>
        private string replaceMapKeyValueDelimiter(string paramDefinition)
        {
            bool inMapDef = false;
            int currentIndex = 0;
            StringBuilder replacedStringBuilder = new StringBuilder(paramDefinition);
            foreach (char c in paramDefinition)
            {
                if (c == '<')
                    inMapDef = true;
                if (c == '>')
                    inMapDef = false;

                if (c == ',' && !inMapDef)
                {
                    replacedStringBuilder.Remove(currentIndex, 1);
                    replacedStringBuilder.Insert(currentIndex, ';');
                }
                currentIndex++;
            }
            return replacedStringBuilder.ToString();
        }

        /// <summary>
        /// Returns a valid SinTDType-object for a type that is specified for a parameter or service function return
        /// type. Performs a SinTD-Lookup for types and creates array or map typed SinTDTypes
        /// </summary>
        /// <param name="typeDefinition">String defining the type of a Service Function or Parameter</param>
        /// <returns>The respective SinTD Type object</returns>
        private SinTDType getSinTDType(string typeDefinition)
        {
            SinTDType paramType;
            if (typeDefinition.StartsWith("array<"))
                paramType = ArrayParser.Instance.ParseArray(typeDefinition);
            else if (typeDefinition.StartsWith("map<"))
                paramType = MapParser.Instance.ParseMap(typeDefinition);
            else
                paramType = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType(typeDefinition);

            return paramType;
        }

        /// <summary>
        /// Finalizes Parsing of a service, invoked by the closing paranthese '}'. Takes into account possible
        /// remaining content in the same line and treats it as part of the service, if appearing before the
        /// bracket, or a new object definition, if appearing after the bracket.
        /// </summary>
        /// <param name="lastLine">The complete IDL line containing the closing bracket</param>
        /// <param name="lineNumber">Number of this line in the IDL</param>
        private void finalizeServiceParsing(string lastLine, int lineNumber)
        {
            // if closing bracket is not first character in line, the part before the line should be treated
            // as actual content of the currently parsed service
            if (lastLine.IndexOf('}') != 0)
                parseLineOfService(lastLine.Split()[0].Trim(), lineNumber);

            // Add the service that was parsed to the service registry.
            IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.services.Add(currentlyParsedService.Name, currentlyParsedService);

            // End service parsing in main IDL parser. If there is still content following, treat it as new
            // line in the IDL
            IDLParser.Instance.currentlyParsing = IDLParser.ParseMode.NONE;
            if (lastLine.IndexOf('}') == 0 && lastLine.Length > 1)
                IDLParser.Instance.parseLine(lastLine.Split('}')[1].Trim());
        }

        SINFONIService currentlyParsedService;
    }
}
