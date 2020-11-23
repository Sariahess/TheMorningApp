using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TheMorningApp
{
    public partial class MorningApp : Form
    {
        public MorningApp()
        {
            InitializeComponent();

            // start up at Windows start
            StartUpConfig();

            // show user name
            welcomeMessage.Text = $"Good morning, {Environment.UserName.ToUpper()}!!";

            // ipDATA : location
            string ipdataAPI = "ipDataApiToken";
            string addressForLocation = "https://api.ipdata.co?api-key=";

            IpData.Rootobject locationObj = JsonConvert.DeserializeObject<IpData.Rootobject>(ReturnString(ipdataAPI, addressForLocation));

            getIP.Text = locationObj.ip;
            getCity.Text = locationObj.city;
            getProvince.Text = locationObj.region;
            getCountry.Text = locationObj.country_name;


            // weather : temperature
            string weatherAPI = "openWeatherAppApiToken";
            string addressForTemperature = "http://api.openweathermap.org/data/2.5/weather?q=" + $"{locationObj.city}&appid=";

            WeatherData.Rootobject weatherObj = JsonConvert.DeserializeObject<WeatherData.Rootobject>(ReturnString(weatherAPI, addressForTemperature));

            getTemp.Text = (weatherObj.main.temp - 273.15).ToString("0.##") + "  °C";


            // currency : Korean Won vs. Canadian Dollar
            string currencyAPI = "";
            string addressForCurrency = "https://api.exchangeratesapi.io/latest";

            CurrencyData.Rootobject rateObj = JsonConvert.DeserializeObject<CurrencyData.Rootobject>(ReturnString(currencyAPI, addressForCurrency));

            getCAD.Text = $"{(rateObj.rates.KRW / rateObj.rates.CAD).ToString("0.##")}" + " KRW / CAD";
        }

        public string ReturnString(string apiKey, string apiAddress)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.GetAsync($"{apiAddress}" + $"{apiKey}").Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = response.Content.ReadAsStringAsync().Result;
                return jsonResult;
            }
            else
            {
                return response.StatusCode + response.ReasonPhrase;
            }
        }

        public void StartUpConfig()
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser
                .OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            key.SetValue("TheMorningApp", Application.ExecutablePath);
        }
    }
}
