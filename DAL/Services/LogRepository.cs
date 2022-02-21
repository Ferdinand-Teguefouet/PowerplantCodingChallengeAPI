using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class LogRepository : ILogRepository
    {
        #region variables
        private const string FILE_PATH = @".\log.dat";
        #endregion

        #region WriteInLogFile method
        public void WriteInLogFile(DateTime date, string message)
        {
            try
            {
                WriteInFile(date, message);
            }
            catch (Exception)
            {
                WriteInFile(date, "DAL layer: a problem occurred while writing in the loh file.");
                throw new Exception("Writing error in the file.");
            }
        }
        #endregion

        #region Private method
        private void WriteInFile(DateTime date, string message)
        {
            if (!File.Exists(FILE_PATH))
            {
                string titleOfLogFile = "********** Errors list occurred while application running **********" + Environment.NewLine;
                File.WriteAllText(FILE_PATH, titleOfLogFile);
                File.AppendAllText(FILE_PATH, "**********************************************************" + Environment.NewLine);
                File.AppendAllText(FILE_PATH, Environment.NewLine);
            }
            string textToSave = date.ToString() + "  |  " + message + $"---{Environment.NewLine}";
            File.AppendAllText(FILE_PATH, textToSave);
        } 
        #endregion
    }
}
