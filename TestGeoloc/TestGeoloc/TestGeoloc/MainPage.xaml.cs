using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.IO;

namespace TestGeoloc
{
    public partial class MainPage : ContentPage
    {
        public int tryb;
        public readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "notesXB.txt");
        public readonly string _pathGeo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Geoloc.txt");
        public int countA;
        public int countS;
        public bool returnValue;

        public MainPage()
        {
            InitializeComponent();
            Application.Current.Properties["countA_send"] = 10;
            Application.Current.Properties["minS_send"] = 600;
            Application.Current.Properties["_path_send"] = _path;
            labelNumber.Text = ReadPath("Nie podano numeru domyślnego");
            tryb = 0;
            LoopGeoSave(50);
        }

        private void LoopGeoSave(int min)
        {
            Device.StartTimer(TimeSpan.FromSeconds(min), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GetLoc("exc");
                });
                return true;
            });
        }

        public void Timer(int min)
        {
            Device.StartTimer(TimeSpan.FromSeconds(min), () =>
            {
                if (tryb == 1)
                {
                    countA = countA - 1;
                    if (countA == 0)
                    {
                        returnValue = false;
                        GetLoc(ReadPath(null));
                    }
                }
                if (tryb == 2)
                {
                    countS = countS + 1;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (tryb == 1)
                    {
                        labelNumber.Text = countA.ToString();
                    }
                    if (tryb == 2)
                    {
                        if (returnValue == true)
                        {
                            GetLoc(ReadPath("Nie dodano numeru"));
                            labelNumber.Text = countS.ToString();
                        }
                    }
                });
                return returnValue;
            });
        }

        public async void GetLoc(string number)
        {
            Location result = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromMinutes(1)));
            string res = $"Lat: { result.Latitude}, Lng: { result.Longitude}";
            File.WriteAllText(_pathGeo, res);
            if (number != "exc")
            {
                if (number != null)
                {
                    await SendSms($"Lat: { result.Latitude}, Lng: { result.Longitude}", number);
                }
                else
                {
                    await DisplayAlert("Failed", "Nie ustawiono domyślnego numeru", "OK");
                }
            }
        }

        public void SetData(string data1, string data2, string data3, int x1, int x2, Thickness data4)
        {
            defaultNumber.Text = "";
            defaultNumber.FontSize = x1;
            defaultNumber.Text = data1;
            labelNumber.Text = "";
            labelNumber.FontSize = x2;
            labelNumber.Text = data2;
            but_Main.Text = data3;
            but_Main.Margin = data4;
        }

        public string ReadPath(string lbl)
        {
            return File.Exists(_path) ? File.ReadAllText(_path) : lbl;
        }

        public async Task SendSms(string messageText, string recipient)
        {
            try
            {
                var message = new SmsMessage(messageText, recipient);
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                await DisplayAlert("Failed", "Sms is not supported on this device.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed", ex.Message, "OK");
            }
        }

        public async Task SaveNumber()
        {
            string result = await DisplayPromptAsync("Dodaj domyślny numer", "Wprowadź numer domyślnego odbiorcy", keyboard: Keyboard.Numeric, maxLength: 9);
            result = result.Insert(3, " - ");
            result = result.Insert(9, " - ");
            File.WriteAllText(_path, result);
            labelNumber.Text = ReadPath("Nie podano numeru domyślnego");
        }

        public async void OnActionSheetSimpleClicked()
        {
            string action = await DisplayActionSheet("Wybierz tryb", "Cancel", null, "Domyślny", "Cykliczny", "Aktywny");

            switch (action)
            {
                case "Aktywny":
                    tryb = 1;
                    returnValue = true;
                    countA = (int)Application.Current.Properties["countA_send"];

                    if (!File.Exists(_path))
                    {
                        try
                        {
                            await SaveNumber();
                        }
                        catch
                        {
                            await DisplayAlert("Cancel", "Konieczne wprowadzenie numeru odbiorcy.", "OK");
                        }
                    }
                    SetData("Po upłynięciu czasu nastąpi wysłanie wiadomości", countA.ToString(), "Przedłóż", 16, 27, new Thickness(0, 70, 0, 0));
                    Timer(1);
                   
                    break;
                case "Domyślny":
                    tryb = 0;
                    returnValue = false;
                    SetData("Domyślny numer telefonu:", ReadPath("Nie podano numeru domyślnego"), "Wyślij lokalizację", 14, 16, new Thickness(0, 82, 0, 0));
                    break;

                case "Cykliczny":
                    tryb = 2;
                    countS = 0;
                    returnValue = false;
                    SetData("Ilość wysłanych koordów", "0", "Start", 16, 21, new Thickness(0, 85, 0, 0));
                    break;

                default:
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (tryb == 0)
            {
                GetLoc(ReadPath(null));
            }
            else if (tryb == 1)
            {
                countA = (int)Application.Current.Properties["countA_send"];
            }
            else if (tryb == 2)
            {
                if (but_Main.Text == "Start")
                {
                    returnValue = true;
                    but_Main.Text = "Stop";
                    Timer((int)Application.Current.Properties["minS_send"]);
                }
                else
                {
                    returnValue = false;
                    but_Main.Text = "Start";
                }
            }
        }

        private async void Button_Add(object sender, EventArgs e)
        {
            try
            {
                await SaveNumber();
            }
            catch
            {
                await DisplayAlert("Cancel", "Zalecamy ustawienie domyślnego odbiorcy.", "OK");
            }
        }

        private void Button_Chng(object sender, EventArgs e)
        {
            OnActionSheetSimpleClicked();
        }

        private async void Button_Shr(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Numer odbiorcy", "Wprowadź numer odbiorcy", keyboard: Keyboard.Numeric, maxLength: 9);
            GetLoc(result);
        }

        public async void Button_Set(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetPage());
            labelNumber.Text = ReadPath("Nie podano numeru domyślnego");
        }
    }
}