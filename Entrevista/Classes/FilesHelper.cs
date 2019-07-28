using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Entrevista.Classes
{
    public class FilesHelper
    {
        public static bool UploadPhoto(HttpPostedFileBase file, string folder, string name)
        {
            string path = string.Empty;

            if (file is null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                if (File.Exists(path))
                {
                    FileInfo antigo = new FileInfo(path);
                    antigo.Delete();
                }
                file.SaveAs(path);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}