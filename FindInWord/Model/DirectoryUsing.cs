using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FindInWord.Model
{
    class DirectoryUsing
    {         
        public static  IEnumerable<string> DirectoryOpen()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            IEnumerable<string> files=null;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dialog.FileName;               
                files = Directory.EnumerateFiles($@"{folder}", "*.docx", SearchOption.AllDirectories);
                
            }
            return (files);
        }
    }
}
