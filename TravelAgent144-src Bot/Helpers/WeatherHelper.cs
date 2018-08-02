using Newtonsoft.Json;
using SimpleEchoBot.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Helpers
{
    public class WeatherHelper
    {
        private string appId;
        private string webApiUri;
        public WeatherHelper()
        {
            this.appId = ConfigurationManager.AppSettings["openMapWeatherAppId"] as string;
            this.webApiUri = ConfigurationManager.AppSettings["openMapWeatherApi"] as string;
        }
        public Data_Models.Weather.Current.WeatherApiModel GetWeatherData(string location)
        {
            Data_Models.Weather.Current.WeatherApiModel weatherCurrentApiModel = new Data_Models.Weather.Current.WeatherApiModel();
            var url = BusinessConstants.WeatherApi;
            url = string.Format(url, location, appId);
            var serviceAgent = new WebApiClient(webApiUri, appId, "");
            using (var response = serviceAgent.GetAsync(url))
            {
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonResponse = result.Content.ReadAsStringAsync().Result;
                    weatherCurrentApiModel = JsonConvert.DeserializeObject<Data_Models.Weather.Current.WeatherApiModel>(jsonResponse);
                }
            }
            return weatherCurrentApiModel;
        }
        public Data_Models.Weather.WeatherForecastApiModel GetForecastWeatherData(string location)
        {
            Data_Models.Weather.WeatherForecastApiModel weatherForecastApiModel = new Data_Models.Weather.WeatherForecastApiModel();
            var url = BusinessConstants.WeatherForecastApi;
            url = string.Format(url, location, appId);
            var serviceAgent = new WebApiClient(webApiUri, appId, "");
            using (var response = serviceAgent.GetAsync(url))
            {
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonResponse = result.Content.ReadAsStringAsync().Result;
                    weatherForecastApiModel = JsonConvert.DeserializeObject<Data_Models.Weather.WeatherForecastApiModel>(jsonResponse);
                }
            }
            return weatherForecastApiModel;
        }
    }
}