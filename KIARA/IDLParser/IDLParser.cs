using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KIARA.Exceptions;

namespace KIARA
{
    public class IDLParser
    {
        internal enum ParseMode
        {
            COMMENT,
            STRUCT,
            SERVICE,
            NONE
        }

        public static IDLParser Instance = new IDLParser();

        internal void parseIDL(string idlString)
        {
            lineNumberParsed = 0;
            currentlyParsing = ParseMode.NONE;

            string[] idlLines =
                idlString.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in idlLines)
            {
                ++lineNumberParsed;
                parseLine(line.Trim());
            }
        }

        internal void parseLine(string line)
        {
            if(lineIsComment(line) || line.Length == 0)
                return;

            line = removeCommentsFromLine(line);

            if (currentlyParsing == ParseMode.NONE)
            {
                startObjectParsing(line);
            }
            else if (currentlyParsing == ParseMode.STRUCT)
            {
                StructParser.Instance.parseLineOfStruct(line);
            }
            else if (currentlyParsing == ParseMode.SERVICE)
            {
                ServiceParser.Instance.parseLineOfService(line, lineNumberParsed);
            }
        }

        private void startObjectParsing(string line)
        {
            if (line.Contains("struct") && line.IndexOf("struct") == 0)
            {
                currentlyParsing = ParseMode.STRUCT;
                StructParser.Instance.startStructParsing(line);
            }
            else if (line.Contains("service") && line.IndexOf("service") == 0)
            {
                currentlyParsing = ParseMode.SERVICE;
                ServiceParser.Instance.startServiceParsing(line);
            }
            else
            {
                throw new IDLParseException(line, lineNumberParsed);
            }
        }

        private bool lineIsComment(string line)
        {
            return currentlyParsing == ParseMode.COMMENT
                || line.Contains("//") && line.IndexOf("//") == 0
                || line.Contains("/*") && line.IndexOf("/*") == 0;
        }

        private string removeCommentsFromLine(string line)
        {
            if (line.Contains("//"))
                line = line.Substring(0, line.IndexOf("//"));

            if (line.Contains("/*"))
            {
                if (line.Contains("*/"))
                    line = line.Remove(line.IndexOf("/*"), line.IndexOf("*/") - line.IndexOf("/*") + 2);
                else
                    line = line.Substring(0, line.IndexOf("/*"));
            }

            return line.Trim();
        }

        internal ParseMode currentlyParsing = ParseMode.NONE;
        ServiceFunctionDescription currentlyParsedService;
        int lineNumberParsed = 0;
    }
}
