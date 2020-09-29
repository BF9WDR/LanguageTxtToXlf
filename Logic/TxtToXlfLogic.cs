using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LanguageTxtToXlf.Logic
{
    public class TxtToXlfLogic
    {
        string logtext = "";
        private const char separator = ':';
        private const string LanguageCodePattern = @"[\s\S]*-(A|a)[\d]{4}-[\s\S]*";

        private string sourceFileName;
        private string destinationFileContent = "";
        private string isoCode;
        private TextBlockLogger Logger;

        private StringBuilder builder;
        

        public TxtToXlfLogic(string sourceFileName, string isoCode, TextBlockLogger logger)
        {
            this.sourceFileName = sourceFileName;
            this.isoCode = isoCode;
            this.Logger = logger;   
        }

        public async Task<bool> Process()
        {
            
            var lines = File.ReadAllLines(sourceFileName);

            string fieldId = "";
            string sourceCaption = "";
            string targetCaption = "";
            int consecutiveSourceCount = 0;
            bool nodeReady = false;

            builder = new StringBuilder("", lines.Length * 50);
            builder.Append(PrepareXmlHeader());

            for (int i = 0; i< lines.Length; i++)
            {
                string line = lines[i];
               
                var splittedLine = line.Split(separator);
                if (splittedLine.Length > 0)
                {
                    
                    if (!Regex.Match(splittedLine[0], LanguageCodePattern, RegexOptions.IgnoreCase).Success)
                    {
                        // source line
                        nodeReady = false;
                        fieldId = splittedLine[0];
                        consecutiveSourceCount++;
                        sourceCaption = String.Join(separator, splittedLine.Skip(1).Take(splittedLine.Length - 1));
                        if (consecutiveSourceCount > 1)
                        {
                            targetCaption = sourceCaption;
                        }

                    } else
                    {
                        // target / translation line
                        if (!nodeReady && splittedLine[0].Contains(isoCode))
                        {
                            //we found the correct line
                            if (consecutiveSourceCount > 1)
                            {
                                targetCaption = sourceCaption;
                            }
                            nodeReady = true;
                            targetCaption = String.Join(separator, splittedLine.Skip(1).Take(splittedLine.Length - 1));
                            consecutiveSourceCount = 0;
                        }
                    }
                    if (nodeReady)
                    {
                        
                        builder.Append(CustomizeTransUnitEntry(fieldId, sourceCaption, targetCaption));
                    }                    
                }
                if (i % 1000 == 0)
                {

                    Logger.Log($"feldolgoztam {i} db sort!");
                }

            }
            Logger.Log(logtext);

            builder.Append(PrepareXmlFooter());
            destinationFileContent = builder.ToString();
            WriteToFile();
            
            return true;

        }

        private void WriteToFile()
        {
            var savePath = System.IO.Path.GetTempFileName();
            File.WriteAllText(savePath, destinationFileContent);
            Process fileOpener = new Process();
            fileOpener.StartInfo.FileName = "notepad";
            fileOpener.StartInfo.Arguments = savePath;
            MessageBoxResult result = MessageBox.Show("Szeretné megnyitni a temp fájlt?", "Fájl megnyitása", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                fileOpener.Start();
            }

        }

        #region darkness
        private string PrepareXmlHeader()
        {
            return
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff version=""1.2"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
  <file datatype=""xml"" source-language=""SOURCE-LANGUAGE"" target-language=""TARGET-LANGUAGE"" original=""COMPANY"">
    <body>
      <group id=""body"">
";
        }


        private string PrepareXmlFooter()
        {
            return
@"      </group>
    </body>
  </file>
</xliff>";
        }

        private string CustomizeTransUnitEntry(string transUnitId, string sourceCaption, string targetCaption)
        {
            var res =
@"      <trans-unit id=""ID"">
          <source>SOURCE-CAPTION</source>
          <target>TARGET-CAPTION</target>
       </trans-unit>
";
            transUnitId = transUnitId.Replace("<", null);
            transUnitId = transUnitId.Replace(">", null);

            sourceCaption = transUnitId.Replace("<", null);
            sourceCaption = transUnitId.Replace(">", null);

            transUnitId = transUnitId.Replace("<", null);
            targetCaption = transUnitId.Replace(">", null);

            res = res.Replace("ID", transUnitId);
            res = res.Replace("SOURCE-CAPTION", sourceCaption);
            res = res.Replace("TARGET-CAPTION", targetCaption);
            return res;
        }

        #endregion
    }
}
