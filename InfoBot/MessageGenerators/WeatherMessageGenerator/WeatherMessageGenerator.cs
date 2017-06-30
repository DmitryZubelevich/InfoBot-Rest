using DarkSkyDotNetCore;
using DarkSkyDotNetCore.Model;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfoBot.MessageGenerators.WeatherMessageGenerator
{
    public class WeatherMessageGenerator : IWeatherMessageGenerator
    {
        private readonly string _apiKey;
        private const string NominatimUrlTemplate = "http://nominatim.openstreetmap.org/search?city={0}&format=json";
        private static ILogger Logger = Log.ForContext<WeatherMessageGenerator>();

        public WeatherMessageGenerator(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> GetMessageAsync(string city)
        {
            var cityInfo = await SearchPlaceAsync(city);
            if(cityInfo == null)
            {
                return $"Извините, я не могу найти город {city}";
            }

            var forecast = await GetCurrentForecast(_apiKey, cityInfo, Unit.si, "ru");

            var resultedForecast = new StringBuilder();
            var cityName = cityInfo.display_name.Split(',').First().Trim();
            resultedForecast.Append(GetCurrentForecastString(forecast, cityName));
            //resultedForecast.Append(GetDailyForecastString(forecast));

            return resultedForecast.ToString();
        }

        private async Task<ForecastResponse> GetCurrentForecast(string apiKey, PlaceInfo cityInfo, Unit unit, string lang)
        {
            var latitude = float.Parse(cityInfo.lat);
            var longtitude = float.Parse(cityInfo.lon);
            var request = new ForecastRequest(apiKey, latitude, longtitude, unit, lang);

            return await request.GetAsync();
        }

        private async Task<PlaceInfo> SearchPlaceAsync(string city)
        {
            var url = string.Format(NominatimUrlTemplate, city);
            var request = WebRequest.Create(url);
            request.Method = "GET";

            try
            {
                var content = await Helpers.GetResponseAsync(request);
                return JsonConvert.DeserializeObject<List<PlaceInfo>>(content).FirstOrDefault();
            }catch(Exception e)
            {
                Logger.Error($"SearchPlaceAsync: Exception: {e.Message}. Stacktrace: {e.StackTrace}");
                return null;
            }
        }

        private string GetCurrentForecastString(ForecastResponse response, string cityName)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.Append($"Погода в городе {cityName} на сегодня: \r\n\r\n");
            resultBuilder.Append($"Краткая сводка: {response.Currently.Summary}\r\n");
            resultBuilder.Append($"Температура: {response.Currently.Temperature.ToWholeNumber()}°C \r\n");
            resultBuilder.Append($"Ощущается как: {response.Currently.ApparentTemperature.ToWholeNumber()}°C \r\n");
            resultBuilder.Append($"Влажность: {response.Currently.Humidity.ToPercentString()} \r\n");
            resultBuilder.Append($"Облачность: {response.Currently.CloudCover.ToPercentString()} \r\n");
            resultBuilder.Append($"Ветер: {response.Currently.WindSpeed.ToWholeNumber()}м/с {FindWindDirection(response.Currently.WindBearing)} \r\n");
            resultBuilder.Append($"Давление: {response.Currently.Pressure.ToWholeNumber()} мБар.");

            return resultBuilder.ToString();
        }

        private string FindWindDirection(float windBearing)
        {
            if(windBearing >= 0 && windBearing < 22.5)
            {
                return "(Ю)";
            }

            if(windBearing < 67.5)
            {
                return "(ЮЗ)";
            }

            if(windBearing < 112.5)
            {
                return "(З)";
            }

            if(windBearing < 157.5)
            {
                return "(СЗ)";
            }

            if(windBearing < 202.5)
            {
                return "(С)";
            }

            if(windBearing < 247.5)
            {
                return "(СВ)";
            }

            if(windBearing < 292.5)
            {
                return "(В)";
            }

            if(windBearing < 337.5)
            {
                return "(ЮВ)";
            }

            return "(Ю)";
        }

        private string GetDailyForecastString(ForecastResponse response)
        {
            var result = new StringBuilder();

            var todayDate = DateTime.Now;
            result.Append("============== Прогноз на сегодня =============\r\n");
            var twelveHoursForecast = response.Hourly.Data.Where(s => s.Time.ToDateTime().ToLocalTime() > todayDate).OrderBy(s => s.Time).Take(12).ToList();
            foreach (var hour in twelveHoursForecast)
            {
                //result.Append($"{day.Time.ToDateTime().ToLocalTime().ToString("dddd")} - {day.Summary} {day.TemperatureMin.ToWholeNumber()}-{day.TemperatureMax.ToWholeNumber()}\r\n");
                result.Append($"{hour.Time.ToDateTime().ToLocalTime().ToString("dd-MMM HH:mm")} - {hour.Summary} {hour.ApparentTemperature.ToWholeNumber()}\r\n");
            }

            return result.ToString();
        }
    }
}
