using DarkSkyDotNetCore;
using DarkSkyDotNetCore.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBot.MessageGenerators.WeatherMessageGenerator
{
    public class WeatherMessageGenerator : IWeatherMessageGenerator
    {
        private const float _latitude = 53.9f;
        private const float _longtitude = 27.56667f;
        private readonly string _apiKey;

        public WeatherMessageGenerator(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> GetMessageAsync()
        {
            var forecast = await GetCurrentForecast(_apiKey, DarkSkyDotNetCore.Unit.si, "ru");

            var resultedForecast = new StringBuilder();

            resultedForecast.Append(GetCurrentForecastString(forecast));
            resultedForecast.Append(GetDailyForecastString(forecast));

            return resultedForecast.ToString();
        }

        private async Task<ForecastResponse> GetCurrentForecast(string apiKey, Unit unit, string lang)
        {
            var request = new ForecastRequest(apiKey, _latitude, _longtitude, unit, lang);

            return await request.GetAsync();
        }

        private string GetCurrentForecastString(ForecastResponse response)
        {
            var result = "====================== Прогноз на сегодня ========================\r\n" +
                $"Краткая сводка: {response.Currently.Summary}\r\n" +
                $"Температура: {response.Currently.Temperature.ToWholeNumber()} \r\n" +
                $"Ощущается: {response.Currently.ApparentTemperature.ToWholeNumber()} \r\n" +
                $"Влажность: {response.Currently.Humidity.ToPercentString()} \r\n" +
                $"Давление: {response.Currently.Pressure}\r\n";

            if (response.Alerts != null && response.Alerts.Count > 0)
            {
                result += "======================== Предупреждения ========================\r\n";
                foreach (var alert in response.Alerts)
                {
                    result += $"{alert.Title}\r\n";
                }
            }

            result += "\r\n";

            return result;
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
