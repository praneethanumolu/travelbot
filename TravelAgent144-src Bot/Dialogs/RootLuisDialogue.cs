namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using SimpleEchoBot.Models;
    using Travel_Website.Repository;
    using Chronic;
    using log4net;
    using SimpleEchoBot.Helpers;

    [LuisModel("16816e8d-34d5-4eb4-b2ae-c1c1590d98ba", "d8c7c7db62134d9db0359a9845fb6d8c")]
    //[LuisModel("4bd9e4fa-8d7d-4b52-b968-43a2dd995cdd", "ff0cc11d49a14688844b74873bb9a97c")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        List<string> welcome_messages = new List<string> { "Hi user, welcome to our new bot service. Please feel free to test", "Hello user, We hope you are having a good day. Please feel free to test our new bot", $"Hi User, We are Happy to see you on our site. Please type your query" };

        List<string> bye_messages = new List<string> { "Thanks for using the bot, Hope you enjoyed","Thanks! Hope you recommend it to your friends"};

        List<string> angry_messages = new List<string> {"We regret for the inconvinience, we will try to train our bot to serve your purpose properly", "Appologies! Please try again after some time. We try to improve our bot"};

        List<string> parent_messages = new List<string> {"I dont have a parent. But praneeth has created me"};

        List<string> how_messages = new List<string> { "I am doing good, hope you are doing god as well" };

        List<string> bye_messages = new List<string>
        {

        private const string ToLocation = "Location::ToLocation";

        private const string TravelBookingInfo = "Travel Book Info";

        private const string FromLocation = "Location::FromLocation";

        private const string DateOfTravel = "builtin.datetimeV2.date";

        private const string Class = "Class";

        public const string TravelType = "Travel Type";

        public const string WeatherCity = "city";

        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private const string EntityGeographyCity = "builtin.geography.city";

        //private const string EntityHotelName = "Hotel";

        //private const string EntityAirportCode = "AirportCode";

        //private IList<string> titleOptions = new List<string> { "“Very stylish, great stay, great staff”", "“good hotel awful meals”", "“Need more attention to little things”", "“Lovely small hotel ideally situated to explore the area.”", "“Positive surprise”", "“Beautiful suite and resort”" };



        private async Task<TravelBooking> PromptUserForData(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result, TravelBooking bookingInfo)
        {
            var travelFormDialog = new FormDialog<TravelBooking>(bookingInfo, this.BuildTravelsForm, FormOptions.PromptInStart, result.Entities);
            //if (!IsDataValid(bookingInfo))
            //    context.Post()
            context.Call(travelFormDialog, this.ResumeAfterTravelFormDialog);
            return bookingInfo;
        }

        private bool IsDataValid(TravelBooking travelBooking)
        {
            if (string.IsNullOrEmpty(travelBooking.FromLocation) || string.IsNullOrEmpty(ToLocation) || string.IsNullOrEmpty(travelBooking.TravelType))
                return false;
            if (ParseChronicDate(travelBooking.DateOfTravel).HasValue)
                return true;
            return false;
        }

        private DateTime? ParseChronicDate(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
                return null;
            Parser parser = new Parser();
            var dateResult = parser.Parse(dateTime);
            return dateResult.Start;
        }

        [LuisIntent("")]
        [LuisIntent("None")]

        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance. Huff .... These programmers !!!! I have so much intelligence but my programmers need to work on the way to harness it. PS:: Not my fault :p";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Welcome")]

        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            Random rnd = new Random();
            int messageSelected = rnd.Next(0, welcome_messages.Count);
            string message = welcome_messages[messageSelected];

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Bye")]

        public async Task Bye(IDialogContext context, LuisResult result)
        {
            Random rnd1 = new Random();
            int messageSelected = rnd1.Next(0, bye_messages.Count);
            string message = bye_messages[messageSelected];

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        private async Task<TravelBooking> UnWrapEntities(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            TravelBooking bookingInfo = new TravelBooking();

            EntityRecommendation travelEntityDate;
            EntityRecommendation travelEntityToLocation;
            EntityRecommendation travelEntityFromLocation;
            EntityRecommendation travelEntityClass;
            EntityRecommendation travelEntityTravelType;

            if (result.TryFindEntity(TravelType, out travelEntityTravelType))
            {
                bookingInfo.TravelType = travelEntityTravelType.Entity;
            }


            if (result.TryFindEntity(FromLocation, out travelEntityFromLocation))
            {
                bookingInfo.FromLocation = travelEntityFromLocation.Entity;
            }

            if (result.TryFindEntity(ToLocation, out travelEntityToLocation))
            {
                bookingInfo.ToLocation = travelEntityToLocation.Entity;
            }

            if (result.TryFindEntity(DateOfTravel, out travelEntityDate))
            {
                //bookingInfo.DateOfTravel = travelEntityDate.Entity;
                bookingInfo.DateOfTravel = travelEntityDate.Entity;
                Parser parser = new Parser();
                var parsedDate = parser.Parse(bookingInfo.DateOfTravel).Start;
                bookingInfo.ConvertedDateTime = parsedDate.HasValue ? parser.Parse(travelEntityDate.Entity).Start.Value : DateTime.Today;
                if (!parsedDate.HasValue)
                    bookingInfo.DateOfTravel = string.Empty;
            }

            if (result.TryFindEntity(Class, out travelEntityClass))
            {
                bookingInfo.Class = travelEntityClass.Entity;
            }

            //while(!IsDataValid(bookingInfo))
            await PromptUserForData(context, activity, result, bookingInfo);
            //var hotelsQuery = new HotelsQuery();

            //EntityRecommendation cityEntityRecommendation;

            //if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            //{
            //    cityEntityRecommendation.Type = "Destination";
            //}

            //var hotelsFormDialog = new FormDialog<HotelsQuery>(hotelsQuery, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);
            //context.Call(travelFormDialog, this.ResumeAfterTravelFormDialog);
            return bookingInfo;
        }
        [LuisIntent("Book Bus")]
        //[LuisIntent("")]
        public async Task TravellingBus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Bus finder! We are analyzing your message: '{message.Text}'...");
            var entityResult = UnWrapEntities(context, activity, result).Result;
        }


        [LuisIntent("Book Flight")]
        //[LuisIntent("")]
        public async Task TravellingFlight(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Bus finder! We are analyzing your message: '{message.Text}'...");
            var entityResult = UnWrapEntities(context, activity, result).Result;
        }

        [LuisIntent("Weather.GetCondition")]
        [LuisIntent("Weather.GetForecast")]
        //[LuisIntent("")]
        public async Task WeatherCondition(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the weather Predictor! We are analyzing your message: '{message.Text}'...");
            //var entityResult = UnWrapEntities(context, activity, result).Result;
            EntityRecommendation weatherCityEntity;
            EntityRecommendation weatherDateEntity;

            string city = string.Empty;

            SimpleEchoBot.Helpers.WeatherHelper helper = new SimpleEchoBot.Helpers.WeatherHelper();

            if (result.TryFindEntity(WeatherCity, out weatherCityEntity))
            {
                city = weatherCityEntity.Entity;
                //bookingInfo.TravelType = travelEntityTravelType.Entity;
            }

            city = string.IsNullOrEmpty(city) ? "Hyderabad" : city;

            var weatherForecast = helper.GetForecastWeatherData(city);

            if (result.TryFindEntity(FromLocation, out weatherDateEntity))
            {
                //bookingInfo.FromLocation = travelEntityFromLocation.Entity;
            }
            await context.PostAsync($"Please wait while I make some quick calls to authorities ");
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            List<DateTime> datesAdded = new List<DateTime>();
            foreach (var weatherDay in weatherForecast.list)
            {
                var currentDateTime = Convert.ToDateTime(weatherDay.dt_txt);
                if (!datesAdded.Contains(currentDateTime) && DateTime.Now > currentDateTime && datesAdded.Count <= 4)
                {
                    var weatherDetails = weatherDay.weather.First();
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = weatherDetails.main + " " + PrettyDateHelper.GetPrettyDate(currentDateTime),
                        Subtitle = $"{weatherDetails.description}",
                        Text = $"On {weatherDay.dt_txt} at {city} Max Temperature - {weatherDay.main.temp_max} Min Temperature - {weatherDay.main.temp_min} Mostly {weatherDetails.main}",
                        //Images = new List<CardImage>()
                        //{
                        //    new CardImage() { Url = routeType == null ? nonAcURL : (routeType.busCondition.Contains("nonac") ? nonAcURL : acUrl) }
                        //},
                        //Buttons = new List<CardAction>()
                        //    {
                        //        new CardAction()
                        //        {
                        //            Title = "Book Now",
                        //            Type = ActionTypes.OpenUrl,
                        //            Value = $"https://www.google.com"
                        //        }
                        //    }
                    };
                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }
                datesAdded.Add(currentDateTime);
                //await context.PostAsync(response);
            }
            await context.PostAsync(resultMessage);
        }

        public string KelvinToCentigrade(float kelvin)
        {
            return kelvin - 272.15 + "C";
        }

        public static string ToAbsoluteUrl(string relativeUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(relativeUrl))
                    return relativeUrl;

                if (HttpContext.Current == null)
                    return relativeUrl;

                if (relativeUrl.StartsWith("/"))
                    relativeUrl = relativeUrl.Insert(0, "~");
                if (!relativeUrl.StartsWith("~/"))
                    relativeUrl = relativeUrl.Insert(0, "~/");

                var url = HttpContext.Current.Request.Url;
                var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

                return String.Format("{0}://{1}{2}{3}",
                    url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
            }
            catch (Exception ex)
            {
                return relativeUrl;
            }
        }
        private async Task ResumeAfterTravelFormDialog(IDialogContext context, IAwaitable<TravelBooking> result)
        {
            var travelInfo = await result;
            if (string.IsNullOrEmpty(travelInfo.TravelType))
                travelInfo.TravelType = "Buses";
            if (!string.IsNullOrEmpty(travelInfo.DateOfTravel) && ParseChronicDate(travelInfo.DateOfTravel).HasValue)
                travelInfo.ConvertedDateTime = ParseChronicDate(travelInfo.DateOfTravel).Value;
            if (travelInfo.ConvertedDateTime == DateTime.MinValue)
                travelInfo.ConvertedDateTime = DateTime.Now.Date;
            //await context.PostAsync($"Now {travelInfo.TravelType} will be searched");
            await context.PostAsync("Now I am going to make some quick calls to get the details... Hold on tight .... It may take a minute since we are using all cheap resources to accumulate the data .");
            Log.Info($"String Entered {travelInfo.DateOfTravel} date Detected : { travelInfo.ConvertedDateTime}");
            BusSearch busSearchHelper = new BusSearch();
            Log.Info("After Bus Search Init");
            var bussesAndFlights = busSearchHelper.SearchBusses(travelInfo.FromLocation, travelInfo.ToLocation, travelInfo.ConvertedDateTime);
            Log.Info("After Bus Search Method");
            if (bussesAndFlights.data == null || bussesAndFlights.data.onwardflights == null || bussesAndFlights.data.onwardflights.Count() == 0)
                await context.PostAsync($"Sorry I cant find any {travelInfo.TravelType} from {travelInfo.FromLocation} to {travelInfo.ToLocation}");
            else
            {
                var busResultAfterSortingLogic = bussesAndFlights.data.onwardflights.AsEnumerable();
                if (!string.IsNullOrEmpty(travelInfo.Class))
                {
                    if (travelInfo.Class.ToLower().Contains("fast"))
                    {
                        busResultAfterSortingLogic = bussesAndFlights.data.onwardflights.OrderBy(x => x.arrdate - x.depdate);
                    }
                    else if (travelInfo.Class.ToLower().Contains("cheap"))
                    {
                        busResultAfterSortingLogic = bussesAndFlights.data.onwardflights.OrderBy(x => x.fare.totalfare);
                    }
                }

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();
                //var nonAcURL = "";//ToAbsoluteUrl("/Images/Non AC bus.jpg");
                //var acUrl = ""; ToAbsoluteUrl("/Images/AC bus.jpg");
                foreach (var busOrFlight in busResultAfterSortingLogic.Take(5))
                {
                    var routeType = busOrFlight.RouteSeatTypeDetail.list.FirstOrDefault();
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = busOrFlight.TravelsName,
                        Subtitle = $"{busOrFlight.BusType} starts at {busOrFlight.DepartureTime} to {busOrFlight.destination}  @Rs . {busOrFlight.fare.totalfare} ",
                        Text = $"Departs on {busOrFlight.depdate} at {busOrFlight.DepartureTime} Origin - {busOrFlight.origin} service number - {busOrFlight.BusServiceID} reaches {busOrFlight.destination} at {busOrFlight.arrdate} @Rs . {busOrFlight.fare.totalfare}",
                        //Images = new List<CardImage>()
                        //{
                        //    new CardImage() { Url = routeType == null ? nonAcURL : (routeType.busCondition.Contains("nonac") ? nonAcURL : acUrl) }
                        //},
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "Book Now",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.google.com"
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }
                await context.PostAsync(resultMessage);
            }
        }

        private IForm<TravelBooking> BuildTravelsForm()
        {
            OnCompletionAsyncDelegate<TravelBooking> TravelBookingSearch = async (context, state) =>
            {
                var message = "Searching for ";
                if (!string.IsNullOrEmpty(state.TravelType))
                {
                    message += $" {state.TravelType}...";
                }
                //else if (!string.IsNullOrEmpty(state.AirportCode))
                //{
                //    message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
                //}

                await context.PostAsync(message);
            };

            return new FormBuilder<TravelBooking>()
                .Field(nameof(TravelBooking.FromLocation), (state) => string.IsNullOrEmpty(state.FromLocation))
                .Field(nameof(TravelBooking.ToLocation), (state) => string.IsNullOrEmpty(state.ToLocation))
                .Field(nameof(TravelBooking.DateOfTravel), (state) => string.IsNullOrEmpty(state.DateOfTravel))
                .OnCompletion(TravelBookingSearch)
                .Build();
        }


        //[LuisIntent("ShowHotelsReviews")]
        //public async Task Reviews(IDialogContext context, LuisResult result)
        //{
        //    EntityRecommendation hotelEntityRecommendation;

        //    if (result.TryFindEntity(EntityHotelName, out hotelEntityRecommendation))
        //    {
        //        await context.PostAsync($"Looking for reviews of '{hotelEntityRecommendation.Entity}'...");

        //        var resultMessage = context.MakeMessage();
        //        resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //        resultMessage.Attachments = new List<Attachment>();

        //        for (int i = 0; i < 5; i++)
        //        {
        //            var random = new Random(i);
        //            ThumbnailCard thumbnailCard = new ThumbnailCard()
        //            {
        //                Title = this.titleOptions[random.Next(0, this.titleOptions.Count - 1)],
        //                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris odio magna, sodales vel ligula sit amet, vulputate vehicula velit. Nulla quis consectetur neque, sed commodo metus.",
        //                Images = new List<CardImage>()
        //                {
        //                    new CardImage() { Url = "https://upload.wikimedia.org/wikipedia/en/e/ee/Unknown-person.gif" }
        //                },
        //            };

        //            resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
        //        }

        //        await context.PostAsync(resultMessage);
        //    }

        //    context.Wait(this.MessageReceived);
        //}

        [LuisIntent("OnDevice.Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi I am currently learning on how to become a travel assistant ..... I am still very young and trying my best ... I am here to help! Try asking me things like 'Busses from Hyderabad to Banglore',  'weather in Bombay'");

            context.Wait(this.MessageReceived);
        }

        //private IForm<HotelsQuery> BuildHotelsForm()
        //{
        //    OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
        //    {
        //        var message = "Searching for hotels";
        //        if (!string.IsNullOrEmpty(state.Destination))
        //        {
        //            message += $" in {state.Destination}...";
        //        }
        //        else if (!string.IsNullOrEmpty(state.AirportCode))
        //        {
        //            message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
        //        }

        //        await context.PostAsync(message);
        //    };

        //    return new FormBuilder<HotelsQuery>()
        //        .Field(nameof(HotelsQuery.Destination), (state) => string.IsNullOrEmpty(state.AirportCode))
        //        .Field(nameof(HotelsQuery.AirportCode), (state) => string.IsNullOrEmpty(state.Destination))
        //        .OnCompletion(processHotelsSearch)
        //        .Build();
        //}

        //private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        //{
        //    try
        //    {
        //        var searchQuery = await result;

        //        var hotels = await this.GetHotelsAsync(searchQuery);

        //        await context.PostAsync($"I found {hotels.Count()} hotels:");

        //        var resultMessage = context.MakeMessage();
        //        resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //        resultMessage.Attachments = new List<Attachment>();

        //        foreach (var hotel in hotels)
        //        {
        //            HeroCard heroCard = new HeroCard()
        //            {
        //                Title = hotel.Name,
        //                Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
        //                Images = new List<CardImage>()
        //                {
        //                    new CardImage() { Url = hotel.Image }
        //                },
        //                Buttons = new List<CardAction>()
        //                {
        //                    new CardAction()
        //                    {
        //                        Title = "More details",
        //                        Type = ActionTypes.OpenUrl,
        //                        Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
        //                    }
        //                }
        //            };

        //            resultMessage.Attachments.Add(heroCard.ToAttachment());
        //        }

        //        await context.PostAsync(resultMessage);
        //    }
        //    catch (FormCanceledException ex)
        //    {
        //        string reply;

        //        if (ex.InnerException == null)
        //        {
        //            reply = "You have canceled the operation.";
        //        }
        //        else
        //        {
        //            reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
        //        }

        //        await context.PostAsync(reply);
        //    }
        //    finally
        //    {
        //        context.Done<object>(null);
        //    }
        //}

        //private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
        //{
        //    var hotels = new List<Hotel>();

        //    // Filling the hotels results manually just for demo purposes
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        var random = new Random(i);
        //        Hotel hotel = new Hotel()
        //        {
        //            Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
        //            Location = searchQuery.Destination ?? searchQuery.AirportCode,
        //            Rating = random.Next(1, 5),
        //            NumberOfReviews = random.Next(0, 5000),
        //            PriceStarting = random.Next(80, 450),
        //            Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
        //        };

        //        hotels.Add(hotel);
        //    }

        //    hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

        //    return hotels;
        //}
    }
}
