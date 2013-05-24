using System.Windows.Input;
using Caliburn.Micro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using RestSharp;
using WeatherGuru.Model;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace WeatherGuru.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// The <see cref="DayList" /> property's name.
        /// </summary>
        public const string DayListPropertyName = "DayList";

        private ObservableCollection<Day> _dayList = null;

        /// <summary>
        /// Sets and gets the DayList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Day> DayList
        {
            get
            {
                return _dayList;
            }

            set
            {
                if (_dayList == value)
                {
                    return;
                }

                RaisePropertyChanging(DayListPropertyName);
                _dayList = value;
                RaisePropertyChanged(DayListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentDay" /> property's name.
        /// </summary>
        public const string CurrentDayPropertyName = "CurrentDay";

        private Day _currentDay = null;

        /// <summary>
        /// Sets and gets the CurrentDay property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Day CurrentDay
        {
            get
            {
                return _currentDay;
            }

            set
            {
                if (_currentDay == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentDayPropertyName);
                _currentDay = value;
                RaisePropertyChanged(CurrentDayPropertyName);
            }
        }
        public const string CurrentCityPropertyName = "CurrentCity";
        private string _currentCity = null;

        /// <summary>
        /// Sets and gets the CurrentDay property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentCity
        {
            get
            {
                return _currentCity;
            }

            set
            {
                if (_currentCity == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentCityPropertyName);
                _currentCity = value;
                RaisePropertyChanged(CurrentCityPropertyName);
            }
        }

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                DayList = new ObservableCollection<Day>();
                DayList.Add(new Day { temp = 20, Time = DateTime.Now, weather = new System.Collections.Generic.List<Weather>{ new Weather { icon ="01d"}} });
                DayList.Add(new Day { temp = 20, Time = DateTime.Now.AddDays(1), weather = new System.Collections.Generic.List<Weather> { new Weather { icon = "04d" } } });
                DayList.Add(new Day { temp = 20, Time = DateTime.Now.AddDays(2), weather = new System.Collections.Generic.List<Weather> { new Weather { icon = "09d" } } });
                DayList.Add(new Day { temp = 20, Time = DateTime.Now.AddDays(3), weather = new System.Collections.Generic.List<Weather> { new Weather { icon = "10d" } } });
                DayList.Add(new Day { temp = 20, Time = DateTime.Now.AddDays(4), weather = new System.Collections.Generic.List<Weather> { new Weather { icon = "13d" } } });
                CurrentDay = DayList[0];
            }
            else
            {
                GetResponseObject();

            }
        }

        private void DoNothing()
        {
            
        }

        public void GetResponseObject()
        {
            var client = new RestClient("http://api.openweathermap.org/data/2.2/forecast/city?q="+CurrentCity+"&mode=daily_compact&units=metric");

            var request = new RestRequest(Method.GET);
            
            client.ExecuteAsync(request, response =>
            {
                if (response.ResponseStatus == ResponseStatus.Error)
                {
                    
                }
                else
                {
                    ProcessMe(response.Content);
                }
            });
        }


        public ICommand UpdateWeather
        {
            get
            {
                return new RelayCommand<string>((p) =>
                    {
                        GetResponseObject();
                    });
            }
        }

        public void ProcessMe(string content)
        {
            SmartDispatcher._instance.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    RootObject result = JsonConvert.DeserializeObject<RootObject>(content);
                    DayList = new ObservableCollection<Day>(result.list);
                    CurrentDay = DayList[0];
                });
            
        }
    }



    public static class SmartDispatcher
    {
        public static CoreDispatcher _instance;
        private static void RequireInstance()
        {
            try
            {
                _instance = Window.Current.CoreWindow.Dispatcher;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The first time SmartDispatcher is used must be from a user interface thread. Consider having the application call Initialize, with or without an instance.", e);
            }

            if (_instance == null)
            {
                throw new InvalidOperationException("Unable to find a suitable Dispatcher instance.");
            }
        }

        public static void Initialize(CoreDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _instance = dispatcher;
        }

        public static bool CheckAccess()
        {
            if (_instance == null)
            {
                RequireInstance();
            }
            return _instance.HasThreadAccess;
        }

        public static void BeginInvoke(System.Action a)
        {
            if (_instance == null)
            {
                RequireInstance();
            }

            // If the current thread is the user interface thread, skip the
            // dispatcher and directly invoke the Action.
            if (CheckAccess())
            {
                a();
            }
            else
            {
                _instance.RunAsync(CoreDispatcherPriority.Normal, () => { a(); });
            }
        }
    }

}