using FindInWord.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Application = Microsoft.Office.Interop.Word.Application;
using Document = Microsoft.Office.Interop.Word.Document;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace FindInWord.ViewModel
{
    class MainVM : INotifyPropertyChanged
    {
        public ObservableCollection<PathFile> PathFiles { get; private set; }
        public ObservableCollection<PathFile> PathSearchFiles { get; private set; }

        public MainVM()
        {
            PathFiles = new ObservableCollection<PathFile>();
            PathSearchFiles= new ObservableCollection<PathFile>();
            OpenDirectoryCommand = new DelegateCommand(OpenDirectory, CanOpenDirectory);
            SearchCommand = new DelegateCommand(SearchWord, CanSearchWord);
            OpenFileCommand = new DelegateCommand(OpenFile, CanOpenFile);
            BreakSearchCommand = new DelegateCommand(BreakSearch);
        }

        public ICommand OpenDirectoryCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand BreakSearchCommand { get; private set; }

        private void BreakSearch(object obj)
        {
            cancelTokenSource.Cancel();
            BreakButtonVisibality = "Collapsed";         
            searchProcess = false;            
            cancelTokenSource = new CancellationTokenSource();
        }

        private PathFile selectedFindFiles;
        public PathFile SelectedFindFiles
        {
            get
            {
                return selectedFindFiles;
            }
            set
            {               
                selectedFindFiles = value;                
                OnpropertyChanged("SelectedFindFiles");                
                TextInFile = Convert.ToString(FileUsing.OpenWordprocessingDocumentReadonly(SelectedFindFiles.FileName));        
            }
        }

        private bool CanOpenFile(object arg)
        {
            if (selectedFindFiles != null && PathSearchFiles.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CanOpenDirectory(object arg)
        {
            if (!searchProcess)
            {
                return true;
            }
            return false;
        }

        private bool CanSearchWord(object arg)
        {
            if (findText != "" && PathFiles.Count > 0 && findText != null && !searchProcess)
            {
                return true;
            }
            return false;
        }

        private bool searchProcess=false;

        private void OpenFile(object obj)
        {
            FileUsing.Openfile(SelectedFindFiles.FileName);                 
        }       

        private void OpenDirectory(object obj)
        {
            Load_indicator = 0;
            TextInFile = "";
            PathSearchFiles.Clear();
            PathFiles.Clear();            
            var files = DirectoryUsing.DirectoryOpen();
            try
            {
                if (files != null)
                {
                    foreach (string filename in files)
                    {
                        PathFiles.Add(new PathFile { FileName = filename });
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string findText;

        public string FindText
        {
            get { return findText; }
            set
            {
                findText = value;
                OnpropertyChanged("findText");
            }
        }

        private string textInFile;
        public string TextInFile
        {
            get 
            { 
            return textInFile;
            }
            set
            {
                textInFile = value;                
                OnpropertyChanged("textInFile");
            }
        }

        private int load_indicator;
        public int Load_indicator
        {
            get
            {
                return load_indicator;
            }
            set
            {
                load_indicator = value;
                OnpropertyChanged("load_indicator");
            }
        }

        private string breakButtonVisibality="Collapsed";
        public string BreakButtonVisibality
        {
            get
            {
                return breakButtonVisibality;
            }
            set
            {
                breakButtonVisibality = value;
                OnpropertyChanged("breakButtonVisibality");
            }
        }

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        async void SearchWord(object obj)
        {
            CancellationToken token = cancelTokenSource.Token;
           
            PathSearchFiles.Clear();
            Load_indicator = 0;
            searchProcess = true;            
            await Task.Run(() =>
            {               
                BreakButtonVisibality = "Visible";
                foreach (var path in PathFiles)
                {
                    if (token.IsCancellationRequested)
                    {
                        //MessageBox.Show("Операция прервана");
                        return;
                    }
                    Load_indicator++;
                    string text = Convert.ToString(FileUsing.OpenWordprocessingDocumentReadonly(path.FileName));                    
                    try
                    {                        
                        if (text.Contains(FindText))
                        {
                            App.Current.Dispatcher.Invoke((System.Action)delegate
                            {
                                PathSearchFiles.Add(new PathFile { FileName = path.FileName });// так как список может обновится только
                                                //из потока, где он был создан проделегируем выполнение данного действия основному потоку
                            });
                            // MessageBox.Show(text);   
                        }
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show(ex.Message);
                    }
                }                
                searchProcess = false;
                BreakButtonVisibality = "Collapsed";
            });
           // MessageBox.Show(Convert.ToString(PathSearchFiles.Count));
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnpropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }       
    }    
}
