﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Common
{
    public class BusinessConstants
    {
        public const string GoibiboAuthParameter = "&app_id={app_id}&app_key={app_key}&format={format}";
        public const string GoibiboBusSearchApi = "/api/bus/search/?source={0}&destination={1}&dateofdeparture={2}";
        public const string WeatherApi = "/data/2.5/weather?q={0}&units=metric&appid={1}";
        public const string WeatherForecastApi = "/data/2.5/forecast?q={0}&units=metric&appid={1}"; 
    }
}