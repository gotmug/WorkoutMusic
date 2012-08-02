using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Un4seen.Bass;
using System.Windows.Interop;
using Un4seen.Bass.Misc;
using System.Timers;

namespace WorkoutMusic.ViewModels
{
    public class TapperVM : NotificationObject
    {
        private string _tapCountManual = "0";
        private DelegateCommand _tapCommand;
        BPMCounter _bpmCounterManual = new BPMCounter(20, 44100);
        BPMCounter _bpmCounterAuto = new BPMCounter(20, 44100);
        private string _tapCountAuto = "0";
        int _stream;
        List<double> _tappedAvgs = new List<double>();
        List<double> _calculatedAvgs = new List<double>();
        Timer _avgTimer;
        private string _averageTapped = "0";
        private string _averageCalculated = "0";
        private string _tapTime;
        private DateTime? _startTapTime;
        private string _numberOfTaps;
        private string _numberOfCalculations;
        private string _saveCaption = "Save";
        private int _selectedBPM;
        BASS_CHANNELINFO _info = new BASS_CHANNELINFO();
        private DelegateCommand _resetCommand;

        

        public TapperVM(string filename)
        {
            WindowInteropHelper helper = new WindowInteropHelper(App.Current.MainWindow);
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, helper.Handle);

            // create a stream
            _stream = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT);

            // get the samplerate of that stream
            Bass.BASS_ChannelGetInfo(_stream, _info);

            // and start playing the and also start the BPM counter
            if (_stream != 0 && Bass.BASS_ChannelPlay(_stream, false))
            {
                //playing...
                _bpmCounterAuto.Reset(_info.freq);
                _bpmCounterManual.Reset(_info.freq);

                _bpmCounterAuto.MaxBPM = 250;
                _bpmCounterManual.MaxBPM = 250;

                // start our bpm timer callback
                System.Timers.Timer timer = new System.Timers.Timer(20);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }

            _avgTimer = new Timer(1000);
            _avgTimer.Elapsed += new ElapsedEventHandler(_avgTimer_Elapsed);
            _avgTimer.Start();


            this.TapCommand = new DelegateCommand(new Action(() =>
            {
                if (!_startTapTime.HasValue)
                    _startTapTime = DateTime.Now;

                _bpmCounterManual.TapBeat();
                double tapped = _bpmCounterManual.TappedBPM;
                if (!Double.IsNaN(tapped))
                {
                    _tappedAvgs.Add(tapped);
                    string temp = tapped.ToString("0");
                    this.TapCountManual = temp;
                }
            }));

            this.ResetCommand = new DelegateCommand(Reset);


        }

        public DelegateCommand ResetCommand
        {
            get { return _resetCommand; }
            set { _resetCommand = value; RaisePropertyChanged(() => ResetCommand); }
        }

        public int SelectedBPM
        {
            get { return _selectedBPM; }
            set 
            { 
                _selectedBPM = value; 
                this.SaveCaption = "Save (" + _selectedBPM.ToString() + ")"; 
            }
        }

        public string SaveCaption
        {
            get { return _saveCaption; }
            set { _saveCaption = value; RaisePropertyChanged(() => SaveCaption); }
        }

        public string NumberOfCalculations
        {
            get { return _numberOfCalculations; }
            set { _numberOfCalculations = value; RaisePropertyChanged(() => NumberOfCalculations); }
        }

        public string NumberOfTaps
        {
            get { return _numberOfTaps; }
            set { _numberOfTaps = value; RaisePropertyChanged(() => NumberOfTaps); }
        }

        public string TapTime
        {
            get { return _tapTime; }
            set { _tapTime = value; RaisePropertyChanged(() => TapTime); }
        }

        public string AverageCalculated
        {
            get { return _averageCalculated; }
            set { _averageCalculated = value; RaisePropertyChanged(() => AverageCalculated); }
        }

        public string AverageTapped
        {
            get { return _averageTapped; }
            set { _averageTapped = value; RaisePropertyChanged(() => AverageTapped); }
        }

        public string TapCountAuto
        {
            get { return _tapCountAuto; }
            set { _tapCountAuto = value; RaisePropertyChanged(() => TapCountAuto); }
        }
        public DelegateCommand TapCommand
        {
            get { return _tapCommand; }
            set { _tapCommand = value; RaisePropertyChanged(() => TapCommand); }
        }

        public string TapCountManual
        {
            get { return _tapCountManual; }
            set { _tapCountManual = value; RaisePropertyChanged(() => TapCountManual); }
        }


        void _avgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_tappedAvgs.Count > 0)
            {
                double avg = RemoveOutliers(_tappedAvgs.ToArray()).Average();
                this.AverageTapped = avg.ToString("0");
                this.NumberOfTaps = "# Taps: " + _tappedAvgs.Count.ToString();
            }

            if (_calculatedAvgs.Count > 0)
            {
                this.AverageCalculated = RemoveOutliers(_calculatedAvgs.ToArray()).Average().ToString("0");
                this.NumberOfCalculations = "# Calc: " + _calculatedAvgs.Count.ToString();
            }

            if (_startTapTime.HasValue)
            {
                TimeSpan difference = DateTime.Now - _startTapTime.Value;
                this.TapTime = "Tap Time: " + difference.ToString("mm\\:ss");
            }
        }

        private static IEnumerable<double> RemoveOutliers(IEnumerable<double> withOutliers)
        {
            // Calculate standard deviation
            double stdDev = CalculateStdDev(withOutliers);

            // Calculate average
            double avg = withOutliers.Average();

            // Remove outliers outside of 3 deviations away from the average
            var removedOutliers = withOutliers.Where(d => Math.Abs(avg - d) <= 3 * stdDev);

            // Return this if there is at least one data point
            if (removedOutliers.Count() > 0)
                return removedOutliers;
            else
                return withOutliers;
        }

        private static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (_stream == 0 || Bass.BASS_ChannelIsActive(_stream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                ((System.Timers.Timer)sender).Stop();
                return;
            }
            bool beat = _bpmCounterAuto.ProcessAudio(_stream, true);
            if (beat)
            {
                // display the live calculated BPM value
                double calculated = _bpmCounterAuto.BPM;
                _calculatedAvgs.Add(calculated);
                string temp = calculated.ToString("0");
                this.TapCountAuto = temp;

            }
        }

        public void CleanUp()
        {
            Bass.BASS_Free();
            _stream = 0;
            _avgTimer.Stop();
        }

        public void Reset()
        {
            _bpmCounterAuto.Reset(_info.freq);
            _bpmCounterManual.Reset(_info.freq);
            _calculatedAvgs.Clear();
            _tappedAvgs.Clear();
            this.AverageCalculated = "0";
            this.AverageTapped = "0";
            this.TapCountAuto = "0";
            this.TapCountManual = "0";
            _startTapTime = null;
        }

        public static double NORMDIST(double x, double mean, double std, bool cumulative)
        {
            if (cumulative)
            {
                return Phi(x, mean, std);
            }
            else
            {
                double tmp = 1 / ((Math.Sqrt(2 * Math.PI) * std));
                return tmp * Math.Exp(-.5 * Math.Pow((x - mean) / std, 2));
            }
        }

        //from http://www.cs.princeton.edu/introcs/...Math.java.html
        // fractional error less than 1.2 * 10 ^ -7.
        static double erf(double z)
        {
            double t = 1.0 / (1.0 + 0.5 * Math.Abs(z));

            // use Horner's method
            double ans = 1 - t * Math.Exp(-z * z - 1.26551223 +
            t * (1.00002368 +
            t * (0.37409196 +
            t * (0.09678418 +
            t * (-0.18628806 +
            t * (0.27886807 +
            t * (-1.13520398 +
            t * (1.48851587 +
            t * (-0.82215223 +
            t * (0.17087277))))))))));
            if (z >= 0) return ans;
            else return -ans;
        }

        // cumulative normal distribution
        static double Phi(double z)
        {
            return 0.5 * (1.0 + erf(z / (Math.Sqrt(2.0))));
        }

        // cumulative normal distribution with mean mu and std deviation sigma
        static double Phi(double z, double mu, double sigma)
        {
            return Phi((z - mu) / sigma);
        }

        public void Save()
        {
            if (this.SelectedBPM == 0)
            {
                if (this.AverageTapped != "0")
                    this.SelectedBPM = int.Parse(this.AverageTapped);
                else if (this.AverageCalculated != "0")
                    this.SelectedBPM = int.Parse(this.AverageCalculated);
                else if (this.TapCountManual != "0")
                    this.SelectedBPM = int.Parse(this.TapCountManual);
                else if (this.TapCountAuto != "0")
                    this.SelectedBPM = int.Parse(this.TapCountAuto);
            }
        }
    }
}
