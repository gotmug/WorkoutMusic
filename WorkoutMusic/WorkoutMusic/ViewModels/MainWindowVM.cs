using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;
using Un4seen.Bass;
using WorkoutMusic.Views;
using System.Media;

namespace WorkoutMusic.ViewModels
{
    public class MainWindowVM : NotificationObject
    {
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private DelegateCommand _playCommand;
        private Tapper _tapper;
        private string _playButtonTitle = "Play";
        private string _selectedItemPath;
        private Process _mediaPlayer;

        public string SelectedItemPath
        {
            get { return _selectedItemPath; }
            set { _selectedItemPath = value; RaisePropertyChanged(() => SelectedItemPath); }
        }

        public string PlayButtonTitle
        {
            get { return _playButtonTitle; }
            set { _playButtonTitle = value; RaisePropertyChanged(() => PlayButtonTitle); }
        }

        public Tapper Tapper
        {
            get { return _tapper; }
            set { _tapper = value; }
        }

        public DelegateCommand PlayCommand
        {
            get { return _playCommand; }
            set { _playCommand = value; RaisePropertyChanged(() => PlayCommand); }
        }

        public Song SelectedSong
        {
            get { return _selectedSong; }
            set { _selectedSong = value; RaisePropertyChanged(() => SelectedSong); }
        }

        public ObservableCollection<Song> Songs
        {
            get { return _songs; }
            set { _songs = value; RaisePropertyChanged(() => Songs); }
        }

        public MainWindowVM()
        {
            BassNet.Registration("nfisikelli@gmail.com", "2X14162920152222");
            this.Songs = new ObservableCollection<Song>();
            _mediaPlayer = new Process();
            _mediaPlayer.StartInfo = new ProcessStartInfo(@"C:\Program Files\Windows Media Player\wmplayer.exe");

            this.PlayCommand = new DelegateCommand(new Action(() =>
                {
                    if (this.SelectedSong == null)
                        return;

                    this.Tapper = new Tapper(this.SelectedSong.File.Name);
                    this.Tapper.ShowDialog();

                    if (this.Tapper.DialogResult == true)
                    {
                        int tapCount = ((TapperVM)this.Tapper.DataContext).SelectedBPM;
                        this.SelectedSong.File.Tag.BeatsPerMinute = (uint)tapCount;
                        this.SelectedSong.Bpm = tapCount.ToString();
                        this.SelectedSong.File.Save();
                    }
                }));
            
        }

        public void FolderSelected(string directory)
        {
            this.SelectedItemPath = directory;
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            var files = dirInfo.GetFilesByExtensions(".mp3", ".m4a");

            this.Songs = new ObservableCollection<Song>(files.Select(f =>
                {
                    TagLib.File file = TagLib.File.Create(f.FullName);
                    return new Song(file);
                }));
        }

        

    }
}
