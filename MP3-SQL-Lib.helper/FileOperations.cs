using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.helper
{
    public static class FileOperations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">"c:\music"</param>
        /// <param name="bSubDirs">Look for files in subdirs?</param>
        /// <returns>filepaths in string[]</returns>
        public static string[] GetFilesInDirectory(string path, bool bSubDirs)
        {
            string[] filePaths = (bSubDirs) ? (Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)) : (Directory.GetFiles(path, "*.*"));
            return filePaths;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">"c:\music"</param>
        /// <param name="patern">"*.*" "*.mp3"</param>
        /// <param name="bSubDirs">find files in subdirs as well?</param>
        /// <returns>path of found files in string[]</returns>
        public static string[] GetFilesInDirectoryByPatern(string path, string patern, bool bSubDirs) 
        {
            string[] filePaths = (bSubDirs) ? (Directory.GetFiles(path, patern, SearchOption.AllDirectories)) : (Directory.GetFiles(path, patern));
            return filePaths;
        }
       
        public static int DeleteAllFilesInDirectory(string path, bool bSubDirs)
        {
            string[] filePaths = GetFilesInDirectory(path, bSubDirs);
            int deletedfiles = 0;
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
                deletedfiles+=1;
            }
            return deletedfiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">"c:\music"</param>
        /// <param name="patern">"*.*" "*.mp3"</param>
        /// <param name="bSubDirs">delete in subdirs as well?</param>
        /// <returns>ammount of deleted files</returns>
        public static int DeleteFilesByPatern(string path, string patern, bool bSubDirs)
        {
            string[] filePaths = GetFilesInDirectoryByPatern(path, patern, bSubDirs);
            int deletedfiles = 0;
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
                deletedfiles += 1;
            }
            return deletedfiles;
        }

        /// <summary>
        /// krijg bytearray van een bestandfilepath (bv voor pictures)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetPictureFromDirectory(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); 
            byte[] data = new byte[fs.Length]; 
            fs.Read(data, 0, (int)fs.Length); 
            fs.Close();
            return data;
        }

        /// <summary>
        /// krijg bytearray van een url path voor bv pictures
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytesFromUrlDownloadData(string url)
        {
            byte[] b = null;
            using (var webClient = new System.Net.WebClient()) 
            { 
                b = await webClient.DownloadDataTaskAsync(new Uri(url));
            }
            return b;
        }

        static public byte[] GetBytesFromUrlBinaryReader(string url)
        {
            byte[] b;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();

            Stream stream = myResp.GetResponseStream();
            //int i;
            using (BinaryReader br = new BinaryReader(stream))
            {
                //i = (int)(stream.Length);
                b = br.ReadBytes(500000);
                br.Close();
            }
            myResp.Close();
            return b;
        }

        /// <summary>
        /// Kan gebrukt worden op image,txt, ... op te slaan als een bestand
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        static public void WriteBytesToFile(string fileName, byte[] content)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            try
            {
                w.Write(content);
            }
            finally
            {
                fs.Close();
                w.Close();
            }

        }


        public static string[] ReadFileToStringArray(string path)
        {
            string[] readText = null;
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        //String line = sr.ReadToEnd();
                        //Console.WriteLine(line);
                        readText = File.ReadAllLines(path);
                        /*foreach (string s in readText)
                        {
                            Console.WriteLine(s);
                        }*/
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return readText;
        }

        public static StringBuilder ReadFileToStringBuilder(string path)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        String line = sr.ReadToEnd();
                        sb.AppendLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return sb;
        }

        public static void OpenFileInDefaultProgram(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
