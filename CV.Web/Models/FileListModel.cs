using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class FileListModel
    {
        public FileListModel()
        {
            Files = new List<string>();

            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/images/custom"));

            foreach( FileInfo fi in di.GetFiles() )
            {
                Files.Add(fi.Name);
            }
        }

        public List<string> Files { get; set; }
    }

}