using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace WorkoutMusic
{
    public class Song : NotificationObject
    {

        private string _name;
        private string _artist;
        private string _bpm;
        private string _length;

        public Song(TagLib.File file)
        {
            File = file;
            Artist = file.Tag.AlbumArtists.FirstOrDefault();
            Bpm = file.Tag.BeatsPerMinute.ToString();
            Length = file.Properties.Duration.ToString("mm\\:ss");
            Name = file.Tag.Title;
        }

        public string Length
        {
            get { return _length; }
            set { _length = value; RaisePropertyChanged(() => Length); }
        }

        public string Bpm
        {
            get { return _bpm; }
            set { _bpm = value; RaisePropertyChanged(() => Bpm); }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; RaisePropertyChanged(() => Artist); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }
        public TagLib.File File { get; private set; }
    }
}
